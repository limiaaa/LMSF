using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        //写在路径初始化后面
        LocalJsonDataUtils.Instance.LoadAll();
    }


}
