using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SG.UI
{
    public class UITweenAnimation : AUIAnimation
    {
        
        public UITweenAnimation(Transform panel) : base(panel)
        {
        
        }

        public override void PlayStartAnimation(string name , Action OnComplated=null)
        {
            if (mUIPanel == null)
            {
                OnComplated?.Invoke();
                return;
            }
            mUIPanel.localScale = Vector3.zero;
            mUIPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.InSine);
            DelyTimerManager.Instance.Delay(0.5f, OnComplated);
        }

        public override void PlayEndAnimation(string name , Action OnComplated=null)
        {
            if (mUIPanel == null)
            {
                OnComplated?.Invoke();
                return;
            }
            mUIPanel.localScale = Vector3.one;
            mUIPanel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutSine);
            DelyTimerManager.Instance.Delay(0.5f, OnComplated);
        }
    }
}

