using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;
using LMSF.Utils;

public class PreloadManager : MonoSingleton<PreloadManager>
{
    private string SpriteAtlasPath = "Assets/MainApp/Art/UIAtlas/UI/{0}.spriteatlas";
    public void Init()
    {
        Debug.Log("开始预加载___");
        PreloadUiAtlas();
        //StartCoroutine(getCountryCode());
    }
    void PreloadUiAtlas()
    {
        //ResourceManager.Load<SpriteAtlas>(string.Format(SpriteAtlasPath, "Lobby"),true);
        //ResourceManager.Load<SpriteAtlas>(string.Format(SpriteAtlasPath, "Game"), true);
    }

}
