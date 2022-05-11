using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SG.AssetBundleBrowser.AssetBundlePacker;
using SG.Common;
using SG.EventSystem.EventDispatcher;
using SG.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace SG.UI
{
    /// <summary>
    /// UI页面加载模式
    /// </summary>
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

    /// <summary>
    /// UI管理器，
    /// </summary>
    public class UIManager : MonoSingleton<UIManager>
    {
        private string UIRootPath = "UI_Root.prefab";
        private string UIAtlasPath = "Assets/MainApp/Atlas/UI/";
        private Dictionary<string, ABaseUI> _mAllPages;
        public Transform mUIRoot;
        public ABaseUI mCurrentPage;//当前的页面

        private Camera mUICamera;
        private Dictionary<PageType, Transform> canvas_layer;
        private GameObject mClickMask;

        private Action<string> openEventCallBack = null;
        private Action<string> closeEventCallBack = null;
        private Action openPlaySoundCallBack = null;
        private Action closePlaySoundCallBack = null;

        private List<ABaseUI> openedPageList = null;

        public UILogicTimer mUILogicTimer = null;

        private Dictionary<string, int> UIAnimationPlayDic = new Dictionary<string, int>();
        public void InitEvent(Action<string> openevent, Action<string> closeevent, Action playopensound, Action playclosesound)
        {
            openEventCallBack = openevent;
            closeEventCallBack = closeevent;
        }

        private void InitTimer()
        {
            if (mUILogicTimer == null)
            {
                GameObject go = new GameObject("UILogicTimer");
                go.transform.SetParent(transform, false);
                mUILogicTimer = go.AddComponent<UILogicTimer>();
                mUILogicTimer.Init();
            }
        }

        public void AddOnePageInAnimDic(string pageName)
        {
            if (!UIAnimationPlayDic.ContainsKey(pageName))
            {
                UIAnimationPlayDic.Add(pageName, 0);
            }
            else
            {
                UIAnimationPlayDic[pageName]++;
            }
        }

        public void RemoveOnePageInAnimDic(string pageName)
        {

            if (!UIAnimationPlayDic.ContainsKey(pageName))
            {
                DebugUtils.Log(pageName + "不存在");
            }
            else
            {
                UIAnimationPlayDic.Remove(pageName);
            }
        }


        public bool CheckAnimDic()
        {
            return UIAnimationPlayDic.Count > 0;
        }

        public void Init(string uiroot_path, bool forceReset = false, UIPageLoadType rootLoadtype = UIPageLoadType.OverrideGameSetting)
        {
            this.UIRootPath = uiroot_path;
            InitTimer();
            _mAllPages = new Dictionary<string, ABaseUI>();
            if (forceReset && mUIRoot != null)
            {
                GameObject.Destroy(mUIRoot.gameObject);
                mUIRoot = null;
            }
            if (mUIRoot == null || forceReset)
            {
                GameObject pb = null;
                if (rootLoadtype == UIPageLoadType.OverrideGameSetting)
                {
                    pb = this._GameSettingLoadPrefab(uiroot_path);
                }
                else if (rootLoadtype == UIPageLoadType.ForceResources)
                {
                    pb = this._ResLoadPrefab(uiroot_path);
                }
                if (pb != null)
                {
                    mUIRoot = Instantiate<GameObject>(pb).transform;
                }
                else
                {
                    mUIRoot = CreateDefaultUIRoot().transform;
                }

                mUIRoot.name = "UI_Root";
                mUIRoot.transform.position = Vector3.zero;
            }

            mUICamera = mUIRoot.GetComponentInChildren<Camera>();
            canvas_layer = new Dictionary<PageType, Transform>();
            canvas_layer.Add(PageType.Map, mUIRoot.Find("Canvas0"));
            canvas_layer.Add(PageType.Normal, mUIRoot.Find("Canvas1"));
            canvas_layer.Add(PageType.Fixed, mUIRoot.Find("Canvas2"));
            canvas_layer.Add(PageType.PopBox, mUIRoot.Find("Canvas3"));
            canvas_layer.Add(PageType.Effect, mUIRoot.Find("Canvas4"));

            SetSubCanvas(canvas_layer[PageType.Map], 10);
            SetSubCanvas(canvas_layer[PageType.Normal], 20);
            SetSubCanvas(canvas_layer[PageType.Fixed], 30);
            SetSubCanvas(canvas_layer[PageType.PopBox], 40);
            SetSubCanvas(canvas_layer[PageType.Effect], 50);
            if (mClickMask == null)
            {
                mClickMask = mUIRoot.Find("ClickMask").gameObject;
                if (mClickMask == null)
                    DebugUtils.Log("注意UIRoot下丢失ClickMask，会影响全局点击遮罩功能！！！");
            }
            EnableMask(false);
            DontDestroyOnLoad(mUIRoot);

            SpriteAtlasManager.atlasRequested += OnLoadAtlas;

            openedPageList = new List<ABaseUI>();
        }

        public void EnableMask(bool enable)
        {
            if (mClickMask == null) return;
            mClickMask.gameObject.SetActive(enable);
        }

        private string atlasPrifix = ".spriteatlas";
        private void OnLoadAtlas(string atlasName, Action<SpriteAtlas> act)
        {
            string path = UIAtlasPath + atlasName + atlasPrifix;

            var sa = ResourcesManager.Load<SpriteAtlas>(path);
            if (sa == null)
            {
                Debug.LogError("加载图集失败:[" + path + "]");
            }
            act(sa);
        }


        /// <summary>
        /// 设置UGUI图集统一路径（用到了UGUI的图集/否则可以不管）
        /// </summary>
        /// <param name="_path"></param>
        public void SetUGUIAtlasPath(string _path)
        {
            this.UIAtlasPath = _path;
        }

        public GameObject CreateDefaultUIRoot()
        {
            GameObject mroot = new GameObject("UI_Root");
            mroot.AddComponent<RectTransform>();
            Canvas canvas = mroot.AddComponent<Canvas>();

            CanvasScaler scaler = mroot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(768, 1366);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1;
            scaler.referencePixelsPerUnit = 100;

            GraphicRaycaster raycaster = mroot.AddComponent<GraphicRaycaster>();
            raycaster.ignoreReversedGraphics = true;
            raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            GameObject evensystemobj = new GameObject("EventSystem");
            UnityEngine.EventSystems.EventSystem evensystem = evensystemobj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            evensystem.firstSelectedGameObject = null;
            evensystem.sendNavigationEvents = true;
            evensystem.pixelDragThreshold = 10;

            StandaloneInputModule inputModule = evensystemobj.AddComponent<StandaloneInputModule>();
            inputModule.horizontalAxis = "Horizontal";
            inputModule.verticalAxis = "Vertical";
            inputModule.submitButton = "Submit";
            inputModule.cancelButton = "Cancel";
            inputModule.cancelButton = "Cancel";
            inputModule.inputActionsPerSecond = 10;
            inputModule.repeatDelay = 0.5f;
            inputModule.forceModuleActive = false;
            evensystemobj.transform.SetParent(mroot.transform);

            GameObject cameraObj = new GameObject("UICamera");
            Camera cam = cameraObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Nothing;
            cam.cullingMask = LayerMask.NameToLayer("UI");
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.depth = 1;
            cameraObj.transform.SetParent(mroot.transform, false);
            cam.transform.position = Vector3.back * 100;

            for (int i = 0; i < 5; i++)
            {
                GameObject subcanvasObj = new GameObject("Canvas" + i);
                Canvas subcanvas = subcanvasObj.AddComponent<Canvas>();

                GraphicRaycaster subRaycaster = subcanvasObj.AddComponent<GraphicRaycaster>();
                subRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
                subRaycaster.ignoreReversedGraphics = true;
                subcanvasObj.transform.SetParent(mroot.transform);

                RectTransform rec = subcanvas.GetComponent<RectTransform>();
                rec.anchorMin = Vector2.zero;
                rec.anchorMax = Vector2.one;
                rec.pivot = Vector2.one * 0.5f;
                rec.offsetMax = Vector2.zero;
                rec.offsetMin = Vector2.zero;
            }

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 100;
            canvas.sortingLayerName = "Default";
            canvas.sortingOrder = 0;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            mClickMask = new GameObject("ClickMask");
            RectTransform mask_rect = mClickMask.AddComponent<RectTransform>();
            mask_rect.anchorMin = Vector2.zero;
            mask_rect.anchorMax = Vector2.one;
            mask_rect.pivot = Vector2.one * 0.5f;
            mask_rect.offsetMax = Vector2.zero;
            mask_rect.offsetMin = Vector2.zero;

            Canvas clicksubcanvas = mClickMask.AddComponent<Canvas>();
            clicksubcanvas.overrideSorting = true;
            clicksubcanvas.sortingOrder = 9999;
            GraphicRaycaster clicksubRaycaster = mClickMask.AddComponent<GraphicRaycaster>();
            clicksubRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            clicksubRaycaster.ignoreReversedGraphics = true;
            mask_rect.SetAsLastSibling();
            return mroot;
        }

        private void SetSubCanvas(Transform canvas, int order)
        {
            Canvas c = canvas.GetComponent<Canvas>();
            c.overrideSorting = true;
            c.sortingOrder = order;
        }

        public T SetPageLayer<T>(PageType type) where T : ABaseUI
        {
            T page = GetPage<T>();
            page.transform.SetParent(canvas_layer[type], false);
            return page;
        }

        public T OpenPage<T>(params object[] parmas) where T : ABaseUI
        {
            ABaseUI page = OpenPage(typeof(T), parmas);
            if (openEventCallBack != null)
            {
                openEventCallBack.Invoke(typeof(T).Name);
            }
            if (page != null && page.mIsPlayOpenSound)
            {
                openPlaySoundCallBack?.Invoke();
            }
            openedPageList.Add(page);
            return page as T;
        }
        private ABaseUI OpenPage(System.Type type, object[] parmas = null)
        {
            UIResPathAttribute attr = GetUIPageDefine(type);
            string _pageName = attr == null ? "" : attr.mPath;
            if (string.IsNullOrEmpty(_pageName)) return null;
            ABaseUI pageSrc = null;
            if (!_HasPage(_pageName))
            {
                pageSrc = _LoadSetPage(type, parmas);
            }
            pageSrc = _mAllPages[_pageName];

            if (pageSrc == null)
            {
                Debug.LogError("打开页面失败@：" + _pageName);
                return null;
            }
            if (pageSrc.mPageType == PageType.Normal)
            {
                if (mCurrentPage != null)
                {
                    if (pageSrc.mPageType == PageType.PopBox)
                    {
                        mCurrentPage.CloseSelf(false, parmas);
                    }
                }
            }
            if (pageSrc.mPageType != PageType.PopBox)
            {
                mCurrentPage = pageSrc;
            }
            pageSrc.gameObject.SetActive(true);
            pageSrc.OpenPage(parmas);
            pageSrc.GetComponent<RectTransform>().SetAsLastSibling();
            return pageSrc;
        }

        /// <summary>
        /// 获取已打开的最顶层的Page
        /// </summary>
        /// <returns></returns>
        public ABaseUI GetTopOpenedPage()
        {
            var count = openedPageList.Count;
            if (count > 0)
            {
                return openedPageList[count - 1];
            }
            return null;
        }

        public void ClosePage<T>(bool forceDestroy = false, params object[] parmas) where T : ABaseUI
        {
            string _pageName = GetUIPath(typeof(T));
            ClosePage(_pageName, forceDestroy, parmas);
        }


        /// <summary>
        /// 关闭所有界面
        /// </summary>
        public void CloseAllPage()
        {
            foreach (var item in _mAllPages)
            {
                ClosePage(item.Key, false);
            }
        }

        public void CloseAllPageWithOut(List<Type> lst, bool IsDestroy = false)
        {
            List<string> pathList = new List<string>();
            for (int i = 0; i < lst.Count; i++)
            {
                var pathName = GetUIPath(lst[i]);
                pathList.Add(pathName);
            }
            List<string> needClose = new List<string>();
            foreach (var item in _mAllPages)
            {
                if (!pathList.Contains(item.Key))
                {
                    needClose.Add(item.Key);
                }
            }
            for (int i = 0; i < needClose.Count; i++)
            {
                ClosePage(needClose[i], IsDestroy);
            }
        }

        /// <summary>
        /// 关闭除某个以外的所有界面
        /// </summary>
        public void CloseAllWithoutPage<T>()
        {
            string _pageName = GetUIPath(typeof(T));
            foreach (var item in _mAllPages)
            {
                if (!_pageName.EndsWith(item.Key))
                    ClosePage(item.Key, false);
            }
        }

        public List<ABaseUI> GetShowPages()
        {
            List<ABaseUI> list = new List<ABaseUI>();
            foreach (var item in _mAllPages)
            {
                if (item.Value.gameObject.activeSelf)
                {
                    list.Add(item.Value);
                }
            }
            return list;
        }

        public void ClosePage(Type type, bool forceDestroy = false, params object[] parmas)
        {
            string _pageName = GetUIPath(type);
            var pageTemp = GetPage(_pageName);
            if (closeEventCallBack != null)
            {
                closeEventCallBack.Invoke(type.Name);
            }
            if (pageTemp != null && pageTemp.mIsPlayCloseSound)
            {
                closePlaySoundCallBack?.Invoke();
            }
            ClosePage(_pageName, forceDestroy, parmas);
        }

        public Camera GetUICamera()
        {
            return mUICamera;
        }

        private void ClosePage(string __pageName, bool forceDestroy = false, object[] parmas = null)
        {
            var pageTemp = GetPage(__pageName);
            if (pageTemp != null && openedPageList.Contains(pageTemp))
            {
                openedPageList.Remove(pageTemp);
            }

            if (_HasPage(__pageName))
            {
                ABaseUI page = _mAllPages[__pageName];
                if (forceDestroy)
                {
                    _mAllPages.Remove(__pageName);
                    page.ClosePage(() =>
                    {
                        GameObject.Destroy(page.gameObject);
                        page = null;
                    });
                }
                else
                {
                    page.ClosePage(() =>
                    {
                        page.gameObject.SetActive(false);
                    }, parmas);
                }

            }
        }

        public void RefreshPage<T>(params object[] parmas) where T : ABaseUI
        {
            string pagePath = GetUIPath(typeof(T));
            RefreshPage(pagePath, parmas);
        }

        private void RefreshPage(string __pageName, object[] parmas = null)
        {
            if (_HasPage(__pageName))
            {
                _mAllPages[__pageName].RefreshPage(parmas);
                //_mAllPages[__pageName].SendMessage("RefreshPage", parmas, SendMessageOptions.DontRequireReceiver);
            }
        }

        public Transform GetCanvasLayer(PageType pageType)
        {
            if (!canvas_layer.ContainsKey(pageType))
                return null;
            return canvas_layer[pageType];
        }

        public T GetPage<T>() where T : ABaseUI
        {
            string _pageName = GetUIPath(typeof(T));
            ABaseUI p = GetPage(_pageName, null);
            return p as T;
        }
        public ABaseUI GetPage(string __pageName, object[] parmas = null)
        {
            ABaseUI page = null;
            if (_HasPage(__pageName))
            {
                page = _mAllPages[__pageName];
            }
            return page;
        }

        private bool _HasPage(string __pageName)
        {
            return _mAllPages.ContainsKey(__pageName);
        }

        private ABaseUI _LoadSetPage(System.Type type, params object[] param)
        {
            UIResPathAttribute attr = GetUIPageDefine(type);
            if (attr == null) return null;
            string path = attr.mPath;

            GameObject pg_prefab = null;
            GameObject pg = null;
            if (attr.mPageLoadType == UIPageLoadType.OverrideGameSetting)
            {
                pg_prefab = this._GameSettingLoadPrefab(path);
            }
            else if (attr.mPageLoadType == UIPageLoadType.ForceResources)
            {
                pg_prefab = this._ResLoadPrefab(path);
            }

            pg = GameObject.Instantiate(pg_prefab) as GameObject;

            ABaseUI pageSrc = pg.GetComponent<ABaseUI>();
            if (pageSrc == null)
            {
                pageSrc = pg.AddComponent(type) as ABaseUI;
            }
            if (pageSrc != null)
            {
                pageSrc.SetPageTypeByAttribute();
                pageSrc.Init(param);
                pageSrc.mPageState = PageState.Created;
            }
            Vector3 orgPos = pg_prefab.transform.position;
            Quaternion orgLocRot = pg_prefab.transform.localRotation;
            Vector3 orgLocScale = pg_prefab.transform.localScale;
            SetPageByType(pageSrc);
            pg.transform.position = orgPos;
            pg.transform.localRotation = orgLocRot;
            pg.transform.localScale = orgLocScale;

            RectTransform _old = pg_prefab.GetComponent<RectTransform>();
            RectTransform _new = pg.GetComponent<RectTransform>();
            _new.anchoredPosition = _old.anchoredPosition;
            _new.anchorMax = _old.anchorMax;
            _new.anchorMin = _old.anchorMin;
            _new.sizeDelta = _old.sizeDelta;
            _mAllPages.Add(path, pageSrc);
            pageSrc.SetSafeArea();
            pageSrc.SetAnimationDefine();
            return pageSrc;
        }


        private void SetPageByType(ABaseUI pageSrc)
        {
            PageType pagetype = PageType.Normal;
            if (pageSrc == null)
            {
                return;
            }
            pagetype = pageSrc.mPageType;

            pageSrc.transform.SetParent(canvas_layer[pagetype], false);
        }

        private GameObject _GameSettingLoadPrefab(string __resName)
        {
            GameObject pg_prefab = ResourcesManager.Load<GameObject>(__resName);
            if (pg_prefab == null)
            {
                Debug.LogError("资源加载失败:[" + __resName + "] 请检查资源是否存在或名称是否正确!!!");
                return null;
            }
            return pg_prefab;
        }

        private GameObject _ResLoadPrefab(string __resName)
        {
            GameObject pg_prefab = Resources.Load<GameObject>(__resName);
            if (pg_prefab == null)
            {
                Debug.LogError("资源加载失败:Resources.Load(" + __resName + ") 请检查资源是否存在或名称是否正确!!!");
                return null;
            }
            return pg_prefab;
        }

        private string GetUIPath(Type uitype, out UIResPathAttribute attr)
        {
            string ui_path = "";
            Type t = uitype;

            attr = t.GetCustomAttribute(typeof(UIResPathAttribute)) as UIResPathAttribute;
            if (attr != null)
                ui_path = attr.mPath;
            return ui_path;
        }

        private string GetUIPath(Type uitype)
        {
            string ui_path = "";
            Type t = uitype;

            UIResPathAttribute attr = GetUIPageDefine(t);
            if (attr != null)
                ui_path = attr.mPath;
            return ui_path;
        }

        private UIResPathAttribute GetUIPageDefine(Type uitype)
        {
            UIResPathAttribute attr = uitype.GetCustomAttribute(typeof(UIResPathAttribute)) as UIResPathAttribute;
            return attr;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            SpriteAtlasManager.atlasRequested -= OnLoadAtlas;
        }
    }
}