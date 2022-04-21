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
    [Serializable]
    public class AndroidBuildData
    {
        public string mAppName="请填写app名称";
        public string mPackageName = "com.xx.xx";
        public string mVersion = "0.0.1";
        public int mBundleVersionCode = 1;
        public string mKeystorePath = "";
        public string mKeystorePassword = "";
        public string mKeystoreAliasName = "";
        public string mKeyaliasPass = "";
    }

    public static class BuildParamGeneratorEnv
    {
        private const string FilePath =  "/BuildParam/";
        private const string AndroidFileName = "build_android.json";
        private const string IOSFileName = "build_ios.json";
        
        private static string GetProjectRootPath() {
            return System.IO.Path.GetFullPath(".");
        }

        public static string GetAndroidParamFilePathRoot()
        {
            return GetProjectRootPath() + FilePath ;
        }
        public static string GetAndroidParamFilePath()
        {
            return GetProjectRootPath() + FilePath + AndroidFileName;
        }
    }
    

    /// <summary>
    /// 打包参数文件生成
    /// </summary>
    public class BuildParamGenerator
    {
        [MenuItem("SG/AutoBuild/生成安卓打包参数模板")]
        private static void CreateAndroidParamFile()
        {
            string path = BuildParamGeneratorEnv.GetAndroidParamFilePathRoot();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filePath = BuildParamGeneratorEnv.GetAndroidParamFilePath();
            if (!File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                if (fs != null)
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);
                    AndroidBuildData data = new AndroidBuildData();
                    string jsonStr = JsonConvert.SerializeObject(data);
                    Debug.Log(jsonStr);
                    byte[] bytes = Encoding.Default.GetBytes(jsonStr);

                    fs.Write(bytes, 0, bytes.Length);
                }

                fs.Close();
            }
        }

        /// <summary>
        /// 获取安卓打包参数
        /// </summary>
        /// <returns></returns>
        public static AndroidBuildData GetAndroidBuildData()
        {
            string path = BuildParamGeneratorEnv.GetAndroidParamFilePath();
            return GetBuildData<AndroidBuildData>(path);
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
}

