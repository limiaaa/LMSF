using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Networking;

public class Utils 
{
    // 获取文件下所有文件大小
    public static int GetAllFileSize(string filePath)
    {
        int sum = 0;
        if (!Directory.Exists(filePath))
        {
            return 0;
        }

        DirectoryInfo dti = new DirectoryInfo(filePath);

        FileInfo[] fi = dti.GetFiles();

        for (int i = 0; i < fi.Length; ++i)
        {
            sum += Convert.ToInt32(fi[i].Length / 1024);
        }

        DirectoryInfo[] di = dti.GetDirectories();

        if (di.Length > 0)
        {
            for (int i = 0; i < di.Length; i++)
            {
                sum += GetAllFileSize(di[i].FullName);
            }
        }

        return sum;
    }
    // 获取指定文件大小
    public static long GetFileSize(string file_path)
    {
        long sum = 0;
        if (!File.Exists(file_path))
        {
            return 0;
        }
        else
        {
            FileInfo Files = new FileInfo(file_path);
            sum += Files.Length;
        }

        return sum;
    }
    //   获得取除路径扩展名的路径
    public static string GetPathWithoutExtension(string full_name)
    {
        int last_idx = full_name.LastIndexOfAny(".".ToCharArray());
        if (last_idx < 0)
            return full_name;

        return full_name.Substring(0, last_idx);
    }
}
