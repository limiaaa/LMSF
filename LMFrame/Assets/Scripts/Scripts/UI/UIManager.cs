using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF;
using LMSF.Utils;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

public class UIManager : MonoSingleton<UIManager>
{ 
    string UIRootPath = "UI_Root.prefab";
    string UIAtlasPath = "Assets/MainApp/Atlas/UI/";
    Dictionary<string, BaseUI> AllPages;
    Transform UIRoot;
    Camera UICamera;
    Dictionary<PageType, Transform> LayerDic;
    public void Init(string uiRoot_Path,bool forceReset=false,UIPageLoadType rootLoadType = UIPageLoadType.OverrideGameSetting)
    {
        UIRootPath = uiRoot_Path;
        AllPages = new Dictionary<string, BaseUI>();
        LayerDic = new Dictionary<PageType, Transform>();
        UICamera = UIRoot.GetComponentInChildren<Camera>();

        if (forceReset && UIRoot != null)
        {
            Destroy(UIRoot.gameObject);
            UIRoot = null;
        }
        if (UIRoot == null || forceReset)
        {
            GameObject root = null;
            if (rootLoadType == UIPageLoadType.OverrideGameSetting)
            {
                root = null;
            }
            else if (rootLoadType == UIPageLoadType.ForceResources)
            {
                root = null;
            }
            if (root != null)
            {
                UIRoot = Instantiate<GameObject>(root).transform;
            }
            else
            {
                UIRoot = CreateDefaultUIRoot().transform;
            }
            UIRoot.name = "UI_Root";
            UIRoot.E_Reset();
        }
        LayerDic.Add(PageType.Map, UIRoot.Find("Canvas0"));
        LayerDic.Add(PageType.Normal, UIRoot.Find("Canvas1"));
        LayerDic.Add(PageType.Fixed, UIRoot.Find("Canvas2"));
        LayerDic.Add(PageType.PopBox, UIRoot.Find("Canvas3"));
        LayerDic.Add(PageType.Effect, UIRoot.Find("Canvas4"));

        SetSubCanvas(LayerDic[PageType.Map],10);
        SetSubCanvas(LayerDic[PageType.Normal],20);
        SetSubCanvas(LayerDic[PageType.Fixed],30);
        SetSubCanvas(LayerDic[PageType.PopBox],40);
        SetSubCanvas(LayerDic[PageType.Effect],50);
        DontDestroyOnLoad(gameObject);
    }
    private GameObject CreateDefaultUIRoot()
    {
        GameObject mroot = new GameObject("UI_Root");
        mroot.AddComponent<RectTransform>();
        Canvas canvas = mroot.AddComponent<Canvas>();

        CanvasScaler scaler = mroot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(720, 1280);
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

        return mroot;
    }
    //设置层级
    private void SetSubCanvas(Transform canvas, int order)
    {
        Canvas c = canvas.GetComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = order;
    }
    //设置父物体
    public T SetPageLayer<T>(PageType type) where T : BaseUI
    {
        T page = GetPage<T>();
        page.transform.SetParent(LayerDic[type], false);
        return page;
    }
    public T OpenPage<T>(params object[] parmas) where T : BaseUI
    {
        BaseUI page = OpenPage(typeof(T), parmas);
        return page as T;
    }
    BaseUI OpenPage(System.Type type, object[] parmas = null)
    {
        UIResPathAttribute attr = GetUIPageDefine(type);
        string _pageName = attr == null ? "" : attr.mPath;
        if (string.IsNullOrEmpty(_pageName)) return null;
        BaseUI pageSrc = null;
        if (!_HasPage(_pageName))
        {
            pageSrc = LoadPage(type, parmas);
        }
        pageSrc = AllPages[_pageName];
        if (pageSrc == null)
        {
            Debug.LogError("打开页面失败@：" + _pageName);
            return null;
        }
        pageSrc.gameObject.SetActive(true);
        pageSrc.OpenPage(parmas);
        pageSrc.GetComponent<RectTransform>().SetAsLastSibling();

        return pageSrc;
    }
    BaseUI LoadPage(System.Type type, params object[] param)
    {
        UIResPathAttribute attr = GetUIPageDefine(type);
        if (attr == null) return null;
        string path = attr.mPath;
        GameObject pg_prefab = null;
        GameObject pg = null;
        if (attr.mPageLoadType == UIPageLoadType.OverrideGameSetting)
        {
            pg_prefab = LoadPrefab(path);
        }
        else if (attr.mPageLoadType == UIPageLoadType.ForceResources)
        {
            pg_prefab = LoadResPrefab(path);
        }
        pg = Instantiate(pg_prefab);
        BaseUI pageSrc = pg.GetComponent<BaseUI>();
        if (pageSrc == null)
        {
            pageSrc = pg.AddComponent(type) as BaseUI;
        }
        if (pageSrc != null)
        {
            pageSrc.SetPageTypeByAttribute();
            pageSrc.Init(param);
            pageSrc.PageState = PageState.Created;
        }
        pageSrc.transform.SetParent(LayerDic[pageSrc.PageLevel], false);
        pg.transform.E_Reset();
        AllPages.Add(path, pageSrc);
        pageSrc.SetSafeArea();
        return pageSrc;
    }
    public void ClosePage<T>(bool forceDestroy = false, params object[] parmas) where T : BaseUI
    {
        string _pageName = GetUIPath(typeof(T));
        ClosePage(_pageName, forceDestroy, parmas);
    }
    void ClosePage(string pageName, bool forceDestroy = false, object[] parmas = null)
    {
        if (_HasPage(pageName))
        {
            AllPages[pageName].ClosePage(() =>
            {
                if (forceDestroy)
                {
                    BaseUI page = AllPages[pageName];
                    AllPages.Remove(pageName);
                    GameObject.Destroy(page.gameObject);
                    page = null;
                }
                else
                {
                    AllPages[pageName].gameObject.SetActive(false);
                }
            }, parmas);

        }
    }
    public void RefreshPage<T>(params object[] parmas) where T : BaseUI
    {
        string pagePath = GetUIPath(typeof(T));
        RefreshPage(pagePath, parmas);
    }
    void RefreshPage(string pageName, object[] parmas = null)
    {
        if (_HasPage(pageName))
        {
            AllPages[pageName].RefreshPage(parmas);
        }
    }
    public T GetPage<T>() where T : BaseUI
    {
        string _pageName = GetUIPath(typeof(T));
        BaseUI p = GetPage(_pageName, null);
        return p as T;
    }
    public BaseUI GetPage(string __pageName, object[] parmas = null)
    {
        BaseUI page = null;
        if (_HasPage(__pageName))
        {
            page = AllPages[__pageName];
        }
        return page;
    }
    public void CloseAllPage(bool forceDestroy)
    {
        foreach (var item in AllPages)
        {
            ClosePage(item.Key, forceDestroy);
        }
    }
    //获取相机
    public Camera GetUICamera()
    {
        return UICamera;
    }
    private bool _HasPage(string __pageName)
    {
        return AllPages.ContainsKey(__pageName);
    }
    private GameObject LoadPrefab(string resName)
    {
        //GameObject prefab = ResourceManager.Load<GameObject>(resName);
        GameObject prefab = null;
        if (prefab == null)
        {
            Debug.LogError("资源加载失败:[" + resName + "] 请检查资源是否存在或名称是否正确!!!");
            return null;
        }
        return prefab;
    }
    GameObject LoadResPrefab(string resName)
    {
        GameObject resPrefeb = Resources.Load<GameObject>(resName);
        if (resPrefeb == null)
        {
            Debug.LogError("Resource加载失败__" + resName);
            return null;
        }
        return resPrefeb;
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
    //主要给基类用,基类不知道具体类型
    public void ClosePage(Type type, bool forceDestroy = false, params object[] parmas)
    {
        string pageName = GetUIPath(type);
        ClosePage(pageName, forceDestroy, parmas);
    }
}
