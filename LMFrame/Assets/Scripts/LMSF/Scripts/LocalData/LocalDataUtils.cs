using System.Collections.Generic;
using UnityEngine;
namespace LMSF.Utils
{
    public static class LocalDataUtils
    {
        //第几次登录
        private static int TotalLoginTime = 0;
        //当前登录版本
        private static string LoginVersion;
        //上次登录版本
        private static string LastLoginVersion;

        public static List<string> LocalDataList = new List<string>();

        public static void InitLocalDataFunc()
        {
            RefeshLoginTimeNumber();
        }
        private static void RefeshLoginTimeNumber()
        {
            TotalLoginTime = GetLocalData("TotalLoginTime", 0);
            TotalLoginTime++;
            SetLocalData("TotalLoginTime", TotalLoginTime);
            LastLoginVersion = GetLocalData("LoginVersion", "0");
            LoginVersion = Application.version;
            SetLocalData("LoginVersion", LoginVersion);
        }

        public static void SetLocalData(string Key, int localNumber)
        {
            if (!LocalDataList.Contains(Key))
            {
                LocalDataList.Add(Key);
            }
            PlayerPrefs.SetInt(Key, localNumber);
        }
        public static void SetLocalFloatData(string Key, float localNumber)
        {
            if (!LocalDataList.Contains(Key))
            {
                LocalDataList.Add(Key);
            }
            PlayerPrefs.SetFloat(Key, localNumber);
        }
        public static void SetLocalData(string Key, string localString)
        {
            if (!LocalDataList.Contains(Key))
            {
                LocalDataList.Add(Key);
            }
            PlayerPrefs.SetString(Key, localString);
        }
        public static int GetLocalData(string Key, int DefaultData)
        {
            if (!LocalDataList.Contains(Key))
            {
                LocalDataList.Add(Key);
            }
            return PlayerPrefs.GetInt(Key, DefaultData);
        }
        public static float GetLocalFloatData(string Key, float DefaultData)
        {
            if (!LocalDataList.Contains(Key))
            {
                LocalDataList.Add(Key);
            }
            return PlayerPrefs.GetFloat(Key, DefaultData);
        }
        public static string GetLocalData(string Key, string DefaultData)
        {
            if (!LocalDataList.Contains(Key))
            {
                LocalDataList.Add(Key);
            }
            return PlayerPrefs.GetString(Key, DefaultData);
        }
        public static void ClearGameData()
        {
            Debug.Log(LocalDataList.Count);
            if (LocalDataList == null)
                return;
            foreach (var item in LocalDataList)
            {
                PlayerPrefs.DeleteKey(item);
            }
        }
        public static int GetLoginTimeNumber()
        {
            return TotalLoginTime;
        }
        /// <summary>
        /// 本次登录版本号
        /// </summary>
        /// <returns></returns>
        public static string GetLoginVersion()
        {
            return LoginVersion;
        }
        /// <summary>
        /// 上次登录版本号
        /// </summary>
        /// <returns></returns>
        public static string GetLastVersion()
        {
            return LastLoginVersion;
        }
    }
}