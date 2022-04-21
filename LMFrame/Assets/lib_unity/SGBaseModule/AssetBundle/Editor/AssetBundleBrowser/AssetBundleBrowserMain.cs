using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("Unity.AssetBundleBrowser.Editor.Tests")]

namespace SG.AssetBundleBrowser
{
    public class AssetBundleBrowserMain : EditorWindow, IHasCustomMenu, ISerializationCallbackReceiver
    {
        private static AssetBundleBrowserMain s_instance = null;

        internal static AssetBundleBrowserMain instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = GetWindow<AssetBundleBrowserMain>();
                return s_instance;
            }
        }

        internal const float kButtonWidth = 150;

        enum Mode
        {
            Browser,
            Builder,
            Inspect,
            Config,
            Upload,
            UploadResource
        }

        [SerializeField] Mode m_Mode = Mode.Builder;

        [SerializeField] int m_DataSourceIndex;

        [SerializeField] public AssetBundleManageTab m_ManageTab;

        [SerializeField] public AssetBundleBuildTab m_BuildTab;

        [SerializeField] public AssetBundleInspectTab m_InspectTab;

        [SerializeField] public AssetBundleConfigTab m_ConfigTab;

        [SerializeField] public AssetBundleUploadTab m_UploadTab;
        
        [SerializeField] public AssetBundleUploadResourceTab m_UploadResourceTab;

        private Texture2D m_RefreshTexture
        {
            get { return EditorGUIUtility.FindTexture("Refresh"); }
        }

        const float k_ToolbarPadding = 15;
        const float k_MenubarPadding = 32;

        [MenuItem("Window/AssetBundle Browser", priority = 2050)]
        public static void ShowWindow()
        {
            s_instance = null;
            instance.titleContent = new GUIContent("AssetBundles");
            instance.Show();
        }

        [SerializeField] internal bool multiDataSource = false;
        List<AssetBundleDataSource.ABDataSource> m_DataSourceList = null;

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (menu != null)
                menu.AddItem(new GUIContent("Custom Sources"), multiDataSource, FlipDataSource);
        }

        internal void FlipDataSource()
        {
            multiDataSource = !multiDataSource;
        }

        public void OnEnable()
        {
            Rect subPos = GetSubWindowArea();
            if (m_ManageTab == null)
                m_ManageTab = new AssetBundleManageTab();
            m_ManageTab.OnEnable(subPos, this);
            if (m_BuildTab == null)
                m_BuildTab = new AssetBundleBuildTab();
            m_BuildTab.OnEnable(this);
            if (m_InspectTab == null)
                m_InspectTab = new AssetBundleInspectTab();
            m_InspectTab.OnEnable(subPos);
            if (m_ConfigTab == null)
            {
                m_ConfigTab = new AssetBundleConfigTab();
            }

            m_ConfigTab.OnEnable(this);

            if (m_UploadTab == null)
            {
                m_UploadTab = new AssetBundleUploadTab();
            }

            m_UploadTab.OnEnable(this);

            if (m_UploadResourceTab == null)
            {
                m_UploadResourceTab = new AssetBundleUploadResourceTab();
            }

            m_UploadResourceTab.OnEnable(this);

            InitDataSources();
        }

        private void InitDataSources()
        {
            //determine if we are "multi source" or not...
            multiDataSource = false;
            m_DataSourceList = new List<AssetBundleDataSource.ABDataSource>();
            foreach (var info in AssetBundleDataSource.ABDataSourceProviderUtility.CustomABDataSourceTypes)
            {
                m_DataSourceList.AddRange(
                    info.GetMethod("CreateDataSources").Invoke(null, null) as List<AssetBundleDataSource.ABDataSource>);
            }

            if (m_DataSourceList.Count > 1)
            {
                multiDataSource = true;
                if (m_DataSourceIndex >= m_DataSourceList.Count)
                    m_DataSourceIndex = 0;
                AssetBundleModel.Model.DataSource = m_DataSourceList[m_DataSourceIndex];
            }
        }

        private void OnDisable()
        {
            if (m_BuildTab != null)
                m_BuildTab.OnDisable();
            if (m_InspectTab != null)
                m_InspectTab.OnDisable();
            if (m_ConfigTab != null)
                m_ConfigTab.OnDisable();
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }

        private Rect GetSubWindowArea()
        {
            float padding = k_MenubarPadding;
            if (multiDataSource)
                padding += k_MenubarPadding * 0.5f;
            Rect subPos = new Rect(0, padding, position.width, position.height - padding);
            return subPos;
        }

        private void Update()
        {
            switch (m_Mode)
            {
                case Mode.Builder:
                    break;
                case Mode.Inspect:
                    break;
                case Mode.Config:
                    break;
                case Mode.Upload:
                    break;
                case Mode.Browser:

                default:
                    m_ManageTab.Update();
                    break;
            }
        }

        private void OnGUI()
        {
            ModeToggle();

            switch (m_Mode)
            {
                case Mode.Builder:
                    m_BuildTab.OnGUI();
                    break;
                case Mode.Inspect:
                    m_InspectTab.OnGUI(GetSubWindowArea());
                    break;
                case Mode.Config:
                    m_ConfigTab.OnGUI();
                    break;
                case Mode.Upload:
                    m_UploadTab.OnGUI();
                    break;
                case Mode.UploadResource:
                    m_UploadResourceTab.OnGUI();
                    break;
                default:
                    m_ManageTab.OnGUI(GetSubWindowArea());
                    break;
            }
        }

        void ModeToggle()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(k_ToolbarPadding);
            bool clicked = false;
            Mode oldMode = m_Mode;

            switch (m_Mode)
            {
                case Mode.Browser:
                    clicked = GUILayout.Button(m_RefreshTexture);
                    if (clicked)
                        m_ManageTab.ForceReloadData();
                    break;
                case Mode.Builder:
                    GUILayout.Space(m_RefreshTexture.width + k_ToolbarPadding);
                    break;
                case Mode.Inspect:
                    clicked = GUILayout.Button(m_RefreshTexture);
                    if (clicked)
                        m_InspectTab.RefreshBundles();
                    break;
            }

            float toolbarWidth = position.width - k_ToolbarPadding * 4 - m_RefreshTexture.width;
            //string[] labels = new string[2] { "Configure", "Build"};
            string[] labels = {"Browser", "Build", "Inspect", "Config AssetBundle", "Upload Game","Upload Resource"};
            m_Mode = (Mode) GUILayout.Toolbar((int) m_Mode, labels, "LargeButton", GUILayout.Width(toolbarWidth));

            if (oldMode != m_Mode)
            {
//                Debug.LogError(oldMode +" -> " + m_Mode);
                switch (oldMode)
                {
                    case Mode.Browser:
                        break;
                    case Mode.Builder:
                        m_BuildTab.SaveData();
                        break;
                    case Mode.Inspect:
//                        m_InspectTab.SaveEditorData();
                        break;
                    case Mode.Config:
                        m_ConfigTab.SaveData();
                        break;
                    case Mode.Upload:
                        m_UploadTab.SaveData();
                        break;
                    case Mode.UploadResource:
                        m_UploadResourceTab.SaveData();
                        break;
                }


                // 切换tab之后强制刷新数据
                switch (m_Mode)
                {
                    case Mode.Browser:
                        break;
                    case Mode.Builder:
                        m_BuildTab.LoadData();
                        break;
                    case Mode.Inspect:
                        break;
                    case Mode.Config:
                        m_ConfigTab.LoadData();
                        break;
                    case Mode.Upload:
                        m_UploadTab.LoadData();
                        break;
                    case Mode.UploadResource:
                        m_UploadResourceTab.LoadData();
                        break;
                }

                oldMode = m_Mode;
            }


            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (multiDataSource)
            {
                //GUILayout.BeginArea(r);
                GUILayout.BeginHorizontal();

                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    GUILayout.Label("Bundle Data Source:");
                    GUILayout.FlexibleSpace();
                    var c = new GUIContent(
                        string.Format("{0} ({1})", AssetBundleModel.Model.DataSource.Name,
                            AssetBundleModel.Model.DataSource.ProviderName), "Select Asset Bundle Set");
                    if (GUILayout.Button(c, EditorStyles.toolbarPopup))
                    {
                        GenericMenu menu = new GenericMenu();

                        for (int index = 0; index < m_DataSourceList.Count; index++)
                        {
                            var ds = m_DataSourceList[index];
                            if (ds == null)
                                continue;

                            if (index > 0)
                                menu.AddSeparator("");

                            var counter = index;
                            menu.AddItem(new GUIContent(string.Format("{0} ({1})", ds.Name, ds.ProviderName)), false,
                                () =>
                                {
                                    m_DataSourceIndex = counter;
                                    var thisDataSource = ds;
                                    AssetBundleModel.Model.DataSource = thisDataSource;
                                    m_ManageTab.ForceReloadData();
                                }
                            );
                        }

                        menu.ShowAsContext();
                    }

                    GUILayout.FlexibleSpace();
                    if (AssetBundleModel.Model.DataSource.IsReadOnly())
                    {
                        GUIStyle tbLabel = new GUIStyle(EditorStyles.toolbar);
                        tbLabel.alignment = TextAnchor.MiddleRight;

                        GUILayout.Label("Read Only", tbLabel);
                    }
                }

                GUILayout.EndHorizontal();
                //GUILayout.EndArea();
            }
        }
    }
}