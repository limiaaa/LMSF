using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LocalFileUtils 
{
    static bool USE_ENCRYPT = false;
    static string KEY = "qcDY6X+aPLw=";

    public static void SaveToFile<T>(T data)
    {
        string jsonPath = ResourcePath.localPath + typeof(T).Name + ".json";
        FileStream fs = new FileStream(jsonPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        if (fs != null)
        {
            //移动指针到开始第0个字符
            fs.Seek(0, SeekOrigin.Begin);
            //将流置0
            fs.SetLength(0);
            string jsonStr = JsonConvert.SerializeObject(data);
            byte[] bytes = Encoding.Default.GetBytes(jsonStr);
            if (USE_ENCRYPT)
            {
                byte[] newbyte = EnBytes(bytes);
                fs.Write(newbyte, 0, newbyte.Length);
            }
            else
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        fs.Close();
    }
    public static T LoadFromFile<T>() where T:class,new()
    {
        T t = null;
        string jsonPath = ResourcePath.localPath + typeof(T).Name + ".json";
        if (!File.Exists(jsonPath))
        {
            return null;
        }
        FileStream fs = new FileStream(jsonPath, FileMode.Open);
        if (fs != null)
        {
            string jsonStr = null;
            byte[] by = new byte[fs.Length];
            if (USE_ENCRYPT)
            {
                fs.Read(by, 0, (int)fs.Length);
                byte[] newby = DeBytes(by);
                jsonStr = Encoding.Default.GetString(newby);
            }
            else
            {
                int result = fs.Read(by, 0, by.Length);
                if (result > 0)
                    jsonStr = Encoding.Default.GetString(by);
            }
            if (!string.IsNullOrEmpty(jsonStr))
            {
                t = JsonConvert.DeserializeObject<T>(jsonStr);
            }
        } 
        fs.Close();
        return t;
    }
    //异或特质就是亦或一次变为另外的byte串，再次异或就会返回刚才
    public static byte[] EnBytes(byte[] array)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(KEY);
        int num1 = bytes.Length;
        int num2 = 0;
        for (int i = 0; i < array.Length; i++)
        {
            array[i] ^= bytes[num2];
            num2 = (num2 + 1) % num1;
        }
        return array;
    }

    public static byte[] DeBytes(byte[] array)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(KEY);
        int num1 = bytes.Length;
        int num2 = 0;
        for (int i = 0; i < array.Length; i++)
        {
            array[i] ^= bytes[num2];
            num2 = (num2 + 1) % num1;
        }
        return array;
    }



}
