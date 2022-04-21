using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using SG.Utils;
using UnityEngine;

namespace SG.AssetBundleBrowser
{
    /// <summary>
    ///     1. AmazonS3 尽支持已知格式的gzip压缩，AB包不支持gzip，所以此处所有都采用identity
    ///     https://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/ServingCompressedFiles.html
    ///     2. 因为采用了identity, 文件长度统一为：byteRange + contentLength;
    ///     3. 支持断点续传
    ///     4. 支持限速
    ///     5. 支持协程调用
    /// </summary>
    public class WebRequestDownload
    {
        public enum ErrorCodeEnum
        {
            None, // 无
            Cancel, // 取消下载
            NoResponse, // 服务器未响应
            DownloadError, // 下载出错
            TimeOut, // 超时
            Abort, // 强制关闭
            NotFound, // 404
            Forbidden,
            SpaceNotEnough,

            UnmatchedLength
            //TODO: unzip error code
        }

        public const int TIMEOUT_TIME = 1000 * 30;

        private DateTime beginTime;

        public long completedSize;

        private ContentDownload contentFile;

        public ErrorCodeEnum errorCode;

        private HttpWebRequest httpWebRequest;

        /// <summary>
        ///     锁对象，用于保证线程安全
        /// </summary>
        private readonly object lock_obj_ = new object();

        private Action<WebRequestDownload> onDownloadError;

        /// <summary>
        ///     本次下载字节数   下载速度KB/S  总完成进度
        /// </summary>
        private Action<WebRequestDownload, long, float, float> onDownloadUpdate;

        private RegisteredWaitHandle registered_wait_handle_;

        public long speedLimit;

        /// <summary>
        ///     File size
        /// </summary>
        public long totalSize;

        public string url;
        private WaitHandle wait_handle_;

        public WebRequestDownload(string url)
        {
            this.url = url;
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        }

        public float progress
        {
            get { return (completedSize + contentFile.lastTimeCompletedLength) * 1f / totalSize; }
        }

        /// <summary>
        ///     this request length
        /// </summary>
        public long length { get; private set; }

        public string localName { get; private set; }
        public string rootPath { get; private set; }

        public bool isDone { get; private set; }

        public string fullName
        {
            get
            {
                return string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(localName)
                    ? null
                    : Path.Combine(rootPath, localName);
            }
        }

        public void Start(string root, string localFileName, long fullLength = 0,
            Action<WebRequestDownload, long, float, float> notify = null, Action<WebRequestDownload> error = null,
            long speedLimit = 0)
        {
            lock (lock_obj_)
            {
                Abort();

                // totalSize = fullLength;
                rootPath = root;
                localName = localFileName;
                this.speedLimit = speedLimit;
                isDone = false;
                errorCode = ErrorCodeEnum.None;

                onDownloadUpdate = notify;

                completedSize = 0;

                onDownloadError = error;

                contentFile = new ContentDownload(fullName, false);

                Download();
            }
        }

        private void Download(long range = 0)
        {
            try
            {
                lock (lock_obj_)
                {
                    beginTime = DateTime.Now;
                    //尝试下载资源，携带If-Modified-Since
                    httpWebRequest = WebRequest.Create(url + localName) as HttpWebRequest;
//                    DebugUtils.Log("Download: " + http_request_.RequestUri.AbsoluteUri);

                    httpWebRequest.Timeout = TIMEOUT_TIME;
                    httpWebRequest.KeepAlive = false;
                    // AmazonS3 尽支持已知格式的gzip压缩，AB包不支持gzip，所以此处所有都采用identity
                    httpWebRequest.Headers.Add("Accept-Encoding", "identity");
                    if (range == 0)
                        httpWebRequest.IfModifiedSince = contentFile.lastModified;
                    else
                        httpWebRequest.AddRange((int) range);

                    var result =
                        httpWebRequest.BeginGetResponse(_OnResponseCallback, httpWebRequest);
                    RegisterTimeOut(result.AsyncWaitHandle);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("WebRequestDownload - \"" + localName + "\" download failed!"
                               + "\nMessage:" + e.Message + "\n" + e.StackTrace);
                UnregisterTimeOut();
                OnFailed(ErrorCodeEnum.NoResponse);
            }
        }

        private void RegisterTimeOut(WaitHandle handle)
        {
            wait_handle_ = handle;
            registered_wait_handle_ = ThreadPool.RegisterWaitForSingleObject(handle
                , _OnTimeoutCallback
                , httpWebRequest
                , TIMEOUT_TIME
                , true);
        }

        private void UnregisterTimeOut()
        {
            if (registered_wait_handle_ != null && wait_handle_ != null)
                registered_wait_handle_.Unregister(wait_handle_);
        }

        private void _OnTimeoutCallback(object state, bool timedOut)
        {
            lock (lock_obj_)
            {
                if (timedOut) OnFailed(ErrorCodeEnum.TimeOut);

                UnregisterTimeOut();
            }
        }

        private void OnFailed(ErrorCodeEnum code)
        {
            Debug.LogError("OnFailed: " + code);
            lock (lock_obj_)
            {
                if (contentFile != null)
                {
                    contentFile.state = ContentDownload.ContentStateEnum.Failed;
                    contentFile.Close();
                    contentFile = null;
                }

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                    httpWebRequest = null;
                }

                isDone = true;
                errorCode = code;

                Debug.LogError(url + localName + "\n==>ERROR_CODE:" + errorCode);

                if (onDownloadError != null) onDownloadError(this);
            }
        }

