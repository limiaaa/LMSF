using LMSF.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

#if RECEIPT_VALIDATION
    using UnityEngine.Purchasing.Security;
#endif
public class IAPHelper : MonoSingleton<IAPHelper>, IStoreListener
{
    // Unity IAP objects
    private IStoreController m_Controller;

    private IAppleExtensions m_AppleExtensions;
    private ISamsungAppsExtensions m_SamsungExtensions;
    private IMicrosoftExtensions m_MicrosoftExtensions;
    private ITransactionHistoryExtensions m_TransactionHistoryExtensions;
    private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;

    private bool m_IsGooglePlayStoreSelected;
    private bool m_IsSamsungAppsStoreSelected;
    private bool m_PurchaseInProgress;
    private CrossPlatformValidator validator;

    private ConfigurationBuilder builder;
    private Dictionary<string, Product> m_ProductDic = new Dictionary<string, Product>();

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_Controller = controller;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_SamsungExtensions = extensions.GetExtension<ISamsungAppsExtensions>();
        m_MicrosoftExtensions = extensions.GetExtension<IMicrosoftExtensions>();
        m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();
        m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
#if SUBSCRIPTION_MANAGER
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
#endif
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log("IAP_=>" + string.Join(" - ",
                    new[]
                    {
                            item.metadata.localizedTitle,
                            item.metadata.localizedDescription,
                            item.metadata.isoCurrencyCode,
                            item.metadata.localizedPrice.ToString(),
                            item.metadata.localizedPriceString,
                            item.transactionID,
                            item.receipt
                    }));
                //根据苹果的促销IAP功能，设置所有这些产品在用户的App Store中可见  
#if INTERCEPT_PROMOTIONAL_PURCHASES
                // Set all these products to be visible in the user's App Store according to Apple's Promotional IAP feature
                // https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/StoreKitGuide/PromotingIn-AppPurchases/PromotingIn-AppPurchases.html
                m_AppleExtensions.SetStorePromotionVisibility(item, AppleStorePromotionVisibility.Show);
#endif
                //SubscriptionInfo类使用，查看商品信息
#if SUBSCRIPTION_MANAGER
                // this is the usage of SubscriptionManager class
                if (item.receipt != null) {
                    if (item.definition.type == ProductType.Subscription) {
                        if (checkIfProductIsAvailableForSubscriptionManager(item.receipt)) {
                            string intro_json =
    (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                            SubscriptionManager p = new SubscriptionManager(item, intro_json);
                            SubscriptionInfo info = p.getSubscriptionInfo();
                            Debug.Log("IAP_product id is: " + info.getProductId());
                            Debug.Log("IAP_purchase date is: " + info.getPurchaseDate());
                            Debug.Log("IAP_subscription next billing date is: " + info.getExpireDate());
                            Debug.Log("IAP_is subscribed? " + info.isSubscribed().ToString());
                            Debug.Log("IAP_is expired? " + info.isExpired().ToString());
                            Debug.Log("IAP_is cancelled? " + info.isCancelled());
                            Debug.Log("IAP_product is in free trial peroid? " + info.isFreeTrial());
                            Debug.Log("IAP_product is auto renewing? " + info.isAutoRenewing());
                            Debug.Log("IAP_subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                            Debug.Log("IAP_is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                            Debug.Log("IAP_the product introductory localized price is: " + info.getIntroductoryPrice());
                            Debug.Log("IAP_the product introductory price period is: " + info.getIntroductoryPricePeriod());
                            Debug.Log("IAP_the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                        } else {
                            Debug.Log("IAP_This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                        }
                    } else {
                        Debug.Log("IAP_the product is not a subscription product");
                    }
                } else {
                    Debug.Log("IAP_the product should have a valid receipt");
                }
#endif
            }       
        }

        AddProducts(m_Controller.products.all);
        IAPManager.Instance.IapInitSuccessful();
        //for(int i = 0; i <= 5; i++)
        //{
        //    DoRestore();
        //}
        IAPManager.Instance.IapFirstInit(m_ProductDic);
    }
    //延期
    private void OnDeferred(Product item)
    {
        Debug.Log("IAP_Purchase deferred: " + item.definition.id);
        //IAP3_GPA.3311-1167-2197-85267
        //IAP5_gghjfbjemlennagnhbappkkf.AO-J1OyfxefCWrFr2G03qetBI1XTNSpF7tSiPnCDgY29Q_GdbB7DcUQ3_zmB5wQ1hYbOh-a8WbmpaXbdwAeVr2Q-yuaeWRGanA
    }
    //订阅管理
#if SUBSCRIPTION_MANAGER
    //检查“产品是否可供订阅管理器使用”
    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt) {
        var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
        if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload")) {
            Debug.Log("IAP_The product receipt does not contain enough information");
            return false;
        }
        var store = (string)receipt_wrapper ["Store"];
        var payload = (string)receipt_wrapper ["Payload"];

        if (payload != null ) {
            switch (store) {
            case GooglePlay.Name:
                {
                    var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                    if (!payload_wrapper.ContainsKey("json")) {
                        Debug.Log("IAP_The product receipt does not contain enough information, the 'json' field is missing");
                        return false;
                    }
                    var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                    if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload")) {
                        Debug.Log("IAP_The product receipt does not contain enough information, the 'developerPayload' field is missing");
                        return false;
                    }
                    var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                    var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                    if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial")) {
                        Debug.Log("IAP_The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                        return false;
                    }
                    return true;
                }
            case AppleAppStore.Name:
            case AmazonApps.Name:
            case MacAppStore.Name:
                {
                    return true;
                }
            default:
                {
                    return false;
                }
            }
        }
        return false;
    }
