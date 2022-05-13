using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIMaskPage
{
    //[UIBinder("SingleMask")]
    //GameObject SingleMask;
    //[UIBinder("AniWithMask")]
    //GameObject AniWithMask;


    //int SingleDelayTimeId = 0;
    //int AniDelayTimeId = 0;

    //Action SingleMaskFunc;
    //Action AniMaskFunc;
    //protected void OnInit(params object[] parmas)
    //{
    //    InitMask();
    //}
    //void InitMask()
    //{
    //    SingleMask.SetActive(false);
    //    AniWithMask.SetActive(false);
    //}
    //public void ShowSingleMask(float DelayTime, Action MaskEndFunc,bool ForceCallBack=false)
    //{
    //    if (ForceCallBack)
    //    {
    //        if (MaskEndFunc != null)
    //        {
    //            MaskEndFunc();
    //        }
    //    }
    //    DelayTimeUtils.Instance.cancel_delay_run(SingleDelayTimeId);
    //    if (this.gameObject.activeSelf == false)
    //    {
    //        this.gameObject.SetActive(true);
    //    }
    //    SingleMask.SetActive(true);
    //    AniWithMask.SetActive(false);
    //    SingleDelayTimeId = DelayTimeUtils.Instance.delay_time_run(DelayTime, () =>
    //    {
    //        if (MaskEndFunc != null)
    //        {
    //            MaskEndFunc();
    //        }
    //        SingleMask.SetActive(false);
    //        AniWithMask.SetActive(false);
    //    });
    //    SingleMaskFunc = MaskEndFunc;
    //}
    //public void ShowAniMask(float DelayTime, Action MaskEndFunc, bool ForceCallBack = false)
    //{
    //    if (ForceCallBack)
    //    {
    //        if (AniMaskFunc != null)
    //        {
    //            AniMaskFunc();
    //        }
    //    }
    //    DelayTimeUtils.Instance.cancel_delay_run(AniDelayTimeId);
    //    AniWithMask.SetActive(true);
    //    SingleMask.SetActive(false);
    //    AniDelayTimeId = DelayTimeUtils.Instance.delay_time_run(DelayTime, () =>
    //    {
    //        if (AniMaskFunc != null)
    //        {
    //            AniMaskFunc();
    //        }
    //        SingleMask.SetActive(false);
    //        AniWithMask.SetActive(false);
    //    });
    //    AniMaskFunc = MaskEndFunc;
    //}
    //public void CloseSingleMask()
    //{
    //    DelayTimeUtils.Instance.cancel_delay_run(SingleDelayTimeId);
    //    SingleMask.SetActive(false);
    //}
    //public void CloseAniMask()
    //{
    //    DelayTimeUtils.Instance.cancel_delay_run(AniDelayTimeId);
    //    AniWithMask.SetActive(false);
    //}

}
