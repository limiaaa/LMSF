using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class Util
{
    public static string ParseThousandFormatToNumber(string numberStr , string splitSymbol)
    {
        if (string.IsNullOrEmpty(numberStr))
            numberStr = "0";
        var tempNumStr = numberStr;
        var symbols = new StringBuilder();
        foreach (var c in numberStr)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z' ))
            {
                symbols.Append(c);
            }
        }

        var symbolsStr = "";
        
        if (symbols.Length > 0)
        {
            symbolsStr = symbols.ToString();
            tempNumStr = numberStr.Replace(symbolsStr, "");
        }

        var result = String.Join("", tempNumStr.Split(splitSymbol.ToCharArray()[0]));
        var num =  new StringBuilder((long.Parse(result)).ToString());
        if (symbolsStr.Length > 0)
        {
            for (var i = 0; i < symbolsStr.Length; i++)
            {
                switch (symbolsStr[i])
                {
                    case 'K':
                        num.Append("000");
                        break;
                    case 'M':
                        num.Append("000000");
                        break;
                    case 'B':
                        num.Append("000000000");
                        break;
                    case 'T':
                        num.Append("000000000000");
                        break;
                    case 'k':
                        num.Append("000");
                        break;
                    case 'm':
                        num.Append("000000");
                        break;
                    case 'b':
                        num.Append("000000000");
                        break;
                    case 't':
                        num.Append("000000000000");
                        break;
                }
            }
        }

        return num.ToString();
    }
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }
    public static string md5file(string file)
    {
        long size = 0;
        return md5file(file, out size);
    }
    public static string md5file(string file, out long size)
    {
        try
        {
            //FileStream fs = new FileStream(file, FileMode.Open);
            FileStream fs = File.OpenRead(file);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            size = fs.Length;
            fs.Close();
            fs.Dispose();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }
    public static string md5Data(byte[] data)
    {
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(data);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < retVal.Length; i++)
        {
            sb.Append(retVal[i].ToString("x2"));
        }
        return sb.ToString();
    }
    /// <summary>
    /// 清理内存
    /// </summary>
    public static void ClearMemory()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
        //LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
        //if (mgr != null)
        //    mgr.LuaGC();
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
            string game = AppConst.AppName.ToLower();
            if (Application.isMobilePlatform)
            {
                //只能用到Android路径
                //return Application.persistentDataPath + "/" + game + "/";
                if (Application.platform == RuntimePlatform.Android)
                {
                    return Application.persistentDataPath + "/" + game + "/";
                    //不采用下面的流氓软件模式
                    //if (myPersistentDataPath == "")
                    //{
                    //    string[] ss = Application.persistentDataPath.Split('/');
                    //    myPersistentDataPath = "";
                    //    for (int i = 0; i < ss.Length - 4; i++)
                    //    {
                    //        myPersistentDataPath += (ss[i] + "/");
                    //    }
                    //    //.AndroidData前面的.表示隐藏目录
                    //    myPersistentDataPath += "system/.AndroidData";
                    //    if (!Directory.Exists(myPersistentDataPath))
                    //    {
                    //        Directory.CreateDirectory(myPersistentDataPath);
                    //    }
                    //}
                    //return myPersistentDataPath;
                }
                else
                {
                    return Application.temporaryCachePath + "/" + game + "/";
                }
            }
            if (AppConst.DebugMode)
            {
                return Application.dataPath + "/" + AppConst.AssetDir + "/";
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return Application.persistentDataPath + "/cache/";
                int i = Application.persistentDataPath.LastIndexOf('/');
                return Application.persistentDataPath.Substring(0, i + 1) + game + "/";
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return Application.persistentDataPath + "/" + game + "/";
            }
            //return Application.dataPath.Replace("Assets", "Download/");//"c:/" + game + "/";
            return Application.persistentDataPath + "/" + game + "/";
        }
    }
    public static string GetRelativePath()
    {
        if (Application.isEditor)
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/" + AppConst.AssetDir + "/";
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
            return "file:///" + DataPath;
        else // For standalone player.
            return "file://" + Application.streamingAssetsPath + "/";
    }
    public static string StreamingAssetsPath
    {
        get
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = Application.streamingAssetsPath + "/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.streamingAssetsPath + "/";
                    break;
                default:
                    path = Application.streamingAssetsPath + "/";
                    break;
            }
            return path;
        }
    }
    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
                break;
            default:
                path = Application.dataPath + "/" + AppConst.AssetDir + "/";
                break;
        }
        return path;
    }







    /// <summary>
    /// 网络可用
    /// </summary>
    public static bool NetAvailable
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
    /// <summary>
    /// 是否是无线
    /// </summary>
    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }
    //检查网络是否连接上,1表示未连接网络状态，2表示连接本地网络（网线或者wifi），3表示连接移动网络
    public static int CheckNetWork()
    {
        //当网络不可用时              
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return 1;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            //如果项目需要耗费的流量比较大，可以通过下面的方法判断，并提示用户
            //当用户使用WiFi时 
            return 2;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            //当用户使用移动网络时 
            return 3;
        }
        return 1;
    }
    /// <summary>
    /// 获取运行时平台, 0 WEB, 1 WINDOWS, 2Andorid , 3 IOS, 4 Other , 5 Editor
    /// </summary>
    public static int GetRunPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WebGLPlayer:
                return 0;
            case RuntimePlatform.WindowsPlayer:
                return 1;
            case RuntimePlatform.Android:
                return 2;
            case RuntimePlatform.IPhonePlayer:
                return 3;
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
                return 5;
            default:
                return 4;
        }
    }
    public static byte[] StringToBytes(string str)
    {
        return Encoding.GetEncoding("utf-8").GetBytes(str);
    }
    public static void ExitApp()
    {
#if UNITY_ANDROID
        Application.Quit();
#endif
    }
    /// <summary>
    /// 是不是苹果平台
    /// </summary>
    /// <returns></returns>
    public static bool isApplePlatform
    {
        get
        {
            return Application.platform == RuntimePlatform.IPhonePlayer ||
                   Application.platform == RuntimePlatform.OSXEditor ||
                   Application.platform == RuntimePlatform.OSXPlayer;
        }
    }
    /// <summary>
    /// 判断手机信号，暂时针对iPhone X
    /// </summary>
    /// <returns></returns>
    public static int getPhoneType()
    {
        string deviceModelType = SystemInfo.deviceModel;
        if (deviceModelType.Contains("iPhone10"))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public static Texture2D CaptureCamera(Camera camera, Rect rect, string filepath)
    {
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = filepath;// Application.dataPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        return screenShot;

    }
    public static bool isFileExist(string path)
    {
        // #if !UNITY_EDITOR && UNITY_ANDROID
        var pathStartIndex = path.IndexOf(Application.streamingAssetsPath);
#if !UNITY_EDITOR && UNITY_ANDROID
        if (pathStartIndex != -1) {
			path = path.Replace(Util.StreamingAssetsPath,"");
            AndroidJavaClass androidJavaClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject androidJavaObject = null;
            if (androidJavaClass != null) {
                androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject> ("currentActivity");
                if (androidJavaObject == null) {
                    return false;
                }
            }
            return androidJavaObject.Call<bool> ("isFileExistInAsset", path);
        } else {
#endif
        return File.Exists(path);
#if !UNITY_EDITOR && UNITY_ANDROID
		}
#endif
    }
    public static string ReadFile(string path)
    {
        var buffer = loadAsset(path);
        if (buffer == null) { return string.Empty; }
        return Encoding.UTF8.GetString(buffer);
    }
    public static byte[] loadAsset(string path)
    {
        byte[] data = null;
#if !UNITY_EDITOR && UNITY_ANDROID
        var pathStartIndex = path.IndexOf (Application.streamingAssetsPath);
        if (pathStartIndex != -1) {
            path = path.Substring (pathStartIndex + Application.streamingAssetsPath.Length + 1);
            AndroidJavaClass androidJavaClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject androidJavaObject = null;
            if (androidJavaClass != null) {
                androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject> ("currentActivity");
                if (androidJavaObject == null) {
                    return null;
                }
            }

            //Util.Log ("Trying to load asset from: " + path);
            data = androidJavaObject.Call<byte []> ("loadDataFromAsset", path);
            if (data == null) {
                //Util.Log (path + " load failed!");
            } else {
                //Util.Log (path + " loaded, length = " + data.Length);
            }
            return data;
        } else {
#endif
        if (File.Exists(path))
        {
            using (var file = File.OpenRead(path))
            {
                data = new byte[file.Length];
                file.Read(data, 0, (int)file.Length);
            }
        }
        return data;
#if !UNITY_EDITOR && UNITY_ANDROID
        }
#endif
    }
    public static IEnumerator loadAssetAsync(string path, Action<byte[]> completion)
    {
        if (path.IndexOf(Application.streamingAssetsPath) != 0 || path.IndexOf("://") == -1)
        {
            path = "file://" + path;
        }
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (completion != null)
        {
            completion(request.downloadHandler.data);
        }
    }
    static string[] ImageExtensions = {
        ".jpg", ".png", ".jpeg"
    };
    public static IEnumerator loadFileAsync(string path, int timeOut, CancellationTokenSource source, Action<bool, DownloadHandler> completion)
    {
        if (path.IndexOf(Application.streamingAssetsPath) != 0 || path.IndexOf("://") == -1)
        {
            path = "file://" + path;
        }
        var ext = Path.GetExtension(path);

        UnityWebRequest request = null;
        if (Array.IndexOf(ImageExtensions, ext) != -1)
        {
            request = UnityWebRequestTexture.GetTexture(path);
        }
        else
        {
            request = UnityWebRequest.Get(path);
        }                                

        // Util.Log ("Begin load " + path + " from file...");
        var operation = request.SendWebRequest();

        Task timeOutTask = null;
        if (timeOut == 0)
        {
            yield return operation;
        }
        else
        {
            timeOutTask = Task.Delay(timeOut, source.Token);
            while (!timeOutTask.IsCompleted && !(request.isDone || request.isNetworkError || request.isHttpError) && !source.IsCancellationRequested && !operation.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        if ((timeOutTask != null && timeOutTask.IsCompleted) || source.IsCancellationRequested || request.isNetworkError || request.isHttpError || request.error == "Request timeout")
        {
            // Util.Log ("File " + path + " load timed out or error happend: " + request.error);
            request.Abort();
            completion(timeOutTask.IsCompleted || request.error == "Request timeout", null);
        }
        else
        {
            // Util.Log ("File " + path + " load finished");
            completion(false, request.downloadHandler);
        }
    }
    public static IEnumerator saveFileAsync(string path, byte[] buffer, Action completion = null)
    {
        var dirName = Path.GetDirectoryName(path);
        if (!Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        using (var stream = File.Create(path))
        {
            var task = stream.WriteAsync(buffer, 0, buffer.Length);
            while (!task.IsCompleted)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        if (completion != null)
        {
            completion();
        }
    }
    static IEnumerator getWebContent1(string url, int timeOut, CancellationTokenSource source, Action<bool, DownloadHandler> completion)
    {
        var request = UnityWebRequest.Get(url);
        yield return getWebContent2(request, timeOut, source, completion);
    }
    static IEnumerator getWebContent2(UnityWebRequest request, int timeOut, CancellationTokenSource source, Action<bool, DownloadHandler> completion)
    {
        request.timeout = timeOut;
        var operation = request.SendWebRequest();
        var timeOutTask = Task.Delay(timeOut, source.Token);
        while (!timeOutTask.IsCompleted && !request.isDone && !source.IsCancellationRequested && !operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        if (timeOutTask.IsCompleted || source.IsCancellationRequested || request.isNetworkError || request.isHttpError || request.error == "Request timeout")
        {
            request.Abort();
            completion(timeOutTask.IsCompleted || request.error == "Request timeout", null);
        }
        else
        {
            completion(false, request.downloadHandler);
        }
    }
    public static Vector3 GetClickPos(Transform trans,Vector3 Vec,Transform came)
    {
        Vector2 uipos = Vector3.one;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(trans.parent.parent.transform as RectTransform,Vec, came.GetComponent<Camera>(), out uipos);
        return uipos;
    }
}
