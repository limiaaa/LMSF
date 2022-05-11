using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SG;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using SG.Utils;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    public class AssetBundleBaseGameManager<T> : MonoSingleton<T> where T : MonoSingleton<T>
    {
        /// <summary>
        /// 子游戏包名(ex:158_candycakeshop)
        /// 可以为 mainapp
        /// </summary>
        public string GamePackName;

        /// <summary>
        ///   最新的资源版本
        /// </summary>
        public uint Version;

        /// <summary>
        ///   是否准备完成
        /// </summary>
        public bool IsReady { get; set; }


        public bool Do_Initial_Copy ;


        public float LoadingProgress = 0.0f;

        /// <summary>
        ///   是否出错
        /// </summary>
        public bool IsFailed
        {
            get { return ErrorCode != emErrorCode.None; }
        }

        /// <summary>
        /// 
        /// </summary>
        public emErrorCode ErrorCode { get; set; }

        /// <summary>
        ///   主AssetBundleMainfest
        /// </summary>
        public AssetBundleManifest MainManifest { get; protected set; }

        /// <summary>
        ///   资源描述数据
        /// </summary>
        public ResourcesManifest ResourcesManifest { get; set; }

        /// <summary>
        ///   资源包数据
        /// </summary>
        public ResourcesPackages ResourcesPackages { get; protected set; }


        /// <summary>
        /// 临时的AssetBundle
        /// </summary>
        protected static Dictionary<string, AssetBundle> assetbundleDic;


        public string ResRoot
        {
            get
            {
#if UNITY_WEBGL
                if (!SettingReader.ScriptableObject.IsLoadFromWeb)
                    return Common.INITIAL_PATH + "/" + GamePackName.ToLower() + "/";
                return SettingReader.ScriptableObject.GetBaseUrl() + "StreamingAssets/" + Common.ROOT_FOLDER_NAME +
                       "/" + GamePackName.ToLower() + "/";
#else
                return Common.PATH + "/" + GamePackName.ToLower() + "/";
#endif
            }
        }

        protected string IsCacheClearedKey
        {
            get { return "IsCleared_" + AppSeting.GetThisAppVerName(); }
        }

        protected string IsCopiedAllInitialFilesKey
        {
            get { return GamePackName + "_CopyAllInitialFiles_IsFinished"; }
        }

        protected string CopiedVersionKey
        {
            get { return "Copied_Version_" + GamePackName; }
        }

        protected virtual IEnumerator Preprocess()
        {
            //创建资源根目录
            FileHelper.ConfirmDirectory(ResRoot);

            //判断主资源文件是否存在，不存在则拷贝备份资源至资源根目录
            Do_Initial_Copy = false;
            ResourcesManifest current = null;
            string full_name = Common.GetFileFullName(Common.RESOURCES_MANIFEST_FILE_NAME, GamePackName.ToLower());
            if (!File.Exists(full_name))
            {
                Do_Initial_Copy = true;
            }
            else
            {
                // 拷贝安装包初始化目录中的ResourcesManifest，并判断是否重新拷贝初始化目录下的所有文件
                string initial_full_name =
                    Common.GetInitialFileFullName(Common.RESOURCES_MANIFEST_FILE_NAME, GamePackName.ToLower());
                string cache_full_name =
                    Common.GetCacheFileFullName(Common.RESOURCES_MANIFEST_FILE_NAME, GamePackName.ToLower());
                yield return Common.StartCopyFile(initial_full_name, cache_full_name);

                //判断安装包初始目录是否完整
                ResourcesManifest initial = Common.LoadResourcesManifestByPath(cache_full_name);
                // if (initial == null || initial.Data.AssetTable == null)
                // {
                //     Error(emErrorCode.PreprocessError
                //         , "Initial path don't contains "
                //             + Common.RESOURCES_MANIFEST_FILE_NAME + "!");
                //     yield break;
                //     Debug.LogError(GamePackName + "  initial: StreamingAssets ResourcesManifest Null");
                // }
                if (initial != null && initial.RawData != null)
                {
                    current = Common.LoadResourcesManifestByPath(full_name);
                    if (current == null)
                    {
                        DebugUtils.Log(GamePackName + "  COPY TRUE: ResourcesManifest Null");
                        Do_Initial_Copy = true;
                    }
                    else if (current.RawData.Version < initial.RawData.Version)
                    {
                        DebugUtils.Log(GamePackName + "  COPY TRUE: Current Version smaller than Resource Version :" +
                                  current.RawData.Version +
                                  "  <  " + initial.RawData.Version);
                        Do_Initial_Copy = true;
                    }
                    else if (current.RawData.Version == initial.RawData.Version &&
                             PlayerPrefs.GetInt(IsCopiedAllInitialFilesKey, 1) == 0)
                    {
                        DebugUtils.Log(GamePackName + "  COPY TRUE: Current Version == Resource Version :" +
                                  current.RawData.Version + "  ==  " +
                                  initial.RawData.Version + "     " + IsCopiedAllInitialFilesKey + ":0");
                        Do_Initial_Copy = true;
                    }
                    else if (SG.SettingReader.ScriptableObject.isSingleGameApp &&
                             AppSeting.GetThisAppVerName() != PlayerPrefs.GetString(CopiedVersionKey))
                    {
                        DebugUtils.Log(GamePackName + "  do_initial_copy ===> Sub true");
                        Do_Initial_Copy = true;
                        SG.SettingReader.ScriptableObject.IsNeedHotFix = true;
                    }
                }


                //删除缓存中的文件
                if (File.Exists(cache_full_name))
                    File.Delete(cache_full_name);
            }

            if (Do_Initial_Copy)
            {
                yield return CopyAllInitialFiles(current);
                current = Common.LoadResourcesManifestByPath(full_name);
            }

            if (!PreprocessFinished(current))
            {
                DebugUtils.LogError(GamePackName + "  assetbundle manager Launch Fail!");
            }

            if (current == null)
            {
                DebugUtils.LogError("Manifest Is Null");
            }
            //TODO:  确认一下
            // if (do_initial_copy || !LIBII.SettingReader.ScriptableObject.IsNeedHotFix)
            // {
            //     yield return UnbundleStoreFile();
            // }
        }

        public IEnumerator ForceCopyAllInitialFiles()
        {
            string full_name = Common.GetFileFullName(Common.RESOURCES_MANIFEST_FILE_NAME, GamePackName.ToLower());
            var current = Common.LoadResourcesManifestByPath(full_name);
            if (current != null)
                yield return CopyAllInitialFiles(current);
        }

        public bool GetIsNeedCopyFiles()
        {
            return Do_Initial_Copy;
        }

        protected virtual IEnumerator CopyAllInitialFiles(ResourcesManifest current)
        {
            DebugUtils.Log(GamePackName + "  Start => CopyAllInitialFiles");
            ABUtils.PlayerPrefsSetInt(IsCopiedAllInitialFilesKey, 0);
            //拷贝所有配置文件
            for (int i = 0; i < Common.MAIN_CONFIG_NAME_ARRAY.Length; ++i)
            {
                yield return Common.StartCopyInitialFile(Common.MAIN_CONFIG_NAME_ARRAY[i], GamePackName);
            }

            //拷贝AssetBundle文件
            if (current == null)
            {
                current = Common.LoadResourcesManifest(GamePackName);
            }

            if (current == null)
            {
                Debug.LogWarning(GamePackName + "  Can't load ResourcesManifest file!");
                yield break;
            }

            if (!SG.SettingReader.ScriptableObject.IsNeedHotFix)
            {
                ABUtils.PlayerPrefsSetInt(IsCopiedAllInitialFilesKey, 1);
                yield break;
            }

            DebugUtils.Log(GamePackName + "   => CopyAllInitialFiles : copy assetbundle");
            var itr = current.RawData.AssetBundles.GetEnumerator();
            float NowProgressValue = 0;
            float TotalProgressValue = current.RawData.AssetBundles.Count;
            while (itr.MoveNext())
            {
                if (itr.Current.Value.AssetBundleName.ToLower().StartsWith(GamePackName.ToLower()))
                    //if (itr.Current.Value.IsNative)
                {
                    DebugUtils.Log(GamePackName + "    => CopyAllInitialFiles : " + itr.Current.Value.AssetBundleName);
                    string assetbundlename = itr.Current.Value.AssetBundleName;
                    string dest = Common.GetFileFullName(assetbundlename, GamePackName);

                    //保证路径存在
                    string directory = Path.GetDirectoryName(dest);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        DebugUtils.Log(GamePackName + "   Delete directory:" + directory);
                        Directory.Delete(directory, true);
                    }

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (!SG.SettingReader.ScriptableObject.IsRealtimeHotFix)
                    {
                        //拷贝数据
                        yield return Common.StartCopyInitialFile(assetbundlename, GamePackName);
                    }
                    else
                    {
                        yield return null;
                    }
                }
                NowProgressValue++;
                LoadingProgress = NowProgressValue/TotalProgressValue;
            }
            itr.Dispose();
            ABUtils.PlayerPrefsSetInt(IsCopiedAllInitialFilesKey, 1);
            ABUtils.PlayerPrefsSetString(CopiedVersionKey, AppSeting.GetThisAppVerName());
            DebugUtils.Log(GamePackName + " => CopyAllInitialFiles End");
            foreach (var item in assetbundleDic)
            {
                DebugUtils.Log("catchbundle:"+ item.Key);
            }
        }
        public float GetNowLoadingValue()
        {
            return LoadingProgress;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        bool PreprocessFinished(ResourcesManifest current)
        {
            //MainManifest
            MainManifest = Common.LoadMainManifest(GamePackName.ToLower());
            if (MainManifest == null)
            {
                Error(emErrorCode.LoadMainManifestFailed
                    , GamePackName + " Can't load MainManifest file!");
                return false;
            }

            ResourcesManifest = current;

            if (ResourcesManifest == null)
                ResourcesManifest = Common.LoadResourcesManifest(GamePackName.ToLower());
            if (ResourcesManifest == null)
            {
                Error(emErrorCode.LoadResourcesManiFestFailed
                    , GamePackName + " Can't load ResourcesInfo file!");
                return false;
            }

            // ResourcesPackages
            ResourcesPackages = Common.LoadResourcesPackages(GamePackName.ToLower());

            //记录当前版本号
            Version = ResourcesManifest.RawData.Version;
            //标记已准备好
            IsReady = ErrorCode == emErrorCode.None;

            DebugUtils.Log(GamePackName + " AssetbundleSubGameManager launch finish:" + GamePackName.ToLower());
            return true;
        }

        IEnumerator UnbundleStoreFile()
        {
            if (ErrorCode != emErrorCode.None)
                yield break;

            string[] STORE_CONFIG_NAME_ARRAY =
            {
                "Store/Chinese/productInfo.plist.txt",
                "Store/English/productInfo.plist.txt",
                "Store/HUAWEI/productInfo.plist.txt"
            };

            //从缓存中拷贝主配置文件覆盖旧文件
            for (int i = 0; i < STORE_CONFIG_NAME_ARRAY.Length; ++i)
            {
                var text = ResourcesManager.LoadByRelativePath(STORE_CONFIG_NAME_ARRAY[i]) as TextAsset;
                if (!text) continue;

                string dest = ResRoot + STORE_CONFIG_NAME_ARRAY[i];
                if (File.Exists(FileHelper.GetPathWithoutExtension(dest)))
                {
                    yield break;
                }

                dest = FileHelper.GetPathWithoutExtension(dest); // 去掉.txt 变成.plist
                FileHelper.WriteTextToFile(dest, text.text);
                yield return null;
            }
        }

        /// <summary>
        /// 等待启动完毕，启动完毕返回True,
        /// </summary>
        public bool WaitForLaunch()
        {
            if (IsReady || IsFailed)
                return true;

            return false;
        }

#if UNITY_WEBGL
        public Dictionary<string, UnityWebRequest> mCacheRequests = new Dictionary<string, UnityWebRequest>();
        [NonSerialized] public Func<bool> OnRetryFailed;
        protected int mRetryCount = 0;
        protected static int MAX_RETRY_COUNT = 5;

        private IEnumerator LoadMainManifest()
        {
            var assetbundleName = "AssetBundle";
            var path = ResRoot + assetbundleName;
            yield return PreloadAssetBundle(assetbundleName, path, ab =>
            {
                if (ab != null)
                {
                    MainManifest = (AssetBundleManifest) ab.LoadAsset("AssetBundleManifest");

                    ab.Unload(false);
                }
            });
        }

        private IEnumerator LoadResourcesManifest()
        {
            var path = ResRoot + "ResourcesManifest.cfg";
            DebugUtils.Log("----> " + path);
            var uri = new Uri(path);
            var request = UnityWebRequest.Get(uri);
            yield return request.SendWebRequest();
            ResourcesManifest = ResourcesManifest.LoadContent(request.downloadHandler.text);
        }


        public IEnumerator PreloadInitialFiles()
        {
            //MainManifest
            yield return LoadMainManifest();
            if (MainManifest == null)
            {
                Debug.LogError("Not Main AssetBundle Loaded");
                yield break;
            }

            yield return LoadResourcesManifest();
            if (ResourcesManifest == null)
            {
                Debug.LogError("Not ResourcesManifest Loaded");
                yield break;
            }

            // ResourcesPackages
            ResourcesPackages = Common.LoadResourcesPackages();

            //记录当前版本号
            Version = ResourcesManifest.RawData.Version;
            //标记已准备好
            IsReady = ErrorCode == emErrorCode.None;
        }

        protected IEnumerator PreloadAssetBundle(string assetbundleName, string path, Action<AssetBundle> onSucceed,
            Action<float> progressAction = null, bool retry = true)
        {
            if (!assetbundleName.Contains(GamePackName.ToLower()) && assetbundleName != "AssetBundle")
            {
                yield break;
            }

            // DebugUtils.Log("PreloadAssetBundle----> " + path);

            if (assetbundleDic.ContainsKey(assetbundleName))
            {
                if (onSucceed != null)
                    onSucceed(assetbundleDic[assetbundleName]);

                if (progressAction != null)
                {
                    progressAction(1);
                }

                yield break;
            }

            UnityWebRequest request = null;
            if (mCacheRequests.ContainsKey(path))
            {
                yield return new WaitUntil(() => mCacheRequests[path].isDone);
            }
            else
            {
                var crc = ResourcesManifest == null ? 0 : ResourcesManifest.GetAssetBundleCrc(assetbundleName);
                request = UnityWebRequestAssetBundle.GetAssetBundle(new Uri(path), crc);
                mCacheRequests[path] = request;
            }

            request.redirectLimit = 5;
            request.timeout = 60;
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                var downloadDataProgress = request.downloadProgress;
                if (progressAction != null)
                {
                    progressAction(downloadDataProgress);
                }

                yield return null;
            }

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error + "\nError Fetch:" + path);
                if (retry)
                {
                    mCacheRequests.Remove(path);
                    if (mRetryCount >= MAX_RETRY_COUNT)
                    {
                        mRetryCount = 0;
                        if (OnRetryFailed != null && !OnRetryFailed())
                        {
                            yield break;
                        }
                    }

                    mRetryCount++;
                    yield return PreloadAssetBundle(assetbundleName, path, onSucceed, progressAction);
                }
                else
                {
                    if (onSucceed != null)
                        onSucceed(null);
                }
            }

            var ab = DownloadHandlerAssetBundle.GetContent(request);
            if (ab != null)
            {
                DebugUtils.Log("PreloadAssetBundle cached ----> \n" + path);
                assetbundleDic[assetbundleName] = ab;
            }

            if (onSucceed != null) onSucceed(ab);
        }


        public IEnumerator PreloadAllAssetBundles()
        {
            var assetBundleNames = MainManifest.GetAllAssetBundles();
            for (int i = 0; i < assetBundleNames.Length; i++)
            {
                var name = assetBundleNames[i];
                if (!name.ToLower().Contains(GamePackName.ToLower()) || assetbundleDic.ContainsKey(name))
                {
                    continue;
                }

                var path = ResRoot + name;
                yield return PreloadAssetBundle(name, path, null);
            }

            yield return null;
        }

        public void PreloadAllAssetBundlesAsync()
        {
            var assetBundleNames = MainManifest.GetAllAssetBundles();
            for (int i = 0; i < assetBundleNames.Length; i++)
            {
                var name = assetBundleNames[i];
                if (!name.ToLower().Contains(GamePackName.ToLower()) || assetbundleDic.ContainsKey(name))
                {
                    continue;
                }

                var path = ResRoot + name;
                StartCoroutine(PreloadAssetBundle(name, path, null));
            }
        }

        public IEnumerator PreloadSceneAssetBundles(string sceneName, Action<float> loadProgressAction)
        {
            string assetbundlesname = FindAssetBundleNameByScene(sceneName);

            if (!string.IsNullOrEmpty(assetbundlesname))
            {
                string[] deps = MainManifest.GetAllDependencies(assetbundlesname);
                yield return PreloadGivenAssetBundles(deps.Concat(new[] {assetbundlesname}).ToArray(),
                    loadProgressAction);
            }

            yield return null;
        }


        /// <summary>
        /// 加载相同优先级的一系列AB包
        /// </summary>
        /// <param name="loadOrder"></param>
        /// <param name="loadProgressAction"></param>
        /// <returns></returns>
        public IEnumerator PreloadSameLoadOrderAssetBundles(int loadOrder = 0, bool isGeneral = true,
            Action<float> loadProgressAction = null)
        {
            string[] assetbundlenames = ResourcesManifest.GetSameLoadOrderAssetBundles(loadOrder, isGeneral);
            var appbundles = assetbundlenames.Where(t => t.Contains(GamePackName.ToLower())).ToArray();
            if (appbundles.IsNotEmpty())
            {
                yield return PreloadGivenAssetBundles(appbundles, loadProgressAction);
            }

            yield return null;
        }

        public IEnumerator PreloadSameLoadOrderSceneAssetBunles(int loadOrder = 0, bool isGeneral = false,
            Action<float> loadProgressAction = null)
        {
            string[] assetbundlenames = ResourcesManifest.GetSameLoadOrderAssetBundles(loadOrder, isGeneral);
            var appbundles = assetbundlenames.Where(t => t.Contains(GamePackName.ToLower())).ToArray();
            if (appbundles.IsNotEmpty())
            {
                yield return PreloadGivenAssetBundles(appbundles, loadProgressAction);
            }

            yield return null;
        }

        protected IEnumerator PreloadGivenAssetBundles(string[] assetbundles, Action<float> loadProgressAction = null)
        {
            float wholeProgress = 0;
            Action<float> progressAction = progress =>
            {
                wholeProgress += progress / (assetbundles.Length);
                if (loadProgressAction != null)
                    loadProgressAction(wholeProgress);
            };


            foreach (string dep in assetbundles)
            {
                var path = ResRoot + dep;
                yield return PreloadAssetBundle(dep, path, null, progressAction);
            }
        }
