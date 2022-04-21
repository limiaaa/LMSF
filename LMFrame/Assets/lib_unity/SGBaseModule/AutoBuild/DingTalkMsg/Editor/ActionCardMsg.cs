using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SG.AutoBuild
{
    public enum MsgType{
        text,
        link,
        markdown,
        actionCard,
        feedCard
    }

    [Serializable]
    public abstract class ANotifyMsg{
        public string msgtype = "";
        public At at = new At();

        public string ToJson(){
            string json = JsonConvert.SerializeObject(this);
            return json;
        }
    }

    [Serializable]
    public class At{
        public List<string> atMobiles = new List<string>();
        public List<string> atUserIds = new List<string>();
        public bool isAtAll = false;
    }

    [Serializable]
    public class TextData
    {
        public string content = "";
    }

    [Serializable]
    public class LinkData
    {
        public string text = "";
        public string title = "";
        public string picUrl = "";
        public string messageUrl = "";
    }

    //----
    [Serializable]
    public class MarkdownData{
        public string title = "";
        public string text = "";
    }

    [Serializable]
    public class ActionCardData
    {
        public string title = "";
        public string text = "";
        public string btnOrientation = "0";
        public List<ActionButton> btns = new List<ActionButton>();
    }

    [Serializable]
    public class ActionButton{
        public string title = "";
        public string actionURL = "";
    }
    //--------

    [Serializable]
    public class FeedCardData{
        public string title = "";
        public string picURL = "";
        public string messageURL = "";
    }
    //---------




    [Serializable]
    public class TextMsg : ANotifyMsg
    {
        public TextData text = new TextData();

        public TextMsg(){
            this.msgtype = MsgType.text.ToString();
        }
    }

    [Serializable]
    public class LinkMsg : ANotifyMsg
    {
        public LinkData link = new LinkData();

        public LinkMsg()
        {
            this.msgtype = MsgType.link.ToString();
        }
    }

    [Serializable]
    public class MarkdownMsg : ANotifyMsg
    {
        public MarkdownData markdown = new MarkdownData();

        public MarkdownMsg()
        {
            this.msgtype = MsgType.markdown.ToString();
        }
    }

    [Serializable]
    public class ActionCardMsg : ANotifyMsg
    {
        public ActionCardData actionCard = new ActionCardData();

        public ActionCardMsg()
        {
            this.msgtype = MsgType.actionCard.ToString();
        }
    }

    [Serializable]
    public class FeedCardMsg : ANotifyMsg
    {
        public FeedCardData feedCard = new FeedCardData();

        public FeedCardMsg()
        {
            this.msgtype = MsgType.feedCard.ToString();
        }
    }
}
