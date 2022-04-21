using System;
using System.Collections;
using System.Collections.Generic;
using SG.Utils;
using UnityEngine;

public class CoroutineMgr : MonoSingleton<CoroutineMgr>
{
     public void Delay(float delay, Action callback)
     {
          StartCoroutine(new CoroutineData().DelayCall(delay , callback));
     }
     
     public class CoroutineData
     {
          public IEnumerator DelayCall(float delay , Action callback)
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
