#if !USE_OPEN_SDK
namespace SG.SDK.OpenSDKHelper{
    public class OpenSDKPIapHelper : FakeHelper
    {

        public OpenSDKPIapHelper()
        {
        }

    }
}
#else
using Open;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SG.Utils;

namespace SG.SDK.OpenSDKHelper
{



    public class OpenSDKIapHelper : IIAPHelper
    {
        private const string TAG = "OpenSDKIapHelper";

        public OpenSDKIapHelper()
        {

        }

        void IIAPHelper.Init()
        {
            //DebugUtils.Log("OpenSDKIapHelper.{0} ", DebugUtils.FuncName());

            OpenSDK.Agent.InitBilling();
        }


        /// <summary>
        /// 查询商品列表
        /// </summary>
        /// <param name="onQueryBillingProductListEnd"></param>
        public void QueryBillingProductList(List<string> productIdList, Action<ResultBillingProductList> onQueryBillingProductListEnd)
        {
            //DebugUtils.Log("OpenSDKIapHelper.{0} ", DebugUtils.FuncName());
            OpenSDK.Agent.QueryBillingProductList(productIdList, resultJsonStr =>
            {
                DebugUtils.Log("QueryBillingProductList 订单信息:{0}", resultJsonStr);
                ResultBillingProductList resultData = null;
                try
                {
                    resultData = JsonConvert.DeserializeObject<ResultBillingProductList>(resultJsonStr);
                }
                catch (Exception)
                {
                    DebugUtils.LogErrorWithEvent(-2000, TAG, "{0} err: data = {1}", "QueryBillingProductList", resultJsonStr);
                }

                if (onQueryBillingProductListEnd != null)
                {
                    onQueryBillingProductListEnd.Invoke(resultData);
                }
            });
        }

        /// <summary>
        /// 消耗成功付费但未发放奖励的订单
        /// 如果有多个商品未发放奖励, 回调会被调用多次
        /// </summary>
        /// <param name="onQueryPurchaseEnd"></param>
        public void QueryPurchase(Action<ResultQueryPurchase> onQueryPurchaseEnd)
        {
            //DebugUtils.Log("OpenSDKIapHelper.{0} ", DebugUtils.FuncName());
            OpenSDK.Agent.QueryPurchase(resultJsonStr =>
            {
                DebugUtils.Log("QueryPurchase 订单信息:{0}", resultJsonStr);

                ResultQueryPurchase resultData = null;
                try
                {
                    resultData = JsonConvert.DeserializeObject<ResultQueryPurchase>(resultJsonStr);
                }
                catch (Exception)
                {
                    DebugUtils.LogErrorWithEvent(-2000, TAG, "{0} err: data = {1}", "QueryPurchase", resultJsonStr);
                }

                if (onQueryPurchaseEnd != null)
                {
                    onQueryPurchaseEnd.Invoke(resultData);
                }
            });


        }


        /// <summary>
        /// 购买商品
        /// 如果返回成功, 可直接发货
        /// </summary>
        /// <param name="onLaunchBillingEnd"></param>
        public void LaunchBilling(string productId, Action<ResulLaunchBilling> onLaunchBillingEnd)
        {
            //DebugUtils.Log("OpenSDKIapHelper.{0} ", DebugUtils.FuncName());
            OpenSDK.Agent.LaunchBilling(productId, resultJsonStr =>
            {
                DebugUtils.Log("LaunchBilling 订单信息:{0}", resultJsonStr);

                ResulLaunchBilling resultData =  null;
                try
                {
                    resultData = JsonConvert.DeserializeObject<ResulLaunchBilling>(resultJsonStr);
                }
                catch (Exception)
                {
                    DebugUtils.LogErrorWithEvent(-2000, TAG, "{0} err: data = {1}", "LaunchBilling",  resultJsonStr);
                }

                if (onLaunchBillingEnd != null)
                {
                    onLaunchBillingEnd.Invoke(resultData);
                }
            });
        }

        /// <summary>
        /// 请求历史购买订单
        /// </summary>
        /// <param name="onQueryHistoryPurchasedEnd"></param>
        public void QueryHistoryPurchased(Action<ResultQueryHistoryPurchased> onQueryHistoryPurchasedEnd)
        {
            //DebugUtils.Log("OpenSDKIapHelper.{0} ", DebugUtils.FuncName());
            OpenSDK.Agent.QueryHistoryPurchased( resultJsonStr =>
            {
                DebugUtils.Log("QueryHistoryPurchased 订单信息:{0}", resultJsonStr);

                ResultQueryHistoryPurchased resultData = null;
                try
                {
                    resultData = JsonConvert.DeserializeObject<ResultQueryHistoryPurchased>(resultJsonStr);
                }
                catch (Exception)
                {
                    DebugUtils.LogErrorWithEvent(-2000, TAG, "{0} err: data = {1}", "QueryHistoryPurchased", resultJsonStr);
                }

                if (onQueryHistoryPurchasedEnd != null)
                {
                    onQueryHistoryPurchasedEnd.Invoke(resultData);
                }
            });
        }

        private void testParseJson()
        {
            string jsonStr = @"{
    'resultCode': 1,
    'data':{
                'productId':'testing1110',
        'billingChannel':'GooglePay',
        'isAutoRenewing':false,
        'purchaseTime': 1636619077936,   
        'orderId':'xxxxxxx',            
        'purchaseToken':'xxxxxxx'
    },
    'msg': ''
 }";

            try
            {
                var obj = JsonConvert.DeserializeObject(jsonStr);

            }
            catch (Exception)
            {

                throw;
            }


        }

    }



}
#endif