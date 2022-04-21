using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;


[UIResPath("Assets/MainApp/Art/Prefabs/UI/UI_Debug.prefab", PageType.PopBox)]
public class UIDebug : BaseUI
{
    [UIBinder("content")]
    private GameObject mcontent;
    [UIBinder("content/fps")]
    private Text fps;

    [UIBinder("content/ScrollView/Viewport/Content/ItemBtn")]
    private GameObject mBtnItem;
    [UIBinder("content/Input/InputField")]
    private InputField mInputField;
    [UIBinder("content/Input2/InputField")]
    private InputField mInputFieldFireBaseToken;
    [UIBinder("content/RemoteConfigInfo/Info")]
    private Text mRemoteConfigInfo;
    [UIBinder("content/ScrollView/Viewport/Content")]
    private Transform mBtnItemRoot;

    [UIBinder("GM")]
    private Button mGMBtn;
    

    private bool isOpen = true;
    protected override void OnInit(params object[] parmas)
    {
        base.OnInit(parmas);
        AddDebug("上一关", () =>
        {
  
        });
        AddDebug("下一关", () =>
       {

       });

        AddDebug("跳关", () =>
       {
     
       });
        
        AddDebug("加金币", () =>
        {
            
        });

        AddDebug("签到刷新", () =>
        {

        });
        AddDebug("转盘刷新", () =>
        {

        });

        mGMBtn.onClick.AddListener(ClickDebug);
        mBtnItem.gameObject.SetActive(false);
    }
    private void ClickDebug()
    {
        isOpen = !isOpen;
        mcontent.SetActive(isOpen);
    }
    protected override void OnOpenPage(params object[] parmas)
    {
        base.OnOpenPage(parmas);
        isOpen = false;
        mcontent.SetActive(false);
    }

    private void AddDebug(string name, Action callback)
    {
        GameObject btn = GameObject.Instantiate(mBtnItem) as GameObject;
        btn.transform.SetParent(mBtnItemRoot, false);

        btn.GetComponent<Button>().onClick.AddListener(() => callback());

        Text t = btn.GetComponentInChildren<Text>();
        t.text = name;
    }

    private void FPS(string s)
    {
        fps.text = s;
    }
}
