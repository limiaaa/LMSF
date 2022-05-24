using System;
using System.Security.Cryptography;
using System.Text;

public class AESProgram
{
	public static byte[] AESEncrypt(string Data, string keyTmp, string iv)
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

	public static sbyte[] byteToSbyte(byte[] Data)
	{
		sbyte[] array = new sbyte[Data.Length];
		for (int i = 0; i < Data.Length; i++)
		{
			array[i] = (sbyte)Data[i];
		}
		return array;
	}

	public static string byteTostring(byte[] Data)
	{
		string text = "";
		for (int i = 0; i < Data.Length; i++)
		{
			text += Data[i];
		}
		return text;
	}

	public static string MD5Byte(byte[] by)
	{
		string text = "";
		MD5 mD = MD5.Create();
		byte[] array = mD.ComputeHash(by);
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString("X");
		}
		return text;
	}

	public static string GetMD5(string myString)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.UTF8.GetBytes(myString);
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		mD5CryptoServiceProvider.Clear();
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString("X").PadLeft(2, '0');
		}
		return text.ToUpper();
	}
}
