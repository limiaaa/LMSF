using System;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Collections;

public static class CryptUtils
{
    #region Libii 加密解密

        #region C

        private static string EncryptC(string content, string pw, string iv)
        {
            byte[] data;
            data = Encoding.UTF8.GetBytes(content);

            // 将byte数组的奇数字节异或0xA	
            for (int i = 0; i < data.Length; i += 2)
                data[i] ^= 0xA;
            // 将byte数组的偶数字节异或0xC	
            for (int i = 1; i < data.Length; i += 2)
                data[i] ^= 0xC;

            // 对这个byte数组用3DES算法加密	
            data = TripleDESEncrypt(data, pw, iv);

            // 	将加密后的byte数组用Base64编码并返回	
            return Convert.ToBase64String(data);
        }

        private static string DecryptC(string content, string pw, string iv)
        {
            // 将data解码成byte数组	
            byte[] data = Convert.FromBase64String(content);

            // 对这个byte数组用3DES算法解密	
            data = TripleDESDecrypt(data, pw, iv);

            // 将byte数组的奇数字节异或0xA	
            for (int i = 0; i < data.Length; i += 2)
                data[i] ^= 0xA;
            // 将byte数组的偶数字节异或0xC	
            for (int i = 1; i < data.Length; i += 2)
                data[i] ^= 0xC;

            return Encoding.UTF8.GetString(data);
        }
   
        #endregion

        #region T

        private static string EncryptT(string content, string pw, string iv)
        {
            byte[] data;
            data = Encoding.UTF8.GetBytes(content);

            // 将byte数组的奇数字节异或0x9	
            for (int i = 0; i < data.Length; i += 2)
                data[i] ^= 0x9;
            // 将byte数组的偶数字节异或0x7	
            for (int i = 1; i < data.Length; i += 2)
                data[i] ^= 0x7;

            // 对这个byte数组用AES算法加密	
            data = AESEncrypt(data, pw, iv);

            // 	将加密后的byte数组用Base64编码并返回	
            return Convert.ToBase64String(data);
        }

        private static string DecryptT(string content, string pw, string iv)
        {
            // 将data解码成byte数组	
            byte[] data = Convert.FromBase64String(content);

            // 对这个byte数组用AES算法解密	
            data = AESDecrypt(data, pw, iv);

            // 将byte数组的奇数字节异或0x9	
            for (int i = 0; i < data.Length; i += 2)
                data[i] ^= 0x9;
            // 将byte数组的偶数字节异或0x7	
            for (int i = 1; i < data.Length; i += 2)
                data[i] ^= 0x7;

            return Encoding.UTF8.GetString(data);
        }

        #endregion

        public enum EncryptType
        {
            C = 1,  //效验类型 C (3DES+BASE64)
            T = 2,  //效验类型 T (AES+BASE64)
            N = 3,  //不加解密 N 
        }

        public static string Encrypt(string content, string pw, string iv, string encryptType)
        {
            EncryptType t = (EncryptType)Enum.Parse(typeof(EncryptType), encryptType);
            return Encrypt(content, pw, iv, t);
        }

        public static string Decrypt(string content, string pw, string iv, string decryptType)
        {
            EncryptType t = (EncryptType)Enum.Parse(typeof(EncryptType), decryptType);
            return Decrypt(content, pw, iv, t);
        }

        public static string Encrypt(string content, string pw = "tripledesencryptionkey12", string iv = "12345678", EncryptType type = EncryptType.C)
        {
            string ret = "";
            switch (type)
            {
                case EncryptType.C:
                    ret = EncryptC(content, pw, iv);
                    break;
                case EncryptType.T:
                    ret = EncryptT(content, pw, iv);
                    break;
                case EncryptType.N:
                    ret = content;
                    break;
            }
            return ret;
        }

        public static string Decrypt(string content, string pw = "tripledesencryptionkey12", string iv = "12345678", EncryptType type = EncryptType.C)
        {
            string ret = "";
            switch (type)
            {
                case EncryptType.C:
                    ret = DecryptC(content, pw, iv);
                    break;
                case EncryptType.T:
                    ret = DecryptT(content, pw, iv);
                    break;
                case EncryptType.N:
                    ret = content;
                    break;
            }
            return ret;
        }

    #endregion
    #region 3DES 加密解密

