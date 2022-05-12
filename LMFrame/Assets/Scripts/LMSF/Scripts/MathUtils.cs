using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    /// <summary>
    /// 旋转（1,0）向量
    /// </summary>
    /// <param name="angle">旋转多少度</param>
    /// <returns></returns>
    public static Vector2 RotationVectorRight(float angle)
    {
        float z = angle/Mathf.Rad2Deg;
        float x0 = Vector2.right.x;
        float y0 = Vector2.right.y;
        float x1 = x0 * Mathf.Cos(z) - y0 * Mathf.Sin(z);
        float y1 = x0 * Mathf.Sin(z) + y0 * Mathf.Cos(z);
        return new Vector2(x1 , y1);
    }
    
    /// <summary>
    /// 旋转（1,0）向量
    /// </summary>
    /// <param name="angle">旋转多少度</param>
    /// <returns></returns>
    public static Vector2 RotationVector(Vector2 dir,float angle)
    {
        float z = angle/Mathf.Rad2Deg;
        float x0 = dir.x;
        float y0 = dir.y;
        float x1 = x0 * Mathf.Cos(z) - y0 * Mathf.Sin(z);
        float y1 = x0 * Mathf.Sin(z) + y0 * Mathf.Cos(z);
        return new Vector2(x1 , y1);
    }
    
    public static Color ColorFromRGBString(string rgbstr)
    {
        int ivalue;
        try
        {
            ivalue = Convert.ToInt32(rgbstr, 16);
        }
        catch (Exception)
        {
            ivalue = 0xffffff;
        }

        byte a = 0xff;// (byte)((ivalue >> 24) & 0xff);
        byte r = (byte)((ivalue >> 16) & 0xff);
        byte g = (byte)((ivalue >> 8) & 0xff);
        byte b = (byte)((ivalue >> 0) & 0xff);

        Color32 c32 = new Color32(r, g, b, a);
        return c32;
    }
}
