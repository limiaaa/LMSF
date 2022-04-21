using System;
using System.Collections.Generic;
using System.ComponentModel;
using SG.Utils;
using UnityEngine;

namespace SG.EventSystem.EventDispatcher
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    public class MsgHandler
    {
        private IMsgReceiver receiver;
        private Action<AEventArgs> callback;

        public MsgHandler(IMsgReceiver receiver, Action<AEventArgs> callback)
        {
            this.receiver = receiver;
            this.callback = callback;
        }

        public IMsgReceiver Receiver
        {
            get { return receiver; }
        }

        public Action<AEventArgs> Callback
        {
            get { return callback; }
        }
    }

    /// <summary>
    /// 全局消息管理器
    /// </summary>
    public static class GloabelMessageManager
    {
        /// <summary>
        /// 所有的全局消息列表
        /// </summary>
        private static Dictionary<string, Dictionary<string, List<MsgHandler>>> handlers =
            new Dictionary<string, Dictionary<string, List<MsgHandler>>>();

        /// <summary>
        /// 清除所有消息
        /// </summary>
        public static void ClearAll()
        {
            handlers.Clear();
        }

        /// <summary>
        /// 清除某种枚举的所有消息
        /// </summary>
        /// <param name="type"></param>
        public static void ClearByEnumType(Enum type)
        {
            Type enumType = type.GetType();
            string typeName = enumType.FullName;

            var msgName = type.ToString();
            if (handlers.ContainsKey(typeName) && handlers[typeName].ContainsKey(msgName))
            {
                handlers[typeName][msgName].Clear();
            }
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="msgName">Message name.</param>
        /// <param name="callback">Callback.</param>
        public static void RegistMsg(this IMsgReceiver self, Enum msgType, Action<AEventArgs> callback)
        {
            if (callback == null)
            {
                DebugUtils.Log("消息监听回调为空...");
                return;
            }

            Type enumType = msgType.GetType();
            string typeName = enumType.FullName;
            if (!handlers.ContainsKey(typeName))
            {
                handlers.Add(typeName, new Dictionary<string, List<MsgHandler>>());
            }

            var handlersOfThisMsg = handlers[typeName];
            var msgName = msgType.ToString();
            if (!handlersOfThisMsg.ContainsKey(msgName))
            {
                handlersOfThisMsg.Add(msgName, new List<MsgHandler>());
            }

            var msgs = handlersOfThisMsg[msgName];
            foreach (var handler in msgs)
            {
                if (handler.Receiver == self && handler.Callback == callback)
                {
                    DebugUtils.Log("该监听已经被注册...");
                    return;
                }
            }

            var eventHandler = new MsgHandler(self, callback);
            handlersOfThisMsg[msgName].Add(eventHandler);
        }

        /// <summary>
        /// 移除消息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msgName"></param>
        /// <param name="callback"></param>
        public static void RemoveMsg(this IMsgReceiver self, Enum msgType, Action<AEventArgs> callback)
        {
            if (callback == null)
            {
                DebugUtils.Log("消息监听回调为空...");
                return;
            }

            Type enumType = msgType.GetType();
            string typeName = enumType.FullName;
            if (!handlers.ContainsKey(typeName))
            {
                Debug.LogWarning("要移除的消息从未被注册...");
                return;
            }

            var handlersOfThisMsg = handlers[typeName];
            var msgName = msgType.ToString();
            if (!handlersOfThisMsg.ContainsKey(msgName))
            {
                DebugUtils.Log("要移除的消息从未被注册...");
                return;
            }

            var msgList = handlersOfThisMsg[msgName];
            var handlerCount = msgList.Count;
            for (int i = handlerCount-1; i >= 0; i--)
            {
                if (msgList[i].Callback == callback)
                {
                    msgList.Remove(msgList[i]);
                }
            }
        }
        
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msgName"></param>
        /// <param name="_params"></param>
        public static void SendMsg(this IMsgSender self, Enum msgType, AEventArgs _param)
        {
            Type enumType = msgType.GetType();
            string typeName = enumType.FullName;
            if (!handlers.ContainsKey(typeName))
            {
                DebugUtils.Log("发送的消息未注册:typeName:{0}" , typeName);
                return;
            }

            var handlersOfThisMsg = handlers[typeName];
            var msgName = msgType.ToString();
            if (!handlersOfThisMsg.ContainsKey(msgName))
            {
                DebugUtils.Log("发送的消息未注册:typeName:{0},msgName:{1}", typeName , msgName);
                return;
            }

            var msgList = handlersOfThisMsg[msgName];
            var handlerCount = msgList.Count;
            for (int i = 0; i < handlerCount; i++)
            {
                MsgHandler handler = msgList[i];
                handler.Callback(_param);
            }
        }
    }
}
