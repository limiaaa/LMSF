using LMSF.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public interface IListenerIap
{
    void OnIapPurchaseFailed(Product Item, string FailedType);
    void OnIapPurchaseSucceed(Product Item);
}

public class IAPManager : MonoSingleton<IAPManager>
{
    IListenerIap IapItem;
    public bool isPlayerClick = false;
    bool IsInitIAP = false;
    private Dictionary<string, Product> StoreDic = new Dictionary<string, Product>();

    public Dictionary<string, Product> m_PendingProducts = new Dictionary<string, Product>();
    public bool NeedShop;
    int SaveTime = 0;
    public void InitIAP()
    {
        IAPHelper.Instance.InitIapHelper(() =>
        {
            //var datas = ShopConfig.GetDatas();
            //foreach (var item in datas)
            //{
            //    if (item.IAPId != "")
            //    {
            //        AddProduct(item.IAPId);
            //    }
            //}
        });
        //设置初始化成功回调
        IAPHelper.Instance.SetInitSuccessCallBack((products) =>
        {
            IapInitSuccessful();
            IapFirstInit(products);
        });
        //设置初始化失败回调
        IAPHelper.Instance.SetInitFailedCallBack((errorInfo) =>
        {
            IapInitFailed(errorInfo);
        });
        IAPHelper.Instance.SetBuyCallBack((id) =>
        {
            DoException(id);
        });
        //设置支付失败回调
        IAPHelper.Instance.SetPayFailedCallBack((item, failureReason) =>
        {
            UIMaskManager.Instance.CloseAniMask();
            UIMaskManager.Instance.CloseSingleMask();
            IapPurchaseFailed(item, failureReason);
        });
        //设置开始验证回调
        IAPHelper.Instance.SetPayProcessCallBack(() =>
        {
            UIMaskManager.Instance.CloseAniMask();
            UIMaskManager.Instance.CloseSingleMask();
        });
        //设置在编辑器下直接支付成功回调
        IAPHelper.Instance.SetPaySuccessInEditorCallBack((product) =>
        {
            IapPurchaseSuccessful(product);
        });
        //设置订单验证回调
        IAPHelper.Instance.SetPendingPurchaseCallBack((product, confirmpendstate, m_Controller) =>
        {
            if (m_PendingProducts.ContainsKey(product.definition.id))
            {
                m_PendingProducts[product.definition.id] = product;
            }
            else
            {
                m_PendingProducts.Add(product.definition.id, product);
            }
            //是游戏内购买验证，反之则是IAP初始化自动调用
            if (isPlayerClick)
            {
                UIMaskManager.Instance.OpenAniMask(2.1f);
                DelyTimerManager.Instance.Delay_WithoutTimeScale(2, () =>
                {
                    Debug.Log("IAP_进入订单验证_" + confirmpendstate);
                    if (confirmpendstate == PurchaseProcessingResult.Pending)
                    {
                        SaveTime = 0;
                        Debug.Log("IAP_订单验证成功,手动处理订单");
                        m_Controller.ConfirmPendingPurchase(product);
                        if (m_PendingProducts.ContainsKey(product.definition.id))
                        {
                            m_PendingProducts.Remove(product.definition.id);
                        }
                        IapPurchaseSuccessful(product);
                    }
                    else
                    {
                        Debug.Log("IAP_订单验证出错,重新开始验证：" + SaveTime);
                        if (SaveTime < 3)
                        {
                            SaveTime++;
                            IAPHelper.Instance.SavePendingPurchase(product);
                        }
                        else
                        {
                            SaveTime = 0;
                            Debug.Log("IAP_执行购买失败回调：订单验证不通过");
                            IapPurchaseFailed(product, "IAP_订单验证出错,重新开始验证");
                        }
                    }
                });
            }
            else
            {
                Debug.Log("进入IAP自动调用验证");
                if (confirmpendstate == PurchaseProcessingResult.Pending)
                {
                    SaveTime = 0;
                    Debug.Log("IAP_订单验证成功,手动处理订单");
                    m_Controller.ConfirmPendingPurchase(product);
                    if (m_PendingProducts.ContainsKey(product.definition.id))
                    {
                        m_PendingProducts.Remove(product.definition.id);
                    }
                    IapPurchaseSuccessful(product, true, product.definition.id);
                }
                else
                {
                    Debug.Log("IAP_订单验证出错,重新开始验证：" + SaveTime);
                    if (SaveTime < 3)
                    {
                        SaveTime++;
                        IAPHelper.Instance.SavePendingPurchase(product);
                    }
                    else
                    {
                        SaveTime = 0;
                        Debug.Log("IAP_执行购买失败回调：订单验证不通过");
                        IapPurchaseFailed(product, "IAP_恢复订单验证出错,重新开始验证", true, product.definition.id);
                    }
                }
            }
        });
        IAPHelper.Instance.SetPayWaitCallBack(() =>
        {
            UIMaskManager.Instance.OpenAniMask(20);
        });
    }
    //检测购买过的非消耗品
    public void IapFirstInit(Dictionary<string, Product> storeData)
    {
        //if (LocalDataFunc.Instance.GetLocalData("IsIapFirstInit", 0) == 0)
        //{
        //    LocalDataFunc.Instance.SetLocalData("IsIapFirstInit", 1);
        //    StoreDic = storeData;
        //    Debug.Log("第一次初始化IAP，开始进行商品查验******************");
        //    //ReStore();
        //    Debug.Log("第一次初始化IAP，商品查验结束**********************");
        //}
    }
    public void ReStore()
    {
        //foreach (var item in StoreDic)
        //{
        //    Debug.Log(item.Value.receipt);
        //    if (item.Value.receipt != null)
        //    {
        //        string storeId = IAPHelper.Instance.VerifyByRepecit(item.Value, true);
        //        Debug.Log("有收据的商品_" + storeId);
        //        var datas = ShopConfig.GetDatas();
        //        foreach (var item2 in datas)
        //        {
        //            if (item2.IAPId == storeId)
        //            {
        //                Debug.Log(item2.ShopItemId + "__有收据的商品");
        //                if (item2.ShopItemId == "remove.ads")
        //                {
        //                    LocalDataManager.Instance.SetAdsState(true);
        //                }
        //            }
        //        }
        //    }
        //}
    }