#endif
    //初始化失败
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("IAP_初始化失败");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("IAP_初始化失败_渠道有问题，不知道APP来源");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                Debug.Log("IAP_初始化失败_没开billing");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                Debug.Log("IAP_初始化失败_商品数据错误_无商品数据");
                break;
        }
        IAPManager.Instance.IapInitFailed(error);
    }
    //购买失败
    public void OnPurchaseFailed(Product item, PurchaseFailureReason failureReason)
    {
        UIMaskManager.Instance.CloseAniMask();
        UIMaskManager.Instance.CloseSingleMask();
        Debug.Log("IAP_购买失败+ID: " + item.definition.id);
        Debug.Log("IAP_购买失败+原因: "+failureReason);
        Debug.Log("IAP_Store specific error code: " + m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
        if (m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
        {
            Debug.Log("IAP_Purchase failure description message: " +
                        m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
        }
        if (PurchaseFailureReason.DuplicateTransaction == failureReason)
        {
        }
        else
        {
        }
        m_PurchaseInProgress = false;
        IAPManager.Instance.IapPurchaseFailed(item, failureReason.ToString());
    }
    //购买成功后会调用此方法，需继续处理
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        UIMaskManager.Instance.CloseAniMask();
        UIMaskManager.Instance.CloseSingleMask();
        Debug.Log("IAP_购买成功+ID: " + purchaseEvent.purchasedProduct.definition.id);
        Debug.Log("IAP_购买成功+Receipt: " + purchaseEvent.purchasedProduct.receipt);
        m_PurchaseInProgress = false;
#if USE_PAYOUTS
        if (purchaseEvent.purchasedProduct.definition.payouts != null) {
            Debug.Log("IAP_Purchase complete, paying out based on defined payouts");
            foreach (var payout in purchaseEvent.purchasedProduct.definition.payouts) {
                Debug.Log(IAP_string.Format("Granting {0} {1} {2} {3}", payout.quantity, payout.typeString, payout.subtype, payout.data));
            }
        }
#endif

#if UNITY_EDITOR
        IAPManager.Instance.IapPurchaseSuccessful(purchaseEvent.purchasedProduct);
        return PurchaseProcessingResult.Complete;
#else
        SavePendingPurchase(purchaseEvent.purchasedProduct);
        return PurchaseProcessingResult.Pending;
#endif
    }

    public Dictionary<string, Product> m_PendingProducts = new Dictionary<string, Product>();
    int SaveTime = 0;
    public void SavePendingPurchase(Product p)
    {
        if (m_PendingProducts.ContainsKey(p.definition.id))
        {
            m_PendingProducts[p.definition.id] = p;
        }
        else
        {
            m_PendingProducts.Add(p.definition.id, p);
        }
        var confirmpendstate = ConfirmPendingPurchase(p);
        Debug.Log("IAP_进行延时");
        //是游戏内购买验证，反之则是IAP初始化自动调用
        if (IAPManager.Instance.isPlayerClick)
        {
            UIMaskManager.Instance.OpenAniMask(2.1f);
            DelayTimeManager.Instance.delay_time_run_without_timescale(2, () => {
                Debug.Log("IAP_进入订单验证_" + confirmpendstate);
                if (confirmpendstate == PurchaseProcessingResult.Pending)
                {
                    SaveTime = 0;
                    Debug.Log("IAP_订单验证成功,手动处理订单");
                    m_Controller.ConfirmPendingPurchase(p);
                    if (m_PendingProducts.ContainsKey(p.definition.id))
                    {
                        m_PendingProducts.Remove(p.definition.id);
                    }
                    IAPManager.Instance.IapPurchaseSuccessful(p);
                }
                else
                {
                    Debug.Log("IAP_订单验证出错,重新开始验证：" + SaveTime);
                    if (SaveTime < 3)
                    {
                        SaveTime++;
                        SavePendingPurchase(p);
                    }
                    else
                    {
                        SaveTime = 0;
                        Debug.Log("IAP_执行购买失败回调：订单验证不通过");
                        IAPManager.Instance.IapPurchaseFailed(p, "IAP_订单验证出错,重新开始验证");
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
                m_Controller.ConfirmPendingPurchase(p);
                if (m_PendingProducts.ContainsKey(p.definition.id))
                {
                    m_PendingProducts.Remove(p.definition.id);
                }
                IAPManager.Instance.IapPurchaseSuccessful(p,true, p.definition.id);
            }
            else
            {
                Debug.Log("IAP_订单验证出错,重新开始验证：" + SaveTime);
                if (SaveTime < 3)
                {
                    SaveTime++;
                    SavePendingPurchase(p);
                }
                else
                {
                    SaveTime = 0;
                    Debug.Log("IAP_执行购买失败回调：订单验证不通过");
                    IAPManager.Instance.IapPurchaseFailed(p, "IAP_恢复订单验证出错,重新开始验证", true, p.definition.id);
                }
            }
        }
        
    }

    private PurchaseProcessingResult ConfirmPendingPurchase(Product p)
    {
        return VerifyByRepecit(p);
    }
    public PurchaseProcessingResult VerifyByRepecit(Product p)
    {
        string appIdentifier = Application.identifier;
        //传入RSA生成
        //validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), appIdentifier);

        if (m_IsGooglePlayStoreSelected ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.tvOS)
        {
            try
            {
                Debug.Log("IAP订单验证凭证_" + p.receipt);
                var result = validator.Validate(p.receipt);
                Debug.Log("IAP_Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log("*********IAP1_" + productReceipt.productID);
                    Debug.Log("*********IAP2_" + productReceipt.purchaseDate);
                    Debug.Log("*********IAP3_" + productReceipt.transactionID);

                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google)
                    {
                        Debug.Log("*********IAP4_" + google.purchaseState);
                        Debug.Log("*********IAP5_" + google.purchaseToken);
                    }

                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (null != apple)
                    {
                        Debug.Log("IAP6_" + apple.originalTransactionIdentifier);
                        Debug.Log("IAP7_" + apple.subscriptionExpirationDate);
                        Debug.Log("IAP8_" + apple.cancellationDate);
                        Debug.Log("IAP9_" + apple.quantity);
                    }

                }
            }
            catch (IAPSecurityException ex)
            {
                return PurchaseProcessingResult.Complete;
            }
            return PurchaseProcessingResult.Pending;
        }
        return PurchaseProcessingResult.Pending;
    }

    public string VerifyByRepecit(Product p,bool IsCheck)
    {
        string appIdentifier = Application.identifier;
        //传入RSA生成
        //validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), appIdentifier);
        string productId ="";
        if (m_IsGooglePlayStoreSelected ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.tvOS)
        {
            try
            {
                Debug.Log("IAP订单验证凭证_" + p.receipt);
                var result = validator.Validate(p.receipt);
                Debug.Log("IAP_Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    productId = productReceipt.productID;
                    Debug.Log("*********IAP1_" + productReceipt.productID);
                    Debug.Log("*********IAP2_" + productReceipt.purchaseDate);
                    Debug.Log("*********IAP3_" + productReceipt.transactionID);

                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google)
                    {
                        Debug.Log("*********IAP4_" + google.purchaseState);
                        Debug.Log("*********IAP5_" + google.purchaseToken);
                    }

                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (null != apple)
                    {
                        Debug.Log("IAP6_" + apple.originalTransactionIdentifier);
                        Debug.Log("IAP7_" + apple.subscriptionExpirationDate);
                        Debug.Log("IAP8_" + apple.cancellationDate);
                        Debug.Log("IAP9_" + apple.quantity);
                    }

                }
            }
            catch (IAPSecurityException ex)
            {
                return "";
            }
            return productId;
        }
        return productId;
    }
    //交易记录恢复输出
    private void OnTransactionsRestored(bool success)
    {
        Debug.Log("IAP_Transactions restored." + success);
    }
    //本地存储商品表
    void AddProducts(Product[] products)
    {
        m_ProductDic.Clear();
        foreach (var product in products)
        {
            m_ProductDic[product.definition.id] = product;
            Debug.Log(string.Format("IAP_id: {0}\nstore-specific id: {1}\ntype: {2}\nenabled: {3}\n", product.definition.id, product.definition.storeSpecificId, product.definition.type.ToString(), product.definition.enabled ? "enabled" : "disabled"));
        }
    }
#region 促销购买相关/延时购买/apple
    private void OnPromotionalPurchase(Product item) {
        Debug.Log("IAP_Attempted promotional purchase: " + item.definition.id);

        // Promotional purchase has been detected. Handle this event by, e.g. presenting a parental gate.
        // Here, for demonstration purposes only, we will wait five seconds before continuing the purchase.
        StartCoroutine(ContinuePromotionalPurchases());
    }
    private IEnumerator ContinuePromotionalPurchases()
    {
        Debug.Log("IAP_Continuing promotional purchases in 5 seconds");
        yield return new WaitForSeconds(5);
        Debug.Log("IAP_Continuing promotional purchases now");
        m_AppleExtensions.ContinuePromotionalPurchases (); // iOS and tvOS only; does nothing on Mac
    }
#endregion


#region 外部调用
    //第一次初始化
    public void InitIapHelper(Action addAction)
    {
        var module = StandardPurchasingModule.Instance();
        module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
        builder = ConfigurationBuilder.Instance(module);
        builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = false;
        addAction?.Invoke();
        m_IsGooglePlayStoreSelected =
            Application.platform == RuntimePlatform.Android && module.appStore == AppStore.GooglePlay;
        builder.Configure<ISamsungAppsConfiguration>().SetMode(SamsungAppsMode.AlwaysSucceed);
        m_IsSamsungAppsStoreSelected =
            Application.platform == RuntimePlatform.Android && module.appStore == AppStore.SamsungApps;
        //促销拦截？只针对IOS
        builder.Configure<IAppleConfiguration>().SetApplePromotionalPurchaseInterceptorCallback(OnPromotionalPurchase);
        Debug.Log("IAP_开始初始化");
        UnityPurchasing.Initialize(this, builder);
    }
    //重新初始化
    public void RefreshProduct()
    {
        if (builder != null)
        {
            Debug.Log("IAP_重新初始化");
            UnityPurchasing.Initialize(this, builder);
        }
    }
    //添加商品
    public void AddProduct(string id, int type)
    {
        if (builder != null)
        {
            id = id.ToLower();
            Debug.Log("IAP_添加本地商品列表：" + id);
            builder.AddProduct(id, (ProductType)type, new IDs
                    {
                        {id, GooglePlay.Name},
                        {id, AppleAppStore.Name}
                    }
            );
        }
    }
    //发送购买
    public void DoPurchase(string productID)
    {
        if (m_PurchaseInProgress == true)
        {
            Debug.Log("IAP_Please wait, purchase in progress");
            IAPManager.Instance.DoException(0);
            return;
        }

        if (m_Controller == null)
        {
            Debug.LogError("IAP_Purchasing is not initialized");
            IAPManager.Instance.DoException(1);
            return;
        }

        if (m_Controller.products.WithID(productID) == null)
        {
            Debug.LogError("IAP_No product has id " + productID);
            IAPManager.Instance.DoException(2);
            return;
        }
        m_PurchaseInProgress = true;
        Time.timeScale = 0;
        productID = productID.ToLower();
        Debug.Log("购买商品：" + productID);
        UIMaskManager.Instance.OpenAniMask(20);
        m_Controller.InitiatePurchase(m_Controller.products.WithID(productID),MiniJson.JsonEncode("XiaoMai"));
    }
    //恢复购买
    public void DoRestore()
    {
        if (m_IsSamsungAppsStoreSelected)
        {
            m_SamsungExtensions.RestoreTransactions(OnTransactionsRestored);
        }
        else if (Application.platform == RuntimePlatform.WSAPlayerX86 ||
                    Application.platform == RuntimePlatform.WSAPlayerX64 ||
                    Application.platform == RuntimePlatform.WSAPlayerARM)
        {
            m_MicrosoftExtensions.RestoreTransactions();
        }
        else if (m_IsGooglePlayStoreSelected)
        {
            m_GooglePlayStoreExtensions.RestoreTransactions(OnTransactionsRestored);
        }
        else
        {
            m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
        }
    }
    public Product GetProduct(string productID)
    {
        if (m_Controller == null)
        {
            Debug.Log("IAP_获取商品失败：" + productID);
            return null;
        }
        var product = m_Controller.products.WithID(productID);
        return product;
    }
#endregion
}
