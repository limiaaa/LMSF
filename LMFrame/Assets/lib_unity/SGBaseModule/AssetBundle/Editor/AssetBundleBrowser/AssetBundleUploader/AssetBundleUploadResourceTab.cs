using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SG.AssetBundleUploader;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace SG.AssetBundleBrowser
{
    [System.Serializable]
    public class AssetBundleUploadResourceTab : BaseTab
    {
        GUIContent m_TargetContent;

        public string CurrentFile, CurrentFilePath;

        private static readonly string[] platforms = {"Android", "iOS", "Windows"};
        private static readonly string[] envStrings = {"Live", "Test"};
        private static readonly string[] langStrings = {"English", "中文"};

        private const string test_url = "192.168.0.23:8090/upload/resource";
        private const string live_url = "http://collectionadmin.libii.com/upload/resource";
        private const string ALL_TOKEN = "123456";

        internal void OnEnable(EditorWindow parent)
        {
            LoadData();
            this.parent = parent;
            m_TargetContent = new GUIContent("Upload Target", "Choose target platform to upload for.");
        }

        internal override bool LoadData()
        {
            var result = base.LoadData();

            return result;
        }

        long GetFileVersion()
        {
            return 0;
        }

        internal void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button(new GUIContent("选择要上传的文件们", "选择的文件会被自动压缩成zip包")))
            {
            }


            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildTarget))
            {
                GUILayout.Label("选择平台");
                AssetBundleBuildTab.ValidBuildTarget tgt =
                    (AssetBundleBuildTab.ValidBuildTarget) EditorGUILayout.EnumPopup(m_TargetContent,
                        m_UserData.m_BuildTarget);
                GUILayout.Space(10);
                m_UserData.m_BuildTarget = tgt;
            }

            GUILayout.Label("上传测试服，还是正式服");
            m_UserData.m_Env = EditorGUILayout.Popup("Live Or Test:", m_UserData.m_Env, envStrings);
            GUILayout.Space(10);

            GUILayout.Label("数据包语言");
            m_UserData.m_Lang = EditorGUILayout.Popup("English Or 中文:", m_UserData.m_Lang, langStrings);
            GUILayout.Space(10);

            GUILayout.Label("MainApp名字");
            m_UserData.m_MainAppName = EditorGUILayout.TextField("MainAppName：", m_UserData.m_MainAppName);
            GUILayout.Space(10);

            GUILayout.Label("SubApp名字");
            m_UserData.m_SubAppName = EditorGUILayout.TextField("SubAppName：", m_UserData.m_SubAppName);
            GUILayout.Space(10);

            GUILayout.Label("数据包版本,格式：19041716");
            GUI.enabled = false;
            EditorGUILayout.LongField("Resources Version：", GetFileVersion());
            GUI.enabled = true;
            GUILayout.Space(10);

            GUILayout.Label("上传人员用户名");
            m_UserData.m_User = EditorGUILayout.TextField("User：", m_UserData.m_User);
            GUILayout.Space(10);


            if (m_UserData.m_Env == 0)
            {
                if (m_UserData.m_Token == ALL_TOKEN)
                {
//                    m_UserData.m_Token = string.Empty;
                }

                GUILayout.Label("上传密钥");
                m_UserData.m_Token = EditorGUILayout.TextField("Token：", m_UserData.m_Token);
                GUILayout.Space(10);
            }
            else
            {
                m_UserData.m_Token = ALL_TOKEN;
            }

            if (EditorGUI.EndChangeCheck())
            {
                LoadData();
            }


            GUILayout.Space(10);


            if (GUILayout.Button("Upload"))
            {
                try
                {
                    if (string.IsNullOrEmpty(m_UserData.m_MainAppName) ||
                        string.IsNullOrEmpty(m_UserData.m_MainMinVersion) ||
                        string.IsNullOrEmpty(m_UserData.m_SubAppName) ||
                        string.IsNullOrEmpty(m_UserData.m_User) ||
                        string.IsNullOrEmpty(m_UserData.m_Token))
                    {
                        throw new Exception("请将参数填写完整！");
                    }

                    var para = new Dictionary<string, string>()
                    {
                        {"appId", m_UserData.m_MainAppName},
                        {"languageCode", m_UserData.m_Lang == 1 ? "chinese" : "english"},
                        {"partCode", m_UserData.m_SubAppName.Trim()},
                        {
                            "platform",
                            m_UserData.m_BuildTarget == AssetBundleBuildTab.ValidBuildTarget.iOS ? "ios" : "android"
                        },

                        {"user", m_UserData.m_User.Trim()},
                        {"version", GetFileVersion() + ""},
                        {
                            "channel",
                            m_UserData.m_BuildTarget == AssetBundleBuildTab.ValidBuildTarget.iOS
                                ? "APP_STORE"
                                : "GOOGLE_PLAY"
                        },
                        {"tk", m_UserData.m_Token.Trim()},
                    };
                    var info = string.Format("{0}", string.Join("\n", para.Values.ToArray()).Replace(ALL_TOKEN, ""));
                    if (EditorUtility.DisplayDialog("确定上传？", info, "上传！"))
                    {
                        if (!File.Exists(CurrentFilePath))
                        {
                            parent.ShowNotification(new GUIContent("找不到要上传的数据包:\n" + CurrentFilePath));
                            return;
                        }

                        ExecuteUpload(para);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    parent.ShowNotification(new GUIContent(e.Message + "\n" + e.StackTrace));
                }
            }

            // if (File.Exists(CurrentFilePath))
            // {
            //     var fileInfo = new FileInfo(CurrentFilePath);
            //
            //     GUILayout.Label("Upload File:\n" + CurrentFilePath + "\n" + fileInfo.Length / 1024 / 1024 + " MB\n");
            // }
            // else
            // {
            //     GUILayout.Label("找不到要上传的数据包:\n" + CurrentFilePath);
            // }
        }

        /// <summary>
        /// 
        /// </summary>
        void ExecuteUpload(Dictionary<string, string> parameters)
        {
            var urls = new string[] {live_url, test_url};
            var url = urls[m_UserData.m_Env];
            // var url = test_url;
            HttpRequestUploader.OnUpload = (progress, uploaded, all) =>
            {
                if (progress <= 0.99f)
                    EditorUtility.DisplayProgressBar("",
                        string.Format("{0:F}MB/{1:F}MB", uploaded / 1024f / 1024, all / 1024f / 1024), progress);
                else
                {
                    EditorUtility.ClearProgressBar();
                }
            };

            var isSuccess = HttpRequestUploader.Upload(url, new List<HttpRequestUploader.UploadFile>
                {
                    new HttpRequestUploader.UploadFile
                    {
                        Name = "file",
                        Filename = CurrentFile,
                        ContentType = "application/zip",
                        FilePath = CurrentFilePath
                    },
                }, parameters
            );

            if (!isSuccess)
            {
                GUILayout.Label("Upload Failed, check log for reasons");
            }
        }
    }
}