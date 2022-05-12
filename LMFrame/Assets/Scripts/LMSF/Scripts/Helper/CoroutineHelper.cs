using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CoroutineHelper :MonoSingleton<CoroutineHelper>
    {
        public Dictionary<string, IEnumerator> FuncDic = null;
        public Dictionary<string, Dictionary<string, IEnumerator>> ModuleDic = new Dictionary<string, Dictionary<string, IEnumerator>>();

        public void Delay(float delay,Action callBack)
        {
            StartCoroutine(new CoroutineData().DelayCall(delay, callBack));
        }
        public class CoroutineData
        {
            public IEnumerator DelayCall(float delay, Action callback)
            {
                yield return new WaitForSeconds(delay);
                callback?.Invoke();
            }
        }
        public void BeginCoroutine(IEnumerator func, string moduleKey="", string corKey="")
        {
            if (moduleKey != "")
            {
                if (!ModuleDic.ContainsKey(moduleKey))
                {
                    ModuleDic.Add(moduleKey, new Dictionary<string, IEnumerator>());
                }
            }
            if (corKey != "")
            {
                if (!ModuleDic[moduleKey].ContainsKey(corKey))
                {
                    ModuleDic[moduleKey].Add(corKey, func);
                }
                else
                {
                    ModuleDic[moduleKey][corKey] = func;
                }
            }
            StartCoroutine(func);
        }
        public void CloseCoroutine(string moduleKey = "", string corKey = "")
        {
   
            if (ModuleDic.ContainsKey(moduleKey))
            {
                if (ModuleDic[moduleKey].ContainsKey(corKey))
                {
                    StopCoroutine(ModuleDic[moduleKey][corKey]);
                }
                else
                {
                    Debug.Log("无此协程Key_" + corKey);
                }
            }
            else
            {
                Debug.Log("无此协程模块_" + moduleKey);
            }
        }

        public void CloseCoroutineByModule(string moduleKey = "")
        {
            if (ModuleDic.ContainsKey(moduleKey))
            {
                foreach(var item in ModuleDic[moduleKey])
                {
                    StopCoroutine(item.Value);
                }
                ModuleDic.Remove(moduleKey);
            }
        }

        public void CloseAllCoroutine()
        {
            foreach (var item in ModuleDic)
            {
                foreach (var cor in item.Value)
                {
                    StopCoroutine(cor.Value);
                }
            }
            ModuleDic = new Dictionary<string, Dictionary<string, IEnumerator>>();
        }
    }
