using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening;
using UnityEngine.UI;
using System;
namespace LMSF.Utils
{
    public static class DoTweenUtils
    {
        public static Dictionary<string, Tween> TweenDic = null;
        public static Dictionary<string, Dictionary<string, Tween>> ModuleTweenDic = new Dictionary<string, Dictionary<string, Tween>>();
        public static void DoFade(Image img, float TargetNumber, float Duartion, float NowNumber = 0,string moduleKey="", string key = "")
        {
            if (img == null)
            {
                Debug.Log("需要做DoFade的图片组件为空");
                return;
            }
            var color = img.color;
            img.color = new Color(color.r, color.g, color.b, NowNumber);
            Tween tween = img.DOFade(TargetNumber, Duartion).OnComplete(() =>
            {
                if (key != ""&& moduleKey!="")
                {
                    CloseTween(moduleKey,key, false);
                }
            });
            if (moduleKey != "")
            {
                if (!ModuleTweenDic.ContainsKey(moduleKey))
                {
                    ModuleTweenDic.Add(moduleKey, new Dictionary<string, Tween>());
                }
            }
            if (key != "")
            {
                if (!ModuleTweenDic[moduleKey].ContainsKey(key))
                {
                    ModuleTweenDic[moduleKey].Add(key, tween);
                }
                else
                {
                    ModuleTweenDic[moduleKey][key]=tween;
                }
            }
        }
        public static void DoFade(Text text, float TargetNumber, float Duartion, float NowNumber = 0, string moduleKey = "", string key = "")
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
                if (key != "" && moduleKey != "")
                {
                    CloseTween(moduleKey, key, false);
                }
            });
            if (moduleKey != "")
            {
                if (!ModuleTweenDic.ContainsKey(moduleKey))
                {
                    ModuleTweenDic.Add(moduleKey, new Dictionary<string, Tween>());
                }
            }
            if (key != "")
            {
                if (!ModuleTweenDic[moduleKey].ContainsKey(key))
                {
                    ModuleTweenDic[moduleKey].Add(key, tween);
                }
                else
                {
                    ModuleTweenDic[moduleKey][key] = tween;
                }
            }
        }
        public static void DoFade(CanvasGroup canvasgroup, float TargetNumber, float Duartion, string moduleKey = "", string key = "")
        {
            if (canvasgroup == null)
            {
                Debug.Log("需要做DoFade的canvasgroup组件为空");
                return;
            }
            Tween tween = canvasgroup.DOFade(TargetNumber, Duartion).OnComplete(() =>
            {
                if (key != "" && moduleKey != "")
                {
                    CloseTween(moduleKey, key, false);
                }
            });
            if (moduleKey != "")
            {
                if (!ModuleTweenDic.ContainsKey(moduleKey))
                {
                    ModuleTweenDic.Add(moduleKey, new Dictionary<string, Tween>());
                }
            }
            if (key != "")
            {
                if (!ModuleTweenDic[moduleKey].ContainsKey(key))
                {
                    ModuleTweenDic[moduleKey].Add(key, tween);
                }
                else
                {
                    ModuleTweenDic[moduleKey][key] = tween;
                }
            }
        }
        public static void RunNumber(float StartNumber, float EndNumber, float time, string moduleKey = "", string key = "", Action<float> UpdateFunc = null, Action EndFunc = null)
        {
            Tween tween = DOTween.To(() => StartNumber, x => StartNumber = x, EndNumber, time).SetEase(Ease.Linear).OnUpdate(() =>
            {
                if (UpdateFunc != null)
                {
                    UpdateFunc(StartNumber);
                }
            }).OnComplete(() =>
            {
                EndFunc?.Invoke();
                if (key != "" && moduleKey != "")
                {
                    CloseTween(moduleKey, key, false);
                }
            });
            if (moduleKey != "")
            {
                if (!ModuleTweenDic.ContainsKey(moduleKey))
                {
                    ModuleTweenDic.Add(moduleKey, new Dictionary<string, Tween>());
                }
            }
            if (key != "")
            {
                if (!ModuleTweenDic[moduleKey].ContainsKey(key))
                {
                    ModuleTweenDic[moduleKey].Add(key, tween);
                }
                else
                {
                    ModuleTweenDic[moduleKey][key] = tween;
                }
            }
        }
        public static void CloseTween(string moduleKey,string key, bool NeedCallBack)
        {
            if (!ModuleTweenDic.ContainsKey(moduleKey))
            {
                Debug.Log(moduleKey + "：此Tween的Module不存在");
                return;
            }
            if (!ModuleTweenDic[moduleKey].ContainsKey(key))
            {
                Debug.Log(key + "：此Tween的key不存在");
                return;
            }
            ModuleTweenDic[moduleKey][key].Kill(NeedCallBack);
            ModuleTweenDic[moduleKey].Remove(key);
        }
        public static void CloseTweenByModule(string moduleKey,bool NeedCallBack=false)
        {
            if (!ModuleTweenDic.ContainsKey(moduleKey))
            {
                Debug.Log(moduleKey + "：此Tween的Module不存在");
                return;
            }
            foreach (var item in ModuleTweenDic[moduleKey])
            {
                item.Value.Kill(NeedCallBack);
            }
            ModuleTweenDic.Remove(moduleKey);
        }
        public static void CloseAllTween(bool NeedCallBack=false)
        {
            foreach (var item in ModuleTweenDic)
            {
                foreach (var tween in item.Value)
                {
                    tween.Value.Kill(false);
                }
            }
            ModuleTweenDic = new Dictionary<string, Dictionary<string, Tween>>();
        }
    }
}