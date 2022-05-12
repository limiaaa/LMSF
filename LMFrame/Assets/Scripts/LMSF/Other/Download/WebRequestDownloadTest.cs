using System;
using System.Collections;
using UnityEngine;

namespace SG.AssetBundleBrowser
{
    public class WebRequestDownloadTest : MonoBehaviour
    {
        private WebRequestDownload request;

        private IEnumerator Start()
        {
            request = new WebRequestDownload(
                "http://resources.collection.libii.cn/PrincessSalonGPNew/android/english/game/HalloweenBoutique/20061814/AssetBundle/halloweenboutique/contents/");

            yield return request.StartYield(
                Application.persistentDataPath,
                "p001main",
                0,
                (t, b, s, p) => { DebugUtils.Log("---->" + b + "   " + s + "KB/S" + "  " + (p * 100) + "%"); },
                t => { Debug.LogError(t.errorCode); },
                400);
   
            Debug.LogError("----Finished");
        }
    }
}