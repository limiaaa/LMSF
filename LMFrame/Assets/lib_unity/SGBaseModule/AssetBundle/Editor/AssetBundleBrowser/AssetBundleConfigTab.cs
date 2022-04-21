using SG.AssetBundleBrowser.AssetBundlePacker;
using System;
using System.Collections.Generic;
using System.IO;
using SG.Utils;
using UnityEditor;
using UnityEngine;

namespace SG.AssetBundleBrowser
{
    [System.Serializable]
    public class AssetBundleConfigTab : BaseTab
    {
        GUIContent m_TargetContent;


        internal AssetBundleConfigTab()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        class SelectResultStatus
        {
            /// <summary>
            /// 操作
            /// </summary>
            public enum Operate
            {
                None,
                Compress,
                General
            }

            public Operate Op;
            public bool IsCompress;
            public bool IsGeneral;
        }


        /// <summary>
        /// 
        /// </summary>
        class NodeGroup : GUILayoutMultiSelectGroup.NodeGroup
        {
            /// <summary>
            /// 
            /// </summary>
            public List<Node> Nodes = new List<Node>();

            /// <summary>
            /// 
            /// </summary>
            public override GUILayoutMultiSelectGroup.OperateResult Draw()
            {
                GUILayoutMultiSelectGroup.OperateResult result = null;
                for (int i = 0; i < Nodes.Count; ++i)
                {
                    if (result == null)
                        result = Nodes[i].Draw();
                    else
                        Nodes[i].Draw();
                }

                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            public override List<GUILayoutMultiSelectGroup.Node> GetRange(int begin, int end)
            {
                List<GUILayoutMultiSelectGroup.Node> temp = new List<GUILayoutMultiSelectGroup.Node>();

                if (begin < 0) begin = 0;
                if (begin >= Nodes.Count) begin = Nodes.Count - 1;
                if (end < 0) end = 0;
                if (end >= Nodes.Count) end = Nodes.Count - 1;

                for (int i = begin; i <= end; ++i)
                {
                    temp.Add(Nodes[i]);
                }

                return temp.Count > 0 ? temp : null;
            }
        }

        /// <summary>
        /// AssetBundle显示节点
        /// </summary>
        class Node : GUILayoutMultiSelectGroup.Node
        {
            /// <summary>
            /// 指向资源
            /// </summary>
            public ResourcesManifestRawData.AssetBundle AssetBundle;

            /// <summary>
            /// 渲染
            /// </summary>
            public override GUILayoutMultiSelectGroup.OperateResult Draw()
            {
                if (AssetBundle == null)
                    return null;

                GUI.backgroundColor = IsSelect ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                GUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(20f));
                {
                    // GUI.contentColor = Color.black;
                    GUILayout.Label(Index.ToString(), GUILayout.Width(24f));
                    GUILayout.Button(AssetBundle.AssetBundleName, "label", GUILayout.Height(20f));
                    float size = AssetBundle.Size / 1024f;
                    GUILayout.Button(size.ToString("F2") + "KB", "label", GUILayout.Width(108f));
                    GUILayout.Space(12f);
                    bool isCompress = GUILayout.Toggle(AssetBundle.IsCompress, "", GUILayout.Width(24f));
                    GUILayout.Space(48f);
                    GUI.color = AssetBundle.LoadOrder == 0 ? Color.yellow : Color.white;
                    GUILayout.Label(AssetBundle.LoadOrder.ToString(), GUILayout.Width(60f));
                    GUI.color = Color.white;
                    GUILayout.Space(12f);
                    bool isGeneral = GUILayout.Toggle(AssetBundle.IsGeneral, "", GUILayout.Width(12));
                    GUILayout.Space(50);

                    GUILayout.Label(AssetBundle.Crc.ToString("X"), GUILayout.Width(88));

                    GUILayout.EndHorizontal();

                    SelectResultStatus.Operate op = SelectResultStatus.Operate.None;
                    if (isCompress != AssetBundle.IsCompress)
                        op = SelectResultStatus.Operate.Compress;

                    if (isGeneral != AssetBundle.IsGeneral)
                        op = SelectResultStatus.Operate.General;


                    if (op != SelectResultStatus.Operate.None)
                    {
                        return new GUILayoutMultiSelectGroup.OperateResult()
                        {
                            SelectNode = this,
                            Status = new SelectResultStatus()
                            {
                                Op = op,
                                IsCompress = isCompress,
                                IsGeneral = isGeneral
                            },
                        };
                    }
                }
                GUI.color = Color.white;
                GUI.backgroundColor = Color.white;


