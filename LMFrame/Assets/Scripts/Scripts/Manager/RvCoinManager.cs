using LMSF.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RvCoinManager : MonoSingleton<RvCoinManager>
{
    int RvCoin=0;
    public void RefeshRvCoinFunc()
    {
        RvCoin = LocalDataUtils.GetLocalData("RvCoin",0);
    }
    public void AddRvCoin()
    {
        RvCoin += 10;
        LocalDataUtils.SetLocalData("RvCoin", RvCoin);
        Debug.Log("Rvcoin增加********************************：" + RvCoin);
    }
    public void ReduceRvCoin()
    {
        RvCoin -= 10;
        if (RvCoin <= 0) RvCoin = 0;
        LocalDataUtils.SetLocalData("RvCoin", RvCoin);
        Debug.Log("Rvcoin减少********************************：" + RvCoin);
    }
    public int GetRvCoin()
    {
        return RvCoin;
    }
    public bool IsNeedPlayAd()
    {
        Debug.Log("是否需要播放插屏********************************：" + RvCoin);
        return RvCoin <= 0 ? true : false;
    }
}
