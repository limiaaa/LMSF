﻿using System;
using System.Collections;
using UnityEngine;

namespace LMSF.Utils
{
    public class CoroutineMgr :MonoSingleton<CoroutineMgr>
    {
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

        public void BeginCoroutine(IEnumerator func)
        {
            StartCoroutine(func);
        }
    }
}