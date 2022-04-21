using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SG.AssetBundleBrowser;
using SG.Utils;
using UnityEngine;
using UnityEditor;
// Create By LJ

namespace SG.AutoBuild
{
    public static class DingTalkGeneratorEnv
    {
        private const string FilePath =  "/BuildParam/";
        private const string dingtalkFileName = "dingtalk.json";
        
        private static string GetProjectRootPath() {
            return System.IO.Path.GetFullPath(".");
        }

        public static string GetTalkFilePathRoot()
        {
            return GetProjectRootPath() + FilePath ;
        }
        public static string GetTalkFilePath()
        {
            return GetProjectRootPath() + FilePath + dingtalkFileName;
        }
    }
    

    /// <summary>
    /// 文件生成
    /// </summary>
    public class DingTalkTemplateGenerator
    {
        [MenuItem("SG/AutoBuild/通知模板生成")]
        private static void CreateAndroidParamFile()
        {
            string path = DingTalkGeneratorEnv.GetTalkFilePathRoot();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filePath = DingTalkGeneratorEnv.GetTalkFilePath();
            if (!File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                if (fs != null)
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);
                    DingTalkData data = new DingTalkData();
                    string jsonStr = JsonConvert.SerializeObject(data);
                    Debug.Log(jsonStr);
                    byte[] bytes = Encoding.Default.GetBytes(jsonStr);

                    fs.Write(bytes, 0, bytes.Length);
                }
                UnityEditor.EditorUtility.DisplayDialog("文件创建成功", "文件创建成功："+filePath, "确认", "取消");
                fs.Close();
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog("文件已经存在", "文件已经存在："+filePath, "确认", "取消");
            }
        }

        /// <summary>
        /// 获取安卓打包参数
        /// </summary>
        /// <returns></returns>
        public static DingTalkData GetDingTalkData()
        {
            string path = DingTalkGeneratorEnv.GetTalkFilePath();
            return GetBuildData<DingTalkData>(path);
        }

        private static T GetBuildData<T>(string path) where T:class,new()
        {
            T data = null;
            if (!File.Exists(path))
            {
                return null;
            }

            FileStream fs = new FileStream(path, FileMode.Open);
            if (fs != null)
            {
                string jsonStr = null;
                    byte[] bytes = new byte[fs.Length];
                    int nResult = fs.Read(bytes, 0, bytes.Length);
                    if (nResult > 0)
                        jsonStr = Encoding.Default.GetString(bytes);
                if (!string.IsNullOrEmpty(jsonStr))
                    data = JsonConvert.DeserializeObject<T>(jsonStr);
            }

            fs.Close();
            return data;
        }
    }

    [Serializable]
    public class DingTalkData
    {
        public string token = "";
        public string keyWord = "";
        public string url = "";
    }
}

