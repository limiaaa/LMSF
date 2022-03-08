using LMSF.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public interface IListenerIap
{
    void OnIapPurchaseFailed(Product Item,string FailedType);
    void OnIapPurchaseSucceed(Product Item);
}

public class IAPManager : Singleton<IAPManager>
{
    IListenerIap IapItem;
    public bool isPlayerClick = false;
    bool IsInitIAP=false;
    public bool NeedShop=false;
    private Dictionary<string, Product> StoreDic = new Dictionary<string, Product>();
    public void InitIAP()
    {
        IAPHelper.Instance.InitIapHelper(()=> {
            //获取本地商品数据
            //SendAct.ActIapInit("init","");
            //var datas = StoreConfig.GetDatas();
            //foreach (var item in datas)
            //{
            //    if (item.PayId != "")
            //    {
            //        AddProduct(item.PayId);
            //    }
            //}
        });
    }
    //检测购买过的非消耗品
    public void IapFirstInit(Dictionary<string, Product> storeData)
    {
        if(LocalDataUtils.GetLocalData("IsIapFirstInit", 0) == 0)
        {
            LocalDataUtils.SetLocalData("IsIapFirstInit", 1);
            StoreDic = storeData;
            Debug.Log("第一次初始化IAP，开始进行商品查验******************");
            ReStore();
            Debug.Log("第一次初始化IAP，商品查验结束**********************");
        }
    }  

    public void ReStore()
    {
        foreach(var item in StoreDic)
        {
            Debug.Log(item.Value.receipt);
            if (item.Value.receipt != null)
            {
               //string storeId=IAPHelper.Instance.VerifyByRepecit(item.Value,true);
               // Debug.Log("有收据的商品_" + storeId);
               // var datas = StoreConfig.GetDatas();
               // foreach (var item2 in datas)
               // {
               //     if (item2.PayId == storeId)
               //     {
               //         Debug.Log(item2.Id+ "__有收据的商品");
               //         if (item2.Id == 1)
               //         {
               //             LocalDataMgr.Instance.SetAdsState(true);
               //         }
               //     }
               // }
            }
        }
    }

