
using Newtonsoft.Json;
using SG.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SG.SDK{

    public enum RESULT_CODE
    {
        FAILED = 0,
        SUCCESS = 1,
    }

    public interface ISDKHelper
    {
        /*
         * 初始化相关接口
         */
        void Init(SDKInitParams datas, Action<bool> onInitFinished);
        bool InitOK();
        /*
         * 用来标记功能是否可用(广告、埋点等)
         * OpenSDK中, SDK的初始是否成功不影响广告相关功能
         * 所以需要添加这么一个奇葩接口来判断功能是否可用
         */
        bool InitFuncsOK();

        string GetDistinctID();

        /*
        * 预加载指定的广告, 不需要预加载的可传null
        */
        void PreloadAd(string bannerKey, string rvKey, string interstitialKey, string nativeKey);

        void SendNormal(string actionId, Dictionary<string, object> data);

        /*
         * 用户属性上报
         */
        bool ReportUserProfile(Dictionary<string, object> datas, bool isOnce = false);
        /*
         * 事件接口
         */
        void AddAction(SDKAction action);
        /*
         * banner接口
         */
        void BannerInit(RectTransform rectTransform);
        void BannerShow();
        void BannerHide();
        void BannerRemove();
        /*
         * 视频广告接口
         */
        bool VAdIsReady();
        bool VAdShow(int adIndex, string from, IListenerVideoAd listener);
        /*
         * 插屏广告接口
         */
        bool InterstitialAdIsReady();
        bool InterstitialAdShow(string from, IListenerInterstitialAd listener);

    }

    /// <summary>
    /// 广告回调接口
    /// </summary>
public interface IListenerVideoAd
{

    void OnAdLoaded();
    void OnAdLoadFailed();
    void OnAdShowFailed();
    void OnAdRewarded();
    void OnAdClicked();
    void OnAdShowed();
    void OnAdClosed();
}

public interface IIAPHelper
{
        void Init();
        /// <summary>
        /// 查询商品列表
        /// </summary>
        /// <param name="onQueryBillingProductListEnd"></param>
        void QueryBillingProductList(List<string> productIdList, Action<ResultBillingProductList> onQueryBillingProductListEnd);

        /// <summary>
        /// 消耗成功付费但未发放奖励的订单
        /// 如果有多个商品未发放奖励, 回调会被调用多次
        /// </summary>
        /// <param name="onQueryPurchaseEnd"></param>
        void QueryPurchase(Action<ResultQueryPurchase> onQueryPurchaseEnd);

        /// <summary>
        /// 购买商品
        /// 如果返回成功, 可直接发货
        /// </summary>
        /// <param name="onLaunchBillingEnd"></param>
        void LaunchBilling(string productId, Action<ResulLaunchBilling> onLaunchBillingEnd);

        /// <summary>
        /// 请求历史购买订单
        /// </summary>
        /// <param name="onQueryHistoryPurchasedEnd"></param>
        void QueryHistoryPurchased(Action<ResultQueryHistoryPurchased> onQueryHistoryPurchasedEnd);

    }

    /// <summary>
    /// 广告回调接口
    /// </summary>
    public interface IListenerInterstitialAd
    {
        void OnAdLoaded();
        void OnAdLoadFailed();
        void OnAdShowFailed();
        void OnAdClicked();
        void OnAdShowed();
        void OnAdClosed();
    }

    /// <summary>
    /// SDK 初始化时的传入参数
    /// </summary>
    public class SDKInitParams
    {
        /// <summary>
        /// 小迈内部的产品id
        /// </summary>
        public string product_id = "undefined";
        /// <summary>
        /// app的版本号
        /// </summary>
        public string app_version = "undefined";

        ///// <summary>
        ///// 埋点数据格式的版本号
        ///// </summary>
        //public string action_version = "undefined";

        /// <summary>
        /// 数数上的项目id
        /// </summary>
        public string thinking_data_appid = "undefined";

        ///// <summary>
        ///// 每次启动判断当前日期与上次启动日期，不一样则本地计数+1，并更新本地记录启动日期
        ///// </summary>
        //public int user_days_opened = -1;

        ///// <summary>
        ///// 每次启动当前日期-安装日期+1
        ///// </summary>
        //public int user_days_install = -1;


    }

    /// <summary>
    /// 用于埋点的action数据
    /// </summary>
    public class SDKAction// : ISDKAction
    {
        public const int FLAG_SENDTO_FB = 1;
        public const int FLAG_SENDTO_TD = 2;
        public const int FLAG_SENDTO_AF = 4;
        public const int FLAG_SENDTO_ALL = 0xfffffff;

        public string id;
        public string version;

        public int user_days_open;
        public int user_days_install;

        public string act;
        public string from;
        public string obj;
        public string scene;
        public string level_type;
        public int level_id;
        public int amount;
        public int sequence;
        public int fps;
        public string remote_tag;
        public int max_level_id;
        private Dictionary<string, object> sendData;

        public int flag_send_lv= FLAG_SENDTO_ALL;

        public string GetId()
        {
            return id;
        }

        public Dictionary<string, object> GetActionData()
        {
            var sendData = new Dictionary<string, object>();
            WriteData(sendData);
            return sendData;
        }

        public void WriteData(Dictionary<string, object> sendData)
        {
            if (sendData == null)
            {
                Debug.Log("SDKActionData.WriteData sendData is null");
                return;
            }
            this.sendData = sendData;

            _writeString("version", version);

            _writeInt("user_days_open", user_days_open);
            _writeInt("user_days_install", user_days_install);


            _writeString("act", act);
            _writeString("from", from);
            _writeString("object", obj);
            _writeString("scene", scene);
            _writeString("level_type", level_type);
            _writeString("remote_tag", remote_tag);

            _writeInt("level_id", level_id);
            _writeInt("amount", amount);
            _writeInt("sequence", sequence);
            _writeInt("fps", fps);
            _writeInt("max_level_id", max_level_id);
            this.sendData = null;
        }


        private void _writeInt(string name, int value)
        {
            if (!sendData.ContainsKey(name))
            {
                sendData.Add(name, value);
            }
        }
        private void _writeString(string name, string value)
        {
            if (!sendData.ContainsKey(name))
            {
                sendData.Add(name, value ?? "");
            }

        }

        public string Dump()
        {
            try
            {
                string dataJsonStr = JsonConvert.SerializeObject(GetActionData());
                return dataJsonStr;
            }
            catch (Exception)
            {

            }
            return "{ Action data error }";
        }
    }

    // 订单数据
    [Serializable]

    public class BillingOrderData
    {
        public string productId;       // 商品id
        public string billingChannel;  // 渠道id
        public bool isAutoRenewing;    // 是否订阅商品
        public long purchaseTime;      // 交易时间
        public string orderId;         // 订单号
        public string purchaseToken;   // 订单token
    }

    // 商品数据
    [Serializable]

    public class BillingProductData
    {
        public string productId; // ":"com.xilm.hss2",
        public string price; // ":"HK$7.20",
        public string title; // ":"一塊錢喝水 (Healthy drink water)",
        public int priceAmount; // ":720,
        public string type; // ":"inapp",
        public string priceCurrencyCode; // ":"HKD",
        public string subscriptionPeriod; // ":""
    }


    [Serializable]
    public class ResultOpenSDKIapBase
    {
        public RESULT_CODE resultCode;
        public string msg;
    }


    // 结果返回: 消耗成功付费但未发放奖励的订单
    [Serializable]
    public class ResultQueryPurchase : ResultOpenSDKIapBase
    {
        public BillingOrderData data;

    }
    // 结果返回: 购买商品

    [Serializable]
    public class ResulLaunchBilling : ResultOpenSDKIapBase
    {
        public BillingOrderData data;

    }


    /*
     * // 结果返回: 请求商品列表
     * {
         "resultCode": 1,
         "data":
            [
                {
                    "productId":"com.xilm.hss2",
                    "price":"HK$7.20",
                    "title":"一塊錢喝水 (Healthy drink water)",
                    "priceAmount":720,
                    "type":"inapp",
                    "priceCurrencyCode":"HKD",
                    "subscriptionPeriod":""
                },
                {
                    "productId":"com.xilm.hss3",
                    "price":"HK$9.90",
                    "title":"测试3 (Healthy drink water)",
                    "priceAmount":990,
                    "type":"inapp",
                    "priceCurrencyCode":"HKD",
                    "subscriptionPeriod":""
                },
                {
                    "productId":"com.xilm.hss6",
                    "price":"HK$19.90",
                    "title":"测试6 (Healthy drink water)",
                    "priceAmount":1990,
                    "type":"inapp",
                    "priceCurrencyCode":"HKD",
                    "subscriptionPeriod":""
                },
                {
                     "productId":"com.xmil.hss",
                    "price":"HK$8.00",
                    "title":"一塊錢喝水1 (Healthy drink water)",
                    "priceAmount":800,
                    "type":"inapp",
                    "priceCurrencyCode":"HKD",
                    "subscriptionPeriod":""
                },
                {
                    "productId":"testing1110",
                    "price":"HK$8.00",
                    "title":"喝水商品1110 (Healthy drink water)",
                    "priceAmount":800,
                    "type":"inapp",
                    "priceCurrencyCode":"HKD",
                    "subscriptionPeriod":""
                }
            ],
            "msg": ""
    }*/


    [Serializable]
    public class ResultBillingProductList : ResultOpenSDKIapBase
    {
        public BillingProductData[] data;
    }




    /*//返回信息
{
    "resultCode": 1,
    "data":[
    {
        "productID":"testing1110",
        "purchaseTime":1646633932935,
        "purchaseToken":"eignpiabbnniebjbbhnnjaff.AO-J1Ow1bivzP6ELa4hmTJXJO0Ev6O06ur8wSl7J-ehPHSAqBE5rNeq6QFcZxeLSBRmk3Mog2wB1rfKrM8o6cx7z21qi1TFKdvlA4yKyOkGZ3H2UF2aYCcs"
    },
    {
        "productID":"com.xilm.hss6",
        "purchaseTime":1642745933192,
        "purchaseToken":"nkfcfjpdeebghabhihbiebdb.AO-J1OwGuCk9-fhWsMJkqQisgW1hlDgSv3cPj5zSWzvwCLyzqX6-mXB8g43SDzAXsRZ9o4D2w8iSzH_i0iTnSOBl0RLO4_QUDGuUBEzOF8k_q3_51_t9STg"
    },
    {
        "productID":"com.xmil.hss",
        "purchaseTime":1637070011053,
        "purchaseToken":"nioehaplmbnmkfkfjlmknjbp.AO-J1OwKBJeaP0CWYBBe1DKBAYkajkRcmkjX1O58K73JHlhoUY_CBTq8Oje_upBYo1bQOd4KX2I-e_4E2epwGq8CqoA3QPi9w3PfgR_BwlG_D1DMlYm1iF0"
    },
    {
        "productID":"com.xilm.hss2",
        "purchaseTime":1636537841454,
        "purchaseToken":"cjkhoefcaogpigheilkebmha.AO-J1OwTHv9l7RHkFpPlb9bVG4G91JP-78LVmSj45XGi968GyAIPCKe2z0IJsZDTeY9Z9j6-SnOBiv9iTroA1UxBCpd1mxglRSd9aGX1Uiml9x-pZlnyoiI"
    }
]

,
    "msg": ""
}*/
    [Serializable]
    public class ResultQueryHistoryPurchased : ResultOpenSDKIapBase
    {
        public BillingOrderData[] data;
    }
}