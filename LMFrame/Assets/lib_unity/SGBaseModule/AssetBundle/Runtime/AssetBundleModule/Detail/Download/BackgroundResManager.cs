using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SG.AssetBundleBrowser;
using SG.ContainerCoreMain;
using UnityEngine;
using SG.Utils;

namespace SG
{
    /// <summary>
    /// 1. hotfix assetbundle
    /// 2. updated/added res assetbundle
    /// 3. updated/added game assetbundle
    /// </summary>
    public class BackgroundResManager : MonoSingleton<BackgroundResManager>
    {
        private static string BG_RES_ROOT = Application.persistentDataPath + "/AssetBundle/background/";
        private static string TASK_INFO_FILE = BG_RES_ROOT + "task_info.json";
        private BGDTasks tasks = new BGDTasks();
        private Action<string> luaExecutor;

        /// <summary>
        /// 1. 从SummaryModel自定义json中获取下载列表
        /// 2. 根据下载列表下载到BG_RES_ROOT目录【解压】【缓存】并【记录】有需要处理的缓存信息
        ///     a. 下载需要限速，保证游戏正常运行
        ///     a. 判断是否后台下载完成是否自动(弹窗)重启游戏
        ///     b. 可以放入一段lua代码用于执行其他额外操作（比如清空错误的数据或者AssetBundle文件），需要再Main Xlua Manager 启动之后
        /// 3. 下一次打开游戏的时候，根据记录的缓存信息进行拷贝
        /// </summary>
        private static IEnumerator RunYield()
        {
            if (Instance.tasks.IsEmpty())
            {
                yield break;
            }

            while (Instance.HasTaskToRun())
            {
                foreach (BackgroundDownloadTask task in Instance.tasks)
                {
                    if (task.state != BackgroundDownloadTask.TaskState.Finished)
                        yield return task.Run(BackgroundDownloadTask.TaskState.ToBeCopied);
                    Instance.Save();
                }

                yield return G.FiveSeconds;
            }
        }

        private bool HasTaskToRun()
        {
            foreach (BackgroundDownloadTask task in Instance.tasks)
            {
                if (task.state != BackgroundDownloadTask.TaskState.ToBeCopied)
                {
                    return true;
                }
            }

            return false;
        }

        private void Save()
        {
            if (!Directory.Exists(BG_RES_ROOT))
            {
                Directory.CreateDirectory(BG_RES_ROOT);
            }

            var content = JsonUtility.ToJson(tasks);
            File.WriteAllText(TASK_INFO_FILE, content);
        }

        public static void Run()
        {
            Instance.StartCoroutine(RunYield());
        }

        public static void SetLuaExecutor(Action<string> action)
        {
            Instance.luaExecutor = action;
        }

        public static void AddTask(string url, string infoLuaCmd)
        {
            if (string.IsNullOrEmpty(url))
                return;
            var task = new BackgroundDownloadTask(url);
            task.luaCmd = infoLuaCmd;
            bool isExist = false;
            for (int j = 0; j < Instance.tasks.Count; j++)
            {
                if (task.url == Instance.tasks[j].url)
                {
                    isExist = true;
                }
            }

            if (!isExist)
            {
                Instance.tasks.Add(task);
            }
        }

        /// <summary>
        /// 当hotfix包下载解压缩完毕之后，下一次运行的时候调用这个接口来拷贝到对应目录
        /// </summary>
        /// <returns></returns>
        public static IEnumerator CheckTaskFinished()
        {
            foreach (BackgroundDownloadTask task in Instance.tasks)
            {
                if (task.state == BackgroundDownloadTask.TaskState.ToBeCopied)
                {
                    yield return task.Run(BackgroundDownloadTask.TaskState.Finished, true);
                    Instance.Save();
                }
            }

            yield return null;
        }

        protected override void Init()
        {
            if (File.Exists(TASK_INFO_FILE))
            {
                tasks = JsonUtility.FromJson<BGDTasks>(File.ReadAllText(TASK_INFO_FILE));
            }
        }


        /// <summary>
        /// 所有后台下载的情况都使用一个zip包的方式,下载完成之后进行解压到目标目录操作
        /// </summary>
        [Serializable]
        internal class BackgroundDownloadTask
        {
            internal enum TaskState
            {
                ToBeDownloaded,
                ToBeUncompressed,
                ToBeCopied,
                ToBeExecuted,
                Finished
            }

            public BackgroundDownloadTask(string url)
            {
                fileName = url.Split('/').Last();
                this.url = url.Replace(fileName, "");
            }

            [SerializeField] public string url;

            [SerializeField] public string md5;

            [SerializeField] public string luaCmd;

            [SerializeField] public string fileName;

            [SerializeField] public string downloadedTime;

            [SerializeField] public string deployedTime;

            [SerializeField] private TaskState _state = TaskState.ToBeDownloaded;

            [NonSerialized] public bool isFinished, isErrorExist;

            public TaskState state
            {
                get { return _state; }
                set
                {
                    DebugUtils.Log(_state + " -> " + value);

                    _state = value;
                }
            }

            public string FileFullName
            {
                get { return BG_RES_ROOT + fileName; }
            }

