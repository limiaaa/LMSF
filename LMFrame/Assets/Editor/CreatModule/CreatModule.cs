using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class CreatModule : Editor  
{
    public static string TempModulePath = "Assets/Editor/CreatModule/ModuleTemp/TempModule.txt";
    public static string TempViewtPath = "Assets/Editor/CreatModule/ModuleTemp/TempView.txt";
    public static string TempDataPath = "Assets/Editor/CreatModule/ModuleTemp/TempData.txt";
    public static string TempPrefebsPath = "Assets/Editor/CreatModule/ModuleTemp/TempPrefebs.prefab";
    public static string TempModuleListPath = "Assets/Editor/CreatModule/ModuleTemp/TempModuleNameList.txt";

    public static string TargetPrefebsPath = "Assets/MainApp/Art/Prefabs/UI/{0}.prefab";
    public static string TargetCsPath = "Assets/MainApp/Scripts/Module/{0}";

    public static string TargetScriptPath = "Assets/MainApp/Scripts";
    public static string TargetModulePath = "Assets/MainApp/Scripts/Module";
    //模块记录
    public static string TargetModuleListPath = "Assets/MainApp/Scripts/Module/Core/ModuleStaticTypes.cs";


    [MenuItem("Tools/源计划:模块创建")]
    static public void ModuleCreat()
    {
        CreatWindows.WindowsOpen();
    }
}

public class CreatWindows : EditorWindow
{
    static CreatWindows window;
    static public void WindowsOpen()
    {
        Creater = PlayerPrefs.GetString("Creater", "");
        Explain = PlayerPrefs.GetString("Explain", "");
        window = EditorWindow.GetWindow<CreatWindows>("模块创建");
    }
    //模块名
    public static string moduleName = "";
    //需要奖励接口
    public static bool needRewardAdInterface = false;
    //需要插屏接口
    public static bool needInsertAdInterface = false;
    //需要安全区域
    public static bool needSafeArea = false;
    //开关需要动画
    public static bool needAnimation = false;
    //layer等级
    public static int openTag = 0;
    //是否添加data模块
    public static bool addDataModule = false;
    public static string[] LayerTag = { "Map", "Normal", "Fixed", "PopBox", "Effect" };
    public static int[] LayerTagInt = { 0, 1, 2, 3, 4 };

    public static string Creater = "";
    public static string Intro = "";
    public static string Explain = "";