        private void _OnResponseCallback(IAsyncResult ar)
        {
            try
            {
                UnregisterTimeOut();

                lock (lock_obj_)
                {
                    var req = ar.AsyncState as HttpWebRequest;
                    if (req == null) return;
                    var response = req.BetterEndGetResponse(ar) as HttpWebResponse;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        length = response.ContentLength;
                        totalSize = length;
                        contentFile.webResponse = response;
                        _BeginRead(_OnReadCallback);
                    }
                    else if (response.StatusCode == HttpStatusCode.NotModified)
                    {
                        if (string.IsNullOrEmpty(req.Headers.Get("Range")))
                        {
                            //表示资源未修改开启断点续传
                            if (httpWebRequest != null)
                            {
                                httpWebRequest.Abort();
                                httpWebRequest = null;
                            }

                            Download(contentFile.lastTimeCompletedLength);
                        }
                        else
                        {
                            OnFailed(ErrorCodeEnum.Abort);
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.PartialContent)
                    {
                        length = contentFile.lastTimeCompletedLength + response.ContentLength;
                        totalSize = length;
                        contentFile.webResponse = response;
                        _BeginRead(_OnReadCallback);
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        response.Close();
                        OnFailed(ErrorCodeEnum.NotFound);
                    }
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        response.Close();
                        OnFailed(ErrorCodeEnum.Forbidden);
                    }
                    else
                    {
                        response.Close();
                        OnFailed(ErrorCodeEnum.NoResponse);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("WebRequestDownload - \"" + localName + "\" download failed!"
                               + "\nMessage:" + e.Message);
                OnFailed(ErrorCodeEnum.DownloadError);
            }
        }

        public void Abort()
        {
            lock (lock_obj_)
            {
                if (contentFile != null && contentFile.state == ContentDownload.ContentStateEnum.Downloading)
                    OnFailed(ErrorCodeEnum.Abort);
            }
        }

        public IAsyncResult _BeginRead(AsyncCallback callback)
        {
            if (contentFile == null || contentFile.responseStream == null)
                return null;

            if (contentFile.state == ContentDownload.ContentStateEnum.Canceling)
            {
                OnFailed(ErrorCodeEnum.Cancel);
                return null;
            }

            if (completedSize * 1f / length > 0.3f)
            {
                // OnFailed(ErrorCodeEnum.Cancel);
            }

            return contentFile.responseStream.BeginRead(contentFile.buffer
                , 0
                , ContentDownload.BUFFER_SIZE
                , callback
                , contentFile);
        }

        private void _OnReadCallback(IAsyncResult ar)
        {
            try
            {
                lock (lock_obj_)
                {
                    var rs = ar.AsyncState as ContentDownload;
                    if (rs == null || rs.responseStream == null)
                        return;
                    if (rs.fs == null) Debug.LogError("WebRequestDownload - rs.FS==null  " + localName);

                    if (rs.buffer == null) Debug.LogError("WebRequestDownload - rs.Buffer==null  " + localName);

                    var read = rs.responseStream.EndRead(ar);
                    if (read > 0)
                    {
                        rs.fs.Write(rs.buffer, 0, read);
                        rs.fs.Flush();
                        completedSize += read;

                        var ts = DateTime.Now - beginTime;
                        var speed = completedSize / 1024f / (float) ts.TotalSeconds;
                        if (speedLimit > 0 && speed > speedLimit) Thread.Sleep(100); // 休息一下.

                        if (onDownloadUpdate != null)
                            onDownloadUpdate(this, read, speed, progress);
                    }
                    else
                    {
                        OnFinish();

                        if (onDownloadUpdate != null)
                            onDownloadUpdate(this, read, 0, 1);
                        return;
                    }

                    _BeginRead(_OnReadCallback);
                }
            }
            catch (WebException e)
            {
                Debug.LogError("WebRequestDownload - \"" + localName + "\" download failed!"
                               + "\nMessage:" + e.Message + "\n" + e.StackTrace);
                OnFailed(ErrorCodeEnum.DownloadError);
            }
            catch (Exception e)
            {
                Debug.LogError("WebRequestDownload - \"" + localName + "\" download failed!"
                               + "\nMessage:" + e.Message + "\n" + e.StackTrace);
                OnFailed(ErrorCodeEnum.DownloadError);
            }
        }

        private void OnFinish()
        {
            lock (lock_obj_)
            {
                if (contentFile != null)
                {
                    contentFile.state = ContentDownload.ContentStateEnum.Completed;
                    contentFile.Close();
                    contentFile = null;
                }

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                    httpWebRequest = null;
                }

                isDone = true;

                var localFileLength = new FileInfo(fullName).Length;
                if (localFileLength != totalSize)
                {
                    errorCode = ErrorCodeEnum.UnmatchedLength;

                    if (onDownloadError != null) onDownloadError(this);
                }
            }
        }

