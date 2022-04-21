
#if !USE_FACEBOOK_SDK
using SG.Utils;

namespace SG.SDK.FacebookHepler
{
    public class FacebookHelper : MonoSingleton<FacebookHelper>
    {

        public FacebookHelper()
        {
            //mName = "FacebookHelper";
        }

    }
}
#else
using SG.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
namespace SG.SDK.FacebookHepler
{
    public class FacebookHelper : MonoSingleton<FacebookHelper>
    {
        private const string TAG = "FacebookHelper";
        private const string UNKNOW_DistinctID = "UNKNOW";

        private bool mInitOk;
        private string mDistinctID = UNKNOW_DistinctID;
        new public void Init()
        {
            DebugUtils.Log("[" + TAG + "]FacebookHelper.Init Enter");
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }

        }
        private void InitCallback()
        {
            DebugUtils.Log("[" + TAG + "]FacebookHelper.InitCallback enter: {0}", FB.IsInitialized);

            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                DebugUtils.Log("[" + TAG + "]Failed to Initialize the Facebook SDK");
            }

            mInitOk = FB.IsInitialized;
            DebugUtils.Log("[" + TAG + "]FacebookHelper.Init InitCallback: {0}" , FB.IsInitialized );

        }

        private void OnHideUnity(bool isGameShown)
        {
            DebugUtils.Log("[" + TAG + "]OnHideUnity.Init enter");

            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        public bool InitOK()
        {
            return mInitOk;
        }

        public void SetDistinctID(string distinct_id)
        {
            mDistinctID = distinct_id ?? UNKNOW_DistinctID;
        }

        //public bool AddAction(SDKAction action)
        //{
        //    DebugUtils.Log("[" + TAG + "]FacebookHelper.AddAction start");
        //    if (!InitOK()) { return false; }

        //    // Log an event with multiple parameters, passed as a struct:
        //    var parameters = makeParamsFromAction(action);
        //    Firebase.Analytics.FirebaseAnalytics.LogEvent(action.id, parameters);
        //    DebugUtils.Log("[" + TAG + "]FacebookHelper.AddAction end:" + action.Dump());
        //    return true;
        //}



    }
}
#endif

