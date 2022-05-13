using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailExample : MonoBehaviour
{
    public Button button;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            string content =
            "Dear players, for your benefit, please do not delete the following information:<br>" +
            "PackageName:{0}<br>" +
            "AppVersion:{1}<br>" +
            "DeviceId:{2}<br>" +
            "Device:{3}<br>" +
            "Platform:{4}<br>" +
            "OS Version:{5}<br>" +
            "Country:{6}<br>" +
            "Please write your feedback below, your support is our biggest motivation:<br>";
            string body = string.Format(content,
                Application.identifier,
                Application.version,
                SystemInfo.deviceUniqueIdentifier,
                SystemInfo.deviceName,
                Application.platform,
                SystemInfo.operatingSystem,
                Application.systemLanguage);
            string Tital = "{0}/{1}/{2}/feedback";
            string TitalFormat = string.Format(Tital, Application.identifier, Application.platform, Application.version);
            Debug.Log("Open");
            Uri uri = new Uri(string.Format("mailto:{0}?subject={1}&body={2}", "sngame@xjoygame.com", TitalFormat, body));
            //第二个参数是邮件的标题 Application.OpenURL(uri.AbsoluteUri);
            Application.OpenURL(uri.AbsoluteUri);
            Debug.Log("Open");
        });
    }
}
