using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        SystemAndDebugInit();
        GameUtilInit();


    }
    void SystemAndDebugInit()
    {
        bool isDebug = SG.SettingReader.ScriptableObject.IsLogEnabled;
        DebugUtils.IsOpenLog = isDebug;
        Debug.unityLogger.logEnabled = isDebug;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Application.logMessageReceived += UnityLogHandler;

    }
    void GameUtilInit()
    {
        //TODO配置表初始化

        //读档
        LocalJsonDataUtils.Instance.LoadAll();
        //工具类初始化
        UtilsInitManager.Instance.InitManager();
        //开启全生命周期迭代，适用于数值的改变,UI刷新需手动控制
        DelayRunManager.Instance.StartTimeDelay();
        UIManager.Instance.Init("UI/UI_Root", true, UIPageLoadType.ForceResources);

    }




    private void UnityLogHandler(string condition, string stackTrace, LogType type)
    {
        throw new NotImplementedException();
    }
    private void Update()
    {
        //捕获Android退出游戏
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyUp(KeyCode.Escape)))
        {
            //GameEventMgr.Instance.SendEvent(GameEventType.QuitGame, null);
        }
        //WindowsEditor模式模拟退出键
        if (Application.platform == RuntimePlatform.WindowsEditor && (Input.GetMouseButtonUp(1) == true))
        {
            //GameEventMgr.Instance.SendEvent(GameEventType.QuitGame, null);
        }
    }

}
