using System.Collections;
using System.Collections.Generic;
using SG.EventSystem.EventDispatcher;
using SG.Utils;
using UnityEngine;

public class NetMessageMgr : Singleton<NetMessageMgr> , IMsgReceiver
{
    public enum NetMsgType
    {
        Login,
        Offline
    }
    
    public class LoginArgs:AEventArgs
    {
        
    }
    public override void Init()
    {
        base.Init();
        this.RegistMsg(NetMsgType.Login , OnMsgLogin);
        
    }

    public override void Dispose()
    {
        
    }

    private void OnMsgLogin(AEventArgs args)
    {
        
    }
}
