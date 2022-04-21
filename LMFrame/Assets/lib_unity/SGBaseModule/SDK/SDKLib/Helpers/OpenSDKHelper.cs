#if !USE_OPEN_SDK
namespace SG.SDK.OpenSDKHelper{
    public class OpenSDKHelper : FakeHelper
    {

        public OpenSDKHelper()
        {
            mName = "OpenSDKHelper";
        }

    }
}
#else
#if UNITY_ANDROID
using Open;

using Newtonsoft.Json;
using SG.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SG.SDK.OpenSDKHelper
{
    public class OpenSDKHelper : ISDKHelper

    {
        private const float AD_MAX_INTERVAL_TIME = 60.0f;



        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 常数
        /////////////////////////////////////////////////////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 成员
        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        bool mInitOk = false;
        Action<bool> mOuterInitFinishedCallback; // 缓存外部的初始化结束回调

        // 三类广告的key列表
        SDKInitParams mStaticDatas;

        private string mDistinctID = "";

        private int mVideoAdIndex;
        /// <summary>
        /// 视频广告流程接口
        /// </summary>
        private IListenerVideoAd mVideoAdListener;
        private float mLastVideoAdTime; // 最近一次成功播放视频广告的时间


        private IListenerInterstitialAd mInterstitialAdListener;

        private string mBannerAdKey;
        private string mRewardAdKey;
        private string mInterstitialAdKey;
        private string mNativeAdKey;

        public void PreloadAd(string bannerKey, string rvKey, string interstitialKey, string nativeKey)
        {

            if (bannerKey != null)
            {
                mBannerAdKey = bannerKey;
            }


            if (rvKey != null)
            {
                mRewardAdKey = rvKey;
                OpenSDK.Agent.PreLoadAd(new List<string>() { rvKey }, null, null);
            }

            if (interstitialKey != null)
            {
                mInterstitialAdKey = interstitialKey;
                OpenSDK.Agent.PreLoadAd(null, new List<string>() { interstitialKey }, null);
            }


            if (nativeKey != null)
            {
                mNativeAdKey = nativeKey;
                OpenSDK.Agent.PreLoadAd(null, null, new List<string>() { nativeKey });
            }
        }


        public OpenSDKHelper()
        {
            mInitOk = false;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 接口
        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool InitOK()
        {
            return mInitOk;
        }
        public bool InitFuncsOK()
        {
            return true; // 无论SDK是否初始化成功, 功能均可用
        }
        public void Init(SDKInitParams datas, Action<bool> onInitFinished)
        {
            if (datas == null)
            {
                DebugUtils.LogErrorWithEvent(23, "OpenSDKHelper", "SDK init datas is null");
                return;
            }

            mStaticDatas = datas;
            mOuterInitFinishedCallback = onInitFinished;
            // 调用SDK的初始化接口
            OpenSDK.Agent.InitOpenSDK(OnSDKInitFinished);
        }

        private void OnSDKInitFinished(string result)
        {
            DebugUtils.Log("OnSDKInitFinished:{0}", result);
            try
            {
                // 初始化完成, 处理数据
                var resultData = JsonConvert.DeserializeObject<OpenSDKResultDefault>(result);
                if (resultData != null && resultData.resultCode == (int)RESULT_CODE.SUCCESS)
                {
                    mInitOk = true;
                    mDistinctID = mStaticDatas.product_id + "-" + OpenSDK.Agent.GetReportDeviceId();
                }
                else
                {
                    // 初始化失败, 需要调用广告的预加载
                    mInitOk = false;
                }
            }
            catch (Exception e)
            {
                DebugUtils.LogErrorWithEvent(24, "OpenSDKHelper", "OnSDKInitError:{0}", result);
            }
            if (mOuterInitFinishedCallback != null)
            {
                mOuterInitFinishedCallback(mInitOk);
            }
        }

        public string GetDistinctID()
        {
            return mDistinctID;
        }



        private void Send(SDKAction action)
        {
            if (action != null)
            {
                _sendToServer(action.GetId(), action.GetActionData(), action.flag_send_lv);
            }
        }

        private void _sendToServer(string actionId, Dictionary<string, object> data, int sendLv= SDKAction.FLAG_SENDTO_TD)
        {
            // 调用SDK的发送接口
            if ((sendLv & SDKAction.FLAG_SENDTO_AF) == SDKAction.FLAG_SENDTO_AF)
            {
                OpenSDK.Agent.ReportAFEvent(actionId, data);
            }
            if ((sendLv & SDKAction.FLAG_SENDTO_TD) == SDKAction.FLAG_SENDTO_TD)
            {
                OpenSDK.Agent.ReportEvent(actionId, data);
            }
        }
        public void SendNormal(string actionId, Dictionary<string, object> data)
        {
            _sendToServer(actionId, data);
        }
        public void AddAction(SDKAction action)

        {
            if (action != null)
            {
                Send(action);
            }
        }
        public void BannerInit(RectTransform rectTransform)
        {
            //居上或者居下恒定为true
            bool referenceScreen = false;
            if (rectTransform == null)
            {
                referenceScreen = true;
            }
            OpenSDK.Agent.ConfigBannerView(rectTransform, referenceScreen);
            OpenSDK.Agent.PlayAd(AdType.BANNER, mBannerAdKey, "", new DefaultAdListener(_onBannerResult));
        }

        /*
         * banner
         */
        public void BannerShow()
        {
            OpenSDK.Agent.SetBannerVisibility(true);
        }

        public void BannerHide()
        {
            OpenSDK.Agent.SetBannerVisibility(false);
        }

        public void BannerRemove()
        {
            OpenSDK.Agent.RemoveBanner();
        }


        private void _onBannerResult(AdListenerMethodName methodName)
        {
            //DebugUtils.Log($"返回的方法是：{methodName.ToString()}");
            DebugUtils.Log("_onBannerResult {0}", methodName.ToString());
            switch (methodName)
            {
                case AdListenerMethodName.OnAdLoaded:
                    break;
                case AdListenerMethodName.OnAdLoadFailed:
                    break;
                case AdListenerMethodName.OnAdShowFailed:
                    break;
                case AdListenerMethodName.OnAdRewarded:
                    break;
                case AdListenerMethodName.OnAdClicked:
                    break;
                case AdListenerMethodName.OnAdShowed:
                    break;
                case AdListenerMethodName.OnAdClosed:
                    break;
                default:
                    break;
            }
        }

        /*
         * @@@@视频广告相关
         **/

        public bool VAdIsReady()
        {
            if (mRewardAdKey == null)
            {
                DebugUtils.LogErrorWithEvent(-1000, "OpenSDKHelper", "VAdIsReady, mRewardAdKey is null");
                return false;
            }

            return OpenSDK.Agent.AdIsReady(AdType.REWARD, mRewardAdKey);
        }

        public bool VAdShow(int index, string from, IListenerVideoAd listener)
        {
            if (mRewardAdKey == null)
            {
                DebugUtils.LogErrorWithEvent(-1000, "OpenSDKHelper", "VAdIsReady, mRewardAdKey is null");
                return false;
            }


            float deltaTime = Time.time - mLastVideoAdTime;
            if (deltaTime > AD_MAX_INTERVAL_TIME)
            {// 超过时间的话, 强制清除广告数据?
                ClearVideoAdDatas();
            }

            if (mVideoAdListener != null)
            {
                DebugUtils.LogErrorWithEvent(25, "OpenSDKHelper", "ShowVideoAd error: mVideoAdListener existed, index={0}, indexOrg={1}!", index, mVideoAdIndex);
                return false;
            }

            bool isReady = VAdIsReady();
            if (!isReady)
            {
                DebugUtils.LogErrorWithEvent(26, "OpenSDKHelper", "ShowVideoAd error: AdIsReady=false!");
                return false;
            }

            mVideoAdIndex = index;
            mVideoAdListener = listener;
            mLastVideoAdTime = Time.time;

            OpenSDK.Agent.PlayAd(AdType.REWARD, mRewardAdKey, from, new DefaultAdListener(_onVAdResult));

            return true;
        }


        private void _onVAdResult(AdListenerMethodName methodName)
        {
            //DebugUtils.Log($"返回的方法是：{methodName.ToString()}");
            DebugUtils.Log("_onVAdResult {0}", methodName.ToString());
            switch (methodName)
            {
                case AdListenerMethodName.OnAdLoaded:
                    if (mVideoAdListener != null) { mVideoAdListener.OnAdLoaded(); }
                    break;
                case AdListenerMethodName.OnAdLoadFailed:
                    if (mVideoAdListener != null) { mVideoAdListener.OnAdLoadFailed(); }
                    break;
                case AdListenerMethodName.OnAdShowFailed:
                    if (mVideoAdListener != null) { mVideoAdListener.OnAdShowFailed(); }
                    break;
                case AdListenerMethodName.OnAdRewarded:
                    if (mVideoAdListener != null) { mVideoAdListener.OnAdRewarded(); }
                    break;
                case AdListenerMethodName.OnAdClicked:
                    if (mVideoAdListener != null) { mVideoAdListener.OnAdClicked(); }
                    break;
                case AdListenerMethodName.OnAdShowed:
                    if (mVideoAdListener != null) { mVideoAdListener.OnAdShowed(); }
                    break;
                case AdListenerMethodName.OnAdClosed:
                    if (mVideoAdListener != null) { mVideoAdListener.OnAdClosed(); }
                    ClearVideoAdDatas();
                    break;
                default:
                    break;
            }
        }

        private void ClearVideoAdDatas()
        {
            mVideoAdIndex = -1;
            mVideoAdListener = null;
        }

        /*
        * @@@@插屏广告相关
        **/

        public bool InterstitialAdIsReady()
        {
            if (mInterstitialAdKey == null)
            {
                DebugUtils.LogErrorWithEvent(-1000, "OpenSDKHelper", "InterstitialAdIsReady, mInterstitialAdKey is null");
                return false;
            }
            return OpenSDK.Agent.AdIsReady(AdType.INTERSTITIAL, mInterstitialAdKey);
        }
        public bool InterstitialAdShow(string from, IListenerInterstitialAd listener)
        {
            if (mInterstitialAdKey == null)
            {
                DebugUtils.LogErrorWithEvent(-1000, "OpenSDKHelper", "InterstitialAdShow, mInterstitialAdKey is null");
                return false;
            }

            if (mInterstitialAdListener != null)
            {
                DebugUtils.LogErrorWithEvent(27, "OpenSDKHelper", "InterstitialAdShow error: mInterstitialAdListener existed");
            }

            bool isReady = InterstitialAdIsReady();
            if (!isReady)
            {
                DebugUtils.LogErrorWithEvent(28, "OpenSDKHelper", "InterstitialAdShow error: AdIsReady=false!");
                return false;
            }

            mInterstitialAdListener = listener;

            OpenSDK.Agent.PlayAd(AdType.INTERSTITIAL, mInterstitialAdKey, from, new DefaultAdListener(_onInterstitialAdResult));
            return true;
        }


        private void _onInterstitialAdResult(AdListenerMethodName methodName)
        {
            DebugUtils.Log("_onInterstitialAdResult {0}", methodName.ToString());
            switch (methodName)
            {
                case AdListenerMethodName.OnAdLoaded:
                    if (mInterstitialAdListener != null) { mInterstitialAdListener.OnAdLoaded(); }
                    break;
                case AdListenerMethodName.OnAdLoadFailed:
                    if (mInterstitialAdListener != null) { mInterstitialAdListener.OnAdLoadFailed(); }
                    break;
                case AdListenerMethodName.OnAdShowFailed:
                    if (mInterstitialAdListener != null) { mInterstitialAdListener.OnAdShowFailed(); }
                    break;
                case AdListenerMethodName.OnAdClicked:
                    if (mInterstitialAdListener != null) { mInterstitialAdListener.OnAdClicked(); }
                    break;
                case AdListenerMethodName.OnAdShowed:
                    if (mInterstitialAdListener != null) { mInterstitialAdListener.OnAdShowed(); }
                    break;
                case AdListenerMethodName.OnAdClosed:
                    if (mInterstitialAdListener != null) { mInterstitialAdListener.OnAdClosed(); }
                    ClearInterstitialAdDatas();
                    break;
                default:
                    break;
            }
        }

        private void ClearInterstitialAdDatas()
        {
            mInterstitialAdListener = null;
        }

        public bool ReportUserProfile(Dictionary<string, object> datas, bool isOnce = false)
        {
            if (!isOnce)
            {
                OpenSDK.Agent.ReportUserProfile(datas);

            }
            else
            {
                OpenSDK.Agent.ReportUserProfileOnce(datas);
            }
            return true;
        }
    }

    class OpenSDKUserData
    {
        long userId;
    }

    public class OpenSDKResultDefault
    {
        public int resultCode;
        public Dictionary<string, object> data;

        public long GetLong(string key)
        {
            return 0;
        }
    }

    class OpenSDKResultSDKInit
    {
        string countryCode;
        long currentTime;
        bool newUser;
        long userId;
    }
}
#endif // END ANDROID
#endif // END OPENSDK
