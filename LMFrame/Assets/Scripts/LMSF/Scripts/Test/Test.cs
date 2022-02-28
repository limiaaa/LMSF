using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;

public class Test : MonoBehaviour
{
    public GameObject Img1;
    public GameObject Img2;
    void Start()
    {
        StartTest();
        //this.gameObject.E_IsNull();
        //this.transform.E_IsNull();
        //this.Img1.GetComponent<Image>().E_IsNull();
    }

    void StartTest()
    {
        DebugUtils.Log("啊啊啊啊");
        DebugUtils.LogWarning("啊啊啊啊");
        DebugUtils.LogError("啊啊啊啊");
        SingleTonTest.Instance.StartTest();
    }
    void Update()
    {
        
    }
}
