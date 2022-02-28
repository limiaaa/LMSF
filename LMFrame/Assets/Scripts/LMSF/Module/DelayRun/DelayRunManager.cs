using LMSF.Utils;
using UnityEngine;

/// <summary>
/// 处理一些需要在开始游戏时进行迭代的函数
/// </summary>
public class DelayRunManager : MonoSingleton<DelayRunManager>
{
   //会随着游戏周期一直迭代，比如体力
    public void StartTimeDelay()
    {
        RecoverHeartByTime();
        RecoverCoinByAdByTime();
        RecoverInterstitialAdTime();
        RecoverReportUserProfileData();
        RecoverPlayerBuff();
    }
    //**************************************************************体力迭代
    private long RemindHeartTime = 0;
    private void RecoverHeartByTime()
    {
        Debug.Log("----------------开始体力迭代----------------");
        int timer = 0;
        //过去了多久+跑了多久
        long Usetime = TimeManager.Instance.ThisLoginAndLastExit+ LocalDataUtils.GetLocalData("HeartTime", 0);

        //先算出过去时间需要加多少体力
        AddHeart(Usetime);
        //再算出溢出值
        //int MoreTime = (int)(Usetime % (ConstConfig.HpReply * 60));
        //初始值就是溢出值
        //timer = MoreTime;
        //Debug.Log("溢出时间:" + MoreTime);
        TimeManager.Instance.AddFuncToTime("AddHeart", (Time) =>
        {
            //如果体力满了
            //if (LocalDataUtils.GetItemNum(GameItemType.Heart) >= ConstConfig.BeginingHp)
            //{
            //    RemindHeartTime = -1;
            //    timer = 0;
            //    return;
            //}
            //timer++;
            //RemindHeartTime = ConstConfig.HpReply * 60 - timer;
            //LocalDataFunc.Instance.SetLocalData("HeartTime", timer);
            //if (timer >= ConstConfig.HpReply*60)
            //{
            //    RemindHeartTime = 0;
            //    timer = 0;
            //    LocalDataMgr.Instance.AddItemNum(GameItemType.Heart, 1, "auto");
            //    UITopManager.Instance.RefeshTop();
            //}
        });
    }
    void AddHeart(long time)
    {
        //int NeedAdd = (int)(time / (ConstConfig.HpReply * 60));
        //if (LocalDataMgr.Instance.GetItemNum(GameItemType.Heart) + NeedAdd > ConstConfig.BeginingHp)
        //{
        //    LocalDataMgr.Instance.SetItemNum(GameItemType.Heart, ConstConfig.BeginingHp, "max_limit");
        //}
        //else
        //{
        //    LocalDataMgr.Instance.AddItemNum(GameItemType.Heart, NeedAdd, "auto");
        //}
    }
    public long GetHeartRemindTime()
    {
        return RemindHeartTime;
    }
    //**************************************************************看广告得金币迭代
    private long RemindCoinByAdTime = 0;
    private bool IsCoinByAdColdTime = false;
    private void RecoverCoinByAdByTime()
    {
        //long timer = 0;
        ////过去了多久+跑了多久
        //timer = TimeManager.Instance.ThisLoginAndLastExit + LocalDataUtils.GetLocalData("CoinByAdTime", 0);
        //if (LocalDataUtils.GetLocalData("CoinByAdTime", 0) != 0)
        //{
        //    IsCoinByAdColdTime=true;
        //}
        ////如果过去的时间已经过了冷却
        //if(timer>= ConstConfig.CoinTimeCd * 60)
        //{
        //    IsCoinByAdColdTime=false;
        //}
        //TimeManager.Instance.AddFuncToTime("CoinByAdTime", (Time) =>
        //{
        //    //如果每日金币次数没了，就不刷新了
        //    if (PropManager.Instance.GetDailyRemindNumber() <= 0)
        //    {
        //        RemindCoinByAdTime = 0;
        //        timer = 0;
        //        IsCoinByAdColdTime = false;
        //        return;
        //    }
        //    //如果不在冷却中
        //    if (!IsCoinByAdColdTime)
        //    {
        //        RemindCoinByAdTime = 0;
        //        timer = 0;
        //        return;
        //    }
        //    timer++;
        //    RemindCoinByAdTime = ConstConfig.CoinTimeCd * 60 - timer;
        //    LocalDataFunc.Instance.SetLocalData("CoinByAdTime", (int)timer);
        //    if (timer >= ConstConfig.CoinTimeCd * 60)
        //    {
        //        RemindCoinByAdTime = 0;
        //        timer = 0;
        //        IsCoinByAdColdTime = false;
        //        this.SendMsg(GameEventType.RefeshCoinShop, null);
        //    }
        //});
    }
    //获取看广告金币的冷却状态
    public bool GetCoinByAdColdState()
    {
        return IsCoinByAdColdTime;
    }
    //获取看广告金币的冷却倒计时
    public long GetCoinByAdColdTime()
    {
        return RemindCoinByAdTime;
    }
    public void SetCoinByAdColdState()
    {
        IsCoinByAdColdTime = true;
        //RemindCoinByAdTime = ConstConfig.CoinTimeCd * 60;
    }
    //**************************************************************插屏CD
    private void RecoverInterstitialAdTime()
    {
        //var AdCd = 0;
        //TimeManager.Instance.AddFuncToTime("InterstitialAd", (Time) =>
        //{
        //    if (SDKManager.Instance.CanInterstitialAd)
        //    {
        //        AdCd = 0;
        //        return;
        //    }
        //    AdCd++;
        //    // DebugUtils.Log("AD CD,{0},{1}", Color.green, AdCd, SDKManager.Instance.CanInterstitialAd);
        //    if (AdCd >= ConstConfig.ScreenAdsCd)
        //    {
        //        AdCd = 0;
        //        SDKManager.Instance.CanInterstitialAd = true;
        //    }
        //});
    }
    //**************************************************************定时发送玩家数据
    private void RecoverReportUserProfileData()
    {
        //var reportCd = 0;
        //TimeManager.Instance.AddFuncToTime("ReportUserProfile", (Time) =>
        //{
        //    reportCd++;
        //    if (reportCd >= LocalDataMgr.ReportUserProfileCD)
        //    {
        //        reportCd = 0;
        //        LocalDataMgr.Instance.UserProfileOnTimer();
        //    }
        //});
    }
    //**************************************************************检查玩家特殊增益
    private void RecoverPlayerBuff()
    {
        //TimeManager.Instance.AddFuncToTime("CheckPlayerBuff", (Time) =>
        //{
        //    PlayerBuffManager.Instance.CheckPlayerStateInDelayTime();
        //});
    }

}
