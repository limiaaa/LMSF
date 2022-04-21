/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/01/18
 * Note  : AssetBundle资源管理
 *         负责游戏中的AssetBundle资源加载
***************************************************************/

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using SG.Utils;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    /// <summary>
    ///   资源管理器
    /// </summary>
    public class AssetBundleManager : AssetBundleBaseGameManager<AssetBundleManager>
    {
        /// <summary>
        /// 重启
        /// </summary>
        public bool Relaunch()
        {
            //必须处于启动状态或者异常才可以重启
            if (!(IsReady || IsFailed))
                return false;

            Launch();

            return true;
        }

        /// <summary>
        ///   启动(仅内部启用)
        /// </summary>
        void Launch()
        {
            GamePackName = "mainapp";
            if (assetbundleDic == null)
                assetbundleDic = new Dictionary<string, AssetBundle>();
            IsReady = false;
            ErrorCode = emErrorCode.None;
            StopAllCoroutines();
            ClearErrorCache();
#if UNITY_WEBGL
            StartCoroutine(PreloadInitialFiles());
#else
            StartCoroutine(Preprocess());
#endif
        }

        void ClearErrorCache()
        {
            var key = IsCacheClearedKey;
            if (SettingReader.ScriptableObject.IsClearCache && PlayerPrefs.GetInt(key, 0) == 0)
            {
                DebugUtils.Log("Delete cached assetbundle: " + Directory.Exists(Common.PATH));
                if (Directory.Exists(Common.PATH))
                {
                    Directory.Delete(Common.PATH, true);
                }

                /*TODO: Modify
                foreach (var gameName in LocalGameStateInfo.Instance.GetGameList())
                {
                    if (Directory.Exists(Application.temporaryCachePath + "/" + gameName))
                    {
                        Directory.Delete(Application.temporaryCachePath + "/" + gameName, true);
                    }
                }*/

                ABUtils.PlayerPrefsSetInt(key, 1);
            }
        }


        #region MonoBahaviour

        /// <summary>
        ///   
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Launch();
        }

        #endregion
    }
}