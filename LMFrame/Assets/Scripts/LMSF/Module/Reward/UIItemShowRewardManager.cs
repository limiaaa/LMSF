//using SG.UI;
//using SG.Utils;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class UIItemShowRewardManager : MonoSingleton<UIItemShowRewardManager>
//{
//    UIItemShowRewardPage ItemRewardPage;
//    public void ShowCombinRwaredItem(string[] itemIdList, Action ShowFunc, Action ClickFunc, string actName, Vector3 StartFlyPos = new Vector3(), Vector3 EndFlyPos = new Vector3(), float maskComeTime = 0.8f, float rewardPopTime = 3f)
//    {
//        ItemRewardPage = UIManager.Instance.OpenPage<UIItemShowRewardPage>();
//        DelayTimeManager.Instance.delay_time_run(0.1f, () =>
//        {
//            ItemRewardPage.StartShowItemReward(itemIdList, ShowFunc, ClickFunc, actName, StartFlyPos, EndFlyPos, maskComeTime, rewardPopTime);
//        });
//    }

//    public void SetRunText(Text text)
//    {
//        ItemRewardPage = UIManager.Instance.OpenPage<UIItemShowRewardPage>();
//        ItemRewardPage.SetRunText(text);
//    }


//}
