using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LitJsonUtils
{
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
        if (Json != "" && Json != null)
        {
            return LitJson.JsonMapper.ToObject<T>(Json);
        }
        DebugUtils.LogError("需要转为JsonData的结构为空");
        return default;
    }
}