#endif


        public bool HasAsset(string asset)
        {
            if (!IsReady || string.IsNullOrEmpty(GamePackName))
                return false;

            asset = asset.ToLower();
            string[] assetbundlesname = FindAllAssetBundleNameByAsset(asset);
            if (assetbundlesname != null)
            {
                for (int i = 0; i < assetbundlesname.Length; ++i)
                {
                    if (assetbundlesname[i].Contains(GamePackName.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool ShouldFallbackFromMain(string mainAsset, string asset)
        {
            return AssetBundleManager.Instance.HasAsset(mainAsset) &&
                   !AssetBundleSubGameManager.Instance.HasAsset(asset);
        }

        /// <summary>
        ///   加载一个资源
        /// </summary>
        public T LoadAsset<T>(string asset)
            where T : Object
        {
            try
            {
                if (!IsReady)
                    return null;

                asset = asset.ToLower();

                T result = null;
                string[] assetbundlesname = FindAllAssetBundleNameByAsset(asset);
                if (assetbundlesname != null)
                {
                    for (int i = 0; i < assetbundlesname.Length; ++i)
                    {
                        AssetBundle ab = LoadAssetBundleAndDependencies(assetbundlesname[i]);
                        if (ab != null)
                        {
                            result = ab.LoadAsset<T>(asset);
                            break;
                        }
                        else
                        {
                            
                        }
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleSubGameManager.LoadAsset is failed!\n" + ex.Message);
            }

            return null;
        }

        public Object LoadAsset(System.Type type, string asset)
        {
            try
            {
                if (!IsReady)
                    return null;

                asset = asset.ToLower();

                Object result = null;
                string[] assetbundlesname = FindAllAssetBundleNameByAsset(asset);
                if (assetbundlesname != null)
                {
                    for (int i = 0; i < assetbundlesname.Length; ++i)
                    {
                        AssetBundle ab = LoadAssetBundleAndDependencies(assetbundlesname[i]);
                        if (ab != null)
                        {
                            result = ab.LoadAsset(asset, type);
                            break;
                        }
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleSubGameManager.LoadAsset is falid!  " + ex.Message + "\n" + ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        ///   异步加载一个资源
        /// </summary>
        public AssetBundleRequest LoadAssetAsync<T>(string asset)
            where T : Object
        {
            try
            {
                if (!IsReady)
                    return null;

                asset = asset.ToLower();

                AssetBundleRequest result = null;
                string[] assetbundlesname = FindAllAssetBundleNameByAsset(asset);
                if (assetbundlesname != null)
                {
                    for (int i = 0; i < assetbundlesname.Length; ++i)
                    {
                        AssetBundle ab = LoadAssetBundleAndDependencies(assetbundlesname[i]);
                        if (ab != null)
                        {
                            result = ab.LoadAssetAsync<T>(asset);
                            break;
                        }
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleSubGameManager.LoadAsset is falid!\n" + ex.Message);
            }

            return null;
        }

        public AssetBundleRequest LoadAssetAsync(System.Type type, string asset)
        {
            try
            {
                if (!IsReady)
                    return null;

                asset = asset.ToLower();

                AssetBundleRequest result = null;
                string[] assetbundlesname = FindAllAssetBundleNameByAsset(asset);
                if (assetbundlesname != null)
                {
                    for (int i = 0; i < assetbundlesname.Length; ++i)
                    {
                        AssetBundle ab = LoadAssetBundleAndDependencies(assetbundlesname[i]);
                        if (ab != null)
                        {
                            result = ab.LoadAssetAsync(asset, type);
                            break;
                        }
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleSubGameManager.LoadAsset is falid!\n" + ex.Message);
            }

            return null;
        }

        public bool CheckIsSceneReady(string sceneName)
        {
            string assetbundlesname = FindAssetBundleNameByScene(sceneName);
            if (!string.IsNullOrEmpty(assetbundlesname))
            {
                AssetBundle ab = LoadAssetBundleAndDependencies(assetbundlesname, false);
                if (ab == null)
                {
                    Debug.LogWarning(
                        "AssetBundleManager.LoadScene() - Can't Load AssetBundle(" + assetbundlesname + ")");
                    return false;
                }

                return true;
            }

            return false;
        }


        /// <summary>
        ///   异步加载场景
        /// </summary>
        public AsyncOperation LoadSceneAsync(string scene_name
            , LoadSceneMode mode = LoadSceneMode.Single)
        {
            try
            {
                if (!IsReady)
                    return null;

                string assetbundlesname = FindAssetBundleNameByScene(scene_name);
                if (!string.IsNullOrEmpty(assetbundlesname))
                {
                    AssetBundle ab = LoadAssetBundleAndDependencies(assetbundlesname);
                    if (ab == null)
                    {
                        Debug.LogWarning("AssetBundleManager.LoadScene() - Can't Load AssetBundle(" + assetbundlesname +
                                         ")");
                        return null;
                    }

                    AsyncOperation result = SceneManager.LoadSceneAsync(scene_name, mode);

                    return result;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleManager.LoadAsset is falid!\n" + ex.Message);
            }

            return null;
        }

        /// <summary>
        ///   判断一个AssetBundle是否存在缓存
        /// </summary>
        public bool IsExist(string assetbundlename)
        {
            if (!IsReady)
                return false;

            if (string.IsNullOrEmpty(assetbundlename))
                return false;

            return File.Exists(Common.GetFileFullName(assetbundlename));
        }

        /// <summary>
        ///   判断一个资源是否存在于AssetBundle中
        /// </summary>
        public bool IsAssetExist(string asset)
        {
            if (!IsReady)
                return false;

            string[] assetbundlesname = FindAllAssetBundleNameByAsset(asset);
            if (assetbundlesname != null)
            {
                for (int i = 0; i < assetbundlesname.Length; ++i)
                    if (IsExist(assetbundlesname[i]))
                        return true;
            }

            return false;
        }

        /// <summary>
        ///   判断场景是否存在于AssetBundle中
        /// </summary>
        public bool IsSceneExist(string scene_name)
        {
            return !string.IsNullOrEmpty(FindAssetBundleNameByScene(scene_name));
        }

        /// <summary>
        ///   获得AssetBundle中的所有资源
        /// </summary>
        public string[] FindAllAssetNames(string assetbundlename)
        {
            AssetBundle bundle = LoadAssetBundle(assetbundlename);
            if (bundle != null)
                return bundle.GetAllAssetNames();
            return null;
        }

        /// <summary>
        ///   获得包含某个资源的所有AssetBundle
        /// </summary>
        public string[] FindAllAssetBundleNameByAsset(string asset)
        {
            if (!IsReady)
            {
                Debug.LogError(this.name + " not ready, can not load " + asset);
                return null;
            }

            if (ResourcesManifest == null)
            {
                Debug.LogErrorFormat("ResourceManifest is null, can not load {0}", asset);
                return null;
            }

            return ResourcesManifest.GetAllAssetBundleName(asset);
        }

        /// <summary>
        ///   获得一个场景的包名
        /// </summary>
        public string FindAssetBundleNameByScene(string scene_name)
        {
            if (!IsReady)
            {
                Debug.LogError(Instance.GetType().Name + " is not ready");
                return null;
            }

            if (ResourcesManifest == null)
            {
                Debug.LogError(Instance.GetType().Name + " ResourcesManifest is null");
                return null;
            }

            return ResourcesManifest.GetAssetBundleNameBySceneLevelName(scene_name);
        }

        /// <summary>
        ///   获得指定资源包的AssetBundle列表
        /// </summary>
        public List<string> FindAllAssetBundleFilesNameByPackage(string package_name)
        {
            if (!IsReady)
                return null;

            if (ResourcesPackages == null)
                return null;

            ResourcesPackagesData.Package pack = ResourcesPackages.Find(package_name);
            if (pack == null)
                return null;

            List<string> result = new List<string>();
            for (int i = 0; i < pack.AssetList.Count; ++i)
            {
                string[] assetbundlename = FindAllAssetBundleNameByAsset(pack.AssetList[i]);
                if (assetbundlename != null && assetbundlename.Length > 0)
                {
                    if (!string.IsNullOrEmpty(assetbundlename[0]))
                    {
                        if (!result.Contains(assetbundlename[0]))
                        {
                            result.Add(assetbundlename[0]);
                        }
                    }
                }
            }

            // scene ?
            //for (int i = 0; i < pack.AssetList.Count; ++i)
            {
                string assetbundlename = FindAssetBundleNameByScene("Game002_TestScene");
                if (assetbundlename != null && assetbundlename.Length > 0)
                {
                    if (!string.IsNullOrEmpty(assetbundlename))
                    {
                        if (!result.Contains(assetbundlename))
                        {
                            result.Add(assetbundlename);
                        }
                    }
                }
            }
            {
                string assetbundlename = FindAssetBundleNameByScene("Game001_TestScene");
                if (assetbundlename != null && assetbundlename.Length > 0)
                {
                    if (!string.IsNullOrEmpty(assetbundlename))
                    {
                        if (!result.Contains(assetbundlename))
                        {
                            result.Add(assetbundlename);
                        }
                    }
                }
            }
            return result.Count > 0 ? result : null;
        }


        /// <summary>
        ///   加载有依赖的AssetBundle
        /// </summary>
        protected AssetBundle LoadAssetBundleAndDependencies(string assetbundlename, bool isLogError = true)
        {
            if (string.IsNullOrEmpty(assetbundlename))
            {
                Debug.LogError("assetbundlename is null!");
                return null;
            }

            if (MainManifest == null)
            {
                Debug.LogError("Main manifest is null!");
                return null;
            }

            string[] deps = MainManifest.GetAllDependencies(assetbundlename);
            for (int index = 0; index < deps.Length; index++)
            {
                //加载所有的依赖AssetBundle
                if (AssetBundleSubGameManager.Instance.LoadAssetBundle(deps[index]) == null &&
                    AssetBundleManager.Instance.LoadAssetBundle(deps[index]) == null)
                {
                    if (isLogError)
                        Debug.LogError(assetbundlename + "'s Dependencie AssetBundle can't find. Name is " +
                                       deps[index] +
                                       "!");

                    return null;
                }
            }

            var ab = AssetBundleSubGameManager.Instance.LoadAssetBundle(assetbundlename);
            if (ab == null)
            {
                ab = AssetBundleManager.Instance.LoadAssetBundle(assetbundlename);
            }

            return ab;
        }

        protected virtual AssetBundle LoadAssetBundle(string assetBundleName)
        {
            return LoadAssetBundle(assetBundleName, "mainapp");
        }

        protected IEnumerator LoadAssetBundleAsync(string assetbundlename, string subapp,
            Action<AssetBundle> onFinished)
        {
            if (assetbundlename == null)
            {
                onFinished(null);
                yield break;
            }

            if (MainManifest == null)
            {
                onFinished(null);
                yield break;
            }

            AssetBundle ab = null;

            if (assetbundleDic.ContainsKey(assetbundlename))
            {
                ab = assetbundleDic[assetbundlename];
            }

            if (ab == null)
            {
                if (!assetbundlename.ToLower().Contains(subapp.ToLower()))
                {
                    onFinished(null);
                    yield break;
                }

                string url =
                    (SG.SettingReader.ScriptableObject.IsNeedHotFix ? Common.PATH : Common.INITIAL_PATH) + "/" +
                    subapp.ToLower() + "/" + assetbundlename;
                var crc = ResourcesManifest.GetAssetBundleCrc(url.Split('/').Last());
#if UNITY_2017
                using (UnityWebRequest request = UnityWebRequest.GetAssetBundle(url, crc))
#else
                using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, crc))
#endif
                {
                    yield return request.SendWebRequest();
                }
            }

            onFinished(ab);
        }


        /// <summary>
        ///   加载AssetBundle
        /// </summary>
        protected AssetBundle LoadAssetBundle(string assetbundlename, string subapp)
        {
            if (assetbundlename == null)
                return null;
            if (MainManifest == null)
                return null;

            AssetBundle ab = null;

            if (assetbundleDic.ContainsKey(assetbundlename))
            {
                ab = assetbundleDic[assetbundlename];
            }
            else
            {
#if UNITY_WEBGL
                return null;
#endif

                if (!assetbundlename.ToLower().Contains(subapp.ToLower()))
                {
                    return null;
                }

                string assetbundle_path =
                    (SG.SettingReader.ScriptableObject.IsNeedHotFix ? Common.PATH : Common.INITIAL_PATH) + "/" +
                    subapp.ToLower() + "/" + assetbundlename;
                DebugUtils.Log("-->" + assetbundle_path);
                if (System.IO.File.Exists(assetbundle_path))
                {
                    ab = AssetBundle.LoadFromFile(assetbundle_path);
                }
                else
                {
                    var streamingFile = Common.INITIAL_PATH + "/" + subapp.ToLower() + "/" + assetbundlename;
                    var persistentFile = Common.PATH + "/" + subapp.ToLower() + "/" + assetbundlename;
                    FileHelper.SyncCopyStreamingAssetsToFile(streamingFile, persistentFile);
                    if (File.Exists(persistentFile))
                    {
                        ab = AssetBundle.LoadFromFile(persistentFile);
                    }

//                    Debug.LogWarning("AssetBundle does NOT exist at: " + assetbundle_path);
                }

                if (ab)
                {
                    // LZ4 压缩方式不需要释放
                    assetbundleDic.Add(assetbundlename, ab);
                }
            }

            return ab;
        }

        public void UnLoadAsset(string asset)
        {
            asset = asset.ToLower();
            if(assetbundleDic.ContainsKey(asset))
            {
                AssetBundle ab = assetbundleDic[asset];
                if (ab != null)
                {
                    ab.Unload(true);
                    assetbundleDic.Remove(asset);
                }
            }
        }

        public void UnLoadAllAsset(string asset)
        {
            asset = asset.ToLower();
            string[] assetbundlesname = FindAllAssetBundleNameByAsset(asset);
            if (assetbundlesname != null)
            {
                for (int i = 0; i < assetbundlesname.Length; ++i)
                {
                    UnLoadAsset(assetbundlesname[i]);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected void Error(emErrorCode ec, string message = null)
        {
            ErrorCode = ec;

            StringBuilder sb = new StringBuilder("[AssetBundleError] - ");
            sb.Append(ErrorCode.ToString());
            if (!string.IsNullOrEmpty(message))
            {
                sb.Append("\n");
                sb.Append(message);
            }

            Debug.LogWarning(sb.ToString());
        }
    }
}