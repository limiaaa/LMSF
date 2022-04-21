﻿/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/03/14
 * Note  : AssetBundle包下载器
***************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SG.Utils;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    public class GameDownloader : MonoBehaviour
    {
        /// <summary>
        ///   状态
        /// </summary>
        public enum emState
        {
            None, // 无
            VerifyURL, // 验证有效的URL
            DownloadManifeset, // 下载配置文件
            DownloadAssetBundle, // 下载AssetBundle
            CopyCacheFile, // 复制缓存下的文件
            Dispose, // 后备工作
            Completed, // 完成
            Failed, // 失败
            Cancel, // 取消
            Abort, // 中断

            Max
        }


        internal System.Action<GameDownloader> OnUpdate;


        internal System.Action<GameDownloader> OnDone;


        internal System.Action<GameDownloader, emErrorCode, string> OnError;

        /// <summary>
        ///   是否结束
        /// </summary>
        public bool IsDone { get; private set; }

        public bool IsDownload { get; private set; }

        /// <summary>
        ///   是否出错
        /// </summary>
        public bool IsFailed
        {
            get { return ErrorCode != emErrorCode.None; }
        }

        /// <summary>
        ///   错误代码
        /// </summary>
        public emErrorCode ErrorCode { get; private set; }

        /// <summary>
        ///   当前状态
        /// </summary>
        public emState CurrentState { get; private set; }

        /// <summary>
        ///   当前状态的完成度
        /// </summary>
        public float CurrentStateCompleteValue { get; private set; }

        /// <summary>
        ///   当前状态的总需完成度
        /// </summary>
        public float CurrentStateTotalValue { get; private set; }

        public float CurrentStateProgress
        {
            get
            {
                if (Math.Abs(CurrentStateTotalValue) < Mathf.Epsilon)
                {
                    return 0;
                }
                // Debug.LogError(CurrentStateCompleteValue+"   ->   " + CurrentStateTotalValue);
                return CurrentStateCompleteValue / CurrentStateTotalValue;
            }
        }

        public float CurrentProgress
        {
            get
            {
                switch (CurrentState)
                {
                    case emState.None:
                    case emState.VerifyURL:
                    case emState.DownloadManifeset:
                    case emState.Failed:
                    case emState.Cancel:
                    case emState.Abort:
                    case emState.Max:
                        return 0;
                    case emState.DownloadAssetBundle:
                        return CurrentStateProgress * .8f;
                        break;
                    case emState.CopyCacheFile:
                        return .8f + (CurrentStateCompleteValue / CurrentStateTotalValue) * .1f;
                        break;
                    case emState.Dispose:
                        return .9f + (CurrentStateCompleteValue / CurrentStateTotalValue) * .05f;
                        break;
                    case emState.Completed:
                        return 1;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 下载地址列表
        /// </summary>
        private List<string> url_group_;

        /// <summary>
        ///   可用的URL
        /// </summary>
        private string current_url_;

        /// <summary>
        ///   资源包名
        /// </summary>
        public string GamePackName;

        public long TotalSize = 0; // byte

        /// <summary>
        ///   
        /// </summary>
        private URLVerifier verifier_;

        /// <summary>
        ///   资源下载器
        /// </summary>
        private AssetBundleDownloader ab_download_;

        /// <summary>
        ///   文件下载器
        /// </summary>
        private WebRequestDownload file_download_;


        private string DOWNLOADCACHE_FILE_PATH = "";

        /// <summary>
        ///   开始下载
        /// </summary>
        internal bool StartDownload(List<string> url_group, string gamePackName)
        {
//            if (!AssetBundleManager.Instance.IsReady)
//                return false;
            if (!IsDone && CurrentState != emState.None)
                return false;

            Reset();

            url_group_ = url_group;
            GamePackName = gamePackName;
            DOWNLOADCACHE_FILE_PATH = Common.CACHE_PATH + "/" + GamePackName.ToLower() + "/DownloadCache.cfg";
            IsDownload = true;
            StartCoroutine(Downloading());

            return true;
        }

        internal bool StartCheckDownloadSize(List<string> url_group, string gamePackName)
        {
//            if (!AssetBundleManager.Instance.IsReady)
//                return false;
            if (!IsDone && CurrentState != emState.None)
                return false;

            Reset();

            url_group_ = url_group;
            GamePackName = gamePackName;
            DOWNLOADCACHE_FILE_PATH = Common.CACHE_PATH + "/" + GamePackName.ToLower() + "/DownloadCache.cfg";
            IsDownload = false;
            StartCoroutine(CheckDownloadingSize());
            return true;
        }

        /// <summary>
        ///   取消下载
        /// </summary>
        internal void CancelDownload()
        {
            StopAllCoroutines();

            if (verifier_ != null)
            {
                verifier_.Abort();
                verifier_ = null;
            }

            if (ab_download_ != null)
            {
                ab_download_.Cancel();
                ab_download_ = null;
            }

            UpdateState(emState.Cancel);
            Done();
        }

        /// <summary>
        ///   中止下载
        /// </summary>
        internal void AbortDownload()
        {
            StopAllCoroutines();

            if (verifier_ != null)
            {
                verifier_.Abort();
                verifier_ = null;
            }

            if (ab_download_ != null)
            {
                ab_download_.Abort();
                ab_download_ = null;
            }

            UpdateState(emState.Abort);
            Done();
        }

        IEnumerator Downloading()
        {
            UpdateState(emState.VerifyURL);
            yield return StartVerifyURL();
            UpdateState(emState.DownloadManifeset);
            yield return StartDownloadMainConfig();
            UpdateState(emState.DownloadAssetBundle);
            yield return StartDownloadPack();
            UpdateState(emState.CopyCacheFile);
            yield return StartCopyCacheFile();
            //TODO: 已解决，在AssetBundleSubGameManager初始化的时候已经做了操作
//            yield return UnbundleStoreFile();
            UpdateState(emState.Dispose);
            yield return StartDispose();
            UpdateState(ErrorCode == emErrorCode.None ? emState.Completed : emState.Failed);

            Done();
        }

        IEnumerator CheckDownloadingSize()
        {
            UpdateState(emState.VerifyURL);
            yield return StartVerifyURL();
            UpdateState(emState.DownloadManifeset);
            yield return StartDownloadMainConfig();
            yield return GetTotalUpdateSize();
            UpdateState(emState.Dispose);
            yield return StartDispose();
            UpdateState(ErrorCode == emErrorCode.None ? emState.Completed : emState.Failed);

            Done();
        }

        IEnumerator StartVerifyURL()
        {
            if (ErrorCode != emErrorCode.None)
                yield break;

            UpdateCompleteValue(0f, 1f);

            if (Common.GetAssetBundleDownloadURLBySubGameFunc == null)
            {
                ErrorCode = emErrorCode.PreprocessError;
                yield break;
            }


            //下载地址重定向为根文件夹
            for (int i = 0; i < url_group_.Count; ++i)
                url_group_[i] = Common.GetAssetBundleDownloadURLBySubGameFunc(url_group_[i], GamePackName);

            //找到合适的资源服务器
            verifier_ = new URLVerifier(url_group_);
            verifier_.Start();
            while (!verifier_.IsDone)
            {
                yield return null;
            }

            current_url_ = verifier_.URL;

            if (string.IsNullOrEmpty(current_url_))
            {
                Error(emErrorCode.InvalidURL, "Empty URL");
                yield break;
            }

            verifier_ = null;
            UpdateCompleteValue(1f, 1f);
        }

        #region DownloadMainFile

        /// <summary>
        ///   开始进行主要文件下载,下载至缓存目录
        /// </summary>
        IEnumerator StartDownloadMainConfig()
        {
            if (ErrorCode != emErrorCode.None)
                yield break;

            string[] MAIN_CONFIG_NAME_ARRAY =
            {
                //"AssetBundle.manifest",
                "ResourcesManifest.cfg",
                "AssetBundle"
            };

            var rootPath = Common.CACHE_PATH + "/" + GamePackName.ToLower();
            //下载主配置文件
            for (int i = 0; i < MAIN_CONFIG_NAME_ARRAY.Length; ++i)
            {
                file_download_ =
                    new WebRequestDownload(current_url_);
                yield return file_download_.StartYield(rootPath, MAIN_CONFIG_NAME_ARRAY[i], 0);

                if (file_download_.errorCode != WebRequestDownload.ErrorCodeEnum.None)
                {
                    Error(emErrorCode.DownloadMainConfigFileFailed
                        , current_url_ + MAIN_CONFIG_NAME_ARRAY[i] + " download failed!");
                    yield break;
                }

                file_download_ = null;
                UpdateCompleteValue(i, MAIN_CONFIG_NAME_ARRAY.Length);
            }

            yield return null;
        }

        #endregion


        /// <summary>
        ///   下载包资源
        /// </summary>
        IEnumerator StartDownloadPack()
        {
            if (ErrorCode != emErrorCode.None)
                yield break;

            UpdateCompleteValue(0f, 0f);

            if (string.IsNullOrEmpty(GamePackName))
            {
                Error(emErrorCode.InvalidPackageName);
                yield break;
            }

            ResourcesManifest old_resource_manifest = Common.LoadResourcesManifest(GamePackName);
            string file = Common.GetCacheFileFullName(Common.RESOURCES_MANIFEST_FILE_NAME, GamePackName);
            ResourcesManifest new_resources_manifest = Common.LoadResourcesManifestByPath(file);
            if (new_resources_manifest == null)
            {
                Error(emErrorCode.LoadNewResourcesManiFestFailed
                    , "Can't load new verion ResourcesManifest!");
                yield break;
            }

            //载入MainManifest
            AssetBundleManifest manifest = Common.LoadMainManifest(GamePackName);
            file = Common.GetCacheFileFullName(Common.MAIN_MANIFEST_FILE_NAME, GamePackName);
            AssetBundleManifest new_manifest = Common.LoadMainManifestByPath(file);
            if (new_manifest == null)
            {
                Error(emErrorCode.LoadNewMainManifestFailed
                    , "Can't find new version MainManifest!");
                yield break;
            }

            //获取需下载的资源列表与删除的资源的列表
            List<string> download_files = new List<string>();
            List<string> delete_files = new List<string>();


            if (old_resource_manifest == null || old_resource_manifest.Data.AssetTable == null || manifest == null)
            {
                download_files.AddRange(new_manifest.GetAllAssetBundles());
                download_files.RemoveAll((assetbundle_name) =>
                {
                    return File.Exists(Common.GetCacheFileFullName(assetbundle_name, GamePackName));
                });
            }
            else
            {
                CompareAssetBundleDifference(ref download_files, ref delete_files
                    , manifest, new_manifest
                    , old_resource_manifest, new_resources_manifest);
            }

            //删除已废弃的文件
            if (delete_files.Count > 0)
            {
                for (int i = 0; i < delete_files.Count; ++i)
                {
                    string full_name = Common.GetFileFullName(delete_files[i], GamePackName);
                    if (File.Exists(full_name))
                    {
                        File.Delete(full_name);
                        yield return 0;
                    }
                }
            }

            //////////////////////////////////////
            string mainapp = "mainapp";
            if (GamePackName != mainapp)
            {
                download_files.RemoveAll((assetbundle_name) => { return assetbundle_name.Contains(mainapp); });
            }
            ////////////////////////////////////

            if (download_files.Count <= 0)
            {
                DebugUtils.Log("local files are right, no need to download again");
                yield break;
            }

            DebugUtils.Log("--> download_files count: " + download_files.Count);

            //更新所有需下载的资源
            ab_download_ = new AssetBundleDownloader(current_url_);
            ab_download_.Start(Common.PATH + "/" + GamePackName.ToLower(), download_files, new_resources_manifest);
            while (!ab_download_.IsDone)
            {
                UpdateCompleteValue(ab_download_.CompletedSize, ab_download_.TotalSize);
                yield return 0;
            }

            if (ab_download_.IsFailed)
            {
                Error(emErrorCode.DownloadAssetBundleFailed, ab_download_.URL + " failed!");
                yield break;
            }

            yield return null;
        }

        IEnumerator GetTotalUpdateSize()
        {
            if (ErrorCode != emErrorCode.None)
                yield break;

            UpdateCompleteValue(0f, 0f);

            //载入新的ResourcesManifest
            ResourcesManifest old_resource_manifest = Common.LoadResourcesManifest(GamePackName);
            string file = Common.GetCacheFileFullName(Common.RESOURCES_MANIFEST_FILE_NAME, GamePackName);
            ResourcesManifest new_resources_manifest = Common.LoadResourcesManifestByPath(file);
            if (new_resources_manifest == null)
            {
                Error(emErrorCode.LoadNewResourcesManiFestFailed
                    , "Can't load new verion ResourcesManifest!");
                yield break;
            }

            //载入MainManifest
            AssetBundleManifest manifest = Common.LoadMainManifest(GamePackName);
            file = Common.GetCacheFileFullName(Common.MAIN_MANIFEST_FILE_NAME, GamePackName);
            AssetBundleManifest new_manifest = Common.LoadMainManifestByPath(file);
            if (new_manifest == null)
            {
                Error(emErrorCode.LoadNewMainManifestFailed
                    , "Can't find new version MainManifest!");
                yield break;
            }

            //获取需下载的资源列表与删除的资源的列表
            List<string> download_files = new List<string>();
            List<string> delete_files = new List<string>();
            if (old_resource_manifest.Data.AssetTable == null || manifest == null)
            {
                download_files.AddRange(new_manifest.GetAllAssetBundles());
                download_files.RemoveAll((assetbundle_name) =>
                {
                    return File.Exists(Common.GetCacheFileFullName(assetbundle_name, GamePackName));
                });
            }
            else
            {
                CompareAssetBundleDifference(ref download_files, ref delete_files
                    , manifest, new_manifest
                    , old_resource_manifest, new_resources_manifest);
            }

            TotalSize = 0;
            for (int i = 0; i < download_files.Count; ++i)
            {
                var ab = new_resources_manifest.Find(download_files[i]);
                if (ab != null)
                {
                    if (ab.IsCompress)
                        TotalSize += ab.CompressSize;
                    else
                        TotalSize += ab.Size;
                }
            }

            DebugUtils.Log("GetTotalDownloadSize:" + TotalSize);
            yield return null;
        }

        #region CopyCacheFile

        /// <summary>
        ///   拷贝文件并覆盖旧数据文件
        /// </summary>
        IEnumerator StartCopyCacheFile()
        {
            if (ErrorCode != emErrorCode.None)
                yield break;

            string[] MAIN_CONFIG_NAME_ARRAY =
            {
                //"AssetBundle.manifest",
                "ResourcesManifest.cfg",
                "AssetBundle"
            };

            //从缓存中拷贝主配置文件覆盖旧文件
            for (int i = 0; i < MAIN_CONFIG_NAME_ARRAY.Length; ++i)
            {
                string str = Common.GetCacheFileFullName(MAIN_CONFIG_NAME_ARRAY[i], GamePackName);
                string dest = Common.GetFileFullName(MAIN_CONFIG_NAME_ARRAY[i], GamePackName);
                UpdateCompleteValue(i, MAIN_CONFIG_NAME_ARRAY.Length);
                DebugUtils.Log(string.Format("<color=blue>COPY:</color>\n{0}\n=>\n{1}", str, dest));
                yield return Common.StartCopyFile(str, dest);
            }
        }

        #endregion

//        IEnumerator UnbundleStoreFile()
//        {
//            if (ErrorCode != emErrorCode.None)
//                yield break;
//
//            string[] STORE_CONFIG_NAME_ARRAY =
//            {
//                "Store/Chinese/productInfo.plist.txt",
//                "Store/English/productInfo.plist.txt",
//                "Store/HUAWEI/productInfo.plist.txt"
//            };
//
//            AssetBundleSubGameManager.Instance.Relaunch(GamePackName);
//            ResourcesManager.SetGameFolderName(GamePackName);
//            yield return new WaitUntil(AssetBundleSubGameManager.Instance.WaitForLaunch);
//            //从缓存中拷贝主配置文件覆盖旧文件
//            for (int i = 0; i < STORE_CONFIG_NAME_ARRAY.Length; ++i)
//            {
//                var text = ResourcesManager.LoadByRelativePath(STORE_CONFIG_NAME_ARRAY[i]) as TextAsset;
//                if (!text) continue;
//                string dest = Common.GetFileFullName(STORE_CONFIG_NAME_ARRAY[i], GamePackName);
//                dest = FileHelper.GetPathWithoutExtension(dest); // 去掉.txt 变成.plist
//                FileHelper.WriteTextToFile(dest, text.text);
//                UpdateCompleteValue(i, STORE_CONFIG_NAME_ARRAY.Length);
//                yield return null;
//            }
//        }


        #region Dispose

        /// <summary>
        ///   清理
        /// </summary>
        IEnumerator StartDispose()
        {
            UpdateCompleteValue(0f, 1f);

            if (ErrorCode != emErrorCode.None)
            {
                //缓存已下载内容,便于下次继续下载
                Debug.LogError("Cache content as Download Errors Happened:" + ErrorCode);
                SaveDownloadCacheData();
            }
            else
            {
                //删除缓存目录
                if (Directory.Exists(Common.CACHE_PATH + "/" + GamePackName.ToLower()))
                    Directory.Delete(Common.CACHE_PATH + "/" + GamePackName.ToLower(), true);

                //重启AssetBundleManager
                if (AssetBundleSubGameManager.Instance.IsReady)
                {
                    //TODO: 需要更多测试，多任务下载模式下，不需要重启！！！
//                    AssetBundleSubGameManager.Instance.Relaunch(GamePackName);
                }
            }

            UpdateCompleteValue(1f, 1f);
            yield return 0;
        }

        #endregion

        /// <summary>
        ///   写入下载缓存信息，用于断点续传
        /// </summary>
        void SaveDownloadCacheData()
        {
            if (CurrentState < emState.DownloadAssetBundle)
                return;

            if (!Directory.Exists(Common.CACHE_PATH + "/" + GamePackName.ToLower()))
                return;

            //载入新的Manifest
            string new_manifest_name =
                Common.GetCacheFileFullName(Common.MAIN_MANIFEST_FILE_NAME, GamePackName.ToLower());
            AssetBundleManifest new_manifest = Common.LoadMainManifestByPath(new_manifest_name);
            if (new_manifest == null)
                return;

            //先尝试读取旧的缓存信息，再保存现在已经下载的数据
            //PS:由于只有版本完整更新完才会移动Cache目录，且玩家可能多次尝试下载更新，所以必须保留旧的缓存信息
            DownloadCache cache = new DownloadCache();
            cache.Load(DOWNLOADCACHE_FILE_PATH);
            if (ab_download_ != null
                && ab_download_.CompleteDownloads != null
                && ab_download_.CompleteDownloads.Count > 0)
            {
                for (int i = 0; i < ab_download_.CompleteDownloads.Count; ++i)
                {
                    string assetbundle_name = ab_download_.CompleteDownloads[i];
                    Hash128 hash_code = new_manifest.GetAssetBundleHash(assetbundle_name);
                    if (hash_code.isValid && !cache.Data.AssetBundles.ContainsKey(assetbundle_name))
                    {
                        DownloadCacheData.AssetBundle elem = new DownloadCacheData.AssetBundle()
                        {
                            AssetBundleName = assetbundle_name,
                            Hash = hash_code.ToString(),
                        };
                        DebugUtils.Log(cache.Data.AssetBundles.Count + " - Cache Add:" + assetbundle_name);
                        cache.Data.AssetBundles.Add(assetbundle_name, elem);
                    }
                }
            }

            if (cache.HasData())
                cache.Save(DOWNLOADCACHE_FILE_PATH);
        }


        /// <summary>
        /// 重置
        /// </summary>
        internal void Reset()
        {
            IsDone = false;
            IsDownload = false;
            ErrorCode = emErrorCode.None;
            CurrentState = emState.None;
            CurrentStateCompleteValue = 0f;
            CurrentStateTotalValue = 0f;
            current_url_ = "";
        }

        /// <summary>
        ///   结束
        /// </summary>
        void Done()
        {
            IsDone = true;
            OnDoneEvent();
        }

        /// <summary>
        ///   设置状态
        /// </summary>
        void UpdateState(emState state)
        {
            CurrentState = state;
            OnUpdateEvent();
        }

        /// <summary>
        ///   更新完成度
        /// </summary>
        void UpdateCompleteValue(float current)
        {
            UpdateCompleteValue(current, CurrentStateTotalValue);
        }

        /// <summary>
        ///   更新完成度
        /// </summary>
        void UpdateCompleteValue(float current, float total)
        {
            CurrentStateCompleteValue = current;
            CurrentStateTotalValue = total;
            OnUpdateEvent();
        }

        /// <summary>
        ///   更新
        /// </summary>
        void OnUpdateEvent()
        {
            if (OnUpdate != null)
                OnUpdate(this);
        }

        /// <summary>
        ///   结束事件
        /// </summary>
        void OnDoneEvent()
        {
            if (OnDone != null)
                OnDone(this);
        }

        /// <summary>
        ///   错误
        /// </summary>
        void Error(emErrorCode ec, string message = null)
        {
            ErrorCode = ec;

            string ms = string.IsNullOrEmpty(message) ? ErrorCode.ToString() : ErrorCode.ToString() + " - " + message;
            Debug.LogError(ms);

            if (OnError != null)
                OnError(this, ec, message);
        }

        #region MonoBehaviour

        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            Reset();
        }

        #endregion


        /// <summary>
        ///   比较AssetBundle差异，获得下载列表与删除列表
        /// </summary>
        void CompareAssetBundleDifference(ref List<string> download_files
            , ref List<string> delete_files
            , AssetBundleManifest old_manifest
            , AssetBundleManifest new_manifest
            , ResourcesManifest old_resourcesmanifest
            , ResourcesManifest new_resourcesmanifest)
        {
            if (download_files != null)
                download_files.Clear();
            if (delete_files != null)
                delete_files.Clear();

            if (old_manifest == null)
                return;
            if (new_manifest == null)
                return;
            if (new_resourcesmanifest == null)
                return;

            //采用位标记的方式判断资源
            //位标记： 0： 存在旧资源中 1： 存在新资源中 2：本地资源标记
            int old_version_bit = 0x1; // 存在旧资源中
            int new_version_bit = 0x2; // 存在新资源中
            int old_version_native_bit = 0x4; // 旧的本地资源
            int new_version_native_bit = 0x8; // 新的本地资源
            Dictionary<string, int> temp_dic = new Dictionary<string, int>();
            //标记旧资源
            string[] all_assetbundle = old_manifest.GetAllAssetBundles();
            for (int i = 0; i < all_assetbundle.Length; ++i)
            {
                string name = all_assetbundle[i];
                _SetDictionaryBit(ref temp_dic, name, old_version_bit);
            }

            //标记新资源
            string[] new_all_assetbundle = new_manifest.GetAllAssetBundles();
            for (int i = 0; i < new_all_assetbundle.Length; ++i)
            {
                string name = new_all_assetbundle[i];
                _SetDictionaryBit(ref temp_dic, name, new_version_bit);
            }

            //标记旧的本地资源
            if (old_resourcesmanifest.RawData != null && old_resourcesmanifest.RawData.AssetBundles != null)
            {
                var resource_manifest_itr = old_resourcesmanifest.RawData.AssetBundles.GetEnumerator();
                while (resource_manifest_itr.MoveNext())
                {
                    //if (resource_manifest_itr.Current.Value.IsNative)
                    {
                        string name = resource_manifest_itr.Current.Value.AssetBundleName;
                        _SetDictionaryBit(ref temp_dic, name, old_version_native_bit);
                    }
                }
            }

            //标记新的本地资源
            if (new_resourcesmanifest.RawData != null && new_resourcesmanifest.RawData.AssetBundles != null)
            {
                var resource_manifest_itr = new_resourcesmanifest.RawData.AssetBundles.GetEnumerator();
                while (resource_manifest_itr.MoveNext())
                {
                    //if (resource_manifest_itr.Current.Value.IsNative)
                    {
                        string name = resource_manifest_itr.Current.Value.AssetBundleName;
                        _SetDictionaryBit(ref temp_dic, name, new_version_native_bit);
                    }
                }
            }

            //获得对应需操作的文件名， 优先级： both > add > delete
            //both: 第0位与第1位都被标记的
            //delete: 仅第0位被标记的
            //add: 第2位未标记，且第3位被标记的
            int both_bit = old_version_bit | new_version_bit; // 二个版本资源都存在
            List<string> add_files = new List<string>();
            List<string> both_files = new List<string>();
            var itr = temp_dic.GetEnumerator();
            while (itr.MoveNext())
            {
                string name = itr.Current.Key;
                int mask = itr.Current.Value;
                if ((mask & new_version_native_bit) == new_version_native_bit
                    && (mask & old_version_native_bit) == 0)
                    add_files.Add(name);
                else if ((mask & both_bit) == both_bit)
                    both_files.Add(name);
                else if ((mask & old_version_bit) == old_version_bit)
                    delete_files.Add(name);
            }

            itr.Dispose();

            //载入下载缓存数据
            DownloadCache download_cache = new DownloadCache();
            download_cache.Load(Common.DOWNLOADCACHE_FILE_PATH);
            if (!download_cache.HasData())
                download_cache = null;

            //记录需下载的文件
            {
                //加入新增的文件
                download_files.AddRange(add_files);
                //比较所有同时存在的文件，判断哪些需要更新
                for (int i = 0; i < both_files.Count; ++i)
                {
                    string name = both_files[i];
                    string full_name = Common.GetFileFullName(name, GamePackName);
                    if (File.Exists(full_name))
                    {
                        //判断哈希值是否相等
                        string old_hash = old_manifest.GetAssetBundleHash(name).ToString();
                        string new_hash = new_manifest.GetAssetBundleHash(name).ToString();
                        if (old_hash.CompareTo(new_hash) == 0)
                            continue;

                        download_files.Add(name);
                    }
                }

                //过滤缓存中已下载的文件
                if (download_cache != null)
                {
                    var cache_itr = download_cache.Data.AssetBundles.GetEnumerator();
                    while (cache_itr.MoveNext())
                    {
                        DownloadCacheData.AssetBundle elem = cache_itr.Current.Value;
                        string name = elem.AssetBundleName;
                        string full_name = Common.GetFileFullName(name);
                        if (File.Exists(full_name))
                        {
                            string cache_hash = elem.Hash;
                            string new_hash = new_manifest.GetAssetBundleHash(name).ToString();
                            if (!string.IsNullOrEmpty(cache_hash)
                                && cache_hash.CompareTo(new_hash) == 0)
                                download_files.Remove(name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void _SetDictionaryBit(ref Dictionary<string, int> dic, string name, int bit)
        {
            if (!dic.ContainsKey(name))
            {
                dic.Add(name, bit);
            }
            else
            {
                dic[name] |= bit;
            }
        }
    }
}