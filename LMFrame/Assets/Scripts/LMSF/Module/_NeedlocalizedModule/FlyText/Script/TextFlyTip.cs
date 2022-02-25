using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LMSF.Utils;

public class TextFlyTip : MonoSingleton<TextFlyTip>
{
    public void FlyText()
    {
        Fly(1,2,3);
    }

    Tween FlyTween;
    private void Fly(params object[] parmas)
    {
        //if (parmas.Length>1)
        //{
        //    Tiptext.UpdateKey(parmas[0].ToString(),parmas[1].ToString());
        //}
        //else
        //{
        //    Tiptext.UpdateKey(parmas[0].ToString());
        //}
        //if (FlyTween!=null)
        //{
        //    FlyTween.Kill(false);
        //}
        ////this.transform.position = new Vector3(0, -200, 0);
        //this.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -200, 0);
        //FlyTween = this.transform.DOLocalMoveY(100, 2.5f, false).SetEase(Ease.Linear).OnComplete(() =>
        //{
        //    UIManager.Instance.ClosePage<TextFlyTip>(true);
        //    FlyTween = null;
        //});
    }
}
