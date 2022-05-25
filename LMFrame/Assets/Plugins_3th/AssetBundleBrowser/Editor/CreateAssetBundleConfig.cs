using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssetBundleBrowser
{
    public class CreateAssetBundleConfig
    {
        private static string _backupMD5File;

        public static void CreateConfig(string targetPath, Dictionary<string, string> backupBundleMD5,string backupMD5FilePath)
        {
            _backupMD5File = backupMD5FilePath;
            string[] buildList = Directory.GetDirectories(targetPath);
            //生成所有资源包内的配置文件
            List<XConfigList.ConfigVo> configList = XConfigList.GetInstance().CreateConfigList(buildList);
            BackupMD5File(backupBundleMD5, configList);
            XConfigList.GetInstance().SaveConfigList(configList);
            AssetDatabase.Refresh();

            //根据上面生成的配置文件生成files.txt
            XConfigList.ConfigVo mainVo = XConfigList.GetInstance().CreateMainConfigVo(targetPath, configList);
            configList.Clear();
            configList.Add(mainVo);
            XConfigList.GetInstance().SaveConfigList(configList);
            AssetDatabase.Refresh();
        }

        private static void BackupMD5File(Dictionary<string, string> backupBundleMD5,
            List<XConfigList.ConfigVo> configList)
        {
            foreach (var configVo in configList)
            {
                foreach (var config in configVo.contentList)
                {
                    var str = config.Split('|');
                    var abName = str[0];
                    var newMD5 = str[1];
                    if (backupBundleMD5.ContainsKey(abName) && backupBundleMD5[abName].Equals(newMD5)) continue;
                    backupBundleMD5[abName] = newMD5;
                }
            }

            if (File.Exists(_backupMD5File)) File.Delete(_backupMD5File);
            File.WriteAllText(_backupMD5File, LitJson.JsonMapper.ToJson(backupBundleMD5));
        }
    }
}