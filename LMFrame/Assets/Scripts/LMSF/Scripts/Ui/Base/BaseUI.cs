using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    Opened,
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
    protected SafeArea safeArea;

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
        onClose?.Invoke();
        this.OnClosePage(parmas);
    }
    public virtual void OnInit(object[] parmas) { }
    public virtual void OnOpenPage(object[] parmas) { }
    public virtual void OnClosePage(object[] parmas) { }

    public virtual void OnDestroy() { }

    public virtual void RefreshPage(params object[] parmas) { }
    public virtual void CloseSelf(bool forceDestroy =false,params object[] o)
    {
        string name = this.GetType().Name;
        UIManager.Instance.ClosePage(this.GetType(), forceDestroy, o);
    }
    public void SetPageTypeByAttribute()
    {
        Type t = this.GetType();
        UIResPathAttribute attr = t.GetCustomAttribute(typeof(UIResPathAttribute)) as UIResPathAttribute;
        if (attr != null)
        {
            PageLevel = attr.mPageType;
        }
    }
    public void SetSafeArea()
    {
        Type t = this.GetType();
        UIResPathAttribute attr = t.GetCustomAttribute(typeof(UIResPathAttribute)) as UIResPathAttribute;
        if (attr != null)
        {
            IsNeedSafeArea = attr.IsSafeArea;
        }
        if (IsNeedSafeArea && safeArea == null)
        {
            safeArea = gameObject.GetComponent<SafeArea>();
            if (safeArea == null) safeArea = gameObject.AddComponent<SafeArea>();
        }
        safeArea.ForceUpdateSafeArea();
    }

    //https://blog.csdn.net/qq_15505341/article/details/81712244
    void BindUIElement()
    {
        //反射用，获取类型中所有成员
        Dictionary<string, FieldInfo> listInfo = new Dictionary<string, FieldInfo>();
        GetAllFieldInfo(GetType(), listInfo);
        Type typeAttr = typeof(UIBinderAttribute);
        foreach(var item in listInfo)
        {
            UIBinderAttribute attr = Attribute.GetCustomAttribute(item.Value, typeAttr) as UIBinderAttribute;
            if (attr != null)
            {
                if (item.Value.FieldType.FullName == typeof(GameObject).FullName)
                {
                    var v = GetChildGameObject(attr, item.Value.FieldType);
                    item.Value.SetValue(this,v);
                }
                else
                {
                    var v = GetChildElement(attr, item.Value.FieldType);
                    item.Value.SetValue(this, v);
                }
            }
        }
        listInfo.Clear();
        listInfo = null;
    }


    Component GetChildElement(UIBinderAttribute attr,Type type)
    {
        Transform tran = transform.Find(attr.mPath);
        if (tran != null)
        {
            return tran.GetComponent(type);
        }
        return null;
    }

    GameObject GetChildGameObject(UIBinderAttribute attr,Type type)
    {
        Transform tran = transform.Find(attr.mPath);
        if (tran != null)
        {
            return tran.gameObject;
        }
        return null;
    }

    void GetAllFieldInfo(Type type,Dictionary<string, FieldInfo> listInfo)
    {
        if (listInfo == null)
        {
            listInfo = new Dictionary<string, FieldInfo>();
        }
        //获取类型所有指定成员 .Instance表对象 
        var listFiledInfo = type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
        foreach(var item in listFiledInfo)
        {
            listInfo[item.Name] = item;
        }
        if (type.BaseType != null)
        {
            GetAllFieldInfo(type.BaseType,listInfo);
        }
    }
}

