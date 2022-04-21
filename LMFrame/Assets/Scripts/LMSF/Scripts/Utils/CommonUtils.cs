﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using LitJson;
using SG.Haptics;

namespace LMSF.Utils
{
    public static class CommonUtils
    {
#region 本机设备信息相关
        public static int GetRunPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WebGLPlayer:
                    return 0;
                case RuntimePlatform.WindowsPlayer:
                    return 1;
                case RuntimePlatform.Android:
                    return 2;
                case RuntimePlatform.IPhonePlayer:
                    return 3;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                    return 5;
                default:
                    return 4;
            }
        }
        //获得设备信息Json
        public static string GetDeviceSystemInfo()
        {
            string systeminfo = "";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                Type t = typeof(SystemInfo);
                System.Reflection.PropertyInfo[] infos = t.GetProperties();
                foreach (var item in infos)
                {
                    object v = item.GetValue(null);
                    dic.Add(item.Name, v.ToString());
                }
                systeminfo = LitJson.JsonMapper.ToJson(dic);
            }
            catch (Exception e)
            {
                DebugUtils.LogError("获取硬件信息失败！！");
            }
            return systeminfo;
        }
        //获取本地国家等数据
        public static IEnumerator getCountryCode()
        {
            string Url = "http://ip-api.com/json";
            UnityWebRequest webRequest = UnityWebRequest.Get(Url);
            yield return webRequest.SendWebRequest();
            if (webRequest.downloadHandler.isDone)
            {
                var jsondata = JsonMapper.ToObject<JsonData>(webRequest.downloadHandler.text);
                foreach (var item in jsondata)
                {
                    Debug.Log(item.ToString());
                }
                Debug.Log(jsondata["country"].ToString());
            }
        }
#endregion
#region LitJson相关
        //从结构体获取字符串
        public static string GetJsonFormIEnum(object dic)
        {
            if (dic != null)
            {
                return LitJson.JsonMapper.ToJson(dic);
            }
            DebugUtils.LogError("需要转为json的字典为空");
            return "";
        }
        //从字符串获取结构体
        public static T GetDataFormJson<T>(string Json)
        {
            if (Json != "" && Json!=null)
            {
                return LitJson.JsonMapper.ToObject<T>(Json);
            }
            DebugUtils.LogError("需要转为JsonData的结构为空");
            return default;
        }
#endregion
#region 查看复杂结构体(可能失败)
        public static string LookDic(IDictionary dic,string LookTitle = "LookDic",bool ExternalUse=true)
        {
            //if (!SG.SettingReader.ScriptableObject.IsLogEnabled)
            //{
            //    return "";
            //}
            StringBuilder st = new StringBuilder();
            if (ExternalUse)
            {
                st.Append(LookTitle);
                st.AppendLine();
            }
            foreach (var item in dic.Keys)
            {
                st.Append(item + "/");
            }
            st.AppendLine();
            foreach (var item in dic.Values)
            {
                if (item is string || item is int || item is float || item is long || item is Boolean || item is decimal)
                {
                    st.Append(item.ToString() + "/");
                }
                else
                {
                    if (item is IList)
                    {
                        st.Append(LookList(item as IList, "", false) + "/");
                    }
                    else if (item is IDictionary)
                    {
                        st.Append(LookDic(item as IDictionary, "", false) + "/");
                    }
                    else
                    {
                        st.Append(LookClass(item, false) + "/");
                    }
                }
            }
            if (ExternalUse)
            {
                Debug.Log(st);
            }
            return st.ToString();
        }
        public static string LookList(IList list,string LookTitle="LookList", bool ExternalUse = true)
        {
            //if (!SG.SettingReader.ScriptableObject.IsLogEnabled)
            //{
            //    return "";
            //}
            StringBuilder st = new StringBuilder();
            if (ExternalUse)
            {
                st.Append(LookTitle);
                st.AppendLine();
            }
            foreach (var item in list)
            {
                if (item is string || item is int || item is float || item is long || item is Boolean || item is decimal)
                {
                    st.Append(item.ToString() + "/");
                }
                else
                {
                    if (item is IList)
                    {
                        st.Append(LookList(item as IList, "", false) + "/");
                    }
                    else if (item is IDictionary)
                    {
                        st.Append(LookDic(item as IDictionary, "", false) + "/");
                    }
                    else
                    {
                        st.Append(LookClass(item, false) + "/");
                    }
                }
            }
            if (ExternalUse)
            {
                Debug.Log(st);
            }
            return st.ToString();
        }
        public static string LookClass<T>(T t, bool ExternalUse = true)
        {
            //if (!SG.SettingReader.ScriptableObject.IsLogEnabled)
            //{
            //    return "";
            //}
            Type type;
            StringBuilder sb = new StringBuilder();
            try
            {
                type = t.GetType();
                sb.Append("Look Class：" + t.ToString());
                sb.AppendLine();
            }
            catch
            {
                sb.AppendLine();
                sb.Append("Null");
                return sb.ToString();
            }

            var Fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
 
            foreach (var finfo in Fields)
            {
                var test = finfo.GetValue(t);
                if (test == null)
                    continue;
                sb.Append(finfo.Name.ToString());
                sb.Append(": ");
                if(test is string||test is int ||test is float ||test is long ||test is Boolean|| test is decimal)
                {
                    sb.Append(test.ToString());
                }
                else
                {
                    if(test is IList)
                    {
                        sb.Append(LookList(test as IList,"",false));
                    }
                    else if (test is IDictionary)
                    {
                        sb.Append(LookDic(test as IDictionary, "", false));
                    }
                }
                sb.AppendLine();
                sb.AppendLine();
            }
            if (ExternalUse)
            {
                Debug.Log(sb.ToString());
            }
            return sb.ToString();
        }
