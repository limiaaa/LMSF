using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SG.AssetBundleBrowser.AssetBundlePacker;
using UnityEditor;
using UnityEngine;

namespace SG.AssetBundleBrowser
{
    public class BaseTab
    {
        internal EditorWindow parent;

        internal string BUILD_ROOT_PATH
        {
            get
            {
                if (m_UserData != null)
                {
                    return m_UserData.m_OutputPath;
                }

                return "";
            }
        }

        internal string RESOURCES_MANIFEST_FILE_PATH
        {
            get
            {
                var path = System.IO.Path.GetFullPath(".");
                path += "/" + BUILD_ROOT_PATH + "/" + SG.AssetBundleBrowser.AssetBundlePacker.Common.RESOURCES_MANIFEST_FILE_NAME;
                path = path.Replace("\\", "/");
                //RESOURCES_MANIFEST_FILE_PATH = RESOURCES_MANIFEST_FILE_PATH.Replace("/", @"\");
                return path;
            }
        }

        [SerializeField] public AssetBundleBuildTab.BuildTabData m_UserData;

        /// <summary>
        ///   AssetBundle信息描述数据
        /// </summary>
        public ResourcesManifest ManifestData;

        string GetDataFilePath()
        {
            var dataPath = System.IO.Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            dataPath += "/Library/AssetBundleBrowserBuild.dat";

            return dataPath;
        }

        private void LoadEditorData()
        {
            //LoadData...
            var dataPath = GetDataFilePath();

            if (File.Exists(dataPath) && m_UserData == null)
            {
                FileStream file = null;
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    file = File.Open(dataPath, FileMode.Open);
                    var data = bf.Deserialize(file) as AssetBundleBuildTab.BuildTabData;
                    if (data != null)
                        m_UserData = data;
                    file.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }

            if (m_UserData == null)
            {
                m_UserData = new AssetBundleBuildTab.BuildTabData();
                m_UserData.m_OnToggles = new List<string>();
            }
        }

        private void SaveEditorData()
        {
            if (m_UserData == null)
            {
                return;
            }

            var dataPath = GetDataFilePath();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(dataPath);

            bf.Serialize(file, m_UserData);
            file.Close();
        }


        /// <summary>
        ///   载入数据
        /// </summary>
        internal virtual bool LoadData()
        {
            bool result = false;
            LoadEditorData();
            if (File.Exists(RESOURCES_MANIFEST_FILE_PATH))
            {
                ManifestData = ResourcesManifest.Load(RESOURCES_MANIFEST_FILE_PATH);
                result = ManifestData != null;
            }

            if (ManifestData == null)
            {
                ManifestData = new ResourcesManifest();
            }

            return result;
        }

        /// <summary>
        ///   保存数据
        /// </summary>
        internal bool SaveData(bool isSaveManifest = false)
        {
            SaveEditorData();
            if (ManifestData != null && isSaveManifest)
                return ManifestData.Save(RESOURCES_MANIFEST_FILE_PATH);

            return false;
        }

        internal uint GetResVersion()
        {
            var year = System.DateTime.Now.Year;
            var month = System.DateTime.Now.Month;
            var day = System.DateTime.Now.Day;
            var hour = System.DateTime.Now.Hour;

            return uint.Parse(year.ToString().Substring(2, 2) +
                              month.ToString("00") +
                              day.ToString("00") +
                              hour.ToString("00"));
        }
    }
}