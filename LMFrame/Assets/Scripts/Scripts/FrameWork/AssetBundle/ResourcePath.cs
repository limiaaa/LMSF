using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourcePath 
{
    public static string StreamAssetPath()
    {
        if (GameSetting.Instance.IsDeveloper)
        {
            return Application.dataPath + "/StreamingAssets/";
        }
        else
        {
            return Application.streamingAssetsPath + "/";
        }
    }
    public static string GetResourceName(string fileName)
    {
        var name = fileName;
        int index = fileName.LastIndexOf("/");
        if (index >= 0)
        {
            return fileName.Substring(index + "/".Length);
        }
        return fileName;
    }
    public static string CombineABPath(string streamingAssets, string subFolderPath, string fileName)
    {
        string localFilePath = streamingAssets + subFolderPath + fileName;
        return localFilePath;
    }
}
