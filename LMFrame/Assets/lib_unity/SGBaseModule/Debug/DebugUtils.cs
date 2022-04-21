using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG.Utils
{
    public static class DebugUtils
    {

        public enum DebugLogLevel
        {
            Off = 0,
            Error = 1,
            Warnning = 2,
            Info = 3,
            Debug = 4
        }
        /// <summary>
        /// 0,1,2,3
        /// 0 全关  1~error 2~warn  3~info 4~debug
        /// </summary>
        public static DebugLogLevel LogLevel = DebugLogLevel.Info;

        /// <summary>
        /// 由外部传入的事件回调, 当发生错误时会进行事件写入
        /// </summary>
        private static Action<int, string, string> ErrorEventCallback;

        private static string logTemplate = "<color=#{1}>{0}</color>";


        /// <summary>
        /// 注册错误事件回调记录, 如果不需要, 可传入null
        /// </summary>
        /// <param name="errEventCallback">回调接口, 如果不需要写入事件, 传null</param>
        /// <param name="UseUnityLogHandler">是否启用全局错误信息上报 默认开启</param>
        public static void Init(Action<int, string, string> errEventCallback, bool UseUnityLogHandler = true)
        {
            ErrorEventCallback = errEventCallback;
            //监听Unity层错误信息 并上报
            if (UseUnityLogHandler)
            {
                Application.logMessageReceived += UnityLogHandler;
            }
        }

        public static void Log(string msg, Color color, params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Info)
            {
                string temp = String.Format(msg, objects);
                Debug.Log(string.Format(logTemplate, temp, ColorUtility.ToHtmlStringRGB(color)));
            }
        }

        public static void Log(string fmt, params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Info)
            {
                Debug.LogFormat(fmt, objects);
            }
        }



        public static void LogWarning(string fmt, params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Warnning)
            {
                Debug.LogWarningFormat(fmt, objects);
            }
        }

        public static void LogError(string fmt, params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Error)
            {
                Debug.LogErrorFormat(fmt, objects);
            }
        }

        /// <summary>
        /// 带事件的报错接口, 需要预先调用 Init 注册event接口
        /// </summary>
        /// <param name="bugIndex">
        /// 负数预留给公共库(包含SDK库)
        /// 游戏逻辑从正数开始使用
        /// -1000~-1999: OpenSdk 公司通用SDK
        /// -2000~-2999: OpenSdk Iap 公司通用SDK内购部分
        /// </param>
        /// <param name="fmt"></param>
        /// <param name="objects"></param>
        public static void LogErrorWithEvent(int bugIndex, string tag, string fmt, params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Error)
            {
                string msg = string.Format(fmt, objects);
                Debug.LogError(msg);
                if (ErrorEventCallback != null)
                {
                    ErrorEventCallback(bugIndex, tag, msg);
                }
            }
        }

        /// <summary>
        /// 获取函数名
        /// </summary>
        /// <returns></returns>
        public static string FuncName()
        {
            var st = new System.Diagnostics.StackTrace();
            var frame = st.GetFrame(1);
            if (frame != null)
            {
                return frame.ToString();
            }
            return "null";
        }
        /// <summary>
        ///  //监听Unity层错误信息 并上报 （Unity固定委托LogCallback 参数无法修改）
        /// </summary>
        /// <param name="condition"> 错误信息</param>
        /// <param name="stackTrace">调用堆栈</param>
        /// <param name="type">错误类型</param>
        public static void UnityLogHandler(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                LogErrorWithEvent(-9999, "UnityErr", "ErrConditon:\n {0}\n Stack:\n{1}", condition, stackTrace);
            }
        }

    }
}

