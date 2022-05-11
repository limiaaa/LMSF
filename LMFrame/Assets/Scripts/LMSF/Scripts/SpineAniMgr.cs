using UnityEngine;
using System.Collections.Generic;
using System;
using SG.Utils;
using Spine.Unity;
using SG.AssetBundleBrowser.AssetBundlePacker;
/// <summary>
/// spine动画管理
/// </summary>
public class SpineAniMgr : MonoSingleton<SpineAniMgr>
{
    //private Dictionary<string, SkeletonAnimation> spineDic = new Dictionary<string, SkeletonAnimation>();

    public SkeletonAnimation LoadSpineAni(string path)
    {
        var obj = ResourcesManager.Load<GameObject>(path);
        if (obj)
        {
            var comp = GameObject.Instantiate(obj).transform.GetComponent<SkeletonAnimation>();
            comp.gameObject.name = path;
            return comp;
        }

        return null;
    }

    /// <summary>
    /// 加载动画
    /// </summary>
    /// <param name="spine"></param>
    ///// <param name="path"></param>
    // public void LoadAsset(SkeletonAnimation spine,string path)
    // {
    //    if(spine!= null)
    //    {
    //        SkeletonDataAsset spineAsset = ResourcesManager.Load<SkeletonDataAsset>(path);
    //        if(spineAsset != null)
    //        {
    //            spine.skeletonDataAsset = spineAsset;
    //        }
    //    }
    // }
   
    public float GetSpineAniTime(SkeletonAnimation spine,string aniname)
    {
        if (spine == null)
            return 0;
        var animationObject = spine.skeletonDataAsset.GetSkeletonData(false).FindAnimation(aniname);
        if (animationObject != null)
        {
            return animationObject.Duration;
        }
        return 0;
    }
    /// <summary>
    /// 判断是否存在动画片段
    /// </summary>
    /// <param name="spine"></param>
    /// <param name="aniname"></param>
    /// <returns></returns>
    public bool CheckIsHaveAniName(SkeletonAnimation spine, string aniname)
    {
        if (spine == null)
            return false;
        var animationObject = spine.skeletonDataAsset.GetSkeletonData(false).FindAnimation(aniname);
        if (animationObject != null)
        {
            return true;
        }
        return false;
    }

    public void PlayAni(SkeletonAnimation spine, string aniname, bool isloop = false, float speed = 1)
    {
        if (string.IsNullOrEmpty(aniname) || spine == null)
        {
            DebugUtils.Log("检查参数传递");
            return;
        }
        spine.gameObject.SetActive(true);
        var animationObject = spine.skeletonDataAsset.GetSkeletonData(false).FindAnimation(aniname);
        if (animationObject != null)
        {

            spine.skeleton.SetToSetupPose();
            spine.state.ClearTracks();
            spine.state.TimeScale = speed;
            spine.state.SetAnimation(0, aniname, isloop);
        }
    }
    /// <summary>
    /// offsettime 回调时间偏移
    /// </summary>
    /// <param name="spine"></param>
    /// <param name="aniname"></param>
    /// <param name="callback"></param>
    /// <param name="offsettime"></param>
    public void PlayAni(SkeletonAnimation spine, string aniname)
    {
        if (string.IsNullOrEmpty(aniname) || spine == null)
        {
            DebugUtils.Log("检查参数传递");
            return;
        }
        spine.gameObject.SetActive(true);
        var animationObject = spine.skeletonDataAsset.GetSkeletonData(false).FindAnimation(aniname);
        if (animationObject != null)
        {
            spine.skeleton.SetToSetupPose();
            spine.state.ClearTracks();
            spine.state.SetAnimation(0, aniname, false);
        }
    }

    public void PlayAni(SkeletonGraphic spine, string aniname,bool isLoop = false, float speed = 1)
    {
        if (string.IsNullOrEmpty(aniname) || spine == null || spine.skeletonDataAsset == null)
        {
            DebugUtils.Log("检查参数传递");
            return;
        }
        spine.gameObject.SetActive(true);
        var animationObject = spine.skeletonDataAsset.GetSkeletonData(false).FindAnimation(aniname);
        if (animationObject != null)
        {
            spine.AnimationState.TimeScale = speed;
            spine.Skeleton.SetToSetupPose();
            spine.AnimationState.ClearTracks();
            spine.AnimationState.SetAnimation(0, aniname, isLoop);
        }
    }
    public float GetSpineAniTime(SkeletonGraphic spine, string aniname)
    {
        if (spine == null || spine.skeletonDataAsset == null)
        {
            DebugUtils.Log("检查参数传递 GetSpineAniTime spine null");
            return 0;
        }
        var animationObject = spine.skeletonDataAsset.GetSkeletonData(false).FindAnimation(aniname);
        if (animationObject != null)
        {
            return animationObject.Duration;
        }
        return 0;
    }
}