        public IEnumerator StartYield(string root, string localFileName, long fullLength = 0,
            Action<WebRequestDownload, long, float, float> notify = null, Action<WebRequestDownload> error = null,
            long speedLimit = 0)
        {
            Start(root, localFileName, fullLength, notify, error, speedLimit);
            yield return new WaitForWebRequestDownload(this);
        }

        public void Cancel()
        {
            lock (lock_obj_)
            {
                if (contentFile != null && contentFile.state == ContentDownload.ContentStateEnum.Canceling)
                    contentFile.state = ContentDownload.ContentStateEnum.Canceling;
                else
                    isDone = true;
            }
        }


        /// <summary>
        ///     Fix Editor Https Certificate Error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
                for (var i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) continue;

                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    var chainIsValid = chain.Build((X509Certificate2) certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }

            return isOk;
        }

        internal class ContentDownload
        {
            public enum ContentStateEnum
            {
                Downloading,
                Canceling,
                Completed,
                Failed
            }

            /// <summary>
            ///     下载文件缓存的Last-Modified字符串大小
            /// </summary>
            public const int FILE_LAST_MODIFIED_SIZE = 32;

            /// <summary>
            ///     缓存大小
            /// </summary>
            public const int BUFFER_SIZE = 1024;

            /// <summary>
            ///     下载中间文件名
            /// </summary>
            public const string TEMP_EXTENSION_NAME = ".download";

            private HttpWebResponse _webResponse;

            public byte[] buffer;

            public string fileFullName;

            public FileStream fs;

            public DateTime lastModified;

            public long lastTimeCompletedLength;

            public ContentStateEnum state;

            public ContentDownload(string fileName, bool isNew = true)
            {
                fileFullName = fileName;
                state = ContentStateEnum.Downloading;
                buffer = new byte[BUFFER_SIZE];

                OpenFile(isNew);
            }

            public Stream responseStream { get; private set; }

            public HttpWebResponse webResponse
            {
                get { return _webResponse; }
                set
                {
                    _webResponse = value;
                    if (_webResponse != null)
                        responseStream = _webResponse.GetResponseStream();
                }
            }

            public string tempFileFullName
            {
                get { return fileFullName + TEMP_EXTENSION_NAME; }
            }

            public void Close()
            {
                if (webResponse != null)
                    CloseFile(webResponse.LastModified);
                else
                    CloseFile();

                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream = null;
                }

