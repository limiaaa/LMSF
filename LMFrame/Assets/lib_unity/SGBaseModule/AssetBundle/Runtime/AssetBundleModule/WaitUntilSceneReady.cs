using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
#if UNITY_WEBGL
    public class WaitUntilSceneReady : CustomYieldInstruction
    {
        private string mSceneName;
        private Action<float> mOnNotifyProgress;
        private bool isJumpped;

        public WaitUntilSceneReady(string sceneName, Action<float> onProgress = null)
        {
            this.mSceneName = sceneName;
            this.mOnNotifyProgress = onProgress;

            if (string.IsNullOrEmpty(sceneName) || AssetBundleSubGameManager.Instance.CheckIsSceneReady(sceneName))
            {
                isJumpped = true;
            }

            CoroutineExecutor.Create(
                AssetBundleSubGameManager.Instance.PreloadSceneAssetBundles(sceneName, mOnNotifyProgress), null);
        }

        public override bool keepWaiting
        {
            get { return !AssetBundleSubGameManager.Instance.CheckIsSceneReady(mSceneName) || isJumpped; }
        }
    }

    public class AsyncSceneLoadOperation : AsyncOperation
    {
        private AsyncOperation ao;
        private bool isJumpped;
        private Action<AsyncOperation> mCompleteCallback;

        public AsyncSceneLoadOperation(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                isJumpped = true;
            }

            CoroutineExecutor.Create(
                AssetBundleSubGameManager.Instance.PreloadSceneAssetBundles(sceneName, null),
                () =>
                {
                    SceneResourcesManager.LoadSceneAsync(out ao, sceneName, null, mode);
                    if (ao != null)
                        ao.completed += t => { mCompleteCallback(t); };
                });
        }


        public bool isDone
        {
            get
            {
                return (ao != null && ao.isDone) || isJumpped;
            }
        }

        public float progress
        {
            get
            {
                if (ao == null)
                    return 0;
                return ao.progress;
            }
        }

        public int priority
        {
            get { return ao.priority; }
        }

        public event Action<AsyncOperation> completed
        {
            add
            {
                if (this.isDone)
                    value(this);
                else
                    this.mCompleteCallback += value;
            }
            remove { this.mCompleteCallback -= value; }
        }
    }

#endif
}