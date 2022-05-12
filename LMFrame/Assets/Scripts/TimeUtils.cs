using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class TimeUtils
    {
        /// <summary>
        /// 将秒转化成：时：分：秒
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static string FormatSecond(int second)
        {
            int hour = second / 3600;
            second = second % 3600;
            int minute = second / 60;
            second = second % 60;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        }

        public static string FormatSecond(long second)
        {
            long hour = second / 3600;
            second = second % 3600;
            long minute = second / 60;
            second = second % 60;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        }

        /// <summary>
        /// 将秒转化成：分：秒
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static string FormatSecondNoHour(int second)
        {
            int minute = second / 60;
            second %= 60;
            return $"{minute:D2}:{second:D2}";
        }

        // 当前时间戳，秒
        public static long TimestampSec => TimestampMillSec / 1000;

        public static long TimestampMillSec
        {
            get
            {
                var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return (long) ts.TotalMilliseconds;
            }
        }

        public static int OffsetHour(long timestamp)
        {
            var current = TimestampMillSec;
            if (timestamp >= current) return 0;
            var offset = current - timestamp;
            var sec = offset / 1000;
            var hour = sec / 3600;
            return Convert.ToInt32(hour);
        }

        // 时间分量转换成实际的秒数
        public static long GetDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            long dateVal = 0;
            try
            {
                DateTime dt = new DateTime(year, month, day, hour, minute, second);
                DateTime dt0 = new DateTime(1970, 1, 1);
                TimeSpan ts = new TimeSpan(dt.Ticks - dt0.Ticks);
                dateVal = (long) ts.TotalSeconds;
            }
            catch (Exception e)
            {
                Debug.LogError("GetDateTime err " + e.ToString());
            }

            return dateVal;
        }

        // 秒数转换成日期
        public static DateTime SecondsToDate(long seconds)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddSeconds(seconds);
            return dt;
        }

        // 返回格式：20201010
        public static string GetDay()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        public static DateTime Timestamp2DateTime(long timestampSec)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0);
            var target = start.AddSeconds(timestampSec);
            return target;
        }

        // 根据时间戳获取所在的月份
        public static int GetMonth(long timestampSec)
        {
            // var start = new DateTime(1970, 1, 1, 0, 0, 0);
            var target = Timestamp2DateTime(timestampSec);
            return target.Month;
        }

        // 根据时间戳获取该月有多少天
        public static int GetMonthDays(long timestampSec)
        {
            var target = Timestamp2DateTime(timestampSec);
            return DateTime.DaysInMonth(target.Year, target.Month);
        }
        public static int GetMonthDays(DateTime dt)
        {
            return DateTime.DaysInMonth(dt.Year, dt.Month);
        }

    static IEnumerator GetTime()
    {
        UnityWebRequest unityWeb = UnityWebRequest.Get("http://time.weather.gov.hk/cgi-bin/time5a.pr?a=7");
        unityWeb.timeout = 15;
        Debug.Log("开始获取网络时间");
        yield return unityWeb.SendWebRequest();
        TimeZoneInfo localZone = TimeZoneInfo.Local;
        if (unityWeb.downloadHandler.text == "" || unityWeb.downloadHandler.text.Trim() == "")//如果获取网络时间失败,改为获取系统时间
        {
        }
        else//成功获取网络时间
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            string[] timeStr = unityWeb.downloadHandler.text.Split('=');
            int time1 = (int)ts.TotalSeconds;
            int time2 = (int)(Convert.ToDecimal(timeStr[1]) / 1000);
        }
    }
}
