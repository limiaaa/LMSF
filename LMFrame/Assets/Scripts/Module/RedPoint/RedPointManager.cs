using LMSF.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RedPointType
{
    Test,
}

public class RedPointManager : MonoSingleton<RedPointManager>
{

    bool OpenRedPointSystem = false;

    Dictionary<RedPointType, Transform> redPointDic = new Dictionary<RedPointType, Transform>();

    public void RegistRedPoint(RedPointType redPointType,Transform trans)
    {
        if (!OpenRedPointSystem)
        {
            return;
        }
        if (!redPointDic.ContainsKey(redPointType))
        {
            redPointDic.Add(redPointType, trans);
        }
    }
    public void RefeshRedPointState(RedPointType redPointType,bool redPointState)
    {
        if (!OpenRedPointSystem)
        {
            return;
        }
        if (redPointDic.ContainsKey(redPointType))
        {
            if (redPointDic[redPointType])
                redPointDic[redPointType].gameObject.SetActive(redPointState);
        }
    }
    public void CloseRedPointSystem()
    {
         OpenRedPointSystem = true;
        foreach(var item in redPointDic)
        {
            if (item.Value != null)
            {
                item.Value.gameObject.SetActive(false);
            }
        }
    }
}
