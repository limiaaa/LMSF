using LMSF.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public interface IListenerIap
{
    void OnIapPurchaseFailed(Product Item);
    void OnIapPurchaseSucceed(Product Item);
}

public class IAPManager : MonoSingleton<IAPManager>
{
    IListenerIap IapItem;
    bool isPlayerClick = false;
    bool IsInitIAP=false;
    public bool NeedShop = false;
    public void InitIAP()
    {
        IAPHelper.Instance.InitIapHelper(()=> {
            //添加本地商品Id
            //var datas = StoreConfig.GetDatas();
            //foreach (var item in datas)
            //{
            //    if (item.PayId != "")
            //    {
            //        AddProduct(item.PayId);
            //    }
            //}
            //SubscriptionInfo info = p.getSubscriptionInfo();

        });
    }
    //获取配置表商品数据
    public void AddProduct(string storeKey)
    {
        IAPHelper.Instance.AddProduct(storeKey, 0);
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
        IsInitIAP = false;
    }
    public void IapInitSuccessful()
    {
        Debug.Log("IAP初始化成功");
        IsInitIAP = true;
    }
    public void IapPurchaseFailed(Product item)
    {
        Debug.Log("IAP付款失败");
        if (IapItem != null)
        {
            IapItem.OnIapPurchaseFailed(item);
            IapItem = null;
        }
        else
        {
            Debug.Log("消息接收器为空1");
        }
        Time.timeScale = 1;
    }
    public void IapPurchaseSuccessful(Product purchasedProduct)
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
        ReserveBuyedData(purchasedProduct);
        Time.timeScale = 1;
    }
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
        //查下购买过广告没有
        //if (LocalDataMgr.Instance.GetAdsState())
        //{
        //    foreach (var item in LocalDataMgr.Instance.mGameData.isBuyedStoreDic)
        //    {
        //        if (StoreConfig.GetDataByPayId(item.Key).Id == 10001)
        //        {
        //            LocalDataMgr.Instance.SetAdsState(true);
        //            break;
        //        }
        //    }
        //}
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
        //switch (exceptionId)
        //{
        //    case 0:
        //        UIManager.Instance.OpenPage<TextFlyTip>("AD_Prepear");
        //        break;
        //    case 1:
        //        UIManager.Instance.OpenPage<TextFlyTip>("AD_Prepear");
        //        break;
        //    case 2:
        //        UIManager.Instance.OpenPage<TextFlyTip>("AD_Prepear");
        //        break;
        //}

    }
}
