
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Networking;

    public static class FileUtls
    {
        /// 写入文件
        public static void WriteTextToFile(string path, string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            WriteBytesToFile(path, bytes, bytes.Length);
        }
        /// 写入文件
        public static void WriteBytesToFile(string path, byte[] bytes, int length)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            FileInfo t = new FileInfo(path);
            using (Stream sw = t.Open(FileMode.Create, FileAccess.ReadWrite))
            {
                if (bytes != null && length > 0)
                {
                    //以行的形式写入信息
                    sw.Write(bytes, 0, length);
                }
                sw.Close();
            }
        }
        /// 获取文件下所有文件大小
        public static int GetAllFileSize(string filePath)
        {
            int sum = 0;
            if (!Directory.Exists(filePath))
            {
                return 0;
            }

            DirectoryInfo dti = new DirectoryInfo(filePath);

            FileInfo[] fi = dti.GetFiles();

            for (int i = 0; i < fi.Length; ++i)
            {
                sum += Convert.ToInt32(fi[i].Length / 1024);
            }

            DirectoryInfo[] di = dti.GetDirectories();

            if (di.Length > 0)
            {
                for (int i = 0; i < di.Length; i++)
                {
                    sum += GetAllFileSize(di[i].FullName);
                }
            }

            return sum;
        }
        /// 获取指定文件大小
        public static long GetFileSize(string file_path)
        {
            long sum = 0;
            if (!File.Exists(file_path))
            {
                return 0;
            }
            else
            {
                FileInfo Files = new FileInfo(file_path);
                sum += Files.Length;
            }
            return sum;
        }
        ///   读取本地AssetBundle文件
        static IEnumerator LoadAssetbundleFromLocal(string path, string name)
        {
            WWW w = new WWW("file:///" + path + "/" + name);

            yield return w;

            if (w.isDone)
            {
                GameObject.Instantiate(w.assetBundle.mainAsset);
            }
        }
        public static IEnumerator CopyStreamingAssetsToFile(string src, string dest)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID
            if (!src.Contains("///"))
                src = "file:///" + src;
#endif
            if (src.Contains("://"))
            {
                UnityWebRequest www = UnityWebRequest.Get(src);
                yield return www.SendWebRequest();
                if (string.IsNullOrEmpty(www.error))
                {
                    while (www.isDone == false)
                        yield return null;
                    FileUtls.WriteBytesToFile(dest, www.downloadHandler.data,
                        www.downloadHandler.data.Length);
                    //Debug.Log("CopyStreamingAssetsToFile:>>src:{0}--des:{1},data:{2}" , src , dest , www.downloadHandler.data.Length);
                }
                else
                {
                    Debug.LogError(www.error + "\nurl:" + www.url + "\nsrc :" + src + "\ndest:" + dest);
                }

                yield return null;
            }
            else
            {
                if (File.Exists(src))
                {
                    var data = File.ReadAllBytes(src);
                    WriteBytesToFile(dest, data, data.Length);
                }
                else
                {
                    Debug.LogError("Not exist: " + src);
                }

                yield return null;
            }
        }
        ///   绝对路径转相对路径
        public static string AbsoluteToRelativePath(string root_path, string absolute_path)
        {
            absolute_path = absolute_path.Replace('\\', '/');
            int last_idx = absolute_path.LastIndexOf(root_path);
            if (last_idx < 0)
                return absolute_path;

            int start = last_idx + root_path.Length;
            if (absolute_path[start] == '/')
                start += 1;

            int length = absolute_path.Length - start;
            return absolute_path.Substring(start, length);
        }
        ///   获得取除路径扩展名的路径
        public static string GetPathWithoutExtension(string full_name)
        {
            int last_idx = full_name.LastIndexOfAny(".".ToCharArray());
            if (last_idx < 0)
                return full_name;

            return full_name.Substring(0, last_idx);
        }
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name));
            }
        }
        public static void MoveAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }
            foreach (var item in source.GetFileSystemInfos())
            {
                if (item is FileInfo)
                {
                    FileInfo di = (FileInfo)item;
                    di.MoveTo(Path.Combine(target.FullName, item.Name));
                }
                if (item is DirectoryInfo)
                {
                    DirectoryInfo di = (DirectoryInfo)item;
                    Debug.Log(Path.Combine(target.FullName, item.Name));
                    di.MoveTo(Path.Combine(target.FullName, item.Name));
                }
            }
        }
    }
