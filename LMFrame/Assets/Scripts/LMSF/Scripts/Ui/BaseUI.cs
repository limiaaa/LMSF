using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum UIPageLoadType
{
    /// <summary>
    /// 继承游戏全局资源加载设置
    /// </summary>
    OverrideGameSetting,
    /// <summary>
    /// 强制通过Resources.Load
    /// </summary>
    ForceResources
}
public enum PageType
{
    Map,
    Normal,
    Fixed,
    PopBox,
    Effect
}
public enum PageState
{
    NotReady,
    Created,
    Opening,
    PlayingOpenAnimation,
    Opened,
    PlayingCloseAnimation,
    Closed
}

[System.AttributeUsage(AttributeTargets.Class)]
public class UIResPathAttribute : Attribute
{
    public string mPath = string.Empty;
    public PageType mPageType = PageType.Normal;
    public UIPageLoadType mPageLoadType = UIPageLoadType.OverrideGameSetting;
    public bool IsSafeArea = false;
    public Type mPageAnimation;
    public UIResPathAttribute(string path)
    {
        this.mPath = path;
    }
    public UIResPathAttribute(string path, PageType pageType)
    {
        this.mPath = path;
        this.mPageType = pageType;
    }
    public UIResPathAttribute(string path, PageType pageType, UIPageLoadType loadType)
    {
        this.mPath = path;
        this.mPageType = pageType;
        this.mPageLoadType = loadType;
    }
    public UIResPathAttribute(string path, PageType pageType, bool isSafeArea)
    {
        this.mPath = path;
        this.mPageType = pageType;
        this.IsSafeArea = isSafeArea;
    }
    public UIResPathAttribute(string path, PageType pageType, UIPageLoadType loadType, bool isSafeArea)
    {
        this.mPath = path;
        this.mPageType = pageType;
        this.mPageLoadType = loadType;
        this.IsSafeArea = isSafeArea;
    }
    public UIResPathAttribute(string path, PageType pageType = PageType.Normal, UIPageLoadType loadType = UIPageLoadType.OverrideGameSetting, bool isSafeArea = false, Type mPageAnim = null)
    {
        this.mPath = path;
        this.mPageType = pageType;
        this.mPageLoadType = loadType;
        this.IsSafeArea = isSafeArea;
        this.mPageAnimation = mPageAnim;
    }
}
[System.AttributeUsage(AttributeTargets.Field)]
public class UIBinderAttribute : Attribute
{
    public string mPath = string.Empty;
    public UIBinderAttribute(string path)
    {
        mPath = path;
    }
}

public class BaseUI : MonoBehaviour
{
    public string OpenAniName = "";
    public string CloseAniName = "";

    public PageType PageLevel;
    public string PageResPath;
    public string PageName;
    public bool IsNeedSafeArea;
    public PageState PageState = PageState.NotReady;
    public Animation PageAnimation;

    public void Init(params object[] parmas)
    {
        this.PageState = PageState.Created;
        this.OnInit(parmas);
    }
    public void OpenPage(params object[] parmas)
    {
        this.PageState = PageState.Opened;
        this.OnOpenPage(parmas);
    }
    public void ClosePage(Action onClose, params object[] parmas)
    {
        this.PageState = PageState.Closed;
        this.OnClosePage(parmas);
    }
    public virtual void OnInit(object[] parmas) { }
    public virtual void OnOpenPage(object[] parmas) { }
    public virtual void OnClosePage(object[] parmas) { }
}

