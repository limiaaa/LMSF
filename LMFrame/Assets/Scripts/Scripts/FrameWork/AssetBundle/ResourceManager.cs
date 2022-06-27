using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    Dictionary<string, AssetBundle> mAssetBundleDic = new Dictionary<string, AssetBundle>();
    Dictionary<string, SpriteAtlas> mSpriteAtlasDic = new Dictionary<string, SpriteAtlas>();
    public GameObject LoadPrefeb(string ABname, string Assetname)
    {
        ABname = ABname.ToLower();
        GameObject obj = LoadAsset<GameObject>(ABname, Assetname);
        return obj;
    }
    public Material LoadMaterial(string ABname, string Assetname)
    {
        ABname = ABname.ToLower();
        Material result = LoadAsset<Material>(ABname, Assetname);
        return result;
    }
    public Sprite LoadSprite(string ABname, string Assetname)
    {
        ABname = ABname.ToLower();
        Sprite result = LoadAsset<Sprite>(ABname, Assetname);
        return result;
    }
    IEnumerator LoadImgAssetBundle(string path)
    {
        UnityWebRequest Web = UnityWebRequest.Get(path);
        yield return Web.SendWebRequest();
        byte[] buffer = Web.downloadHandler.data;
        Texture2D Test = new Texture2D(2, 2);
        Test.LoadImage(buffer);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprites = Sprite.Create(Test, new Rect(0.0f, 0.0f, Test.width, Test.height), pivot, 100.0f);
    }
    public T LoadAsset<T>(string abName,string assetName)where T: UnityEngine.Object
    {
        var abInfo = ManifestManager.Instance.GetPathByFileName(abName);
        if (GameSetting.Instance.IsDeveloper)
        {
            var paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abInfo.abLocalPath,assetName);
            return AssetDatabase.LoadAssetAtPath<T>(paths[0]);
        }
        else
        {
            AssetBundle abundle = LoadAssetBundle(abInfo);
            
            abundle.LoadAsset<T>(assetName);

            if (abundle == null)
            {
                DebugUtils.LogError($"加载AssetBundle为空,abName =>{abName} assetName=>{assetName}");
                return null;
            }
            var res = abundle.LoadAsset<T>(assetName);
            if (res == null)
            {
                Debug.LogError($"加载资源为空,abName =>{abName} assetName=>{assetName}");
                return null;
            }

            return res;
        }
    }
    AssetBundle LoadAssetBundle(AbInfo abInfo)
    {
        AssetBundle abundle;
        if (!mAssetBundleDic.ContainsKey(abInfo.abLocalPath))
        {
            try
            {
                LoadDependencies(abInfo.abLocalPath);
            }
            catch(Exception e)
            {
                Debug.LogError($"加载依赖失败{abInfo.abLocalPath}-----{e.Message}");
            }
            abundle = AssetBundle.LoadFromFile(abInfo.path);
            mAssetBundleDic[abInfo.abLocalPath] = abundle;
        }
        else
        {
            abundle = mAssetBundleDic[abInfo.abLocalPath];
        }
        return abundle;
    }
    private void LoadDependencies(string Aburi)
    {
        var manifest = ManifestManager.Instance.GetMainAssetBundleManifest();
        if (manifest == null)
        {
            DebugUtils.Log("主ManiFest文件为空");
            return;
        }
        string[] dependencies = manifest.GetAllDependencies(Aburi);
        //if (dependencies.Length <= 0)
        //{
        //    DebugUtils.Log("主ManiFest文件依赖长度为空");
        //    return;
        //}
        for (int i = 0; i < dependencies.Length; i++)
        {
            var abInfo = ManifestManager.Instance.GetPathByFileName(dependencies[i]);
            LoadAssetBundle(abInfo);
        }
    }

    public void UnloadAssetBundle(string abName)
    {
        if (mAssetBundleDic.ContainsKey(abName))
        {
            mAssetBundleDic[abName].Unload(true);
            mAssetBundleDic.Remove(abName);
        }
    }
    public void UnloadResource()
    {
        Resources.UnloadUnusedAssets();
    }

    // 加载Atlas
    public SpriteAtlas LoadAtlas(string abName, string assetName)
    {
        var abInfo = ManifestManager.Instance.GetPathByFileName(abName);
        if (mSpriteAtlasDic.ContainsKey(abInfo.abLocalPath))
        {
            return mSpriteAtlasDic[abInfo.abLocalPath];
        }
        SpriteAtlas atlas = LoadAsset<SpriteAtlas>(abName, assetName);
        if (atlas == null)
        {
            Debug.LogError($"图集加载错误{abInfo.abLocalPath}");
        }
        mSpriteAtlasDic.Add(abInfo.abLocalPath, atlas);
        return atlas;
    }

    // 通过图集获取Sprite
    public Sprite GetSpriteByAtlas(string atlasAbName, string atlasAssetName, string spriteName)
    {
        SpriteAtlas atlas = LoadAtlas(atlasAbName, atlasAssetName);
        if (atlas != null)
        {
            return atlas.GetSprite(spriteName);
        }
        else
        {
            Debug.LogError($"想要获取的图集为空图集加载错误{atlasAbName}");
        }

        return null;
    }

    /// <summary>
    /// 已知图集中加载Sprite
    /// </summary>
    /// <param name="atlas">图集</param>
    /// <param name="spriteName">sprite名称</param>
    /// <returns></returns>
    public static Sprite GetSpriteByAtlas(SpriteAtlas atlas, string spriteName)
    {
        if (atlas == null) return null;
        return atlas.GetSprite(spriteName);
    }
}
