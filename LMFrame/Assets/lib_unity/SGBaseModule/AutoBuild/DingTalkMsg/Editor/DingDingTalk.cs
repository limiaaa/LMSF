using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SG.AutoBuild.Editor
{
    public class DingDingTalk
    {
        public enum MsgName
        {
            buildstart,
            buildend,
            builderror
        }
        public static string token = "https://oapi.dingtalk.com/robot/send?access_token=555e4130b4f27737fed0c0d70f2fc3a0b00233edd6be2755b94759cd2db17b99";
        
        public static void StartBuildDingTalk(string token,string content)
        {
            var data = new TextMsg();
            data.text.content = content;
            new DingMgsNet().PostRequest(token, data.ToJson(), null);
        }

        public static void ErrorBuildDingTalk(string _token,string content)
        {
            var data = new TextMsg();
            data.text.content = content;
            new DingMgsNet().PostRequest(_token, data.ToJson(), null);
        }
        
        public static void EndBuildDingTalk(string apkpathname ,string text,string url, string _token )
        {
            ActionCardMsg data = new ActionCardMsg();
            data.actionCard.btnOrientation = "0";
            data.actionCard.text = text;
            data.actionCard.title = "打包成功";
            ActionButton btn = new ActionButton();
            btn.title = "点击下载 :" + apkpathname;
            btn.actionURL = url+apkpathname;//"http://172.16.106.33/games/puzzlex/"+apkname;
            data.actionCard.btns.Add(btn);
            Debug.Log(data.ToJson());
            new DingMgsNet().PostRequest(_token, data.ToJson(), null);
        }

        private static string getjson(string name)
        {
            string path = "Assets/lib_unity/SGBaseModule/AutoBuild/DingTalkMsg/Editor/Msg/{0}.txt";
            var info = AssetDatabase.LoadAssetAtPath<TextAsset>(string.Format(path, name));
            return info.text;
        }
    }
}
