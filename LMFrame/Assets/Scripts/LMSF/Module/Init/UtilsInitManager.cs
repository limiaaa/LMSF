using System.Collections;
using System.Collections.Generic;
using LMSF.Utils;
using UnityEngine;

/// <summary>
/// 处理工具的初始化,在进入游戏时调用
/// </summary>
public class UtilsInitManager : MonoSingleton<UtilsInitManager>
{
    public void InitManager()
    {
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
