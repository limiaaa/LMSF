using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using LitJson;

public static class ComFuncMgr
{
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

    //获取本地国家等数据
    public static IEnumerator getCountryCode()
    {
        string Url = "http://ip-api.com/json";
        UnityWebRequest webRequest = UnityWebRequest.Get(Url);
        yield return webRequest.SendWebRequest();
        if (webRequest.downloadHandler.isDone)
        {
            GetJson(webRequest.downloadHandler.text);
        }
    }

    static void GetJson(string JsonData)
    {
        Debug.Log("Json数据_______" + JsonData);
        var jsondata = JsonMapper.ToObject<JsonData>(JsonData);
        foreach (var item in jsondata)
        {
            Debug.Log(item.ToString());
        }
        Debug.Log(jsondata["country"].ToString());
    }

}
