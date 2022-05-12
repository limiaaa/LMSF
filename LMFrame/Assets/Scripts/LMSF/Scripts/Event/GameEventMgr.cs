using System;
using System.Collections;
using System.Collections.Generic;
using SG.EventSystem.EventDispatcher;
using UnityEngine;

public interface IMsgReceiver
{
}
public interface IMsgSender
{
}
public abstract class AEventArgs
{
    public static T Parse<T>(AEventArgs instance) where T : AEventArgs
    {
        return instance as T;
    }

    public int EventType { get; }
}
public class GameEventMgr : Singleton<GameEventMgr>,IMsgSender , IMsgReceiver
{
    public class GameEventArgs: AEventArgs
    {
        public object data;
    }
    public void AddEvent(Enum msgType, Action<AEventArgs> callback)
    {
        this.RegistMsg(msgType , callback);
    }
    
    public void SendEvent(Enum msgType, GameEventArgs param)
    {
        this.SendMsg(msgType , param);
    }

    public void RemoveEvent(Enum msgType, Action<AEventArgs> callback)
    {
        this.RemoveMsg(msgType , callback);
    }

    /// <summary>
    /// 清除枚舉绑定的事件
    /// </summary>
    /// <param name="msgType"></param>
    public void ClearAllMsg(Enum msgType)
    {
        GloabelMessageManager.ClearByEnumType(msgType);
    }
}
