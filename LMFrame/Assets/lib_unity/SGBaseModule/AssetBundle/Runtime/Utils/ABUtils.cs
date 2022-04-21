using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG.AssetBundleBrowser
{
    public class ABUtils
    {
        private static List<string> keys = new List<string>();

        public static void PlayerPrefsSetInt(string key, int value)
        {
            if (!keys.Contains(key)) keys.Add(key);
            PlayerPrefs.SetInt(key, value);
        }

        public static void PlayerPrefsSetFloat(string key, float value)
        {
            if (!keys.Contains(key)) keys.Add(key);
            PlayerPrefs.SetFloat(key, value);
        }

        public static void PlayerPrefsSetString(string key, string value)
        {
            if (!keys.Contains(key)) keys.Add(key);
            PlayerPrefs.SetString(key, value);
        }

        public static void Clear()
        {
            foreach (var key in keys)
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
    }
}
