using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTTPManager : MonoSingleton<HTTPManager>
{
    HTTPHelper helper;
    protected override void Init()
    {
        base.Init();
        if (helper == null)
        {
            helper = new HTTPHelper();
        }
    }
    public void ClearDic()
    {
        helper.ClearDic();
        //AddGlobaContentDic();
    }
    public void AddContentDic(string key, string value)
    {
        helper.AddContentDic(key, value);
    }
    public void AddGlobaContentDic()
    {
        AddContentDic("machineCode", "machineCode");
        AddContentDic("loginType", "loginType");
        AddContentDic("accessToken", "accessToken");
    }
    public void SendDic(string api, Action<JsonData> success, Action<string> failed)
    {
        helper.SendDic(api, success, failed);
    }

    public void SendDicWithMask(string api, Action<JsonData> success, Action<string> failed)
    {
        helper.SendDic(api, (js) =>
        {
            success(js);
        }, (error) =>
        {
            failed(error);
        });
    }

}
