/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/01/25
 * Note  : 资源加载器
***************************************************************/

using System.IO;
using SG.Utils;
using UnityEngine;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    /// <summary>
    ///   资源加载器
    /// </summary>
    public static class ResourcesManager
    {
        /// <summary>
        ///   资源相对目录
        /// </summary>
        public static readonly string RESOURCES_LOCAL_DIRECTORY = "Assets/Resources/";

        /// <summary>
        /// 资源加载方式，默认采用DefaultLoadPattern
        /// </summary>
        public static ILoadPattern LoadPattern = new DefaultLoadPattern();

        public static string GAME_FOLDER = "";
        public static string SUBGAME_RESOURCES_DIRECTORY = "Assets/Game/" + GAME_FOLDER + "/";
        public static string MAINGAME_RESOURCES_DIRECTORY = "Assets/MainApp/";
        public const string CONTAINER_RESOURCES_DIRECTORY = "Assets/LBLibraryUnityContainerCore/";

        public static void SetGameFolderName(string gameName)
        {
            GAME_FOLDER = gameName.ToLower();
            SUBGAME_RESOURCES_DIRECTORY = "Assets/Game/" + GAME_FOLDER + "/";
        }

        /// <summary>
        ///   加载一个资源
        /// <param name="asset">资源局部路径（"Assets/..."）</param>
        /// </summary>
        public static T Load<T>(string asset, bool isMainApp = false)
            where T : Object
        {
            T result = null;

#if UNITY_EDITOR
            if (LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.EditorAsset
                || LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.All)
            {
                result = ResourcesManager.LoadAssetAtPath<T>(asset);
                if (result != null)
                    return result;

                if (result == null) // 从mainapp中加载，提高资源复用
                {
                    result = ResourcesManager.LoadAssetAtPath<T>(
                        asset.Replace(SUBGAME_RESOURCES_DIRECTORY, MAINGAME_RESOURCES_DIRECTORY));
                    if (result != null)
                    {
                        Debug.LogWarning("Subapp res loading from main: " + asset);
                        return result;
                    }
                }
            }
#endif

            if (LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.AssetBundle
                || LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.All)
            {
                if (asset.Contains(MAINGAME_RESOURCES_DIRECTORY) || isMainApp)
                {
                    result = AssetBundleManager.Instance.LoadAsset<T>(asset);
                    if (result != null)
                        return result;
                    else
                    {
                        DebugUtils.LogWarning("資源加载失败000：》{0}：ismain:{1}", asset, isMainApp);
                    }
                }
                else
                {
                    result = AssetBundleSubGameManager.Instance.LoadAsset<T>(asset);
                    if (result != null)
                        return result;
                    else
                    {
                        DebugUtils.LogWarning( "資源加载失败00：》{0}：ismain:{1}", asset, isMainApp);
                    }
                }

                var mainAsset = asset.Replace(SUBGAME_RESOURCES_DIRECTORY, MAINGAME_RESOURCES_DIRECTORY);
                
                if (result == null && AssetBundleManager.ShouldFallbackFromMain(mainAsset,asset)) // 从mainapp中加载，提高资源复用
                {
                    result = AssetBundleManager.Instance.LoadAsset<T>(mainAsset);
                    if (result != null)
                    {
                        Debug.LogWarning("Subapp res loading from main: " + asset);
                        return result;
                    }
                }
            }

            if (LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.Original
                || LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.All)
            {
                result = ResourcesManager.LoadResources<T>(asset);
                if (result != null)
                    return result;
            }

            return result;
        }

        public static Object Load(System.Type type, string asset, bool isMainApp = false)
        {
            Object result = null;
//            DebugUtils.Log("-->" + asset);
#if UNITY_EDITOR
            if (LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.EditorAsset
                || LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.All)
            {
                result = ResourcesManager.LoadAssetAtPath(type, asset);
                if (result != null)
                    return result;

                if (result == null) // 从mainapp中加载，提高资源复用
                {
                    result = ResourcesManager.LoadAssetAtPath(type,
                        asset.Replace(SUBGAME_RESOURCES_DIRECTORY, MAINGAME_RESOURCES_DIRECTORY));
                    if (result != null)
                    {
                        Debug.LogWarning("Subapp res loading from main: " + asset);
                        return result;
                    }
                }
            }
#endif

            if (LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.AssetBundle
                || LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.All)
            {
                if (asset.Contains(MAINGAME_RESOURCES_DIRECTORY) || isMainApp)
                {
                    result = AssetBundleManager.Instance.LoadAsset(type, asset);
                    if (result != null)
                        return result;
                }
                else
                {
                    result = AssetBundleSubGameManager.Instance.LoadAsset(type, asset);
                    if (result != null)
                    {
                        return result;
                    }
                }


                if (result == null) // 从mainapp中加载，提高资源复用
                {
                    result = AssetBundleManager.Instance.LoadAsset(type,
                        asset.Replace(SUBGAME_RESOURCES_DIRECTORY, MAINGAME_RESOURCES_DIRECTORY));
                    if (result != null)
                    {
                        Debug.LogWarning("Subapp res loading from main: " + asset);
                        return result;
                    }
                }
            }

            if (LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.Original
                || LoadPattern.ResourcesLoadPatternEnum == LoadPatternEnum.All)
            {
                result = ResourcesManager.LoadResources(type, asset);
                if (result != null)
                    return result;
            }

            return result;
        }

        /// <summary>
        ///   加载一个Resources下资源
        /// <param name="asset">资源局部路径（"Assets/..."）</param>
        /// </summary>
        public static T LoadResources<T>(string asset)
            where T : Object
        {
            //去除扩展名
            asset = AssetBundleBrowser.FileHelper.GetPathWithoutExtension(asset);
            //转至以Resources为根目录的相对路径
            asset = AssetBundleBrowser.FileHelper.AbsoluteToRelativePath(RESOURCES_LOCAL_DIRECTORY, asset);
            T a = Resources.Load<T>(asset);
            return a;
        }

        public static Object LoadResources(System.Type type, string asset)

        {
            //去除扩展名
            asset = AssetBundleBrowser.FileHelper.GetPathWithoutExtension(asset);
            //转至以Resources为根目录的相对路径
            asset = AssetBundleBrowser.FileHelper.AbsoluteToRelativePath(RESOURCES_LOCAL_DIRECTORY, asset);
            Object a = Resources.Load(asset, type);
            return a;
        }

        /// <summary>
        ///   文本文件加载
        /// <param name="file_name">全局路径</param>
        /// </summary>
        public static string LoadTextFile(string file_name)
        {
            try
            {
                if (!string.IsNullOrEmpty(file_name))
                {
                    if (File.Exists(file_name))
                        return File.ReadAllText(file_name);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return null;
        }

        /// <summary>
        ///   二进制文件加载
        /// <param name="file_name">全局路径</param>
        /// </summary>
        public static byte[] LoadByteFile(string file_name)
        {
            try
            {
                if (!string.IsNullOrEmpty(file_name))
                {
                    if (File.Exists(file_name))
                        return File.ReadAllBytes(file_name);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        ///   加载一个Resources下资源
        /// <param name="asset">资源局部路径（"Assets/..."）</param>
        /// </summary>
        public static T LoadAssetAtPath<T>(string asset)
            where T : Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(asset);
        }

        public static Object LoadAssetAtPath(System.Type type, string asset)
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath(asset, type);
        }
#endif


        /// <summary>
        /// XLua版无泛型的加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static Object Load(string assetName)
        {
            return Load<Object>(assetName);
        }


        /// <summary>
        /// 加载资源-游戏子目录为根目录
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static Object LoadByRelativePath(string assetName)
        {
            return Load<Object>(SUBGAME_RESOURCES_DIRECTORY + assetName, false);
        }

        public static Object LoadByRelativePath(System.Type systemType, string assetName)
        {
            return Load(systemType, SUBGAME_RESOURCES_DIRECTORY + assetName, false);
        }


        public static Object LoadMainAppResource(string assetName)
        {
            return Load<Object>(MAINGAME_RESOURCES_DIRECTORY + assetName, true);
        }

        public static Object LoadMainAppResource(System.Type systemType, string assetName)
        {
            return Load(systemType, MAINGAME_RESOURCES_DIRECTORY + assetName, true);
        }

        public static Object LoadContainerResource(System.Type systemType, string assetName)
        {
            return Load(systemType, CONTAINER_RESOURCES_DIRECTORY + assetName, true);
        }

        public static Object LoadContainerResource(string assetName)
        {
            return Load<Object>(CONTAINER_RESOURCES_DIRECTORY + assetName, true);
        }
        public static void UnloadAsset(string asset)
        {
            AssetBundleManager.Instance.UnLoadAsset(asset);
        }
        public static void UnLoadAllAsset(string asset)
        {
            AssetBundleManager.Instance.UnLoadAllAsset(asset);
        }
    }
}