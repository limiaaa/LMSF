﻿using DG.Tweening;
using System;

/// <summary>
/// 逻辑时间管理器：
/// 专门处理逻辑类的延迟调用
/// </summary>
public class LogicDelayTimerHelperMgr : MonoSingleton<LogicDelayTimerHelperMgr>
{
    DelayTimerHelper DelayTimerHelper = null;
    protected override void Init()
    {
        base.Init();
        if (DelayTimerHelper == null)
            DelayTimerHelper = new DelayTimerHelper();
    }
    //*******************************************延时调用****************************************************************
    public int Delay(float time, Action callback)
    {
        return DelayTimerHelper.Delay(time, callback);
    }
    public int DelayLoop(float time, Action callback, int loop_num)
    {
        return DelayTimerHelper.DelayLoop(time, callback, loop_num);
    }

    /// <summary>
    /// 停止延迟调用
    /// </summary>
    /// <param name="id"></param>
    public void Stop(int id , bool onComplate = false)
    {
        DelayTimerHelper.Stop(id , onComplate);
    }

    public void StopAll(bool onComplate = false)
    {
        DelayTimerHelper.StopAll(onComplate);
    }

    public void Remove(int id)
    {
        DelayTimerHelper.Remove(id);
    }
    
    public void RemoveAll()
    {
        DelayTimerHelper.RemoveAll();
    }
    public void Delay_WithoutTimeScale(float time, Action callback)
    {
        int A = 0;
        Tween tween = DOTween.To(() => A, x => A = x, 1, time).SetUpdate(true).OnComplete(() => {
            callback?.Invoke();
        });
    }
    private void FixedUpdate()
    {
        if (DelayTimerHelper != null)
            DelayTimerHelper.FixedUpdate();
    }
}
