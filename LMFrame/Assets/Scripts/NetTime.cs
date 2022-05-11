using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using System.Text;

public class NetTime : MonoBehaviour
{
    public Text TextT;
    public string timeURL = "https://time.weather.gov.hk/cgi-bin/time5a.pr?a=1";
    public string timeURL2 = "http://www.baidu.com";
    void Start()
    {
        StartCoroutine(GetTime());
        //Get();
    }
    void Get()
    {
        WebRequest wr = WebRequest.Create(timeURL2);
        Stream s = wr.GetResponse().GetResponseStream();
        StreamReader sr = new StreamReader(s, Encoding.Default);
        string all = sr.ReadToEnd(); //读取网站的数据
        int start = 0;
        int end = 0;
        string sa = "";
        while (all.Contains("<i>"))
        {
            start = all.IndexOf("<i>");
            end = all.IndexOf("</i>");
            string content = all.Substring(start + 3, end - start - 3);
            all = all.Substring(end + 1, all.Length - (end + 1));
            sa += content + "。                    ";
        }
        sr.Close();
        s.Close();
        TextT.text = all;
    }
    string monthAndDay;
    IEnumerator GetTime()
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
            TextT.text = time2.ToString();
        }

    }
}