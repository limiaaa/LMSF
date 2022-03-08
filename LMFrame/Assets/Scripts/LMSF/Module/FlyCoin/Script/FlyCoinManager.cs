using LMSF.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LMSF.Module
{

    public class FlyCoinManager : MonoSingleton<FlyCoinManager>
    {
        string FlyObjPath = Global.FlyCoinPath;
        //public void FlyCoin(Vector3 StartPos, Vector3 EndPos, int FlyCoin, Action EndFunc = null)
        //{
        //    GameObject FlyObj = ResourcesManager.Load(FlyObjPath) as GameObject;
        //    FlyObj = Instantiate<GameObject>(FlyObj);
        //    FlyObj.transform.SetParent(UIManager.Instance.GetCanvasLayer(PageType.PopBox), false);
        //    FlyObj.transform.position = StartPos;
        //    StartFly(FlyObj, EndPos, FlyCoin, EndFunc);
        //    SoundMgr.Instance.PlaySoundMixEffect(SoundTypes.fly.ToString());
        //}

        ///// <summary>
        ///// 整合飞金币与跑金币行为(注意：需在飞金币前将金币结算完，未写进来是因为不好埋金币来源点)
        ///// </summary>
        ///// <param name="StartPos"></param>
        ///// <param name="EndPos"></param>
        ///// <param name="FlyCoin"></param>
        ///// <param name="AddNumber"></param>
        ///// <param name="CoinText"></param>
        ///// <param name="FlyEndFunc"></param>
        ///// <param name="RunCoinEndFunc"></param>
        //public void FlyCoinWithRunNumber(Vector3 StartPos, Vector3 EndPos, int FlyCoin, int AddNumber, Text CoinText, Action FlyEndFunc = null, Action RunCoinEndFunc = null)
        //{
        //    GameObject FlyObj = ResourcesManager.Load(FlyObjPath) as GameObject;
        //    FlyObj = Instantiate<GameObject>(FlyObj);
        //    FlyObj.transform.SetParent(UIManager.Instance.GetCanvasLayer(PageType.PopBox), false);
        //    FlyObj.transform.position = StartPos;
        //    int nowcoin = LocalDataMgr.Instance.GetItemNum(GameItemType.Coin);
        //    Action func = () =>
        //    {
        //        int lastcoin = nowcoin - AddNumber;
        //        if (CoinText == null)
        //            CoinText = UITopManager.Instance.GetTopCoinText();
        //        FXMgr.Instance.ShowTweenText(CoinText, lastcoin, AddNumber, 0, 0.6f, () =>
        //     {

        //            RunCoinEndFunc?.Invoke();
        //            UITopManager.Instance.RefeshTop();
        //        });
        //        FlyEndFunc?.Invoke();
        //    };
        //    StartFly(FlyObj, EndPos, FlyCoin, func);
        //    SoundMgr.Instance.PlaySoundMixEffect(SoundTypes.fly.ToString());
        //}
        //void StartFly(GameObject FlyObj, Vector3 EndPost, int FlyCoin, Action EndFunc)
        //{
        //    FlyCoinHelper FlyComponent = FlyObj.GetComponent<FlyCoinHelper>();
        //    FlyObj.gameObject.SetActive(true);
        //    FlyComponent.BoombToCollectCurrency(FlyCoin, EndPost, EndFunc);
        //}
    }
}
