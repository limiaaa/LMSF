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
        public static void UICloseWithAniName(Animator ani, string name,float callBackTime, Action act = null)
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
            DelayTimeManager.Instance.delay_time_run(callBackTime, () =>
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
        public static void TextFlyNotKey(string key, string FormatNumber = "")
        {
            //if (FormatNumber == "")
            //{
            //    UIManager.Instance.OpenPage<TextFlyTip>(key, "", true);
            //    return;
            //}
            //UIManager.Instance.OpenPage<TextFlyTip>(key, FormatNumber, true);
        }
        //public static void TextFlyByArtText(FlyTip flyTip)
        //{
        //    UIManager.Instance.OpenPage<TextFlyTip>(true, flyTip);
        //}

        static float clickTime = 0;
        static float clickInterval = 0.2f;
        public static void AddButtonFunc(this Button clickButton,bool OpenIntervalCheck,Action clickAction)
        {
            if (clickButton == null)
            {
                DebugUtils.Log("想要添加方法的Button为空");
                return;
            }
            clickButton.onClick.AddListener(() =>
            {
                if (Time.realtimeSinceStartup - clickTime < clickInterval && OpenIntervalCheck)
                {
                    Debug.Log("点击间隔太短");
                    return;
                }
                clickTime = Time.realtimeSinceStartup;
                clickAction?.Invoke();
            });
        }
        public static void RemoveButtonFunc(this Button clickButton)
        {
            if (clickButton == null)
            {
                DebugUtils.Log("想要移除方法的Button为空");
                return;
            }
            clickButton.onClick.RemoveAllListeners();
        }
        public static void SetGameObjectLayer(Transform target, int layer)
        {
            foreach (var item in target.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = layer;
            }
        }


    }
}