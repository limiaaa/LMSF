/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/11/22
 * Note  : Json格式数据读取
***************************************************************/

using UnityEngine;
using System.Collections;
using System.IO;

namespace SG.AssetBundleBrowser
{
    public static class SimpleJsonReader
    {
        /// <summary>
        /// 从文件读取
        /// </summary>
        public static bool ReadFromFile<T>(ref T data, string file_name)
            where T : class
        {
            try
            {
                if (!string.IsNullOrEmpty(file_name))
                {
                    string str = null;
                    if (File.Exists(file_name))
                        str = File.ReadAllText(file_name);

                    return ReadFromString<T>(ref data, str);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            }

            return false;
        }

        /// <summary>
        /// 从字符串中读取
        /// </summary>
        public static bool ReadFromString<T>(ref T data, string str)
            where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                    return false;

                data = SimpleJson.SimpleJson.DeserializeObject<T>(str);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            }

            return false;
        }

        /// <summary>
        /// 从文件读取
        /// </summary>
        public static T ReadFromFile<T>(string file_name)
            where T : class
        {
            try
            {
                if (!string.IsNullOrEmpty(file_name))
                {
                    string str = null;
                    if (File.Exists(file_name))
                        str = File.ReadAllText(file_name);

                    return ReadFromString<T>(str);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// 从字符串中读取
        /// </summary>
        public static T ReadFromString<T>(string str)
            where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                T data = SimpleJson.SimpleJson.DeserializeObject<T>(str);
                return data;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            }

            return null;
        }
    }
}