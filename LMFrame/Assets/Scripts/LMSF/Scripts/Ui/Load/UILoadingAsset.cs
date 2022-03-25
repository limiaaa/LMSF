using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[UIResPath("UI/UI_LoadingAsset" , PageType.PopBox , UIPageLoadType.ForceResources)]
//public class UILoadingAsset : ABaseUI
//{
//    [UIBinder("LoadingSlider")]
//    private Slider mSlider;

//    [UIBinder("LoadingSlider/Tip/Text")]
//    private Text mProgressTipText;

//    private string mTipMsg = "...{0:f}%";

//    private float NowProgressValue = 0.0f;
//    public void SetProgress(float v)
//    {
//        if (mSlider == null) return;
//        mSlider.value = v;

//        mProgressTipText.text = string.Format(mTipMsg, v * 100);
//        NowProgressValue = v;
//    }
//    public void CtrlTipSpine()
//    {
        
//    }

//    public float GetProgress()
//    {
//        return NowProgressValue;
//    }
//}
