using SG.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG.SDK
{
    public class FakeHelper : ISDKHelper
    {

        protected string mName = "FakeHelper";

        void ISDKHelper.Init(SDKInitParams datas, Action<bool> onInitFinished)
        {
            Debug.LogErrorFormat("This is fakesdk init!!!! name = {0}", mName);
            if (onInitFinished != null)
            {
                onInitFinished(true);
            }
        }

        bool ISDKHelper.InitOK()
        {
            return true;
        }

        bool ISDKHelper.InitFuncsOK()
        {
            return false;
        }

        string ISDKHelper.GetDistinctID()
        {
            return "";
        }

        bool ISDKHelper.ReportUserProfile(Dictionary<string, object> datas, bool isOnce)
        {
            return true;
        }
        void ISDKHelper.SendNormal(string actionId, Dictionary<string, object> data)
        {

        }
        void ISDKHelper.AddAction(SDKAction action)
        {
        }

        void ISDKHelper.BannerInit(RectTransform rectTransform)
        {
        }

        void ISDKHelper.BannerShow()
        {
        }

        void ISDKHelper.BannerHide()
        {
        }

        void ISDKHelper.BannerRemove()
        {
        }

        bool ISDKHelper.VAdIsReady()
        {
            return false;
        }

        bool ISDKHelper.VAdShow(int adIndex, string from ,IListenerVideoAd listener)
        {
            return false;
        }

        bool ISDKHelper.InterstitialAdIsReady()
        {
            return false;
        }

        bool ISDKHelper.InterstitialAdShow(string from, IListenerInterstitialAd listener)
        {
            return false;
        }

        void ISDKHelper.PreloadAd(string bannerKey, string rvKey, string interstitialKey, string nativeKey)
        {
        }
    }
}