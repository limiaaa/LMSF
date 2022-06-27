using System;
using DG.Tweening;

using SG.Haptics;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    //UILoadingAssetSub loadingui;
    IEnumerator Start()
    {
        DebugUtils.Log("**************设备信息相关deviceUniqueIdentifier：" + SystemInfo.deviceUniqueIdentifier);
        DebugUtils.Log("**************设备信息相关deviceName：" + SystemInfo.deviceName);
        DebugUtils.Log("**************设备信息相关deviceModel：" + SystemInfo.deviceModel);
        SystemAndOtherInit();
        //工具类初始化，比如迭代器,时间管理工具,本地存储等
        FrameWorkInitManager.Instance.InitManager();
        //启动资源加载
        UIManager.Instance.Init("UI/UI_Root", true, UIPageLoadType.ForceResources);
        //loadingui = UIManager.Instance.OpenPage<UILoadingAssetSub>();
        yield return DoSlider(100);
        InitAfterLoadScene();
        yield return new WaitForSeconds(0.2f);

    }
    //系统数据初始化
    void SystemAndOtherInit()
    {
        //正式开始游戏逻辑
        bool isDebug = GameSetting.Instance.DebugEnable;
        DebugUtils.IsOpenLog = isDebug;
        Debug.unityLogger.logEnabled = isDebug;
        //不休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //添加错误堆栈信息
        Application.logMessageReceived += UnityLogHandler;
    }
    //在切换场景后调用的方法
    void InitAfterLoadScene()
    {
        TimeManager.Instance.LogTimeData();
        FrameWorkInitManager.Instance.InitAfterUIInit();
        PreloadManager.Instance.Init();
        //UIManager.Instance.ClosePage<UILoadingAssetSub>(true);
    }



















    float currentProgressValue = 0;
    IEnumerator DoSlider(float value)
    {
        float waitTime = (value - currentProgressValue) / 100;
        Tween tween = DOTween.To(() => currentProgressValue, x => currentProgressValue = x, value, waitTime).SetEase(Ease.Linear).OnUpdate(() =>
        {
            //loadingui.SetProgress(currentProgressValue / 100);
        });
        yield return new WaitForSeconds(waitTime);
    }
    private void UnityLogHandler(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error)
        {

        }
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
