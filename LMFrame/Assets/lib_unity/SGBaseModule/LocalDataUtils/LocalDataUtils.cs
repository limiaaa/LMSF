using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace SG.LocalData
{
    public class LocalDataUtils : MonoBehaviour
    {
        #region save

        private static bool USE_ENCRYPT = false;
        public static void SaveToFile<T>(T data)
        {
            string path = ResourcePath.localPath + typeof(T).Name + ".json";
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            if (fs != null)
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.SetLength(0);

                string jsonStr = JsonConvert.SerializeObject(data);
                byte[] bytes = Encoding.Default.GetBytes(jsonStr);
                if (USE_ENCRYPT)
                {
                    byte[] newBuff = EncryptBytesSimple(bytes);
                    fs.Write(newBuff, 0, newBuff.Length);
                }
                else
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }

            fs.Close();
        }

        #endregion

        #region Load

        public static T LoadFromFile<T>() where T : class, new()
        {
            T data = null;
            string path = ResourcePath.localPath + typeof(T).Name + ".json";
            if (!File.Exists(path))
            {
                return null;
            }

            FileStream fs = new FileStream(path, FileMode.Open);
            if (fs != null)
            {
                string jsonStr = null;
                if (USE_ENCRYPT)
                {
                    byte[] buff = new byte[fs.Length];
                    fs.Read(buff, 0, (int)fs.Length);
                    byte[] newBuff = DecryptBytesSimple(buff);
                    jsonStr = Encoding.Default.GetString(newBuff);
                }
                else
                {

                    byte[] bytes = new byte[fs.Length];
                    int nResult = fs.Read(bytes, 0, bytes.Length);
                    if (nResult > 0)
                        jsonStr = Encoding.Default.GetString(bytes);
                }
                if (!string.IsNullOrEmpty(jsonStr))
                    data = JsonConvert.DeserializeObject<T>(jsonStr);
            }

            fs.Close();
            return data;
        }

        #endregion

        #region SaveToBin
        public static void SaveBinToFile<T>(T data)
        {
            string path = ResourcePath.localPath + typeof(T).Name + ".bin";
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            if (fs != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, data);
                fs.Close();
            }
        }
        #endregion

        #region LoadFromBin
        public static T LoadBinFromFile<T>() where T : class, new()
        {
            T data = null;
            string path = ResourcePath.localPath + typeof(T).Name + ".bin";
            if (!File.Exists(path))
            {
                return null;
            }

            FileStream fs = new FileStream(path, FileMode.Open);
            if (fs != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                data = bf.Deserialize(fs) as T;
                fs.Close();
            }
            return data;
        }

        #endregion

        public static byte[] EncryptBytesSimple(byte[] array)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(GetKey());
            int num = bytes.Length;
            int num2 = 0;
            for (int i = 0; i < array.Length; i++)
            {
                int expr_27_cp_1 = i;
                array[expr_27_cp_1] ^= bytes[num2];
                num2 = (num2 + 1) % num;
            }
            return array;
        }

        public static byte[] DecryptBytesSimple(byte[] array)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(GetKey());
            int num = bytes.Length;
            int num2 = 0;
            for (int i = 0; i < array.Length; i++)
            {
                int expr_27_cp_1 = i;
                array[expr_27_cp_1] ^= bytes[num2];
                num2 = (num2 + 1) % num;
            }
            return array;
        }

        public static string GetKey()
        {
            return "qcDY6X+aPLw=";
        }
    }
}
