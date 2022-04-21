using System;
using System.Collections;
using UnityEngine;
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
                (t, b, s, p) => { Debug.Log("---->" + b + "   " + s + "KB/S" + "  " + (p * 100) + "%"); },
                t => { Debug.LogError(t.errorCode); },
                G.SPEED_LIMITATION);
   
            Debug.LogError("----Finished");
        }
    }
