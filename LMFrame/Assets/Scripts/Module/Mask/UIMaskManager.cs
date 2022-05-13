using LMSF.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMaskManager : MonoSingleton<UIMaskManager>
{ 
    UIMaskPage uiMaskPage;

    public void InitUIMask()
    {
        //uiMaskPage = UIManager.Instance.OpenPage<UIMaskPage>();
    }
    public void OpenSingleMask(float delaytime = 0.5f, Action MaskEndFunc = null, bool ForceCallBack = false)
    {
        //uiMaskPage.ShowSingleMask(delaytime, MaskEndFunc, ForceCallBack);
    }
    public void OpenAniMask(float delaytime = 0.5f, Action MaskEndFunc = null, bool ForceCallBack = false)
    {
        //uiMaskPage.ShowAniMask(delaytime, MaskEndFunc, ForceCallBack);
    }
    public void CloseSingleMask()
    {
        //uiMaskPage.CloseSingleMask();
    }
    public void CloseAniMask()
    {
        //uiMaskPage.CloseAniMask();
    }


}
