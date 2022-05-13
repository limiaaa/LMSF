using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorHelper : MonoBehaviour {

	[MenuItem("Tools/创建字体")]
	static public void BatchCreateArtistFont()
	{
		ArtistFont.BatchCreateArtistFont();
	}
    [MenuItem("Tools/选中GameSetting")]
    private static void OpenDevSettings()
    {
        var settings = GameSetting.Instance;
        UnityEditor.EditorGUIUtility.PingObject(settings);
        UnityEditor.Selection.activeObject = settings;
    }
    [MenuItem("Tools/生成UI_Root")]
    public static void Build_AB_Android()
    {
        GameObject newGameObject = CreateDefaultUIRoot();

        //新建预制体的处理,例如添加脚本,数据初始化,可以才是用switch来分别对不同的预制体添加不同的脚本
        string path = "Assets/UI_Root.prefab";
        PrefabUtility.SaveAsPrefabAsset(newGameObject, path);
        GameObject.DestroyImmediate(newGameObject);
    }

    private static GameObject CreateDefaultUIRoot()
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

        GameObject mClickMask = new GameObject("ClickMask");
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
        mClickMask.transform.SetParent(mroot.transform, false);
        mask_rect.SetAsLastSibling();
        return mroot;
    }

}
