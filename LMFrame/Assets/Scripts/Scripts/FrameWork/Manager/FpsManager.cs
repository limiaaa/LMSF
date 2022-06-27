using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsManager : MonoSingleton<FpsManager>
{
    protected override void Init()
    {
        
    }
    private void Update()
    {
        float fps = 1.0f / UnityEngine.Time.smoothDeltaTime;
        //m_fps = string.Format("FPS: {0}", 1.0f / UnityEngine.Time.smoothDeltaTime);
        //GUI.Label(m_uiPosition, "FPS: " + fps.ToString());
        GUI.TextArea(new Rect(0, 600,100,200), fps.ToString());
    }
    public void GetFps()
    {
        Debug.Log(1.0f / Time.smoothDeltaTime);
    }

}