                if (webResponse != null)
                {
                    webResponse.Close();
                    webResponse = null;
                }
            }

            private void OpenFile(bool isNew, bool failedRetry = true)
            {
                try
                {
                    var parent = Path.GetDirectoryName(fileFullName);
                    Directory.CreateDirectory(parent);

                    //写入到临时文件中，下载完成后改回来
                    if (isNew || !File.Exists(tempFileFullName))
                    {
                        DebugUtils.Log("WebRequestDownload - OpenFile - create new file\n" + tempFileFullName);

                        //创建新的文件
                        fs = new FileStream(tempFileFullName, FileMode.Create, FileAccess.ReadWrite);
                        lastTimeCompletedLength = 0;
                        lastModified = DateTime.MinValue;
                    }
                    else
                    {
                        DebugUtils.Log("WebRequestDownload - OpenFile - read old file\n" + tempFileFullName);
                        //断点续传
                        fs = new FileStream(tempFileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        lastTimeCompletedLength = fs.Length;
                        if (lastTimeCompletedLength > FILE_LAST_MODIFIED_SIZE
                            && ReadLastModified(ref lastModified))
                        {
                            fs.Seek(lastTimeCompletedLength - FILE_LAST_MODIFIED_SIZE, SeekOrigin.Begin);
                            lastTimeCompletedLength -= FILE_LAST_MODIFIED_SIZE;
                        }
                        else
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            lastTimeCompletedLength = 0;
                            lastModified = DateTime.MinValue;
                        }
                    }

                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogError("WebRequestDownload - OpenFile:" + ex.Message + "\n" + tempFileFullName + "\n" +
                                   ex.StackTrace);

                    if (File.Exists(tempFileFullName)) File.Delete(tempFileFullName);

                    // retry
                    if (failedRetry)
                        OpenFile(true, false);
                }

                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }

            public void CloseFile(DateTime lastModified = default(DateTime))
            {
                if (state == ContentStateEnum.Failed && lastModified != default(DateTime))
                    WriteLastModified(lastModified);

                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }

                if (File.Exists(tempFileFullName))
                {
                    if (state == ContentStateEnum.Completed)
                    {
                        //如果下载完成修正文件名
                        if (File.Exists(fileFullName))
                            File.Delete(fileFullName);
                        File.Move(tempFileFullName, fileFullName);
                    }
                    else
                    {
                        if (lastModified == default(DateTime))
                            //未下载完成且response都没建立的情况，删除缓存文件
                            File.Delete(tempFileFullName);
                    }
                }
            }

            /// <summary>
            ///     写入Last-Modified
            /// </summary>
            private bool WriteLastModified(DateTime last_modified)
            {
                if (fs != null)
                {
                    //写入Last-Modified
                    var str = last_modified.Ticks.ToString("d" + FILE_LAST_MODIFIED_SIZE);
                    var bytes = Encoding.UTF8.GetBytes(str);
                    fs.Write(bytes, 0, bytes.Length);

                    return true;
                }

                return false;
            }

            /// <summary>
            ///     读取Last-Modified
            /// </summary>
            private bool ReadLastModified(ref DateTime last_modified)
            {
                if (fs != null && fs.Length > FILE_LAST_MODIFIED_SIZE)
                {
                    var bytes = new byte[FILE_LAST_MODIFIED_SIZE];
                    fs.Seek(lastTimeCompletedLength - FILE_LAST_MODIFIED_SIZE, SeekOrigin.Begin);
                    fs.Read(bytes, 0, FILE_LAST_MODIFIED_SIZE);

                    try
                    {
                        var ticks = long.Parse(Encoding.Default.GetString(bytes));
                        last_modified = new DateTime(ticks);
                    }
                    catch (Exception e)
                    {
                        DebugUtils.Log("WebRequestDownload - ReadLastModified: " + e.Message + "\n" + e.StackTrace);
                        last_modified = DateTime.Now;
                        return false;
                    }


                    return true;
                }

                return false;
            }
        }

        public class WaitForWebRequestDownload : CustomYieldInstruction
        {
            private readonly WebRequestDownload _request;

            public WaitForWebRequestDownload(WebRequestDownload request)
            {
                _request = request;
            }

            public override bool keepWaiting
            {
                get { return !_request.isDone; }
            }
        }
    }
}