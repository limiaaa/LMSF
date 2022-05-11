using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening.Core;
using DG.Tweening;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;
using LitJson;

public class Utils :MonoBehaviour
{
    public static Utils Instance;

    private HTTPManager _ht;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
        _ht=transform.GetComponent<HTTPManager>();
    }

    public void ClearDic()
    {
        _ht.ClearDic();
        //AddGlobaContentDic();
    }
    public void AddContentDic(string key, string value)
    {
        _ht.AddContentDic(key, value);
    }
    public void AddGlobaContentDic()
    {
        AddContentDic("machineCode","machineCode");
        AddContentDic("loginType", "loginType");
        AddContentDic("accessToken", "accessToken");
    }
    public void SendDic(string api,Action<JsonData> success, Action<string> failed)
    {
        _ht.SendDic(api, success,failed);
    }

    public void SendDicWithMask(string api,Action<JsonData> success, Action<string> failed)
    {
        _ht.SendDic(api, (js) =>
        {
            success(js);
        }, (error) =>
        {
            failed(error);
        });
    }
  

    public void CtrlObjScaleBig(Transform trans,Transform Mask=null,Action action=null)
    {
        trans.localScale = Vector3.zero;
        if (Mask != null)
        {
            var MaskImg = Mask.GetComponent<Image>();
            MaskImg.color = new Vector4(MaskImg.color.r, MaskImg.color.g, MaskImg.color.b, 0);
            MaskImg.DOFade(0.8f, 0.3f);
        }
        
        trans.DOScale(new Vector3(1.2f,1.2f,1.2f), 0.2f).OnComplete( () =>
        {
            action?.Invoke();
            trans.DOScale(new Vector3(1,1,1), 0.2f).OnComplete(() =>
            {
            });
        });
    }
    public void CtrlObjScaleSmall(Transform trans,Transform Mask=null,Action action=null)
    {
        if (Mask != null)
        {
            var MaskImg = Mask.GetComponent<Image>();
            MaskImg.DOFade(0, 0.3f);
        }
        trans.DOScale(new Vector3(1.2f,1.2f,1.2f), 0.2f).OnComplete( () =>
        {
    
            trans.DOScale(new Vector3(0,0,0), 0.2f).OnComplete(() =>
            {
                action?.Invoke();
                Destroy(trans.gameObject);
            });
        });
    }

    public void ClickScale(Transform trans, Action action=null)
    {
        if (trans.GetComponent<Button>()!=null)
        {
            trans.GetComponent<Button>().enabled = false;
        }
        trans.DOScale(new Vector3(0.8f,0.8f,1f), 0.1f).OnComplete( () =>
        {
            trans.DOScale(new Vector3(1,1,1), 0.1f).OnComplete(() =>
            {
                action?.Invoke();
                if (trans.GetComponent<Button>()!=null)
                {
                    trans.GetComponent<Button>().enabled = true;
                }
            });
        });
    }
    public string GetEquipmentCode()
    {
        string equipCode = PlayerPrefs.GetString("EquipmentCode", "");
        if (equipCode == "")
        {
            if (equipCode == "")
            {
                equipCode = SystemInfo.deviceUniqueIdentifier;
            }
            PlayerPrefs.SetString("EquipmentCode", equipCode);
        }
        return equipCode;
    }
}
