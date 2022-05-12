using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

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

}
