using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum MsgType{
    text,
    link,
    actionCard,

}

public class DingMgsNet
{
    public string test_actionCard = "{" +
        "\"actionCard\": {" +
        "\"title\": \"乔布斯 20 年前想打造一间苹果咖啡厅，而它正是 Apple Store 的前身\", " +
        "\"text\": \"打包：test\", " +
        "\"hideAvatar\": \"0\", \"btnOrientation\": \"0\", " +
        "\"btns\": [" +
        "{" +
        "\"title\": \"内容不错\", " +
        "\"actionURL\": \"http://localhost/aaa.mp4\"" +
        "}, " +
        "{" +
        "\"title\": \"不感兴趣\"," +
        "\"actionURL\": \"http://192.168.1.12/1.zip\"" +
        "}" +
        "]" +
        "}," +
        "\"msgtype\": \"actionCard\"}";

    public void PostRequest(string methodName, string jsonString, Action<string> callback)
    {
        string url = methodName;
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            
            webRequest.SendWebRequest();
            while (!webRequest.isDone) {; }
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                if (callback != null)
                {
                    callback(null);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(webRequest.downloadHandler.text);
                }
            }
        }
    }
}