                return null;
            }
        }


        private GUILayoutMultiSelectGroup gui_multi_select_;

        public bool isStrictMode = true;
        public bool isAllSelect = true;

        internal override bool LoadData()
        {
            var result = base.LoadData();

            Build();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Build(bool isSelect = true)
        {
            NodeGroup group = new NodeGroup();
            int index = 0;
            foreach (var ab in ManifestData.RawData.AssetBundles.Values)
            {
                //过滤主AssetBundle文件
                if (ab.AssetBundleName == AssetBundleBrowser.AssetBundlePacker.Common.MAIN_MANIFEST_FILE_NAME)
                    continue;
                ab.IsGeneral = true;
                Node node = new Node()
                {
                    Index = index++,
                    IsSelect = isSelect,
                    AssetBundle = ab,
                };
                group.Nodes.Add(node);
            }

            gui_multi_select_ = new GUILayoutMultiSelectGroup(group);
        }

        /// <summary>
        /// 执行已做的修改
        /// </summary>
        public void ExecuteModified()
        {
            if (isStrictMode && !SG.SettingReader.ScriptableObject.IsRealtimeHotFix)
            {
                EditorUtility.DisplayDialog("错误", "开启严格模式，但没有开启实时拷贝", "确认");

                return;
            }

            if (!isStrictMode && SG.SettingReader.ScriptableObject.IsRealtimeHotFix)
            {
                EditorUtility.DisplayDialog("错误", "开启实时拷贝，但没有开启严格模式", "确认");

                return;
            }


            var oldResourcesManifestData = ResourcesManifest.Load(RESOURCES_MANIFEST_FILE_PATH);

            bool compress = BuildAssetBundle.CompressAssetBundles(oldResourcesManifestData
                , ref ManifestData);
            bool save = compress && SaveData(true);

            string gameFolder = Utils.GetGameFolder(ManifestData);

            bool copy = save && BuildAssetBundle.CopyNativeAssetBundleToStreamingAssets(ManifestData, isStrictMode);
            bool succeed = compress && copy;

            string title = "执行配置AssetBundle" + (succeed ? "成功" : "失败");
            string compress_desc = "压缩资源 - " + (compress ? "成功" : "失败");
            string save_desc = "保存配置文件 - " + (save ? "成功" : "失败");
            string copy_desc = "拷贝初始资源至安装包目录 - " + (copy ? "成功" : "失败");
            string desc = compress_desc + "\n"
                                        + save_desc + "\n"
                                        + copy_desc + "\n\n";

            EditorUtility.DisplayDialog(title, desc, "确认");
        }

        void CreateServerZip()
        {
            try
            {
                string gameFolder = Utils.GetGameFolder(ManifestData);
                var gameCode = Utils.GetSubAppCode(ManifestData);

                //从GameDownloader中过滤非本subapp的资源
//                BuildAssetBundle.RemoveOtherSubappRes(ref Manifest, gameFolder);
                SaveData(true);


                bool copied =
                    BuildAssetBundle.DoCopyNativeAssetBundleToStreamingAssets(ManifestData, gameFolder, isStrictMode);

                if (copied)
                {
                    // 生成zip包
//                    string srcPath = "AssetBundles" + System.IO.Path.DirectorySeparatorChar + m_UserData.m_BuildTarget.ToString();
                    string srcPath = SG.AssetBundleBrowser.AssetBundlePacker.Common.INITIAL_PATH;


                    var toReanme = string.Empty;
                    var dirs = Directory.GetDirectories(SG.AssetBundleBrowser.AssetBundlePacker.Common.INITIAL_PATH);
                    foreach (var dir in dirs)
                    {
                        if (dir.Contains(gameFolder))
                        {
                            toReanme = dir;
                            continue;
                        }

                        Directory.Delete(dir, true);
                    }

                    var newDir = toReanme.Replace(gameFolder, "AssetBundle");
                    Directory.Move(toReanme, newDir);
                    AssetDatabase.Refresh();

                    string destZipFileName = "AssetBundles";
                    destZipFileName += "\\" + m_UserData.m_BuildTarget.ToString().ToLower() + "_" + gameCode + "_" +
                                       ManifestData.RawData.Version + ".zip";

                    ZipHelper.CreateZip(srcPath, destZipFileName);
                    EditorUtility.DisplayDialog("Success!", "File location:\n" + destZipFileName, "ok");

                    Directory.Delete(newDir, true);
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", "Found error:\n" + e.Message + "\n" + e.StackTrace, "ok");
            }
        }


        #region Select Operate

        /// <summary>
        /// 更新选中操作
        /// </summary>
        void UpdateSelectOperate(GUILayoutMultiSelectGroup.OperateResult result)
        {
            if (result != null)
            {
                SelectResultStatus status = result.Status as SelectResultStatus;
                if (status.Op != SelectResultStatus.Operate.None
                    && gui_multi_select_.SelectNodes != null)
                {
                    for (int i = 0; i < gui_multi_select_.SelectNodes.Count; ++i)
                    {
                        Node node = gui_multi_select_.SelectNodes[i] as Node;
                        if (status.Op == SelectResultStatus.Operate.Compress)
                            node.AssetBundle.IsCompress = status.IsCompress;
                        if (status.Op == SelectResultStatus.Operate.General)
                            node.AssetBundle.IsGeneral = status.IsGeneral;
                        //else if (status.Op == SelectResultStatus.Operate.Permanent)
                        //    node.AssetBundle.IsPermanent = status.IsPermanent;
                    }
                }
            }
        }

        #endregion

        internal void OnDisable()
        {
            SaveData();
        }

        internal void OnEnable(EditorWindow parent)
        {
            this.parent = parent;

            m_TargetContent = new GUIContent("Build Target", "Choose target platform to build for.");

            LoadData();
        }

        internal void OnGUI()
        {
            GUILayout.Space(3f);
            GUILayout.BeginHorizontal();


            AssetBundleBuildTab.ValidBuildTarget tgt =
                (AssetBundleBuildTab.ValidBuildTarget) EditorGUILayout.EnumPopup(m_TargetContent,
                    m_UserData.m_BuildTarget);
            if (tgt != m_UserData.m_BuildTarget)
            {
                m_UserData.m_BuildTarget = tgt;
                if (m_UserData.m_UseDefaultPath)
                {
                    m_UserData.m_OutputPath = "AssetBundles/";
                    m_UserData.m_OutputPath += m_UserData.m_BuildTarget.ToString();
                    m_UserData.m_OutputPath += "/AssetBundle";
                    //EditorUserBuildSettings.SetPlatformSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), "AssetBundleOutputPath", m_OutputPath);
                }

                LoadData();
            }


            GUILayout.Label("资源版本号：", GUILayout.Width(96f));
            if (ManifestData.RawData.Version == 0)
            {
                ManifestData.RawData.Version = GetResVersion();
            }

            EditorGUI.BeginChangeCheck();

            ManifestData.RawData.Version = (uint) EditorGUILayout.LongField( ManifestData.RawData.Version);
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Manifest PATH:" + RESOURCES_MANIFEST_FILE_PATH);
            var ori = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            isStrictMode = GUILayout.Toggle(isStrictMode, " 是否开启严格模式");
            var isAll = GUILayout.Toggle(isAllSelect, "全选通用资源");
            if (isAllSelect != isAll)
            {
                isAllSelect = isAll;
                foreach (var ab in ManifestData.RawData.AssetBundles.Values)
                {
                    //过滤主AssetBundle文件
                    if (ab.AssetBundleName == AssetBundleBrowser.AssetBundlePacker.Common.MAIN_MANIFEST_FILE_NAME)
                        continue;
                    ab.IsGeneral = isAll;
                }
            }
            GUI.backgroundColor = ori;
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("编号", "OL Title", GUILayout.Width(32f));
            GUILayout.Label("资源", "OL Title");
            GUILayout.Label("大小", "OL Title", GUILayout.Width(124f));
            GUILayout.Label("压缩", "OL Title", GUILayout.Width(72f));
            GUILayout.Label("处理优先级", "OL Title", GUILayout.Width(72));
            GUILayout.Label("通用资源", "OL Title", GUILayout.Width(72));
            GUILayout.Label("CRC校验码", "OL Title", GUILayout.Width(112));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayoutMultiSelectGroup.OperateResult result = gui_multi_select_.Draw();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            

            if (result != null)
                UpdateSelectOperate(result);

            if (EditorGUI.EndChangeCheck())
            {
                DebugUtils.Log("Saved Data");
                SaveData(true);
            }

            // bool restore = GUILayout.Button("还原");
            bool execute = GUILayout.Button("执行修改（拷贝初始文件至安装包目录）");
            bool zip = GUILayout.Button("创建服务器zip文件");


            // if (restore)
            // LoadData();
            if (execute)
            {
                Utils.Clear();
                ExecuteModified();
            }

            if (zip)
                CreateServerZip();
        }
    }
}