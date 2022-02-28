using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace LMSF.Utils
{
    public static class UIUtils
    {
        //*********************************************************其他UI方法
        //点击物体有个动态弹动
        public static void ClickWithScaleChange(Transform trans, bool IsRecoverBtn, Action action = null)
        {
            if (trans.GetComponent<Button>() != null)
            {
                trans.GetComponent<Button>().enabled = false;
            }
            trans.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.1f).OnComplete(() =>
            {
                trans.DOScale(new Vector3(1, 1, 1), 0.1f).OnComplete(() =>
                {
                    action?.Invoke();
                    if (trans.GetComponent<Button>() != null || IsRecoverBtn)
                    {
                        trans.GetComponent<Button>().enabled = true;
                    }
                });
            });
        }

        //图片置灰
        public static void GrayToImage(Image image, bool IsGray, bool CtrlEnabled = false)
        {
            if (image == null)
                return;

            if (IsGray)
            {
                image.material = Resources.Load<Material>("Gray");
                if (CtrlEnabled)
                {
                    Button btn = image.transform.GetComponent<Button>();
                    if (btn)
                    {
                        btn.enabled = false;
                    }
                }
            }
            else
            {
                image.material = null;
                if (CtrlEnabled)
                {
                    Button btn = image.transform.GetComponent<Button>();
                    if (btn)
                    {
                        btn.enabled = true;
                    }
                }
            }

        }

        //动态打开一个窗口
        public static void UIShowWithScale(Transform obj)
        {
            if (obj == null)
            {
                Debug.Log("需要打开的object为空");
                return;
            }
            UIMaskManager.Instance.OpenSingleMask(0.8f);
            var Mask = obj.Find("Mask");
            if (Mask)
            {
                Image MaskImg = Mask.GetComponent<Image>();
                DoTweenUtils.DoFade(MaskImg, 0.8f, 0.5f);
            }
            var Main = obj.Find("Main");
            if (Main)
            {
                Main.localScale = Vector3.zero;
                Main.DOScale(1, 0.8f).SetEase(Ease.OutBounce);
            }
        }
        //动态关闭一个窗口
        public static void UICloseWithScale(Transform obj, Action closeAct = null)
        {
            if (obj == null)
            {
                Debug.Log("需要关闭的object为空");
                return;
            }
            UIMaskManager.Instance.OpenSingleMask(0.5f);
            var Mask = obj.Find("Mask");
            if (Mask)
            {
                Image MaskImg = Mask.GetComponent<Image>();
                DoTweenUtils.DoFade(MaskImg, 0f, 0.4f, 0.8f, "");
            }
            var Main = obj.Find("Main");
            if (Main)
            {
                Main.DOScale(0, 0.4f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    closeAct?.Invoke();
                });
            }
        }


        public static void UICloseWithAniName(Animator ani, string name, Action act = null)
        {
            UIMaskManager.Instance.OpenSingleMask(0.25f);
            if (ani == null)
            {
                //SoundMgr.Instance.PlaySoundEffect(SoundTypes.tap_button.ToString());
                act?.Invoke();
                return;
            }

            ani.Play(name);
            //SoundMgr.Instance.PlaySoundEffect(SoundTypes.tap_button.ToString());
            DelayTimeUtils.delay_time_run(0.25f, () =>
            {
                act?.Invoke();
            });
        }
        public static void TextFly(string key, string FormatNumber = "")
        {
            if (FormatNumber == "")
            {
                //UIManager.Instance.OpenPage<TextFlyTip>(key);
                return;
            }
            //UIManager.Instance.OpenPage<TextFlyTip>(key, FormatNumber);
        }

    }
}