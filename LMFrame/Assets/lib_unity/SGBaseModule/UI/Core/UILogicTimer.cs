using UnityEngine;
using System;

/// <summary>
/// UI类延迟处理器逻辑
/// </summary>
public class UILogicTimer : MonoBehaviour
{
    public Timer timer;
    
    public void Init()
    {
        if (timer == null)
            timer = new Timer();
    }
    //*******************************************延时调用****************************************************************
    public int Delay(float time, Action callback)
    {
        return timer.Delay(time, callback);
    }
    public int DelayLoop(float time, Action callback, int loop_num)
    {
        return timer.DelayLoop(time, callback, loop_num);
    }

    /// <summary>
    /// 停止延迟调用
    /// </summary>
    /// <param name="id"></param>
    public void Stop(int id , bool onComplate = false)
    {
        timer.Stop(id , onComplate);
    }

    public void StopAll(bool onComplate = false)
    {
        timer.StopAll(onComplate);
    }

    public void Remove(int id)
    {
        timer.Remove(id);
    }
    
    public void RemoveAll()
    {
        timer.RemoveAll();
    }

    private void FixedUpdate()
    {
        if (timer != null)
            timer.FixedUpdate();
    }
}
