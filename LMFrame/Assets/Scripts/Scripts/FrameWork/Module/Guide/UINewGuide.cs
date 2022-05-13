using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class UINewGuidePage :MonoBehaviour, IDragHandler, IEndDragHandler
{
    //[UIBinder("Mask")]
    //Button mMaskBtn;
    //[UIBinder("DragMask")]
    //Transform mDragMask;

    //[UIBinder("Mask3D")]
    //Transform mMask3D;
    //[UIBinder("Pointer/Pointer1")]
    //Transform mpoint1;
    //[UIBinder("Pointer/Pointer2")]
    //Transform mpoint2;
    //[UIBinder("Pointer/Pointer3")]
    //Transform mpoint3;
    //[UIBinder("ActionTips")]
    //Transform mActionTips;
    //[UIBinder("TipsBg/TipsBg1")]//UI层 在手指后
    //Transform mTipBg1;
    //[UIBinder("TipsBg/TipsBg1/TipText")]
    //UGUILanguageText mTipBgText1;
    //[UIBinder("TipsBg/TipsBg2")]//UITile层 在手指前
    //Transform mTipBg2;
    //[UIBinder("TipsBg/TipsBg2/TipText")]
    //UGUILanguageText mTipBgText2;
    //[UIBinder("Camera")]
    //Transform mtitleCamera;

    //Transform CurrentTipBg;
    //UGUILanguageText CurrentTipBgText;

    //Transform CurrentFinger;

    //public void InitGuideUI()
    //{
    //    mTipBg1.gameObject.SetActive(false);
    //    mTipBg2.gameObject.SetActive(false);
    //    mpoint1.gameObject.SetActive(false);
    //    mpoint2.gameObject.SetActive(false);
    //    mpoint3.gameObject.SetActive(false);
    //    mActionTips.gameObject.SetActive(false);
    //    mMask3D.gameObject.SetActive(false);
    //    mMaskBtn.gameObject.SetActive(false);
    //    mDragMask.gameObject.SetActive(false);
    //    mtitleCamera.gameObject.SetActive(false);
    //}
    ////DistanceState传0是在手指前，传其他是在手指后
    //public void ShowText(string key, float PosY,int DistanceState)
    //{
    //    if (DistanceState == 0)
    //    {
    //        CurrentTipBg = mTipBg2;
    //        CurrentTipBgText = mTipBgText2;
    //    }
    //    else
    //    {
    //        CurrentTipBg = mTipBg1;
    //        CurrentTipBgText = mTipBgText1;
    //    }
    //    CurrentTipBg.position =new Vector3(0, PosY, mMask3D.transform.position.z-1);
    //    CurrentTipBgText.UpdateKey3d(key);
    //    CurrentTipBg.gameObject.SetActive(true);
    //}
    ////设置手指
    //public void ShowFinger(int FingerType,Vector3 Pos,Vector3 Rotation)
    //{
    //    switch (FingerType)
    //    {
    //        case 1://普通2D手指
    //            CurrentFinger = mpoint1;
    //            break;
    //        case 2://3D手指。中间镂空
    //            CurrentFinger = mpoint2;
    //            break;
    //        case 3://3D手指
    //            CurrentFinger = mpoint3;
    //            break;
    //    }
    //    CurrentFinger.position = Pos;
    //    CurrentFinger.rotation = Quaternion.Euler(Rotation);
    //    CurrentFinger.gameObject.SetActive(true);
    //}
    ////打开旋转提示
    //public void ShowTip()
    //{
    //    mActionTips.gameObject.SetActive(true);
    //}
    //public void ShowMask3D(Vector3 pos)
    //{
    //    mMask3D.gameObject.SetActive(true);
    //    pos.z = -1;
    //    mMask3D.position = pos;
    //}

    //public void ShowMaskBtn(Action GuideAction)
    //{
    //    mMaskBtn.onClick.RemoveAllListeners();
    //    mMaskBtn.onClick.AddListener(() =>
    //    {
    //        Debug.Log("Click Guide Action");
    //       GuideAction?.Invoke();
    //    });
    //    mMaskBtn.gameObject.SetActive(true);
    //}
    //Action DragCheckFunc;
    //Dictionary<GuideSteps, Action> EndDragDic = new Dictionary<GuideSteps, Action>();
    //Dictionary<GuideSteps, Action<PointerEventData>> DragingDic = new Dictionary<GuideSteps, Action<PointerEventData>>();
    //public void SaveDragFunc(GuideSteps step,Action<PointerEventData> DragingAction, Action EndDragAction)
    //{
    //    if (DragingDic.ContainsKey(step))
    //    {
    //        DragingDic[step] = DragingAction;
    //    }
    //    else
    //    {
    //        DragingDic.Add(step, DragingAction);
    //    }
    //    if (EndDragDic.ContainsKey(step))
    //    {
    //        EndDragDic[step] = EndDragAction;
    //    }
    //    else
    //    {
    //        EndDragDic.Add(step, EndDragAction);
    //    }
    //    mDragMask.gameObject.SetActive(true);
    //}
    //public Action<PointerEventData> GetDragingFunc(GuideSteps step)
    //{
    //    if (DragingDic.ContainsKey(step))
    //    {
    //        return DragingDic[step];
    //    }
    //    return null;
    //}
    //public Action GetEndDragFunc(GuideSteps step)
    //{
    //    if (EndDragDic.ContainsKey(step))
    //    {
    //        return EndDragDic[step];
    //    }
    //    return null;
    //}
    //public void ShowCamera()
    //{
    //    mtitleCamera.gameObject.SetActive(true);
    //}
    public void OnDrag(PointerEventData eventData)
    {
        if (NewGuideManager.Instance.GetCurrentGuideStep() == GuideSteps.Guide_4)
        {
            //GetDragingFunc(GuideSteps.Guide_4)?.Invoke(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (NewGuideManager.Instance.GetCurrentGuideStep() == GuideSteps.Guide_4)
        {
            //GetEndDragFunc(GuideSteps.Guide_4)?.Invoke();
        }
    }
}
