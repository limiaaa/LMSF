using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPHelper
{
    private string SendMsgValue;
    private int gameType=4;
    public int gameEnum;
    private string key = "mZRhWQz1YlbgAZOWrYb4oqYJECnDWqSQ";
    private Dictionary<string, string> SendContent=new Dictionary<string, string>();
    private Dictionary<string, string> RequireMsg = new Dictionary<string, string>();

    public void ClearDic()
    {
        SendContent.Clear();
        RequireMsg.Clear();
    }
    public void AddContentDic(string key,string value)
    {
        SendContent.Add(key, value);
    }
    public void AddMsgDic(string content,string api)
    {
        RequireMsg.Add("content", content);
        RequireMsg.Add("gameType", gameType.ToString());
        RequireMsg.Add("gameEnum", gameEnum.ToString());
        RequireMsg.Add("key", key);
        RequireMsg.Add("api", api);
    }
    string Token;
    string AesKey;
    string WebUrl;
    
    public void SendDic(string msgtip,Action<JsonData> success, Action<string> failed)
    {
        string values = CreatContente(SendContent);
        if(Token != "")
        {
            values = values + "&" +"token=" + Token;
        }
        values = AesEncrypt(values, AesKey);

        AddMsgDic(values, msgtip);
        SendMsg(values, success,failed);
    }
    public void SendMsg(string value, Action<JsonData> success, Action<string> failed)
    {
        SendMsgValue = value;
        //StartCoroutine(Get());
        CoroutineManager.Instance.BeginCoroutine(Post(success, failed));
    }

    public string CreatContente(Dictionary<string, string> value)
    {
        string Content = "";
        foreach(KeyValuePair<string, string> kvp in value)
        {
            Content = Content + kvp.Key + "=" + kvp.Value + "&";
        }
        Content = Content.Substring(0, Content.Length - 1);
        
        return Content;
    }
    IEnumerator Post(Action<JsonData> success , Action<string> failed)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(WebUrl, RequireMsg);
        webRequest.timeout = 15;
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.LogError("Post服务器错误：" + webRequest.error);
            failed?.Invoke(webRequest.error);
        }
        else
        {
            if (webRequest.downloadHandler.text.Contains("code"))
            {
                var jsondata = JsonMapper.ToObject<JsonData>(webRequest.downloadHandler.text);
                if (jsondata["code"].ToString() == "1001")
                {
                    Debug.Log("获取消息成功");
                    Debug.Log(webRequest.downloadHandler.text);
                    success?.Invoke(jsondata);
                }
                else
                {
                    //SUCCESS(1001),//成功
                    //ERROR(404),//失败
                    // API_COLSE(405),//api配置关闭
                    // NODE_NOT_FOUND(406),//服务节点未找到
                    // PARAMETER_ERROR(407),//参数错误
                    // SERVER_ERROR(408),//服务器内部错误
                    //VERIFY_ERROR(409),//验证失败
                    //TOKEN_INVALID(410),//token失效
                    Debug.Log("获取消息失败："+ jsondata["code"].ToString());
                    Debug.Log("失败消息Content："+ jsondata["content"].ToString());
                    failed?.Invoke(jsondata["code"].ToString());
                }
            }
            else
            {
                Debug.LogError("Post-downloadHandler.text错误+"+webRequest.downloadHandler.text);
                failed?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }
    IEnumerator Get()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(WebUrl);
        webRequest.timeout = 10;
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log("Get服务器错误：" + webRequest.error);
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
        }
    }

    public string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 8, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds).ToString().ToString();
    }
    public static string AesEncrypt(string str, string key)
    {
        if (string.IsNullOrEmpty(str)) return null;
        Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
        byte[] keyArray = null;
        using (var sha1 = new SHA1CryptoServiceProvider())
        {
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(key));
            var rd = sha1.ComputeHash(hash);
            keyArray = rd.Take(16).ToArray();
        }
        RijndaelManaged rm = new RijndaelManaged
        {
            Key = keyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };
        ICryptoTransform cTransform = rm.CreateEncryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return byteToHexStr(resultArray);
    }
    public static string byteToHexStr(byte[] bytes)
    {
        string returnStr = "";
        if (bytes != null)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                returnStr += bytes[i].ToString("X2");
            }
        }
        return returnStr;
    }
    public static string AESDecrypt(string str, string key)
    {
        if (string.IsNullOrEmpty(str)) return null;
        Byte[] toEncryptArray = Convert.FromBase64String(str);


        byte[] keyArray = null;
        using (var sha1 = new SHA1CryptoServiceProvider())
        {
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(key));
            var rd = sha1.ComputeHash(hash);
            keyArray = rd.Take(16).ToArray();
        }

        RijndaelManaged rm = new RijndaelManaged
        {
            Key = keyArray,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };

        ICryptoTransform cTransform = rm.CreateDecryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return Encoding.UTF8.GetString(resultArray);
    }
}
