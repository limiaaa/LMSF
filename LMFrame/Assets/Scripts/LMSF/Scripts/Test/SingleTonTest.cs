using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;

public class SingleTonTest : MonoSingleton<SingleTonTest>
{
    public void StartTest()
    {
        Debug.Log("Start SingleTon Test");
    }
   
}
