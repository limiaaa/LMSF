using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG.UI
{
    [Serializable]
    public abstract class AUIAnimation
    {
        protected Transform mUIPanel;

        public AUIAnimation(Transform panel)
        {
            mUIPanel = panel;
        }

        public abstract void PlayStartAnimation(string name , Action OnComplated=null);
        public abstract void PlayEndAnimation(string name , Action OnComplated=null);
    }
}

