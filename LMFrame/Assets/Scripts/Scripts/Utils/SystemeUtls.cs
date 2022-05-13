using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;
using System;
using UnityEngine.Networking;
using LitJson;

public static class SystemeUtls
{
    public static bool IsLowDevice()
    {
        string gpuType = SystemInfo.graphicsDeviceType.ToString();
        if (SystemInfo.graphicsMemorySize <= 1024)
            return true;
        if (SystemInfo.systemMemorySize <= 2048)
            return true;

        return false;
    }
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
}
