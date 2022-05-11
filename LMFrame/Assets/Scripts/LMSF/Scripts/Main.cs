using System;
using DG.Tweening;
using SG.AssetBundleBrowser.AssetBundlePacker;
using SG.Haptics;
using SG.UI;
using SG.Utils;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    IEnumerator Start()
    {
        SystemAndDebugInit();
        GameUtilInit();
        ResourceChangePath();

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

        //开始资源加载
        if (SG.SettingReader.ScriptableObject.isLoadFromAssetBundle || !Application.isEditor)
        {
            SceneResourcesManager.LoadPattern = new AssetBundleLoadPattern();
            ResourcesManager.LoadPattern = new AssetBundleLoadPattern();
        }
        else
        {
            SceneResourcesManager.LoadPattern = new DefaultLoadPattern();
            ResourcesManager.LoadPattern = new DefaultLoadPattern();
        }
        StartCoroutine(ResourceChangePath());
    }

    void ResourceChangePath()
    {
        UILoadingAssetSub loadingui = UIManager.Instance.OpenPage<UILoadingAssetSub>();
        float loadingValue = 0;
        if (AssetBundleManager.Instance.GetIsNeedCopyFiles())
        {
            while (AssetBundleManager.Instance.GetNowLoadingValue() < 0.9f)
            {
                loadingui.SetProgress(20f);
            }
            loadingValue = 20;
        }



        TimeManager.Instance.LogTimeData();
        AsyncOperation op = SceneManager.LoadSceneAsync("game");
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            ;
        }

        HapticsManager.Init();
        HapticsManager.Active = LocalJsonDataUtils.Instance.gameData.IsVibration;
        op.allowSceneActivation = true;
        UtilsInitManager.Instance.InitAfterUIInit();
        PreloadManager.Instance.Init();
    }

    void SetProgress(float loadingValue)
    {
        Tween tween = DOTween.To(() => loadingValue, x => loadingValue = x, 100, 1.5f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            loadingui.SetProgress(loadingValue / 100);
        });
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
