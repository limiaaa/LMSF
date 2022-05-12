using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;

namespace SG.UI
{
    public class UIUnityAnimation : AUIAnimation
    {
        protected Animator _animator;
        
        public UIUnityAnimation(Transform panel) : base(panel)
        {
            _animator = panel.GetComponent<Animator>();
        }
        
        public override void PlayStartAnimation(string name , Action OnComplated=null)
        {
            if (_animator != null)
            {
                _animator.Play(name);
                var mController = new AnimatorOverrideController();
                mController.runtimeAnimatorController = _animator.runtimeAnimatorController;
                try
                {
                    AnimationClip mClipDic = mController[name];
                    float animTime = mClipDic.length;
                    _animator.Play(name, 0, 0);
                    DelyTimerManager.Instance.Delay(animTime, OnComplated);
                }
                catch (Exception e)
                {
                    DebugUtils.LogError("页面[{0}]打开动画缺失：>动画名称：【{1}】", mUIPanel.name, name);
                    OnComplated?.Invoke();
                }
            }
        }
    
        public override void PlayEndAnimation(string name , Action OnComplated=null)
        {
            if (_animator != null)
            {
                _animator.Play(name);
                var mController = new AnimatorOverrideController();
                mController.runtimeAnimatorController = _animator.runtimeAnimatorController;
                try
                {
                    AnimationClip mClipDic = mController[name];
                    float animTime = mClipDic.length;
                    _animator.Play(name, 0, 0);
                    DelyTimerManager.Instance.Delay(animTime, OnComplated);
                }
                catch (Exception e)
                {
                    DebugUtils.LogError("页面[{0}]关闭动画缺失：>动画名称：【{1}】", mUIPanel.name, name);
                    OnComplated?.Invoke();
                }
            }
        }
    }
}