    private static byte[] TripleDESEncrypt(byte[] data, string key, string iv)
    {
        TripleDESCryptoServiceProvider des;
        des         = new TripleDESCryptoServiceProvider();
        des.Key     = Encoding.ASCII.GetBytes(key);
        des.IV      = Encoding.ASCII.GetBytes(iv);
        des.Mode    = CipherMode.CBC;
        des.Padding = PaddingMode.PKCS7;
        return des.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
    }

    private static byte[] TripleDESDecrypt(byte[] data, string key, string iv)
    {
        TripleDESCryptoServiceProvider des;
        des         = new TripleDESCryptoServiceProvider();
        des.Key     = Encoding.ASCII.GetBytes(key);
        des.IV      = Encoding.ASCII.GetBytes(iv);
        des.Mode    = CipherMode.CBC;
        des.Padding = PaddingMode.PKCS7;
        return des.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
    }

    #endregion
    #region AES 加密解密
    private static byte[] AESEncrypt(byte[] data, string key, string iv)
    {
        Rijndael aes = Rijndael.Create();
        aes.Key     = Encoding.ASCII.GetBytes(key);
        aes.IV      = Encoding.ASCII.GetBytes(iv);
        aes.Mode    = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        return aes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
    }
    private static byte[] AESDecrypt(byte[] data, string key, string iv)
    {
        Rijndael aes = Rijndael.Create();
        aes.Key     = Encoding.ASCII.GetBytes(key);
        aes.IV      = Encoding.ASCII.GetBytes(iv);
        aes.Mode    = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        return aes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
    }
    public static byte[] EncryptAES(string Data, string keyTmp, string iv)
    {
        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.Mode = CipherMode.CBC;
        rijndaelManaged.Padding = PaddingMode.Zeros;
        rijndaelManaged.KeySize = 128;
        rijndaelManaged.BlockSize = 128;
        byte[] bytes = Encoding.UTF8.GetBytes(keyTmp);
        byte[] array = new byte[16];
        int num = bytes.Length;
        if (num > array.Length)
        {
            num = array.Length;
        }
        Array.Copy(bytes, array, num);
        rijndaelManaged.Key = array;
        rijndaelManaged.IV = Encoding.UTF8.GetBytes(iv);
        ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
        byte[] bytes2 = Encoding.UTF8.GetBytes(Data);
        return cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
    }
    public static byte[] DecodeAES(string text, string key, string iv)
    {
        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.Mode = CipherMode.CBC;
        rijndaelManaged.Padding = PaddingMode.Zeros;
        rijndaelManaged.KeySize = 128;
        rijndaelManaged.BlockSize = 128;
        byte[] array = Convert.FromBase64String(text);
        byte[] bytes = Encoding.UTF8.GetBytes(key);
        byte[] array2 = new byte[16];
        int num = bytes.Length;
        if (num > array2.Length)
        {
            num = array2.Length;
        }
        Array.Copy(bytes, array2, num);
        rijndaelManaged.Key = array2;
        rijndaelManaged.IV = Encoding.UTF8.GetBytes(iv);
        ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
        return cryptoTransform.TransformFinalBlock(array, 0, array.Length);
    }
#endregion
    #region AssetBundle加密解密
//16进制key值
public static string encryKey = "limilimi";
//加密AssetBundle
public static void EncryptAssetBundle(string abPath)
{
    byte[] EncryKey = Encoding.UTF8.GetBytes(encryKey);
    byte[] bytes = File.ReadAllBytes(abPath);
    for (int i = 0; i < bytes.Length; ++i)
    {
        bytes[i] ^= EncryKey[i % 4];
    }
    File.WriteAllBytes(abPath, bytes);
}
//解密AssetBundle
public static void DecryptAssetBundle(byte[] data)
{
    byte[] EncryKey = Encoding.UTF8.GetBytes(encryKey);
    for (int i = 0; i < data.Length; ++i)
    {
        data[i] ^= EncryKey[i % 4];
    }
}
public static IEnumerator DecryptAssetBundleAsync(byte[] data)
{
    byte[] EncryKey = Encoding.UTF8.GetBytes(encryKey);
    for (int i = 0; i < data.Length; ++i)
    {
        data[i] ^= EncryKey[i % 4];

        if (i % 2048 == 0)
        {
            yield return new UnityEngine.WaitForEndOfFrame();
        }
    }
}
#endregion
}

