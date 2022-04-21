#if !USE_FIREBASE_SDK
using SG.Utils;

namespace SG.SDK.FirebaseHelper{
	public interface IFirebaseCallbacks
	{
	    void OnRemoteFetchFinish(bool success);
	    void OnFirebaseTokenFinished(bool success, string token);
	}
	
    public class FirebaseHelper : FakeHelper
    {

        public FirebaseHelper()
        {
            mName = "FirebaseHelper";

        }

    }
}
#else
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Firebase.Installations;
using Firebase.Messaging;
#if USE_FIRESTORE_SDK
using Firebase.Firestore;
#endif
using SG.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace SG.SDK.FirebaseHelper{
public interface IFirebaseCallbacks
{
    void OnRemoteFetchFinish(bool success);
    void OnFirebaseTokenFinished(bool success, string token);
}
public class FirebaseHelper : MonoSingleton<FirebaseHelper>
{
    private const string TAG = "FirebaseHelper";
    private const string UNKNOW_DistinctID = "UNKNOW";

    private Firebase.FirebaseApp mApp;
    private bool mInitOk;
    private bool mRemoteConfigFetchOK;
    private string mDistinctID = UNKNOW_DistinctID;
    private string mToken = "";
    private string mMessagingToken = "";

    private string mUserID = null;

#if USE_FIRESTORE_SDK
    private FirebaseFirestore mFirestoreDB;
#endif

    private Dictionary<string, object> mRemoteConfigs;
    //private Action<bool> onRemoteConfigFinish = null;
    //private Action<bool, string> onGetTokenFinished = null;

    private IFirebaseCallbacks mCallbacks;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultConfigs"> 默认的参数列表 </param>
    /// <param name="_onRemoteConfigFinish"></param> 远程参数回调
    /// <param name="needToken"></param>
    /// <param name="_onGetTokenFinished"></param>
    new public void Init(Dictionary<string, object> defaultConfigs, bool needToken, IFirebaseCallbacks callbacks)
    {
        mCallbacks = callbacks;
       //onGetTokenFinished = callbacks.OnFirebaseTokenFinished;

        DebugUtils.Log("[" + TAG + "]FirebaseHelper.Init Enter");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;


            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                mApp = Firebase.FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                InitializeFirebaseMessaging(); // init message module

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                mInitOk = true;
                DebugUtils.Log("[" + TAG + "]FirebaseHelper.Init OK");

                if (defaultConfigs != null)
                {
                    mRemoteConfigs = defaultConfigs;
                    // These are the values that are used if we haven't fetched data from the
                    // server
                    // yet, or if we ask for values that the server doesn't have:
                    FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaultConfigs).ContinueWithOnMainThread(task_remote_config => { });

                    RemoteConfigFetchDataAsync();
                    if (needToken) { 
                        InitTokenAsync(); 
                    }
                }
            }
            else
            {
                //DebugUtils.LogError(System.String.Format("[" + TAG + "]" +
                //    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                DebugUtils.LogErrorWithEvent(22, "FirebaseHelper", System.String.Format("[" + TAG + "]" +
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                mInitOk = false;
            }
        });
    }

    public bool InitOK()
    {
        return mInitOk;
    }

    void InitializeFirebaseMessaging()
    {
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.SubscribeAsync(TAG).ContinueWithOnMainThread(task => {
            LogTaskCompletion(task, "SubscribeAsync");
        });
        DebugUtils.Log("["+TAG+"]Firebase Messaging Initialized");

        // This will display the prompt to request permission to receive
        // notifications if the prompt has not already been displayed before. (If
        // the user already responded to the prompt, thier decision is cached by
        // the OS and can be changed in the OS settings).
        FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
          task => {
              LogTaskCompletion(task, "RequestPermissionAsync");
          }
        );

        FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(
        task => {
            if(task.IsCompleted)
            {
                mMessagingToken = task.Result;
            }
            else
            {
                mMessagingToken = "";
            }
            DebugUtils.Log("[" + TAG + "]Messaging GetTokenAsync {0}", mMessagingToken);
            LogTaskCompletion(task, "GetTokenAsync");
      }
);
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        DebugUtils.Log("[" + TAG + "]Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        DebugUtils.Log("[" + TAG + "]From: " + e.Message.From);
        DebugUtils.Log("[" + TAG + "]Message ID: " + e.Message.MessageId);
        DebugUtils.Log("[" + TAG + "]Received a new message");
        var notification = e.Message.Notification;
        if (notification != null)
        {
            DebugUtils.Log("[" + TAG + "]title: " + notification.Title);
            DebugUtils.Log("[" + TAG + "]body: " + notification.Body);
            var android = notification.Android;
            if (android != null)
            {
                DebugUtils.Log("[" + TAG + "]android channel_id: " + android.ChannelId);
            }
        }
        if (e.Message.From.Length > 0)
            DebugUtils.Log("[" + TAG + "]from: " + e.Message.From);
        if (e.Message.Link != null)
        {
            DebugUtils.Log("[" + TAG + "]link: " + e.Message.Link.ToString());
        }
        if (e.Message.Data.Count > 0)
        {
            DebugUtils.Log("[" + TAG + "]data:");
            foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
                     e.Message.Data)
            {
                DebugUtils.Log("[" + TAG + "]  " + iter.Key + ": " + iter.Value);
            }
        }
    }

    bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            DebugUtils.Log("[" + TAG + "]" + operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            DebugUtils.Log("[" + TAG + "]" + operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string errorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    errorCode = String.Format("[" + TAG + "]" + "Error.{0}: ",
                      ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
                }
                DebugUtils.Log(errorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            DebugUtils.Log("[" + TAG + "]" + operation + " completed");
            complete = true;
        
        }
        return complete;
    }

    public void SetDistinctID(string distinct_id)
    {
        mDistinctID = distinct_id ?? UNKNOW_DistinctID;
    }

    public bool AddAction(SDKAction action)
    {
        //DebugUtils.Log("["+ TAG + "]FirebaseHelper.AddAction start");
        if (!InitOK()) { return false; }
        if ((action.flag_send_lv & SDKAction.FLAG_SENDTO_FB) != SDKAction.FLAG_SENDTO_FB) { return false; }

        // Log an event with multiple parameters, passed as a struct:
        var parameters = makeParamsFromAction(action);
        FirebaseAnalytics.LogEvent( action.id, parameters);
        //DebugUtils.Log("{0}","["+ TAG + "]FirebaseHelper.AddAction end:" + action.Dump());
        return true;
    }


    private Parameter[] makeParamsFromAction(SDKAction action)
    {
        // Log an event with multiple parameters, passed as a struct:
        Parameter[] parameters = {
            new Parameter("distinct_id", mDistinctID),
            new Parameter("version", action.version),
            new Parameter("user_days_open", action.user_days_open),
            new Parameter("user_days_install", action.user_days_install),
            new Parameter("act", action.act),
            new Parameter("from", action.from),
            new Parameter("object", action.obj),
            new Parameter("scene", action.scene),
            new Parameter("level_type", action.level_type),
            new Parameter("level_id", action.level_id),
            new Parameter("amount", action.amount),
            new Parameter("sequence", action.sequence),
            new Parameter("fps", action.fps),
            new Parameter("remote_tag", action.remote_tag),
            new Parameter("max_level_id", action.max_level_id),
        };
        return parameters;
    }

    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    private Task RemoteConfigFetchDataAsync()
    {
        DebugUtils.Log("[" + TAG + "]FirebaseHelper.RemoteConfigFetchDataAsync");
        Task fetchTask =  FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(RemoteConfigFetchComplete);
    }

    private void RemoteConfigFetchComplete(Task fetchTask)
    {
        bool success = false;
        if (fetchTask.IsCanceled)
        {
            DebugUtils.Log("[" + TAG + "]Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            DebugUtils.Log("[" + TAG + "]Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            DebugUtils.Log("[" + TAG + "]Fetch completed successfully!");
        }

        var info = FirebaseRemoteConfig.DefaultInstance.Info;

        DebugUtils.Log("[" + TAG + "]FirebaseHelper.RemoteConfigFetchComplete:status"+ info.LastFetchStatus.ToString() + "," + mRemoteConfigs);
        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(task => {
                    DebugUtils.Log(String.Format("[" + TAG + "]FirebaseHelperRemote data loaded and ready (last fetch time {0}, count:{1}).",
                                   info.FetchTime,  mRemoteConfigs.Count));
                    foreach (var item in mRemoteConfigs)
                    {
                        DebugUtils.Log("[" + TAG + "]xxx:key:{0},default:{1}, value:{2} ", item.Key, item.Value, FirebaseRemoteConfig.DefaultInstance.GetValue(item.Key).StringValue);

                    }
                });
                success = true;
                break;
            case LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case FetchFailureReason.Error:
                        DebugUtils.Log("[" + TAG + "]FirebaseHelperFetch failed for unknown reason");
                        break;
                    case FetchFailureReason.Throttled:
                        DebugUtils.Log("[" + TAG + "]FirebaseHelperFetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case LastFetchStatus.Pending:
                DebugUtils.Log("[" + TAG + "]Latest Fetch call still pending.");
                break;
        }

            if(mCallbacks != null)
            {
                mCallbacks.OnRemoteFetchFinish(success);
            }
        DebugUtils.Log("[" + TAG + "]RemoteConfigFetchComplet end remote config count:{0}.", mRemoteConfigs.Count);

        foreach (var item in mRemoteConfigs)
        {
            DebugUtils.Log("[" + TAG + "]xxx:key:{0},default:{1}, value:{2} " , item.Key, item.Value, FirebaseRemoteConfig.DefaultInstance.GetValue(item.Key).StringValue);
        }
        mRemoteConfigFetchOK = true;

    }

    public bool RemoteConfigFetchOK()
    {
        return mRemoteConfigFetchOK;
    }

    public ConfigValue RemoteConfigGet(string key)
    {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key);
    }


    public string RemoteConfigGetString(string key)
    {
        ConfigValue value = RemoteConfigGet(key);
        if (value.StringValue!=null)
        {
            return value.StringValue;
        }
        return null;
    }

    public double RemoteConfigGetDouble(string key)
    {
        var value = RemoteConfigGet(key);
        if (typeof(double) == value.GetType())
        {
            return value.DoubleValue;
        }
        return 0;
    }

    public long RemoteConfigGetLong(string key)
    {
        var value = RemoteConfigGet(key);
        if (typeof(long) == value.GetType())
        {
            return value.LongValue;
        }
        return 0;
    }


    public bool RemoteConfigGetBool(string key)
    {
        var value = RemoteConfigGet(key);
        if (typeof(bool) == value.GetType())
        {
            return value.BooleanValue;
        }
        return false;
    }

    public IEnumerable<byte> RemoteConfigGetByteArray(string key)
    {
        var value = RemoteConfigGet(key);
        if (typeof(IEnumerable<byte>) == value.GetType())
        {
            return value.ByteArrayValue;
        }
        return null;
    }

    private void InitTokenAsync()
    {
        
         FirebaseInstallations.DefaultInstance.GetTokenAsync( true ).ContinueWith(
          task =>
          {
              bool success = false;
              string token = "";
              if (!(task.IsCanceled || task.IsFaulted) && task.IsCompleted)
              {
                  Debug.Log(String.Format("["+TAG+"]Installations token success{0}", task.Result));
                  mToken = task.Result;
                  success = true;
                  token = mToken;
              }
              else
              {
                  mToken = "";
                  Debug.Log(String.Format("[" + TAG + "]Installations token failed {0}", task.Result));
              }

              if (mCallbacks != null)
              {
                  mCallbacks.OnFirebaseTokenFinished(success, token);
               }
          });
    }

    public string GetToken()
    {
        return mToken;
    }

    public string GetMessagingToken()
    {
        return mMessagingToken;
    }


    private void _test()
    {

    }


    public bool FirestoreInit()
    {
#if USE_FIRESTORE_SDK
            if( mFirestoreDB == null)
            {
                mFirestoreDB = FirebaseFirestore.DefaultInstance;
            }

            return true;
#else
            return false;
#endif
        }
        // 检查云端数据库是否已就绪
        public bool FirestoreIsReady()
        {
#if USE_FIRESTORE_SDK
            if (string.IsNullOrEmpty(mToken)) { return false; }
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// 向云端数据写入玩家数据
        /// </summary>
        /// <param name="docName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool FirestoreSaveUserData(string colName, string docName, Dictionary<string, object> data, Action<bool> onSaveUserDataEnd)
        {
                if (!FirestoreIsReady() ) { return false; }

    #if USE_FIRESTORE_SDK

                if( data == null)
                {
                    DebugUtils.LogErrorWithEvent(-1, TAG, "FirestoreSaveUserData: data is null");
                    return false;
                }

                if ( string.IsNullOrEmpty(colName) || colName.Length < 8 || string.IsNullOrEmpty(docName) || docName.Length < 8)
                {
                    DebugUtils.LogErrorWithEvent(-1, TAG, "FirestoreSaveUserData: invalid key:{0}.{1}",colName, docName);
                    return false;
                }

                // 文档句柄
                var docRef = mFirestoreDB.Collection(colName).Document(docName);
                if (docRef == null)
                {
                    return false;
                }

                // 传送数据
                docRef.SetAsync(data).ContinueWithOnMainThread(task => {

                    bool success = false;
                    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        success = true;
                        // 操作成功
                        DebugUtils.Log("[" + TAG + "]FirestoreSaveUserData success");

                    }
                    else
                    {
                        // 操作失败
                        DebugUtils.Log("[" + TAG + "]FirestoreSaveUserData failed, IsCompleted={0}, IsCanceled={1}, IsFaulted={2}",
                            task.IsCompleted, task.IsCanceled, task.IsFaulted);
                    }

                    if (onSaveUserDataEnd != null)
                    {
                        onSaveUserDataEnd.Invoke(success);
                    }
                });

                return true;

    #else
                return false;
    #endif
         }

        /// <summary>
        /// 从云端数据库读取玩家数据
        /// </summary>
        /// <param name="colName"> 数据集名称 </param>
        /// <param name="docName"> 文档名称(key) </param>
        /// <param name="onLoadEnd"> 结果回调
        /// 回调返回值说明:
        ///  bool true~表示操作成功 false~表示操作失败, 需要注意, 操作成功的情况下, 也有可能没有数据
        ///  data 在成功的情况下, null表示远端没有数据, 也就是没有存档, 否则为远端存档数据
        /// </param>
        /// <returns> true ~发起请求成功  false~发起请求失败 </returns>
        public bool FirestoreLoadUserData(string colName, string docName, Action<bool, Dictionary<string, object>> onLoadEnd)
        {
            if (!FirestoreIsReady()) { return false; }

#if USE_FIRESTORE_SDK
            if (string.IsNullOrEmpty(colName) || colName.Length < 8 || string.IsNullOrEmpty(docName) || docName.Length < 8)
            {
                DebugUtils.LogErrorWithEvent(-1, TAG, "FirestoreLoadUserData: invalid key:{0}.{1}", colName, docName);
                return false;
            }

            // 数据句柄
            var docRef = mFirestoreDB.Collection(colName).Document(docName);

            // 获取数据
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {

                bool success = false;
                Dictionary<string, object> data = null; // 数据集
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    success = true;
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                        data = snapshot.ToDictionary();
                    }
                    else
                    {
                        Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
                    }

                    // 调用成功了
                    DebugUtils.Log("[" + TAG + "]FirestoreLoadUserData success");

                }
                else
                {
                    mToken = "";
                    DebugUtils.Log("[" + TAG + "]FirestoreLoadUserData failed, IsCompleted={0}, IsCanceled={1}, IsFaulted={2}",
                        task.IsCompleted, task.IsCanceled, task.IsFaulted);
                }

                if (onLoadEnd != null)
                {
                    onLoadEnd.Invoke(success, data);
                }
            });


            return true;

#else
            return false;
#endif
        }

    }
}
#endif

