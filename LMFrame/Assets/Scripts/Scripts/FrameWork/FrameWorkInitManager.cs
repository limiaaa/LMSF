using System;
using System.Collections;
using System.Collections.Generic;
using LMSF.Utils;
using SG.Haptics;
using UnityEngine;

/// <summary>
/// 处理工具的初始化,在进入游戏时调用
/// </summary>
public class FrameWorkInitManager : MonoSingleton<FrameWorkInitManager>
{
    public void InitManager()
    {
        LocalJsonDataUtils.LoadAll(); // 读档
        ManifestManager.Instance.InitManifestManager();
        InitHaptics();
        InitPathAndAssetBundle();
        TimeManager.Instance.InitGameFunc();
        LocalDataUtils.InitLocalDataFunc();
        RvCoinManager.Instance.RefeshRvCoinFunc();
        if (IAPManager.Instance.NeedShop)
        {
            IAPManager.Instance.InitIAP();
        }
        if (TimeManager.Instance.IsNewDay)
        {
            RefeshEveryDay();
        }
        //通过本机语言设置游戏语言
        if (PlayerPrefs.GetInt("SetDefaultLang", 0) == 0)
        {
            SetDefaultLang();
            PlayerPrefs.SetInt("SetDefaultLang", 1);
        }

        //开启全生命周期迭代，适用于数值的改变,UI刷新需手动控制
        DelayRunManager.Instance.StartTimeDelay();
    }
    public void InitAfterUIInit()
    {
        UIMaskManager.Instance.InitUIMask();
        //初始化玩家buff
        NewGuideManager.Instance.InitGuideManager();
        SomeThingRefeshByZeroTime();
        if (IAPManager.Instance.NeedShop)
        {
            //UIShopManager.Instance.InitUIShopManager();
        }
    }

    //初始化震动工具
    void InitHaptics()
    {
        //震动
        HapticsManager.Init();
        HapticsManager.Active = LocalJsonDataUtils.GetGameData().IsVibration;
    }

    //初始化路径工具
    void InitPathAndAssetBundle()
    {
        //路径初始化
        if (!GameSetting.Instance.IsDeveloper || !Application.isEditor)
        {
        }
        else
        {
        }
    }

    //设置默认语言
    void SetDefaultLang()
    {
        LangKey();
        string curLangKey = Global.Runtime.mRuntimeLanguage.ToString(); // TODO: 获取当前语言项
        string DefaultLang = Application.systemLanguage.ToString();
        for (int i = 0; i < Global.LANGUAGE_VALUES.Length; i++)
        {
            //有获取的本地语言
            if (LangMap.ContainsKey(Global.LANGUAGE_VALUES[i][0]))
            {
                if (DefaultLang == LangMap[Global.LANGUAGE_VALUES[i][0]])
                {
                    Global.GameLanguage _mLanguage = (Global.GameLanguage)Enum.Parse(typeof(Global.GameLanguage), Global.LANGUAGE_VALUES[i][0]);
                    Global.Runtime.mRuntimeLanguage = _mLanguage;
                }
            }
        }
    }
    Dictionary<string, string> LangMap = new Dictionary<string, string>();
    void LangKey()
    {
        LangMap.Add("en", "English");
        LangMap.Add("zh", "ChineseSimplified");
        LangMap.Add("jp", "Japanese");
        LangMap.Add("cht", "ChineseTraditional");
        LangMap.Add("kor", "Korean");
        LangMap.Add("pt", "Portuguese");
        LangMap.Add("de", "German");
        LangMap.Add("spa", "Spanish");
        LangMap.Add("it", "Italian");
        LangMap.Add("fra", "French");
        LangMap.Add("ru", "Russian");
        LangMap.Add("cs", "Czech");
        LangMap.Add("dan", "Danish");
        LangMap.Add("nl", "Dutch");
    }


    //需要零点刷新的初始化
    void SomeThingRefeshByZeroTime()
    {
        //新签到相关
        //UIDailySignInManager.Instance.AddDailySignInModuleInRefeshEveryDay();
        //UITurnTableManager.Instance.AddTurnTableModuleInRefeshEveryDay();
    }
    //每次进入游戏如果是新的一天刷新
    public void RefeshEveryDay()
    {
        ////今天是否进入过每日签到 签到相关
        //LocalDataUtils.SetLocalData("DailySignInTodayIsEnter", 1);
        //LocalDataUtils.SetLocalData("TodayDailySingnInRewardIsGet", 1);
        ////今天是否进入过转盘
        //LocalDataUtils.SetLocalData("IsEnterTurnTableInToday", 1);
        ////刷新转盘次数
        //LocalDataUtils.SetLocalData("RemindWheelNumberEveryDay", ConstConfig.TurnTableNum);
    }
}
