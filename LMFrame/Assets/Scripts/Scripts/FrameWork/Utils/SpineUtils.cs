using LMSF.Utils;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LMSF.Utils
{
    public static class SpineUtils
    {
        /// <summary>
        /// 播放spine动画
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="aniName"></param>
        /// <param name="Loop"></param>
        public static void PlaySpineAni(Transform trans, string aniName, bool Loop)
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
        public static void PlayCurrentSpineAni(Transform trans)
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
        public static float GetSpineAniDuartion(SkeletonGraphic spine, string AniName)
        {
            return spine.skeletonDataAsset.GetSkeletonData(false).FindAnimation(AniName).Duration;
        }
#region SkeletonAnimation
/*
        public static SkeletonAnimation LoadSpineAni(string path)
        {
            var obj = ResourceManager.Load<GameObject>(path);
            if (obj)
            {
                var comp = GameObject.Instantiate(obj).transform.GetComponent<SkeletonAnimation>();
                comp.gameObject.name = path;
                return comp;
            }

            return null;
        }
        public static float GetSpineAniTime(SkeletonAnimation spine, string aniname)
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
        public static bool CheckIsHaveAniName(SkeletonAnimation spine, string aniname)
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
        public static void PlayAni(SkeletonAnimation spine, string aniname, bool isloop = false, float speed = 1)
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
        public static void PlayAni(SkeletonAnimation spine, string aniname)
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
        public static void PlayAni(SkeletonGraphic spine, string aniname, bool isLoop = false, float speed = 1)
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
        public static float GetSpineAniTime(SkeletonGraphic spine, string aniname)
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
*/
#endregion
    }
}