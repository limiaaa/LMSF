using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SG.AssetBundleBrowser.AssetBundlePacker;
using UnityEngine;
using SG.Utils;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    public class GameDownloaderManager : MonoSingleton<GameDownloaderManager>
    {
        private const int MAX_WORKER = 5;

        public struct TaskInfo
        {
            public List<string> url_group;
            public string gamePackName;
            public bool isCheckDownloadSize;
            public GameDownloader downloader;

            public bool Start()
            {
                if (downloader.IsDone)
                {
                    return false;
                }

                downloader.Reset();

                if (isCheckDownloadSize)
                {
                    return downloader.StartCheckDownloadSize(url_group, gamePackName);
                }
                else
                {
                    return downloader.StartDownload(url_group, gamePackName);
                }
            }
        }

        private Dictionary<string, GameDownloader> _downloaders = new Dictionary<string, GameDownloader>();

        private Queue<TaskInfo> _taksQueue = new Queue<TaskInfo>();

        private bool isRunning = false;

        private void StartChecking()
        {
            if (!isRunning)
            {
                isRunning = true;
                StartCoroutine(CheckTasks());
            }
        }

        private void AddTask(List<string> url_group, string gamePackName, bool isCheckDownloadSize = false)
        {
            StartChecking();

            var task = new TaskInfo
            {
                url_group = url_group,
                gamePackName = gamePackName,
                isCheckDownloadSize = isCheckDownloadSize,
                downloader = CreateDownloader(gamePackName)
            };

            _taksQueue.Enqueue(task);
        }
        
        

        IEnumerator CheckTasks()
        {
            while (isRunning)
            {
                var downloadingCount = _downloaders.Values.Count(t => t.IsDownload);
//                DebugUtils.Log("<color=red>"+_downloaders.Count + " -> " + _taksQueue.Count + " " + downloadingCount +"</color>");

                if (downloadingCount < MAX_WORKER && _taksQueue.Count > 0)
                {
                    var count = MAX_WORKER - downloadingCount;
                    while (_taksQueue.Count > 0 && count > 0)
                    {
                        var task = _taksQueue.Dequeue();
                        task.Start();
                        count--;
                    }
                }

                yield return G.OneSecond;

                var succeed = _downloaders.Values.FirstOrDefault(t => t.IsDone);
                if (succeed != null)
                {
                    _downloaders.Remove(succeed.GamePackName);
                    Destroy(succeed.gameObject);
                }

                if (downloadingCount == 0 && _taksQueue.Count == 0)
                {
                    isRunning = false;
                }
            }

            yield return null;
            isRunning = false;
        }

        public void StartDownload(List<string> url_group, string gamePackName)
        {
            AddTask(url_group, gamePackName);
        }
        
        
        public void StartDownload(string url, string gamePackName)
        {
            AddTask(new List<string>(){url}, gamePackName);
        }


        public void StartCheckDownloadSize(List<string> url_group, string gamePackName)
        {
            AddTask(url_group, gamePackName, true);
        }
        
        public void StartCheckDownloadSize(string url, string gamePackName)
        {
            AddTask(new List<string>(){url}, gamePackName,true);
        }

        public void CancelDownload(string gamePackName)
        {
            var downloader = CreateDownloader(gamePackName);

            downloader.CancelDownload();
        }

        public int DownloadingCount
        {
            get { return _downloaders.Values.Count(t => t.IsDownload); }
        }


        public void AbortDownload(string gamePackName)
        {
            var downloader = CreateDownloader(gamePackName);

            downloader.AbortDownload();
        }

        private void Reset(string gamePackName)
        {
            var downloader = CreateDownloader(gamePackName);

            downloader.Reset();
        }

        public GameDownloader GetDownloader(string gamePackName)
        {
            GameDownloader downloader = null;
            if (_downloaders.ContainsKey(gamePackName))
            {
                downloader = _downloaders[gamePackName];
            }

            return downloader;
        }


        private GameDownloader CreateDownloader(string gamePackName)
        {
            GameDownloader downloader = null;

            if (_downloaders.ContainsKey(gamePackName))
            {
                downloader = _downloaders[gamePackName];
                downloader.CancelDownload();
                downloader.Reset();
                return downloader;
            }


            GameObject go = new GameObject(gamePackName);
            go.transform.SetParent(this.transform);

            downloader = go.AddComponent<GameDownloader>();

            downloader.OnUpdate = OnDownloaderUpdate;
            downloader.OnDone = OnDownloaderDone;
            downloader.OnError = OnDownloaderError;


            _downloaders[gamePackName] = downloader;

            Debug.Assert(downloader, "get available gamedownloader failed!");
            return downloader;
        }


        #region events

        /// <summary>
        ///   UpdateEvent
        /// </summary>
        public event System.Action<GameDownloader> OnUpdate;

        /// <summary>
        ///   DoneEvent
        /// </summary>
        public event System.Action<GameDownloader> OnDone;

        /// <summary>
        ///   DoneEvent
        /// </summary>
        public event System.Action<GameDownloader, emErrorCode, string> OnError;

        private void OnDownloaderUpdate(GameDownloader downloader)
        {
            if (this.OnUpdate != null)
            {
                this.OnUpdate(downloader);
            }
        }

        private void OnDownloaderDone(GameDownloader downloader)
        {
            if (this.OnDone != null)
            {
                this.OnDone(downloader);
            }
        }

        private void OnDownloaderError(GameDownloader downloader, emErrorCode code, string msg)
        {
            if (this.OnError != null)
            {
                this.OnError(downloader, code, msg);
            }
        }

        #endregion events


        public override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var gameDownloader in _downloaders.Values)
            {
                gameDownloader.CancelDownload();
            }
        }
    }
}