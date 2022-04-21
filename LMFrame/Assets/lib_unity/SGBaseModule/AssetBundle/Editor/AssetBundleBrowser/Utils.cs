using System.Collections.Generic;
using System.IO;
using SG.AssetBundleBrowser.AssetBundlePacker;
using UnityEditor;
using UnityEngine;

namespace SG.AssetBundleBrowser
{
    public class Utils
    {
        public static void Clear()
        {
            ABUtils.Clear();
            if (Directory.Exists(Application.persistentDataPath))
                Directory.Delete(Application.persistentDataPath, true);
            if (Directory.Exists(Application.temporaryCachePath))
            {
                Directory.Delete(Application.temporaryCachePath, true);
            }

            if (Directory.Exists(AssetBundleBrowser.Platform.PERSISTENT_DATA_PATH))
            {
                Directory.Delete(AssetBundleBrowser.Platform.PERSISTENT_DATA_PATH, true);
            }

            AssetDatabase.Refresh();
        }

       

        public static string GetGameFolder(ResourcesManifest manifestData)
        {
            string gameFolder = "mainapp";
            foreach (var ab in manifestData.RawData.AssetBundles.Values)
            {
                //过滤主AssetBundle文件
                if (ab.AssetBundleName == AssetBundleBrowser.AssetBundlePacker.Common.MAIN_MANIFEST_FILE_NAME)
                    continue;
                var result = ab.AssetBundleName.Split('/');
                if (result.Length > 0 && !ab.AssetBundleName.Contains(gameFolder))
                {
                    gameFolder = result[0];
                    break;
                }
            }

            return gameFolder;
        }

        public static string GetSubAppCode(ResourcesManifest manifestData)
        {
            var result = GetGameFolder(manifestData);
//            return result.Replace("mainapp", "main");
            return result;
        }
    }
}