            bool CheckFileStateRight()
            {
                if (!File.Exists(FileFullName))
                {
                    state = TaskState.ToBeDownloaded;
                    return false;
                }

                if (CryptUtils.Md5FromFile(FileFullName) != md5)
                {
                    state = TaskState.ToBeDownloaded;
                    File.Delete(FileFullName);
                    return false;
                }

                return true;
            }

            public IEnumerator Run(TaskState stopAt = TaskState.Finished, bool skipError = false)
            {
                while (state != stopAt)
                {
                    if (isErrorExist && skipError)
                    {
                        yield break;
                    }

                    switch (state)
                    {
                        case TaskState.ToBeDownloaded:
                            yield return Download();
                            break;
                        case TaskState.ToBeUncompressed:
                            yield return Uncompress();
                            break;
                        case TaskState.ToBeCopied:
                            yield return CopyToDest();
                            break;
                        case TaskState.ToBeExecuted:
                            yield return ExecuteCommand();
                            break;
                        case TaskState.Finished:
                            yield break;
                    }
                }

                yield return null;
            }

            /// <summary>
            /// 1. 限速
            /// 2. 断点续传（CDN range是否正确已确认）
            ///     a. 使用Accept-Encoding:identity
            ///     b. Range准确
            /// </summary>
            /// <returns></returns>
            private IEnumerator Download()
            {
                WebRequestDownload request = new WebRequestDownload(url);
                yield return request.StartYield(BG_RES_ROOT, fileName,G.SPEED_LIMITATION);
                if (request.errorCode != WebRequestDownload.ErrorCodeEnum.None)
                {
                    yield break;
                }

                md5 = CryptUtils.Md5FromFile(FileFullName);

                downloadedTime = DateTime.Now.GetLibiiDateString();

                state = TaskState.ToBeUncompressed;
            }

            /// <summary>
            /// 解压到当前文件夹，下一次刚开始运行进行拷贝，防止游戏运行中发生文件占用
            /// </summary>
            /// <returns></returns>
            private IEnumerator Uncompress()
            {
                try
                {
                    ZipHelper.UnzipFile(FileFullName, BG_RES_ROOT + "unzipped/");
                    state = TaskState.ToBeCopied;
                }
                catch (Exception e)
                {
                    CheckFileStateRight();
                    isErrorExist = true;
                }

                yield return null;
            }

            private IEnumerator CopyToDest()
            {
                try
                {
                    var src = BG_RES_ROOT + "unzipped/";
                    if (Directory.Exists(src))
                    {
                        Extensions.MoveDirectory(BG_RES_ROOT + "unzipped/",
                            Platform.PERSISTENT_DATA_PATH + "/AssetBundle/");
                        state = TaskState.ToBeExecuted;
                        deployedTime = DateTime.Now.GetLibiiDateString();
                    }
                    else
                    {
                        state = TaskState.ToBeUncompressed;
                        isErrorExist = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    isErrorExist = true;
                }
                finally
                {
                    File.Delete(FileFullName);
                }

                yield return null;
            }

            /// <summary>
            /// 需要在XluaManagerMain启动之后调用
            /// </summary>
            /// <returns></returns>
            private IEnumerator ExecuteCommand()
            {
                try
                {
                    // TODO:考虑是否做成阻塞主游戏进程，等脚本命令执行完毕才能进主游戏
                    if (!string.IsNullOrEmpty(luaCmd))
                    {
                        Instance.luaExecutor(luaCmd);
                    }

                    state = TaskState.Finished;
                }
                catch (Exception e)
                {
                    isErrorExist = true;
                }

                yield return null;
            }
        }

        [Serializable]
        internal class BGDTasks : IList<BackgroundDownloadTask>
        {
            [SerializeField] public List<BackgroundDownloadTask> tasks;

            public BGDTasks()
            {
                tasks = new List<BackgroundDownloadTask>();
            }

            public bool Remove(BackgroundDownloadTask item)
            {
                return tasks.Remove(item);
            }

            public int Count
            {
                get { return tasks.Count; }
            }

            public bool IsReadOnly { get; private set; }

            public int IndexOf(BackgroundDownloadTask item)
            {
                return tasks.IndexOf(item);
            }

            public void Insert(int index, BackgroundDownloadTask item)
            {
                tasks.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                tasks.RemoveAt(index);
            }

            public bool IsEmpty()
            {
                return tasks == null || Count <= 0;
            }

            public BackgroundDownloadTask this[int index]
            {
                get { return tasks[index]; }
                set { tasks[index] = value; }
            }

            public void Add(BackgroundDownloadTask task)
            {
                this.tasks.Add(task);
            }

            public void Clear()
            {
                tasks.Clear();
            }

            public bool Contains(BackgroundDownloadTask item)
            {
                return tasks.Contains(item);
            }

            public void CopyTo(BackgroundDownloadTask[] array, int arrayIndex)
            {
                tasks.CopyTo(array, arrayIndex);
            }

            public IEnumerator<BackgroundDownloadTask> GetEnumerator()
            {
                return tasks.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void SetData(List<BackgroundDownloadTask> existedTasks)
            {
                tasks = existedTasks;
            }
        }
    }
}