using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening;
using UnityEngine.UI;
using System;

public static class DoTweenManager 
{
    public static Dictionary<string, Tween> TweenDic = new Dictionary<string, Tween>();
    public static void DoFade(Image img,float TargetNumber,float Duartion,float NowNumber=0, string key = "")
    {
        if (img == null)
        {
            Debug.Log("需要做DoFade的图片组件为空");
            return;
        }

        var color = img.color;
        img.color = new Color(color.r, color.g, color.b, NowNumber);

        Tween tween=img.DOFade(TargetNumber, Duartion).OnComplete(()=>
        {

            if (key != "")
            {
                CloseTween(key, false);
            }
        });
        if (key != "")
        {
            TweenDic.Add(key, tween);
        }
    }
    public static void DoFade(Text text, float TargetNumber, float Duartion, float NowNumber = 0, string key = "")
    {
        if (text == null)
        {
            Debug.Log("需要做DoFade的字体组件为空");
            return;
        }
        var color = text.color;
        text.color = new Color(color.r, color.g, color.b, NowNumber);
        Tween tween = text.DOFade(TargetNumber, Duartion).OnComplete(() =>
        {
            if (key != "")
            {
                CloseTween(key, false);
            }
        });
        if (key != "")
        {
            TweenDic.Add(key, tween);
        }
    }
    public static void DoFade(CanvasGroup canvasgroup, float TargetNumber, float Duartion, string key = "")
    {
        if (canvasgroup == null)
        {
            Debug.Log("需要做DoFade的canvasgroup组件为空");
            return;
        }
        Tween tween = canvasgroup.DOFade(TargetNumber, Duartion).OnComplete(() =>
        {
            if (key != "")
            {
                CloseTween(key, false);
            }
        });
        if (key != "")
        {
            TweenDic.Add(key, tween);
        }
    }
    /// <summary>
    /// 数字跑动
    /// </summary>
    /// <param name="StartNumber"></param>
    /// <param name="EndNumber"></param>
    /// <param name="time"></param>
    /// <param name="UpdateFunc"></param>
    /// <param name="EndFunc"></param>
    /// <returns></returns>
    public static void RunNumber(float StartNumber, float EndNumber, float time, string key = "", Action<float> UpdateFunc = null, Action EndFunc=null)
    {
        Tween tween = DOTween.To(() => StartNumber, x => StartNumber = x, EndNumber, time).SetEase(Ease.Linear).OnUpdate(() =>
        {
            if (UpdateFunc != null)
            {
                UpdateFunc(StartNumber);
            }
        }).OnComplete(() => {
            EndFunc?.Invoke();
            if (key != "")
            {
                CloseTween(key, false);
            }
        });
        if (key != "")
        {
            TweenDic.Add(key, tween);
        }
    }
    public static void CloseTween(string key,bool NeedCallBack)
    {
        if (!TweenDic.ContainsKey(key))
        {
            Debug.Log(key + "：此Tween的Key不存在");
        }
        TweenDic[key].Kill(NeedCallBack);
        TweenDic.Remove(key);
    }
    public static void CloseAllTween()
    {
        foreach(var item in TweenDic)
        {
            item.Value.Kill(false);
        }
    }
}
