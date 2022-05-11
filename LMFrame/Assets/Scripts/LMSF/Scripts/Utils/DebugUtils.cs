using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public static class DebugUtils
    {
        public static bool IsOpenLog = true;
        private static string logTemplate = "<color=#{1}>{0}</color>";
        public static void Log(string msg, Color color, params object[] objects)
        {
            if (!IsOpenLog)
            {
                return;
            }
            string temp = msg;
            if (objects.Length > 0)
            {
                temp = String.Format(msg, objects);
            }
            Debug.Log(string.Format(logTemplate, temp, ColorUtility.ToHtmlStringRGB(color)));
        }

        public static void Log(string msg)
        {
            if (!IsOpenLog)
            {
                return;
            }
            Log(msg, Color.white);
        }
    public static void Log(string msg, params object[] objects)
    {
        if (!IsOpenLog)
        {
            return;
        }
        string temp = msg;
        if (objects.Length > 0)
        {
            temp = String.Format(msg, objects);
        }
    }



    public static void LogWarning(string fmt, params object[] objects)
        {
            if (!IsOpenLog)
            {
                return;
            }
            Log(fmt, Color.yellow, objects);
        }
        public static void LogError(string fmt, params object[] objects)
        {
            if (!IsOpenLog)
            {
                return;
            }
            Log(fmt, Color.red, objects);
        }
    }
