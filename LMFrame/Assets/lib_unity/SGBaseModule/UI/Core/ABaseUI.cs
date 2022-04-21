using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SG.Utils;
using UnityEngine;

namespace SG.UI
{
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
        
        public UIResPathAttribute(string path , PageType pageType)
        {
            this.mPath = path;
            this.mPageType = pageType;
        }
        
        public UIResPathAttribute(string path , PageType pageType , UIPageLoadType loadType)
        {
            this.mPath = path;
            this.mPageType = pageType;
            this.mPageLoadType = loadType;
        }
        
        public UIResPathAttribute(string path , PageType pageType , bool isSafeArea)
        {
            this.mPath = path;
            this.mPageType = pageType;
            this.IsSafeArea = isSafeArea;
        }
        
        public UIResPathAttribute(string path , PageType pageType , UIPageLoadType loadType , bool isSafeArea)
        {
            this.mPath = path;
            this.mPageType = pageType;
            this.mPageLoadType = loadType;
            this.IsSafeArea = isSafeArea;
        }
        
        public UIResPathAttribute(string path , PageType pageType = PageType.Normal , UIPageLoadType loadType = UIPageLoadType.OverrideGameSetting , bool isSafeArea = false , Type mPageAnim = null)
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
    /// <summary>
    ///页面基类
    /// </summary>
    public class ABaseUI : MonoBehaviour
    {
        /// <summary>
        /// 强制名称定义
        /// </summary>
        public string OpenAnimationName = "ani_page_open";
        public string CloseAnimationName = "ani_page_close";
        /// <summary>
        /// 页面类型
        /// </summary>
        public PageType mPageType;
        /// <summary>
        /// 资源地址
        /// </summary>
        public string mPageResPath;
        /// <summary>
        /// 页面名称
        /// </summary>
        public string mPageName;
        
        /// <summary>
        /// 是否需要全面屏控制
        /// </summary>
        public bool mIsSafeArea = false;
        
        protected SafeArea mSafeArea;

        

        public PageState mPageState = PageState.NotReady;
        /// <summary>
        /// 页面动画配置
        /// </summary>
        protected AUIAnimation mPageAnimation;
        
        #region Unity CallBack
        protected virtual void Update()
        {
        }
        #endregion

        public void Init(params object[] parmas)
        {
            this.mPageState = PageState.Created;
            BindUIElement();
            this.OnInit();
        }

        public void OpenPage(params object[] parmas)
        {
            this.OnOpenPage(parmas);
            if (this.mPageAnimation == null)
            {
                this.mPageState = PageState.Opened;
            }
            else
            {
                this.mPageState = PageState.Opening;
                UIManager.Instance.EnableMask(true);
                this.mPageAnimation.PlayStartAnimation(OpenAnimationName , ()=>
                {
                    OnEndPlayOpenAnimation();
                    this.mPageState = PageState.Opened;
                    UIManager.Instance.EnableMask(false);
                });
            }
        }

        public virtual void ClosePage(Action onClose,params object[] parmas)
        {
            if (this.mPageAnimation == null)
            {
                this.OnClosePage(parmas);
                this.mPageState = PageState.Closed;
                onClose?.Invoke();
            }
            else
            {
                UIManager.Instance.EnableMask(true);
                this.mPageAnimation.PlayEndAnimation(CloseAnimationName , ()=>
                {
                    OnEndPlayCloseAnimation();
                    this.mPageState = PageState.Closed;
                    UIManager.Instance.EnableMask(false);
                    this.OnClosePage(parmas);
                    onClose?.Invoke();
                });
            }
        }
        /// <summary>
        /// 界面初始化，在页面资源加载完成后调用一次
        /// </summary>
        protected virtual void OnInit(params object[] parmas){ }
        /// <summary>
        /// 每次打开页面调用
        /// </summary>
        protected virtual void OnOpenPage(params object[] parmas){  }
        /// <summary>
        /// 每次关闭页面回调
        /// </summary>
        protected virtual void OnClosePage(params object[] parmas ) { }
        protected virtual void OnStartPlayOpenAnimation() { }
        protected virtual void OnEndPlayOpenAnimation() { }
        protected virtual void OnStartPlayCloseAnimation() { }
        protected virtual void OnEndPlayCloseAnimation() { }
        /// <summary>
        /// 每次需要刷新的时候调用
        /// </summary>
        /// 
        public virtual void RefreshPage(params object[] parmas ) { }
        /// <summary>
        /// 页面被删除的时候调用
        /// </summary>
        public virtual void OnDestroy() { }

