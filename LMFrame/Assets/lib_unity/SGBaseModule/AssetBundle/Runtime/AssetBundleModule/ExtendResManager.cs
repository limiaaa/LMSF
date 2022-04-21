using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using SG.Utils;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    public class ExtendResManager : MonoSingleton<ExtendResManager>
    {
        public Dictionary<string, ExtendResInfo> ExtendResInfoDic = new Dictionary<string, ExtendResInfo>();

        public IEnumerator DownloadOne(string id, Action<WebRequestDownload, long, float, float> notify = null)
        {
            Debug.Assert(ExtendResInfoDic.ContainsKey(id), "ExtendResInfoDic.ContainsKey false:" + id);

            var info = ExtendResInfoDic[id];
            var webrequest = new WebRequestDownload(info.url);
            var fileName = info.localPath.Split('/').Last();
            yield return webrequest.StartYield(info.localPath.Replace(fileName, ""), fileName,
                0, notify);

            if (webrequest.errorCode != WebRequestDownload.ErrorCodeEnum.None)
            {
            }
        }

        /// <summary>
        /// TODO:merge info from web into local embed info
        /// </summary>
        public void MergeInfo(List<ExtendResInfo> localInfos, List<ExtendResInfo> webInfos)
        {
            foreach (var webInfo in webInfos)
            {
                foreach (var localInfo in localInfos)
                {
                    if (localInfo.localPath == webInfo.localPath)
                    {
                        
                    }
                }
            }
        }
    }
}