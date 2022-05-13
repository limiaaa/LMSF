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
    }
    //**************************************************************体力迭代
    private void RecoverHeartByTime()
    {
        Debug.Log("----------------开始体力迭代----------------");
        int DelyTimerManager = 0;
        //过去了多久+跑了多久
        long Usetime = TimeManager.Instance.ThisLoginAndLastExit+ LocalDataUtils.GetLocalData("HeartTime", 0);
        //先算出过去时间需要加多少体力
        //再算出溢出值
        //int MoreTime = (int)(Usetime % (ConstConfig.HpReply * 60));
        //初始值就是溢出值
        //DelyTimerManager = MoreTime;
        //Debug.Log("溢出时间:" + MoreTime);
        TimeManager.Instance.AddFuncToTime("AddHeart", (Time) =>
        {
            //如果体力满了
            //if (LocalDataUtils.GetItemNum(GameItemType.Heart) >= ConstConfig.BeginingHp)
            //{
            //    RemindHeartTime = -1;
            //    DelyTimerManager = 0;
            //    return;
            //}
            //DelyTimerManager++;
            //RemindHeartTime = ConstConfig.HpReply * 60 - DelyTimerManager;
            //LocalDataFunc.Instance.SetLocalData("HeartTime", DelyTimerManager);
            //if (DelyTimerManager >= ConstConfig.HpReply*60)
            //{
            //    RemindHeartTime = 0;
            //    DelyTimerManager = 0;
            //    LocalDataMgr.Instance.AddItemNum(GameItemType.Heart, 1, "auto");
            //    UITopManager.Instance.RefeshTop();
            //}
        });
    }



}
