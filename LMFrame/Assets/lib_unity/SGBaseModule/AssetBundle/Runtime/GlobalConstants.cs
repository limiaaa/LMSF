using System;
using System.IO;
using UnityEngine;

public class G
{
    public static readonly WaitForSeconds QuarterSeconds = new WaitForSeconds(0.25f);
    public static readonly WaitForSeconds HalfSeconds = new WaitForSeconds(0.5f);
    public static readonly WaitForSeconds OneSecond = new WaitForSeconds(1f);
    public static readonly WaitForSeconds TwoSeconds = new WaitForSeconds(2f);
    public static readonly WaitForSeconds ThreeSeconds = new WaitForSeconds(3f);
    public static readonly YieldInstruction FiveSeconds = new WaitForSeconds(5);
    public static readonly YieldInstruction TenSeconds = new WaitForSeconds(10);
    public static long SPEED_LIMITATION = 400;//200KB;

    public const int CACHE_DATA_HOURS = 24;

    public const string FrameworkLuaScriptsPath = "LBLibraryUnityContainerCore/XLua/LuaScripts/";

    public static bool IsCacheValid(string path)
    {
        if (File.Exists(path))
        {
            var span = DateTime.Now - new FileInfo(path).LastWriteTime;
            if (span.TotalHours <= G.CACHE_DATA_HOURS)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// http://192.168.0.23:28080/moregame_manage
    /// http://moregame.libii.cn/moregame_manage
    /// </summary>
#if (DEVELOPMENT_BUILD || UNITY_EDITOR)
    public const string BASE_URL = "http://192.168.0.23:9090";

    public const string BASE_PROMOTE_URL = "http://192.168.0.23:9090/promote_app";
    public const string AD_URL_INTERSITIAL = BASE_PROMOTE_URL + "/cross/ad_info/v1";
#else
        public const string BASE_PROMOTE_URL = "http://gamepromote.libii.cn/promote_app";

        public const string BASE_URL = "http://moregame.libii.cn";
        public const string AD_URL_INTERSITIAL = BASE_PROMOTE_URL +"/cross/ad_info/v1";
#endif
    public const string AD_ICON_ENTRY_URL = BASE_PROMOTE_URL + "/more/icon_info/v1";

    public const string AD_URL_MANAGE_INFO =
        BASE_URL + "/moregame_manage/app/an/ad_manage_info/v0827";


    #region report

#if (DEVELOPMENT_BUILD || UNITY_EDITOR)
    public const string REPORT_BASE_URL = "http://192.168.0.23:28080";
    public const string REPORT_URL_AD = REPORT_BASE_URL + "/log_entry/promo";
#else
        public const string REPORT_BASE_URL = "http://track.libii.cn/v2";
        public const string REPORT_URL_AD = REPORT_BASE_URL + "/promo";
#endif


    public const string AD_ACTION_GET = "get";
    public const string AD_ACTION_BUFFER = "buffer";
    public const string AD_ACTION_SHOW = "show";
    public const string AD_ACTION_CLICK = "click";
    public const string AD_ACTION_INSTALL = "install";

    public const string GAME_ACTION_LAUNCH = "launch";
    public const string GAME_ACTION_INSTALL = "install";
    public const string GAME_ACTION_ACTIVE = "active";
    public const string GAME_ACTION_SESSION_IN = "sessionIn";
    public const string GAME_ACTION_SESSION_OUT = "sessionOut";

    public const string AD_SOURCE_INNER = "inner";
    public const string AD_SOURCE_OUTER = "outer";
    public const string AD_SOURCE_MOREGAME = "moregame";

    public const string AD_SHOW_TYPE_SPLASH = "splash";
    public const string AD_SHOW_TYPE_BANNER = "banner";
    public const string AD_SHOW_TYPE_INTER = "inter";
    public const string AD_SHOW_TYPE_VIDEO = "video";
    public const string AD_SHOW_TYPE_FLOAT = "float";
    public const string AD_SHOW_TYPE_MOREGAME_BUTTON_MAIN_ENTRY = "moregame_butt_main_entry";
    public const string AD_SHOW_TYPE_MOREGAME_BUTTON_GAME_OWN = "moregame_butt_game_own";
    public const string AD_SHOW_TYPE_MOREGAME_PROMOTION = "moregame_promotion";

    #endregion report


    public const string DEFAULT_URL_IOS = "https://apps.apple.com/developer/libii/id372214030";
    public const string DEFAULT_URL_IOS_EDU = "https://apps.apple.com/developer/bowei-zeng/id1474336588";

    public static string DEFAULT_URL_ANDROID
    {
        get
        {
            if (AppSeting.GetChannelName().ToLower() == "google")
            {
                return "https://play.google.com/store/apps/dev?id=7670331570699900357";
            }
            else
            {
                string url_format = "market://details?id={0}";
                return string.Format(url_format, AppSeting.GetAppBundleID());
            }
        }
    }
    
    public static string GetDateString()
    {
        return string.Format("{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    }
}