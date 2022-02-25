using LMSF.Utils;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineManager : MonoSingleton<SpineManager>
{
    /// <summary>
    /// 播放spine动画
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="aniName"></param>
    /// <param name="Loop"></param>
    public void PlaySpineAni(Transform trans,string aniName,bool Loop)
    {
        SkeletonGraphic skele = trans.GetComponent<SkeletonGraphic>();
        if (skele == null)
        {
            Debug.Log("错误，未能获取spine组件");
            return;
        }
        skele.AnimationState.SetAnimation(0, aniName, Loop);
    }
    /// <summary>
    /// 重播当前spine动画
    /// </summary>
    /// <param name="trans"></param>
    public void PlayCurrentSpineAni(Transform trans)
    {
        SkeletonGraphic skele = trans.GetComponent<SkeletonGraphic>();
        if (skele == null)
        {
            Debug.Log("错误，未能获取spine组件");
            return;
        }
        skele.Initialize(true);
        skele.gameObject.SetActive(false);
        skele.gameObject.SetActive(true);
    }



}
