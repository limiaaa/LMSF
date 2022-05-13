using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;

public static class PathUtils
{
#if UNITY_EDITOR
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.dataPath + "/PersistentAssets";
#elif UNITY_STANDALONE_WIN
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.dataPath + "/PersistentAssets";
#elif UNITY_IPHONE
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.persistentDataPath;
#elif UNITY_ANDROID
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.persistentDataPath;
#elif UNITY_WEBGL
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.persistentDataPath;
#else
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.dataPath + "/PersistentAssets";
#endif

    // 存档路径，可以用于存储存档或者热更新包
    //Android : /storage/emulated/0/Android/data/package name/files
    //IOS : /var/mobile/Containers/Data/Application/app sandbox/Documents
    // Windows : C:\Users\username\AppData\LocalLow\company name\product name
    public static string localPath;
    public static string localUrl;
    //StreamingAssets dir
    // Android : jar:file:///data/app/package name-1/base.apk!/assets
    // IOS : /var/containers/Bundle/Application/app sandbox/test.app/Data/Raw
    // windows : 应用的appname_Data/StreamingAssets
    public static string rawPath;
    public static string rawUrl;
    public static void Init()
    {
        string local = Application.persistentDataPath + "/";
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            local = Application.dataPath + "/StreamingAssetsOuts/";
            if (!System.IO.Directory.Exists(local))
            {
                System.IO.Directory.CreateDirectory(local);
            }
        }
        localPath = local;
        localUrl = "file:///" + local;
        //虚拟地址
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                rawPath = Application.dataPath + "!/assets/";
                rawUrl = "jar:file:///" + Application.dataPath + "!/assets/";
                break;
            case RuntimePlatform.IPhonePlayer:
                rawPath = Application.dataPath + "/Raw/";
                rawUrl = "file:///" + Application.dataPath + "/Raw/";
                break;
            default:
                rawPath = Application.dataPath + "/StreamingAssets/";
                rawUrl = "file://" + Application.dataPath + "/StreamingAssets/";
                break;
        }
    }
}