    //获取配置表商品数据
    public void AddProduct(string storeKey)
    {
        if (storeKey == "remove_ads1.99")
        {
            IAPHelper.Instance.AddProduct(storeKey, 1);
        }
        else
        {
            IAPHelper.Instance.AddProduct(storeKey, 0);
        }
    }
    //点击购买
    public void PurchaseClick(string productID, IListenerIap shopitem)
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
    public void IapPurchaseFailed(Product item, string FailedType, bool IsRestore = false, string storeId = "")
    {
        Debug.Log("IAP付款失败_" + isPlayerClick);
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
    public void IapPurchaseSuccessful(Product purchasedProduct, bool IsRestore = false, string storeId = "")
    {
        //Debug.Log("IAP交易验证成功");
        //if (isPlayerClick)
        //{
        //    if (IapItem != null)
        //    {
        //        IapItem.OnIapPurchaseSucceed(purchasedProduct);
        //        IapItem = null;
        //    }
        //    else
        //    {
        //        Debug.Log("消息接收器为空2");
        //    }
        //    isPlayerClick = false;
        //}
        //else
        //{
        //    var datas = ShopConfig.GetDatas();
        //    foreach (var item in datas)
        //    {
        //        if (item.IAPId == storeId)
        //        {
        //            Debug.Log("执行奖励——" + item.IAPId);
        //            HasStoreRewardNeedGive = true;
        //            NeedGiveStoreRewardData = item;
        //        }
        //    }
        //}
        //Time.timeScale = 1;
        //UIMaskManager.Instance.CloseAniMask();
    }
    public bool HasStoreRewardNeedGive = false;
    //public ShopConfig NeedGiveStoreRewardData = null;
    //public void DoStoreReward()
    //{
    //    Debug.Log("进行商品奖励计算_" + HasStoreRewardNeedGive);
    //    if (HasStoreRewardNeedGive)
    //    {
    //        HasStoreRewardNeedGive = false;
    //        if (NeedGiveStoreRewardData == null)
    //        {
    //            return;
    //        }
    //        AddReward(NeedGiveStoreRewardData);
    //        NeedGiveStoreRewardData = null;
    //    }
    //}
    //void AddReward(ShopConfig StoreRewardData)
    //{
    //    if (StoreRewardData.ShopItemId == "remove.ads")
    //    {
    //        LocalDataManager.Instance.SetAdsState(true);
    //    }
    //    else
    //    {
    //        for (int i = 0; i <= StoreRewardData.RewardItem.Length - 1; i++)
    //        {
    //            string name = ItemManager.Instance.GetItemNameById(StoreRewardData.RewardItem[i]);
    //            string ActName = "Restore";
    //            Debug.Log("Reward Id__" + StoreRewardData.RewardItem[i] + "__" + name + "__" + StoreRewardData.RewardCount[i]);
    //            switch (name)
    //            {
    //                case "Coin":
    //                    ItemManager.Instance.AddItemNum(GameItemType.Coin, StoreRewardData.RewardCount[i], ActName);
    //                    break;
    //                case "Star":
    //                    ItemManager.Instance.AddItemNum(GameItemType.Star, StoreRewardData.RewardCount[i], ActName);
    //                    break;
    //                case "Grenade"://手榴弹(全屏)
    //                    ItemManager.Instance.AddItemNum(GameItemType.Grenade, StoreRewardData.RewardCount[i], ActName);
    //                    break;
    //                case "AddBalls"://加球
    //                    ItemManager.Instance.AddItemNum(GameItemType.AddBalls, StoreRewardData.RewardCount[i], ActName);
    //                    break;
    //                case "Hammer"://锤子3*3
    //                    ItemManager.Instance.AddItemNum(GameItemType.Hammer, StoreRewardData.RewardCount[i], ActName);
    //                    break;
    //                case "Boxing"://拳套横向
    //                    ItemManager.Instance.AddItemNum(GameItemType.Boxing, StoreRewardData.RewardCount[i], ActName);
    //                    break;
    //                case "Weight"://秤砣竖向
    //                    ItemManager.Instance.AddItemNum(GameItemType.Weight, StoreRewardData.RewardCount[i], ActName);
    //                    break;
    //                default:
    //                    break;
    //            }
    //            if (StoreRewardData.RewardItem[i] >= 200 & StoreRewardData.RewardItem[i] < 300)
    //            {
    //                SkinManager.Instance.GetNewSkinById(StoreRewardData.RewardItem[i]);
    //            }
    //        }
    //    }
    //}
    public void DoException(int exceptionId)
    {
        //switch (exceptionId)
        //{
        //    case 0:
        //        UIManager.Instance.OpenPage<TextFlyTip>("Shoppage_PaymentFail_orderloading");
        //        break;
        //    case 1:
        //        UIManager.Instance.OpenPage<TextFlyTip>("Shoppage_PaymentFail_paymentsystem");
        //        break;
        //    case 2:
        //        UIManager.Instance.OpenPage<TextFlyTip>("Shoppage_PaymentFail_sellout");
        //        break;
        //}
    }
}

