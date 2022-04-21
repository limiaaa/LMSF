using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class SettingAsset : ScriptableObject
    {
#if UNITY_EDITOR
        [Header("通用 配置项")]
#endif
        [Tooltip("语言")]
        public SystemLanguage Language = SystemLanguage.English;

        [Tooltip("是否是单个游戏")] public bool isSingleGameApp = false;
        [Tooltip("是否从Assetbundle加载")] public bool isLoadFromAssetBundle = false;
        [Tooltip("是否是Debug模式")] public bool isDebug = false;
        [Tooltip("游戏名")] public string GameName = "GameTemplate";
        [Tooltip("容器的名字")] public string MainAppName = "candygame";
        [Tooltip("android的签名验证")] public string signature = "90BA30455CCA35BF61B618E5CE734D13"; //android的签名验证
        [Tooltip("是否在测试模式中使用正式服务器")] public bool isUseProductionServerInDebug = false;
        [Tooltip("打包进StreamingAssets的游戏")] public List<string> nativeInstallGame = new List<string>();
        [Tooltip("是否需要热更新(仅IOS)")] public bool IsNeedHotFix = true;
        [Tooltip("是否实时Hotfix")] public bool IsRealtimeHotFix = false;

        [Tooltip("是否需要从服务器更新Summary Json")] public bool IsUsingServer = false;
        [Tooltip("是否清除旧包的AssetBundle数据")] public bool IsClearCache = false;
        [Tooltip("简便键值对")] public keyValuePair[] PromoteGames;
        [Tooltip("是否启用日志")] public bool IsLogEnabled = false;
        [Tooltip("隐私政策链接")] public string code1 = "新广出审【0000】9999号";
        [Tooltip("出版物号")] public string code2 = "ISBN 999-9-999-9999-9";
        [Tooltip("游戏著作权人")] public string code3 = "游戏著作权人:成都力XX技有限公司";
        [Tooltip("出版服务单位")] public string code4 = "出版服务单位:xxxx科技有限公司";
        [Tooltip("文网文编号")] public string code5 = "川网文〔999〕999-999号";
        [Tooltip("文化部备案编号")] public string code6 = "文网游备字xxxxx号";

#if UNITY_EDITOR

        [Header("WebGL 配置项")]
        [Space(40)]
#endif
        [Tooltip("RPK后台资源url")]
        public string BaseUrl = "";

        [Tooltip("FTP服务器放置目录")] public string RootFolder = "";
        [Tooltip("Build导出时间（自动设置）")] public string BuildTime = "";
        [Tooltip("Build导出时间（自动设置）")] public bool IsLoadFromWeb = false;
        [Tooltip("BuildDingTalkNotify")] public bool IsBuildNotify = true;

        public string GetBaseUrl()
        {
#if GZ_TEST
            var baseUrl = "http://localhost:8001";
            return baseUrl + "/" + RootFolder + "/" + BuildTime + "/";
#endif
            return BaseUrl + "/" + RootFolder + "/" + BuildTime + "/";
        }


        //////////////////////////////////////////////////////////////////////////
        // Unity Editor 方便测试
        //////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR

        [Header("APP_STORE  GOOGLE_PLAY  OPPO  HUAWEI")] [Header("测试 配置项")] [Tooltip("渠道名")] [Space(20)]
        public string channelName = "APP_STORE";
#endif
        public string GetCopyRightString()
        {
            string format = "{0} {1} {2}\n{3} {4} {5}";

            return string.Format(format, code6, code3, code2, code1, code4, code5);
        }
    }

    [Serializable]
    public class keyValuePair
    {
        [SerializeField] public string key;
        [SerializeField] public string value;
    }
}