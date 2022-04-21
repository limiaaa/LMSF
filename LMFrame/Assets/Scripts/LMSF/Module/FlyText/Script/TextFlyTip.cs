//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;


//public enum FlyTip
//{
//    Default,
//    Accurate,
//    Amazing,
//    Good,
//    Great,
//    Complete,
//    Maze,
//    Picture,
//    Unbelievable,
//}

//[UIResPath(Global.PFB_UI_ROOT_PATH + "UI_TextFlyTip.prefab", PageType.Effect)]
//public class TextFlyTip : ABaseUI
//{
//    int isFlytingTime = 0;
//    protected override void OnInit(params object[] parmas)
//    {
//        base.OnInit(parmas);
//    }
//    protected override void OnOpenPage(params object[] parmas)
//    {
//        base.OnOpenPage(parmas);
//        if (typeof(bool) == parmas[0].GetType())
//        {
//            FlyArtTip(parmas);
//        }
//        else
//        {
//            Fly(parmas);
//        }
//    }

//    protected override void OnClosePage(params object[] parmas)
//    {
//        base.OnClosePage(parmas);
//    }
//    GameObject GetCloneObj(FlyTip flyTip)
//    {
//        string flyObjName = flyTip.ToString();
//        if (flyTip != FlyTip.Default)
//        {
//            flyObjName = "Art/" + flyTip.ToString();
//        }
//        GameObject cloneObj = Instantiate(transform.Find(flyObjName).gameObject);
//        cloneObj.transform.SetParent(this.transform);
//        cloneObj.E_Reset();
//        cloneObj.gameObject.SetActive(true);
//        return cloneObj;
//    }
//    FlyTip currentFlyTip=FlyTip.Default;
//    bool isFlyingArtText = false;
//    Tween currentTween;
//    GameObject currentFlyObj;
//    private void FlyArtTip(params object[] parmas)
//    {
//        //不会出现同一个
//        if(currentFlyTip == (FlyTip)parmas[1])
//        {
//            Debug.Log("现在正在弹这个艺术字体");
//            return;
//        }
//        //当出现完成提示并且还有另外个提示在场
//        if ((FlyTip)parmas[1] == FlyTip.Complete&&currentTween!=null)
//        {
//            if (currentTween != null)
//            {
//                currentTween?.Kill(false);
//            }
//            if (currentFlyObj != null)
//            {
//                Destroy(currentFlyObj);
//            }
//            currentFlyTip = FlyTip.Default;
//            isFlyingArtText = false;
//            isFlytingTime--;
//        }

//        if (isFlyingArtText)
//        {
//            Debug.Log("现在正在弹其他艺术字体");
//            return;
//        }
//        currentFlyTip = (FlyTip)parmas[1];
//        isFlyingArtText = true;
//        Transform flyObj =GetCloneObj((FlyTip)parmas[1]).transform;
//        if (currentFlyTip == FlyTip.Maze||currentFlyTip==FlyTip.Picture)
//        {
//            timeList.Clear();
//            timeList.Add(0.25f);
//            timeList.Add(0.75f);
//            timeList.Add(0.25f);
//        }
//        else
//        {
//            timeList.Clear();
//            timeList.Add(0.25f);
//            timeList.Add(0.25f);
//            timeList.Add(0.15f);
//        }
//        ShowArt(flyObj,1f);
//    }
//    private void Fly(params object[] parmas) 
//    {
//        UGUILanguageText flyObj = GetCloneObj(FlyTip.Default).transform.Find("Text").GetComponent<UGUILanguageText>();

//        if (parmas.Length<3) {
//            if (parmas.Length>1)
//            {
//                flyObj.UpdateKey(parmas[0].ToString(),parmas[1].ToString());
//            }
//            else
//            {
//                flyObj.UpdateKey(parmas[0].ToString());
//            }
//        }
//        if (parmas.Length >= 3)
//        {
//            if ((bool)parmas[2] == true)
//            {
//                flyObj.enabled = false;
//                flyObj.transform.GetComponent<Text>().text = parmas[0].ToString();
//            }
//        }
//        FlyStart(flyObj.transform.parent);
//    }
//    void FlyStart(Transform flyObj,float time=2.5f)
//    {
//        isFlytingTime++;
//        flyObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -200, 0);
//        flyObj.DOLocalMoveY(100, time, false).SetEase(Ease.Linear).OnComplete(() =>
//        {
//            Destroy(flyObj.gameObject);
//            isFlytingTime--;
//        });
//    }
//    List<float> timeList = new List<float>();
//    void ShowArt(Transform flyObj, float time = 2.5f)
//    {
//        isFlytingTime++;
//        flyObj.localScale = Vector3.zero;
//        currentFlyObj = flyObj.gameObject;
//        currentTween = flyObj.transform.DOScale(Vector3.one, timeList[0]).SetEase(Ease.OutBack).OnComplete(() =>
//        {
//            currentTween = flyObj.transform.DOScale(Vector3.one, timeList[1]).OnComplete(() =>
//            {
//                currentTween = flyObj.transform.DOScale(Vector3.zero, timeList[2]).SetEase(Ease.InBack).OnComplete(() =>
//                {
//                    currentFlyTip = FlyTip.Default;
//                    isFlyingArtText = false;
//                    currentFlyObj = null;
//                    currentTween = null;
//                    Destroy(flyObj.gameObject);
//                    isFlytingTime--;
//                });
//            });
//        });
//    }
//}