    //获取配置表商品数据
    public void AddProduct(string storeKey)
    {
        if (storeKey == "100001remove_ads2.99")
        {
            IAPHelper.Instance.AddProduct(storeKey, 1);
        }
        else
        {
            IAPHelper.Instance.AddProduct(storeKey, 0);
        }
    }
    //点击购买
    public void PurchaseClick(string productID,IListenerIap shopitem)
    {
        IapItem = shopitem;
        isPlayerClick = true;
        IAPHelper.Instance.DoPurchase(productID);
    }
    public void IapInitFailed(InitializationFailureReason error) 
    {
        Debug.Log("IAP初始化失败");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown://应用上传有问题？不是正规渠道
                break;
            case InitializationFailureReason.PurchasingUnavailable://功能被禁用了
                break;
            case InitializationFailureReason.NoProductsAvailable://数据错误,对不上
                break;
        }
        //SendAct.ActIapInit("fail", error.ToString());
        IsInitIAP = false;
    }
    public void IapInitSuccessful()
    {
        Debug.Log("IAP初始化成功");
        //SendAct.ActIapInit("success","");
        IsInitIAP = true;
    }
    public void IapPurchaseFailed(Product item,string FailedType, bool IsRestore = false, string storeId = "")
    {
        Debug.Log("IAP付款失败_"+ isPlayerClick);
        if (isPlayerClick)
        {
            if (IapItem != null)
            {
                IapItem.OnIapPurchaseFailed(item, FailedType);
                IapItem = null;
            }
            else
            {
                Debug.Log("消息接收器为空1");
            }
        }
        else
        {

        }
        Time.timeScale = 1;
        UIMaskManager.Instance.CloseAniMask();
    }
    public void IapPurchaseSuccessful(Product purchasedProduct,bool IsRestore=false,string storeId="")
    {
        Debug.Log("IAP交易验证成功");
        if (isPlayerClick)
        {
            if (IapItem!=null)
            {
                IapItem.OnIapPurchaseSucceed(purchasedProduct);
                IapItem = null;
            }
            else
            {
                Debug.Log("消息接收器为空2");
            }
            isPlayerClick = false;
        }
        else
        {
            //var datas = StoreConfig.GetDatas();
            //foreach (var item in datas)
            //{
            //    if (item.PayId == storeId)
            //    {
            //        Debug.Log("执行奖励——" + item.PayId);
            //        HasStoreRewardNeedGive = true;
            //        NeedGiveStoreRewardData = item;
            //    }
            //}
        }
        ReserveBuyedData(purchasedProduct);
        Time.timeScale = 1;
        UIMaskManager.Instance.CloseAniMask();
    }
    public bool HasStoreRewardNeedGive = false;
    //public StoreConfig NeedGiveStoreRewardData = null;
    public void DoStoreReward()
    {
        //Debug.Log("进行商品奖励计算_"+HasStoreRewardNeedGive);
        //if (HasStoreRewardNeedGive)
        //{
        //    HasStoreRewardNeedGive = false;
        //    if (NeedGiveStoreRewardData == null)
        //    {
        //        return;
        //    }
        //    AddReward(NeedGiveStoreRewardData);
        //    NeedGiveStoreRewardData = null;
        //}
    }
    //void AddReward(StoreConfig StoreRewardData)
    //{
    //    if (StoreRewardData.Id == 1)
    //    {
    //        LocalDataMgr.Instance.SetAdsState(true);
    //    }
    //    else
    //    {
    //        for (int i = 0; i <= StoreRewardData.Items.Length - 1; i++)
    //        {
    //            switch (StoreRewardData.Items[i])
    //            {
    //                case 1:
    //                    LocalDataMgr.Instance.AddItemNum(GameItemType.Coin, StoreRewardData.ItemsNum[i], "from_shop_restore");
    //                    break;
    //                case 2://撤回道具
    //                    LocalDataMgr.Instance.AddItemNum(GameItemType.CancelNum, StoreRewardData.ItemsNum[i], "from_shop_restore");
    //                    break;
    //                case 3://提示道具
    //                    LocalDataMgr.Instance.AddItemNum(GameItemType.TipsNum, StoreRewardData.ItemsNum[i], "from_shop_restore");
    //                    break;
    //                case 4:
    //                case 5:
    //                case 6:
    //                case 7:
    //                    ItemConfig item1 = ItemConfig.GetData(StoreRewardData.Items[i]);
    //                    PlayerBuffManager.Instance.CreatState(GameStateEnum.Energy, item1.Duration * 60);
    //                    SendAct.ActItemBase(item1.ItemName, "from_shop_restore", 1, 1);
    //                    break;
    //                case 8:
    //                    ItemConfig item2 = ItemConfig.GetData(StoreRewardData.Items[i]);
    //                    PlayerBuffManager.Instance.CreatState(GameStateEnum.Double, item2.Duration * 60);
    //                    SendAct.ActItemBase(item2.ItemName, "from_shop_restore", 1, 1);
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //    }
    //}
    //购买记录存取下来
    public void ReserveBuyedData(Product product)
    {
        //if (!LocalDataMgr.Instance.mGameData.isBuyedStoreDic.ContainsKey(product.definition.id))
        //{
        //    LocalDataMgr.Instance.mGameData.isBuyedStoreDic.Add(product.definition.id, product.definition.id);
        //}
        //else
        //{
        //    LocalDataMgr.Instance.mGameData.isBuyedStoreDic[product.definition.id] = product.definition.id;
        //}
        //LocalDataMgr.Instance.SaveAll();
    }
    public void SetGameDataByBuyedStoreData()
    {
    }
    //检查一个商品有没有被购买过
    public bool GetStoreIsBuyedByPayId(string payId)
    {
        //foreach (var item in LocalDataMgr.Instance.mGameData.isBuyedStoreDic)
        //{
        //    if (StoreConfig.GetDataByPayId(item.Key).PayId == payId)
        //    {
        //        return true;
        //    }
        //}
        return false;
    }

    public void DoException(int exceptionId)
    {
        switch (exceptionId)
        {
            //case 0:
            //    UIManager.Instance.OpenPage<TextFlyTip>("Shoppage_PaymentFail_orderloading");
            //    break;
            //case 1:
            //    UIManager.Instance.OpenPage<TextFlyTip>("Shoppage_PaymentFail_paymentsystem");
            //    break;
            //case 2:
            //    UIManager.Instance.OpenPage<TextFlyTip>("Shoppage_PaymentFail_sellout");
            //    break;
        }

    }
}

