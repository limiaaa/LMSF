using System;
using DG.Tweening;

using SG.Haptics;
using SG.UI;
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
        SelfUtilsAndManagerInit();
        //启动资源加载
        UIManager.Instance.Init("UI/UI_Root", true, UIPageLoadType.ForceResources);
        //loadingui = UIManager.Instance.OpenPage<UILoadingAssetSub>();
        yield return new WaitForSeconds(0.2f);
        //AssetBundleManager.Instance.GetNowLoadingValue();
        //AssetBundleManager.Instance.GetNowLoadingValue();
        //bool needCopy = AssetBundleManager.Instance.GetIsNeedCopyFiles();
        //if (needCopy)
        //{
        //    while (AssetBundleManager.Instance.GetNowLoadingValue() < 0.9f)
        //    {
        //        yield return new WaitForSeconds(0.1f);
        //    }
        //}
        yield return DoSlider(80);
        AsyncOperation op = SceneManager.LoadSceneAsync("game");
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            ;
        }
        yield return DoSlider(100);
        InitAfterLoadScene();
        yield return new WaitForSeconds(0.2f);
        op.allowSceneActivation = true;
        //MyTest.Instance.StartMyTest();
        //MapItemEffective.Init();
    }
    void SystemAndOtherInit()
    {
        
        //正式开始游戏逻辑
        bool isDebug = GameSetting.Instance.DebugEnable;
        DebugUtils.IsOpenLog = isDebug;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.logMessageReceived += UnityLogHandler;
        LocalJsonDataUtils.Instance.LoadAll(); // 读档
        HapticsManager.Init();
        HapticsManager.Active = LocalJsonDataUtils.Instance.gameData.IsVibration;
        //路径初始化
        if (!GameSetting.Instance.IsDeveloper || !Application.isEditor)
        {

        }
        else
        {
          
        }
    }
    void SelfUtilsAndManagerInit()
    {
        //工具类初始化，比如迭代器
        UtilsInitManager.Instance.InitManager();
        //开启全生命周期迭代，适用于数值的改变,UI刷新需手动控制
        DelayRunManager.Instance.StartTimeDelay();
    }
    void InitAfterLoadScene()
    {
        TimeManager.Instance.LogTimeData();
        UtilsInitManager.Instance.InitAfterUIInit();
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
