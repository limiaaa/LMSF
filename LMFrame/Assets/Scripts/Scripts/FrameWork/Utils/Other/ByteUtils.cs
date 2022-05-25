using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ByteUtils 
{
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
}
