using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public static class DebugUtils{
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
        private static Action<int ,string> ErrorEventCallback; 

        private static string logTemplate = "<color=#{1}>{0}</color>";


        /// <summary>
        /// 注册错误事件回调记录, 如果不需要, 可传入null
        /// </summary>
        /// <param name="errEventCallback">回调接口, 如果不需要写入事件, 传null</param>
        public static void RegisterErrorEventCallback(Action<int, string> errEventCallback)
        {
            ErrorEventCallback = errEventCallback;
        }

        public static void Log(string msg , Color color ,params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Info)
            {
                string temp = String.Format(msg , objects);
                Debug.Log(string.Format(logTemplate, temp, ColorUtility.ToHtmlStringRGB(color)));
            }
        }
        
        public static void Log(string fmt , params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Info)
            {
                Debug.LogFormat(fmt , objects);
            }
        }
        
        
    
        public static void LogWarning(string fmt , params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Warnning)
            {
                Debug.LogWarningFormat(fmt, objects);
            }
        }
    
        public static void LogError(string fmt , params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Error)
            {
                Debug.LogErrorFormat(fmt, objects);
            }
        }

        /// <summary>
        /// 带事件的报错接口, 需要预先调用 RegisterErrorEventCallback 注册event接口
        /// </summary>
        /// <param name="bugIndex"></param>
        /// <param name="fmt"></param>
        /// <param name="objects"></param>
        public static void LogErrorWithEvent(int bugIndex, string fmt, params object[] objects)
        {
            if (LogLevel >= DebugLogLevel.Error)
            {
                string msg = string.Format(fmt, objects);
                Debug.LogError(msg);
                if (ErrorEventCallback != null)
                {
                    ErrorEventCallback(bugIndex, msg);
                }
            }
        }
    }

