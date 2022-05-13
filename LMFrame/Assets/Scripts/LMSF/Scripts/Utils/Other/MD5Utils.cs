using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class MD5Utils
{
    public static string Md5FromFile(string filePath)
    {
        return Md5(File.ReadAllBytes(filePath));
    }

    public static string Md5(string text)
    {
        return Md5(Encoding.Default.GetBytes(text));
    }

    public static string Md5(string text, Encoding encoding /*Encoding.Default*/)
    {
        return Md5(encoding.GetBytes(text));
    }

    public static string Md5(byte[] bytes)
    {
        byte[] hash;
        hash = (new MD5CryptoServiceProvider()).ComputeHash(bytes);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
            sb.Append(hash[i].ToString("X2"));
        return sb.ToString();
    }
}
