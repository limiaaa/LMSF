using System;
using System.Collections;
using System.Collections.Generic;
using SG.EventSystem.EventDispatcher;
using SG.UI;
using SG.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace TestEvent
{
    public enum TestEventType
    {
        value1,
        value2
    }
}

namespace SG_EventSystem.TestCase
{
    public enum TestEventType{
        value1 = 1,
        value2 = 2
    }

    public enum TestEventType2
    {
        t2_value1,
        t2_vvalue2
    }

    public class TestEventArgs1 : AEventArgs
    {
        public string context;
        public TestEventArgs1(string _context)
        {
            this.context = _context;
        }
    }
    
    public class TestEventArgs2 : AEventArgs
    {
        public string mItemName;
        public int nItemId;
        public TestEventArgs2(string _itemName, int _itemId)
        {
            this.mItemName = _itemName;
            this.nItemId = _itemId;
        }
    }

    public class Test_Event : MonoBehaviour , IMsgReceiver , IMsgSender
    {
        void Start()
        { 
            //注册消息示例
            this.RegistMsg(TestEventType.value1 , OnEvent1);
            this.RegistMsg(TestEventType2.t2_value1, OnEvent2);
            
            this.RegistMsg(TestEvent.TestEventType.value1 , OnEvent_1);
        }
    
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //发送消息示例
                this.SendMsg(TestEventType.value1 , new TestEventArgs1("触发事件1了..."));
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                //发送消息示例
                this.SendMsg(TestEvent.TestEventType.value1 , new TestEventArgs1("触发事件1了...TestEvent.TestEventType"));
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                //发送消息示例
                this.SendMsg(TestEventType2.t2_value1 , new TestEventArgs2("道具名称", 1001));
                
                this.SendMsg(NetMessageMgr.NetMsgType.Login , new NetMessageMgr.LoginArgs());
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                //清除消息
                this.RemoveMsg(TestEventType.value1 , OnEvent1);
                this.RemoveMsg(TestEventType2.t2_value1 , OnEvent2);
            }
        }

        void OnEvent1(AEventArgs args)
        {
            TestEventArgs1 args1 = args as TestEventArgs1;
            DebugUtils.Log(args1.context);
        }  
        
        void OnEvent_1(AEventArgs args)
        {
            TestEventArgs1 args1 = args as TestEventArgs1;
            DebugUtils.Log(args1.context);
        }
        void OnEvent2(AEventArgs args)
        {
            TestEventArgs2 args1 = AEventArgs.Parse<TestEventArgs2>(args);
            DebugUtils.Log(string.Format("触发事件2：{0},{1}", args1.mItemName, args1.nItemId));
        }

        private void OnDestroy()
        {
            //清理消息
            this.RemoveMsg(TestEventType.value1 , OnEvent1);
            //直接清除所有绑定消息
            GloabelMessageManager.ClearAll();
        }
    }
}