#endregion
#region 射线检测
        //检测点击的是否是此预制体
        public static bool IsPointTarget(GameObject target)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = EventSystem.current.currentInputModule.input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            for (int i = 0; i < results.Count; ++i)
            {
                if (results[i].gameObject == target) return true;
            }

            return false;
        }
        public static GameObject RaycastCube()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject collidered = hit.collider.gameObject;
                //if (collidered.CompareTag(Global.Tag.cubeItemTag))
                //{
                //    string nameToId = collidered.name;
                //    int id = int.Parse(nameToId);
                //    return mCubeItems[id];
                //}
                return collidered;
            }

            return null;
        }
#endregion
#region 时间相关
        //时间参数变为秒数
        public static float GetSubSeconds(DateTime startTime, DateTime endTime)
        {
            System.TimeSpan startSpan = new TimeSpan(startTime.Ticks);
            System.TimeSpan endSpan = new TimeSpan(endTime.Ticks);
            System.TimeSpan subSpan = endSpan.Subtract(startSpan).Duration();
            return (float)subSpan.TotalSeconds;
        }
        //时间参数变为毫秒数
        public static float GetSubMilliSeconds(DateTime startTime, DateTime endTime)
        {
            System.TimeSpan startSpan = new TimeSpan(startTime.Ticks);
            System.TimeSpan endSpan = new TimeSpan(endTime.Ticks);
            System.TimeSpan subSpan = endSpan.Subtract(startSpan).Duration();
            return (float)subSpan.TotalSeconds * 1000;
        }
        //传入秒数，展示倒计时;
        public static List<string> FormatTime(long second)
        {
            long hour = second / 3600;
            second = second % 3600;
            long minute = second / 60;
            second = second % 60;
            List<string> li = new List<string>();
            li.Add(string.Format("{0:D2}", second));
            li.Add(string.Format("{0:D2}", minute));
            li.Add(string.Format("{0:D2}", hour));
            return li;
        }
        public static List<string> FormatTimeOnlyMin(long second)
        {
            long hour = second / 3600;
            second = second % 3600;
            long minute = second / 60 + hour * 60;
            second = second % 60;
            List<string> li = new List<string>();
            li.Add(string.Format("{0:D2}", second));
            li.Add(string.Format("{0:D2}", minute));
            li.Add(string.Format("{0:D2}", 0));
            return li;
        }
        #endregion
#region 邮件相关
        //唤起本地邮件应用并自动填充部分信息
        public static void SendEmail()
        {
            string content =
                 "Dear players, for your benefit, please do not delete the following information:<br>\r\n" +
                 "PackageName:{0}<br>\r\n" +
                 "AppVersion:{1}<br>\r\n" +
                 "DeviceId:{2}<br>\r\n" +
                 "Device:{3}<br>\r\n" +
                 "Platform:{4}<br>\r\n" +
                 "OS Version:{5}<br>\r\n" +
                 "Country:{6}<br>\r\n" +
                 "Please write your feedback below, your support is our biggest motivation:<br>\r\n";
            string body = string.Format(content,
                Application.identifier,
                Application.version,
                SystemInfo.deviceUniqueIdentifier,
                SystemInfo.deviceName,
                Application.platform,
                SystemInfo.operatingSystem,
                Application.systemLanguage);
            string Tital = "{0}/{1}/{2}/feedback";
            string TitalFormat = string.Format(Tital, Application.identifier, Application.platform, Application.version);
            Uri uri = new Uri(string.Format("mailto:{0}?subject={1}&body={2}", "", TitalFormat, body));
            //第二个参数是邮件的标题 Application.OpenURL(uri.AbsoluteUri);
            Application.OpenURL(uri.AbsoluteUri);
        }
        #endregion
        public static void MoreTimeVrate(int size, int time = 1)
        {
            //if (LocalJsonDataUtils.Instance.gameData.IsVibration)
            //{
            //    Debug.Log("嗡嗡嗡");
                CoroutineHelper.Instance.StartCoroutine(MoreTimeVibrate(size, time));
            //}
        }
        static IEnumerator MoreTimeVibrate(int size, int time)
        {
            for (int i = 1; i <= time; i++)
            {
                HapticsManager.Vibrate(size);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}

