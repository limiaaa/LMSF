/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/01/18
 * Note  : AssetBundle资源管理
 *         负责游戏中的AssetBundle资源加载
***************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    /// <summary>
    ///   资源管理器
    /// </summary>
    public class AssetBundleSubGameManager : AssetBundleBaseGameManager<AssetBundleSubGameManager>
    {

        /// <summary>
        /// 重启
        /// </summary>
        public bool Relaunch(string gamePackName)
        {
            if (!IsReady)
            {
                Launch(gamePackName);
                return true;
            }

            //必须处于启动状态且不为同一个package或者异常才可以重启
            if (!(IsReady && (GamePackName != gamePackName)) || IsFailed)
                return false;

            Launch(gamePackName);

            return true;
        }


        /// <summary>
        ///   启动(仅内部启用)
        /// </summary>
        public void Launch(string gamePackName)
        {
            GamePackName = gamePackName.ToLower();
            if (assetbundleDic == null)
                assetbundleDic = new Dictionary<string, AssetBundle>();
            if (assetbundleDic!=null && assetbundleDic.Count>0)
            {
                foreach (var assetBundle in assetbundleDic)
                {
                    assetBundle.Value.Unload(false);
                }
                assetbundleDic.Clear();
            }
            IsReady = false;
            ErrorCode = emErrorCode.None;
            StopAllCoroutines();
#if UNITY_WEBGL
            StartCoroutine(PreloadInitialFiles());
#else
            StartCoroutine(Preprocess());
#endif
        }

        protected override AssetBundle LoadAssetBundle(string assetbundlename)
        {
            if (!string.IsNullOrEmpty(GamePackName))
                return base.LoadAssetBundle(assetbundlename, GamePackName.ToLower());
            else
            {
                return null;
            }
        }
        
 


        #region MonoBahaviour

        /// <summary>
        ///   
        /// </summary>
        // protected override void Awake()
        // {
        //     base.Awake();
        //     //Launch();
        // }

        #endregion
    }
}