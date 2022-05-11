using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public GameObject Img1;
    public GameObject Img2;
    public Button btn;
    void Start()
    {
        StartTest();
        //this.gameObject.E_IsNull();
        //this.transform.E_IsNull();
        //this.Img1.GetComponent<Image>().E_IsNull();
        //***************************************************************************
        //DebugUtils.Log(CommonUtils.GetDeviceSystemInfo());
        //***************************************************************************
        //LitJsonTest ltest = new LitJsonTest();
        //ltest.age = "1";
        //ltest.name = "1";
        //ltest.from = "1";
        //Debug.LogError(CommonUtils.GetJsonFormIEnum(ltest));
        //***************************************************************************
        btn.AddButtonFunc(true,()=>{
            Debug.Log("1111");
            btn.RemoveButtonFunc();
        });
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

public class LitJsonTest
{
    public string name;
    public string age;
    public string from;
}
