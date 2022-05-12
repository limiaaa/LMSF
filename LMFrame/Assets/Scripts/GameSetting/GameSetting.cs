using UnityEngine;

[CreateAssetMenu(fileName = "Tools", menuName = "BuildGameSetting")]
public class GameSetting : ScriptableObject
{
    public bool OpenLog;

    [Header("SeverInfo")]
    public string SeverKey;


    [Header("DownloadInfo")]
    public bool IsDeveloperment = true;

    [Header("本地AB")]
    public string[] BundleFolders;

    public static GameSetting Instance
    {
        get
        {
            GameSetting val = Resources.Load<GameSetting>("GameSetting");
            if (val != null)
            {
                return val;
            }
            else
            {
#if UNITY_EDITOR
                CreateScriptObject();
                return Resources.Load<GameSetting>("GameSetting");
#else
                return null;
#endif
            }
        }
    }

#if UNITY_EDITOR
    static void CreateScriptObject()
    {
        GameSetting config = ScriptableObject.CreateInstance<GameSetting>();
        UnityEditor.AssetDatabase.CreateAsset(config, "Assets/Resources/GameSetting.asset");
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    [UnityEditor.MenuItem("Tools/选中GameSetting")]
    private static void OpenDevSettings()
    {
        var settings = Instance;
        UnityEditor.EditorGUIUtility.PingObject(settings);
        UnityEditor.Selection.activeObject = settings;
    }
#endif
    
}



