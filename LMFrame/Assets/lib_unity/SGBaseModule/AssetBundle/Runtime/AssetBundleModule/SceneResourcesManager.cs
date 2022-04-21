/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/05/05
 * Note  : 场景管理器
***************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using SG.Utils;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    public class SceneResourcesManager
    {
        public static string MAIN_APP_SCENE_PREFIX = "MainApp_";

        /// <summary>
        /// 资源加载方式，默认采用DefaultLoadPattern
        /// </summary>
        public static ILoadPattern LoadPattern = new DefaultLoadPattern();

        /// <summary>
        ///   异步加载场景
        /// </summary>
        public static bool LoadSceneAsync(string scene_name
            , System.Action<string> callback
            , LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation ao;
            return LoadSceneAsync(out ao, scene_name, callback, mode);
        }


#if UNITY_WEBGL
        /// <summary>
        ///   异步加载场景
        /// </summary>
        public static AsyncSceneLoadOperation LoadSceneAsync(string sceneName
            , LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncSceneLoadOperation ao;
            LoadSceneAfterPreloadSync(out ao, sceneName, null, mode);

            return ao;
        }
#else
        /// <summary>
        ///   异步加载场景
        /// </summary>
        public static AsyncOperation LoadSceneAsync(string sceneName
            , LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation ao;
            LoadSceneAsync(out ao, sceneName, null, mode);

            return ao;
        }
#endif

#if UNITY_WEBGL

        private static IEnumerator PreloadSceneAssetBundles(string sceneName)
        {
            yield return new WaitUntilSceneReady(sceneName);
        }

        private static AsyncSceneLoadOperation LoadSceneAfterPreloadSync(out AsyncSceneLoadOperation ao
            , string sceneName
            , System.Action<string> callback = null
            , LoadSceneMode mode = LoadSceneMode.Single)
        {
            ao = new AsyncSceneLoadOperation(sceneName, mode);
            ao.completed += operation =>
            {
                if (callback != null) callback(sceneName);
            };
            return ao;
        }
#endif

        /// <summary>
        ///   异步加载场景
        /// </summary>
        public static bool LoadSceneAsync(out AsyncOperation ao
            , string sceneName
            , System.Action<string> callback = null
            , LoadSceneMode mode = LoadSceneMode.Single)
        {
            ao = null;

#if UNITY_EDITOR
            if (LoadPattern.SceneLoadPatternEnum == LoadPatternEnum.EditorAsset
                || LoadPattern.SceneLoadPatternEnum == LoadPatternEnum.All)
            {
                if (!Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    Debug.LogWarning("Editor Mode.Loading Scene Fail:" + sceneName +
                                     ".Please Add the scene in building setting");
                    DebugUtils.Log("Find assetbundle to load Scene");
                    //return false;
                }
                else
                {
                    ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
                    if (ao != null)
                    {
                        CoroutineExecutor.Create(ao, () =>
                        {
                            if (callback != null) callback(sceneName);
                        });
                        return true;
                    }
                }
            }
#endif

            if (LoadPattern.SceneLoadPatternEnum == LoadPatternEnum.AssetBundle
                || LoadPattern.SceneLoadPatternEnum == LoadPatternEnum.All)
            {
                if (sceneName.Contains(MAIN_APP_SCENE_PREFIX))
                {
                    ao = AssetBundleManager.Instance.LoadSceneAsync(sceneName, mode);
                    if (ao != null)
                    {
                        CoroutineExecutor.Create(ao, () =>
                        {
                            GenerateSceneObject(sceneName);
                            if (callback != null) callback(sceneName);
                        });
                        return true;
                    }
                }
                else
                {
                    ao = AssetBundleSubGameManager.Instance.LoadSceneAsync(sceneName, mode);
                    if (ao != null)
                    {
                        CoroutineExecutor.Create(ao, () =>
                        {
                            GenerateSceneObject(sceneName);
                            if (callback != null) callback(sceneName);
                        });
                        return true;
                    }
                }
            }

            if (LoadPattern.SceneLoadPatternEnum == LoadPatternEnum.Original
                || LoadPattern.SceneLoadPatternEnum == LoadPatternEnum.All)
            {
                if (!Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    Debug.LogError("Editor Mode.Loading Scene Fail:" + sceneName +
                                   ".Please Add the scene in building setting");
                    return false;
                }


                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
                if (ao != null)
                {
                    CoroutineExecutor.Create(ao, () =>
                    {
                        if (callback != null) callback(sceneName);
                    });
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///   读取场景配置文件，生成场景对象
        /// </summary>
        static void GenerateSceneObject(string scene_name)
        {
            //if (!AssetBundleManager.Instance.IsSceneExist(scene_name))
            //    return;
            //var scene_desc = AssetBundleManager.Instance.ResourcesManifest.FindScene(scene_name);
            //if (scene_desc == null)
            //    return;
            //TextAsset text_asset = AssetBundleManager.Instance.LoadAsset<TextAsset>(scene_desc.SceneConfigPath);
            //if (text_asset == null)
            //    return;

            //SceneConfig config = new SceneConfig();
            //config.LoadFromString(text_asset.text);
            //for (int i = 0; i < config.Data.SceneObjects.Count; ++i)
            //{
            //    var obj = config.Data.SceneObjects[i];
            //    var go = ResourcesManager.Load<GameObject>(obj.AssetName);
            //    var parent = GameObject.Find(obj.ParentName);
            //    var instance = GameObject.Instantiate<GameObject>(go);
            //    instance.transform.parent = parent != null ? parent.transform : null;
            //    instance.transform.position = obj.Position;
            //    instance.transform.localScale = obj.Scale;
            //    instance.transform.rotation = obj.Rotation;
            //}
        }
    }
}