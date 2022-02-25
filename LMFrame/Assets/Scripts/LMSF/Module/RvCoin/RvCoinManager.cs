using LMSF.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RvCoinManager : MonoSingleton<RvCoinManager>
{
    int RvCoin=0;
    bool HasBuyOrAdBehaviour;
    public void RefeshRvCoinFunc()
    {
        //RvCoin = LocalDataManager.GetLocalData("RvCoin", ConstConfig.Rvcoin[0]);
    }
    public void AddRvCoin()
    {
        //RvCoin += ConstConfig.Rvcoin[1];
        //LocalDataManager.SetLocalData("RvCoin", RvCoin);
        ////SendAct.ActRVcoin("add","rv_revive");
        //Debug.Log("Rvcoin增加********************************：" + RvCoin);
        ////HasBuyOrAdBehaviour = true;
    }
    public void RefeshRvCoinAdState(bool state=false)
    {
        HasBuyOrAdBehaviour = state;
    }
    public void ReduceRvCoin()
    {
        ////如果没有进行期望行为
        ////if (HasBuyOrAdBehaviour)
        ////{
        ////    HasBuyOrAdBehaviour = false;
        ////    return;
        ////}
        //RvCoin-= ConstConfig.Rvcoin[2];
        //if (RvCoin <= 0) RvCoin = 0;
        //LocalDataManager.SetLocalData("RvCoin", RvCoin);
        //Debug.Log("Rvcoin减少********************************：" + RvCoin);
    }
    public int GetRvCoin()
    {
        return RvCoin;
    }
    public bool IsNeedPlayAd()
    {
        //Debug.Log("是否需要播放插屏********************************：" + RvCoin);
        return RvCoin <= 0 ? true : false;
        return true;
    }
}