    private void OnDestroy()
    {

    }
    private void OnGUI()
    {


        moduleName = EditorGUILayout.TextField("ModuleName",moduleName);
        needRewardAdInterface = EditorGUILayout.Toggle("是否需要激励广告接口", needRewardAdInterface);
        needInsertAdInterface = EditorGUILayout.Toggle("是否需要插屏广告接口", needInsertAdInterface);
        needSafeArea =  EditorGUILayout.Toggle("是否需要安全区域设置", needSafeArea);
        needAnimation = EditorGUILayout.Toggle("是否需要动画控制", needAnimation);
        openTag = EditorGUILayout.IntPopup("Layer等级:", openTag, LayerTag, LayerTagInt);
        addDataModule = EditorGUILayout.Toggle("是否添加Data模块:", addDataModule);

        Creater = EditorGUILayout.TextField("模块创建者：", Creater);
        Intro = EditorGUILayout.TextField("模块简介", Intro);
        Explain = EditorGUILayout.TextField("补充说明", Explain);
        if (GUILayout.Button("创建模块"))
        {
            if (moduleName=="")
            {
                if (window != null)
                    window.ShowNotification(new GUIContent("模块名不能为空"));
                return;
            }
            List<string> moduleList = Directory.GetDirectories(CreatModule.TargetModulePath).ToList();
            for(int i = 0; i <= moduleList.Count-1; i++)
            {
                moduleList[i]= moduleList[i].Replace(@"\","/");
            }
            if (moduleList.Contains(string.Format(CreatModule.TargetCsPath, moduleName)))
            {
                if (window != null)
                    window.ShowNotification(new GUIContent("已经有此模块了"));
                return;
            }
            Creating();
            if(window!=null)
            window.ShowNotification(new GUIContent("加油吧少年"));

            PlayerPrefs.SetString("Creater", Creater);
            PlayerPrefs.SetString("Explain", Explain);
        }
    }
    //开始读取创建
    static public void Creating()
    {
        if (!File.Exists(CreatModule.TargetModulePath))
        {
            Directory.CreateDirectory(CreatModule.TargetModulePath);
        }
        //1.创建文件夹
        DirectoryInfo moduleDirectory = Directory.CreateDirectory(string.Format(CreatModule.TargetCsPath, moduleName));
        //进行代码模块Module块创建修改
        string module_Content = File.ReadAllText(CreatModule.TempModulePath, System.Text.Encoding.UTF8);
        moduleTempTxtChange(string.Format("{0}/M_{1}.cs", moduleDirectory.FullName, moduleDirectory.Name), module_Content);
        //进行模块View块创建修改
        string view_Content = File.ReadAllText(CreatModule.TempViewtPath, System.Text.Encoding.UTF8);
        viewTempTxtChange(string.Format("{0}/V_{1}.cs", moduleDirectory.FullName, moduleDirectory.Name), view_Content);
        if (addDataModule)
        {
            //进行模块Data块创建修改
            string data_Content = File.ReadAllText(CreatModule.TempDataPath, System.Text.Encoding.UTF8);
            dataTempTxtChange(string.Format("{0}/D_{1}.cs", moduleDirectory.FullName, moduleDirectory.Name), data_Content);
        }
        //进行UI创建
        prefabTempChange();


        ForeachRecordModuleName();
        AssetDatabase.Refresh();
    }
    static void moduleTempTxtChange(string targetPath, string content)
    {
        content = content.Replace("#MODULENAME#", moduleName);
        content = content.Replace("#CREATER#", Creater);
        content = content.Replace("#INTRO#", Intro);
        content = content.Replace("#EXPLAIN#", Explain);
        File.WriteAllText(targetPath, content, System.Text.Encoding.UTF8);
    }
    static void viewTempTxtChange(string targetPath, string content)
    {

        if (needRewardAdInterface)
        {
            content = content.Replace("#REWARDADINTERFACE#", ",IListenerVideoAd");
        }
        else
        {
            content = content.Replace("#REWARDADINTERFACE#", "");
        }
        if (needInsertAdInterface)
        {
            content = content.Replace("#INSERTADINTERFACE#", ",IListenerInterstitialAd");
        }
        else
        {
            content = content.Replace("#INSERTADINTERFACE#", "");
        }

        if (needRewardAdInterface|| needInsertAdInterface)
        {
            content = content.Replace("#ADTYPE#", "SDKManager.MY_AD_TYPE AdType;");
            content = content.Replace("#ADBLOCK#", GetAdBlock());
        }
        else
        {
            content = content.Replace("#REWARDADINTERFACE#", "");
            content = content.Replace("#INSERTADINTERFACE#", "");
            content = content.Replace("#ADTYPE#", "");
            content = content.Replace("#ADBLOCK#", "");
        }
        content = content.Replace("#SAFEAREA#", needSafeArea.ToString().ToLower());
        content = content.Replace("#MODULENAME#", moduleName);
        content = content.Replace("#LAYER#", LayerTag[openTag]);
        if (needAnimation)
        {
            content = content.Replace("#ANIMATION#", ",mPageAnimation = typeof(UITweenAnimation)");
        }
        else
        {
            content = content.Replace("#ANIMATION#", "");
        }
        if (addDataModule)
        {
            content = content.Replace("#DATA#", string.Format("D_{0} moduleData;",moduleName));
            content = content.Replace("#NEWDATA#", string.Format("moduleData=new D_{0}();",moduleName));
        }
        else
        {
            content = content.Replace("#DATA#", "");
            content = content.Replace("#NEWDATA#", "");
        }
        content = content.Replace("#CODETEMP#", GetTempCode());
        File.WriteAllText(targetPath, content, System.Text.Encoding.UTF8);
    } 
    static void dataTempTxtChange(string targetPath, string content)
    {
        content = content.Replace("#MODULENAME#", moduleName);
        File.WriteAllText(targetPath, content, System.Text.Encoding.UTF8);
    }
    static void prefabTempChange()
    {
        string prefebs_Content = File.ReadAllText(CreatModule.TempPrefebsPath, System.Text.Encoding.UTF8);
        File.WriteAllText(string.Format(CreatModule.TargetPrefebsPath,"UI_"+moduleName+"Page"), prefebs_Content, System.Text.Encoding.UTF8);
    }

    //广告抽象实现
    static string GetAdBlock()
    {
        return @"        
    public void OnAdLoaded()
    {
    }
    public void OnAdLoadFailed()
    {
    }
    public void OnAdShowFailed()
    {
    }
    public void OnAdRewarded()
    {
    }
    public void OnAdClicked()
    {
    }
    public void OnAdShowed()
    {
    }
    public void OnAdClosed()
    {
        if (SDKManager.Instance.CanReward)
        {

        }
    }
        ";
    }
    //示例代码
    static string GetTempCode()
    {
        return @"        
    //************************部分代码示例*********************
    //发送事件
    //GameEventMgr.Instance.SendEvent(GameEventType.XX, new GameEventMgr.GameEventArgs() { data = true });
    //播放激励视频
    //if (SDKManager.Instance.UIPlayRewardVideo(SDKManager.MY_AD_TYPE.XX, this,EventName))
    //{
    //    ADType = SDKManager.MY_AD_TYPE.XX; 
    //}
    //播放插屏视频
    //SDKManager.Instance.PlayInterstitialAd(this, EventName);
        ";
    }

    static void ForeachRecordModuleName()
    {
        string module_Content = File.ReadAllText(CreatModule.TempModuleListPath, System.Text.Encoding.UTF8);
        string[] moduleList = Directory.GetDirectories(CreatModule.TargetModulePath);
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i <= moduleList.Length - 1; i++)
        {
            moduleList[i] = moduleList[i].Split('\\')[1];
            if (moduleList[i] != "Core")
            {
                stringBuilder.Append(string.Format("\ttypeof(M_{0}),", moduleList[i]));
                stringBuilder.Append("\n");
            }
        }
        Debug.Log(stringBuilder.ToString());
        module_Content = module_Content.Replace("#MODULELIST#", stringBuilder.ToString());
        File.WriteAllText(CreatModule.TargetModuleListPath, module_Content, System.Text.Encoding.UTF8);
    }


}
