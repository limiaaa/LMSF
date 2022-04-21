using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using SG.Utils;
using UnityEngine;

namespace SG.AssetBundleUploader
{
    public class HttpRequestUploader
    {
        public class UploadFile
        {
            public UploadFile()
            {
                ContentType = "application/octet-stream";
            }

            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public string FilePath { get; set; }
        }

        public static Action<float, long, long> OnUpload;
       

        public static bool Upload(string url, List<UploadFile> files,
            Dictionary<string, string> nvc)
        {
            DebugUtils.Log(string.Format("Uploading {0} to {1}", files[0], url));
            var request = WebRequest.Create(url);
            request.Method = "POST";
            var boundary = "---------------------------" +
                           DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;
            using (var requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in nvc.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + "\r\n");
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}",
                        name, "\r\n"));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(nvc[name] + "\r\n");
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + "\r\n");
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format(
                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename,
                        "\r\n"));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType,
                        "\r\n"));
                    requestStream.Write(buffer, 0, buffer.Length);
                    FileStream fileStream = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer1 = new byte[4096];
                    int bytesRead = 0;
                    long uploaded = 0;
                    while ((bytesRead = fileStream.Read(buffer1, 0, buffer1.Length)) != 0)
                    {
                        uploaded += bytesRead;
                        // if (OnUpload != null)
                        // {
                            // OnUpload((uploaded * 1f/fileStream.Length),uploaded,fileStream.Length);
                        // }
                        
                        requestStream.Write(buffer1, 0, bytesRead);
                    }

                    fileStream.Close();
                    buffer = Encoding.ASCII.GetBytes("\r\n");
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            HttpWebResponse wresp = null;
            try
            {
                wresp = (HttpWebResponse) request.GetResponse();
                
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                DebugUtils.Log(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
                
                if (wresp.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }

            catch (Exception ex)
            {
                Debug.LogError("Error uploading file: " + ex.Message);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }

                return false;
            }
            finally
            {
                request = null;
            }
        }
        
        
    }
}