        public virtual void CloseSelf(bool forceDestroy = false, params object[] p )
        {
            string pname = this.GetType().Name;
            UIManager.Instance.ClosePage(this.GetType(), forceDestroy, p);
        }
        
        public void SetPageTypeByAttribute()
        {
            Type t = this.GetType();
            UIResPathAttribute attr =  t.GetCustomAttribute(typeof(UIResPathAttribute)) as UIResPathAttribute;
            if (attr != null)
                mPageType = attr.mPageType;
        }

        public void SetSafeArea()
        {
            Type t = this.GetType();
            UIResPathAttribute attr =  t.GetCustomAttribute(typeof(UIResPathAttribute)) as UIResPathAttribute;
            if (attr != null)
                mIsSafeArea = attr.IsSafeArea;

            if (mIsSafeArea && mSafeArea == null)
            {
                mSafeArea = gameObject.GetComponent<SafeArea>();
                if(mSafeArea==null)  mSafeArea = gameObject.AddComponent<SafeArea>();
            }

            if (mIsSafeArea && mSafeArea != null) mSafeArea.ForceUpdateSafeArea();
        }
        
        public void SetAnimationDefine()
        {
            Type t = this.GetType();
            UIResPathAttribute attr =  t.GetCustomAttribute(typeof(UIResPathAttribute)) as UIResPathAttribute;
            if (attr != null)
            {
                if (attr.mPageAnimation != null)
                {
                    if (attr.mPageAnimation.BaseType.Name != typeof(AUIAnimation).Name)
                    {
                        DebugUtils.LogErrorWithEvent(4, "ResourceManager", "页面动画类型【{0}】,必须继承【AUIAnimation】，当前配置类型错误!!!请检查！！", attr.mPageAnimation.Name);
                    }
                    else
                    {
                        this.mPageAnimation = Activator.CreateInstance(attr.mPageAnimation, (object)this.transform) as AUIAnimation;
                    }
                }
            }
        }
        
        

        private void BindUIElement()
        {
            Dictionary<string,FieldInfo> listInfo = new Dictionary<string, FieldInfo>();
            GetAllFieldInfo(this.GetType(),listInfo);
            Type typeAttr = typeof(UIBinderAttribute);
            foreach (var item in listInfo)
            {
                UIBinderAttribute attr = Attribute.GetCustomAttribute(item.Value,typeAttr) as UIBinderAttribute;
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
                        item.Value.SetValue(this,v);  
                    }
                }
            }
            listInfo.Clear();
            listInfo = null;
        }
        
        private Component GetChildElement(UIBinderAttribute attr, Type type)
        {
            Transform tran = this.transform.Find(attr.mPath);
            
            if (tran != null)
            {
                return tran.GetComponent(type);
            }

            return null;
        }
        
        private GameObject GetChildGameObject(UIBinderAttribute attr, Type type)
        {
            Transform tran = this.transform.Find(attr.mPath);
            if (tran != null)
            {
                return tran.gameObject;
            }

            return null;
        }
        
        private void GetAllFieldInfo(Type type, Dictionary<string, FieldInfo> listInfo)
        {
            if (listInfo == null)
            {
                listInfo = new Dictionary<string, FieldInfo>();
            }

            var listFiledInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var item in listFiledInfo)
            {
                listInfo[item.Name] = item;
            }

            if (type.BaseType != null)
            {
                GetAllFieldInfo(type.BaseType,listInfo);
            }
        }
    }
}