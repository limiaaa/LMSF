using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AbInfo{
    //D:/data/ArtOfWar/Assets/StreamingAssets/abinfo/effect.lo
    public string path;
    //abinfo/effect.lo
    public string abLocalPath;
    public string fileName;
    public string size;

}
public class ManifestManager:MonoSingleton<ManifestManager>
{
    Dictionary<string, AbInfo> Abinfo_Dic = new Dictionary<string, AbInfo>();
    AssetBundleManifest MainAssetBundleManifest;
    AssetBundle MainAssetBundle;

    public void InitManifestManager()
    {
        MainAssetBundle = AssetBundle.LoadFromFile(ResourcePath.StreamAssetPath()+AppConst.MainAbName);
        MainAssetBundleManifest = MainAssetBundle.LoadAsset<AssetBundleManifest>(AppConst.MainMainfestName);
        string[] AllAbInfo = MainAssetBundleManifest.GetAllAssetBundles();
        RecordAllAbInfo(AllAbInfo);
    }

    //item:                                 abinfo/effect.lo
    //LocalPath:                            D:/data/ArtOfWar/Assets/StreamingAssets/abinfo/effect.lo
    //ResourcePath.GetResourceName(item):   effect.lo
    void RecordAllAbInfo(string[]abInfoList)
    {
        foreach(var item in abInfoList)
        {
            string LocalPath = ResourcePath.CombineABPath(ResourcePath.StreamAssetPath(), null, item);
            AddPath(ResourcePath.GetResourceName(item), LocalPath,item);
        }
    }
    //ab名,ab完整路径,ab+上一层
    void AddPath(string fileName,string path,string abPath)
    {
        AbInfo abInfo = new AbInfo();
        abInfo.fileName = fileName.ToLower() + AppConst.ExtName;
        abInfo.path = path.ToLower() + AppConst.ExtName;
        abInfo.abLocalPath = abPath.ToLower() + AppConst.ExtName;
        if (Abinfo_Dic.ContainsKey(fileName))
        {
            DebugUtils.LogError($"已经有此路径了{fileName}");
        }
        else
        {
            Abinfo_Dic[fileName] = abInfo;
        }
    }
    public AbInfo GetPathByFileName(string fileName)
    {
        if (Abinfo_Dic.ContainsKey(fileName))
        {
            return Abinfo_Dic[fileName];
        }
        else
        {
            DebugUtils.LogError($"此路径不存在{fileName}");
            return null;
        }
    }
    public AssetBundle GetMainAssetBundle()
    {
        return MainAssetBundle;
    }
    public AssetBundleManifest GetMainAssetBundleManifest()
    {
        return MainAssetBundleManifest;
    }

}
