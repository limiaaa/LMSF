using System.Collections;
using System.Collections.Generic;
using SG.Utils;
using UnityEngine;

namespace SG.LocalData
{
    public static class ResourcePath
{
    public const string CommonPrefabsPath = "Assets/MainApp/Prefabs/";
    public static string FashionDir { get; private set; } = "Fashion/";
    public static string VFXDir { get; private set; } = "VFX/";
    public static string JSONDir { get; private set; } = "JSON/";
    public static string AnimatorDir { get; private set; } = "Animator/";


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
    static ResourcePath()
    {
        // Used for download or Update or saves

        string local = Application.persistentDataPath + "/";
        if (Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor)
        {
            local = Application.dataPath + "/StreamingAssetsOuts/";
            if (!System.IO.Directory.Exists(local))
            {
                System.IO.Directory.CreateDirectory(local);
            }
        }
        DebugUtils.Log("Init Local Path:" + local);

        localPath = local;
        localUrl = "file://" + localPath;

        if (Application.platform == RuntimePlatform.Android)
        {
            rawPath = Application.dataPath + "!/assets/";
            rawUrl = "jar:file://" + Application.dataPath + "!/assets/";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            rawPath = Application.dataPath + "/Raw/";
            rawUrl = "file://" + Application.dataPath + "/Raw/";
        }
        else
        {
            rawPath = Application.dataPath + "/StreamingAssets/";
            rawUrl = "file://" + Application.dataPath + "/StreamingAssets/";
        }
    }
}
}

