using System;
using System.Collections.Generic;
using UnityEngine;
public static class ComponentExtend
{
    public static bool E_IsNull(this UnityEngine.Object _obj)
    {
        return _obj == null;
    }
    #region Transform
    //------------------------------------------------------Transform
    public static void E_SetLocalX(this Transform _transform, float _x,bool _overly=false)
    {
        Vector3 v = _transform.localPosition;
        v.x = _overly ? v.x + _x : _x;
        _transform.localPosition = v;
    }
    public static void E_SetLocalY(this Transform _transform, float _y, bool _overly = false)
    {
        Vector3 v = _transform.localPosition;
        v.y = _overly ? v.y + _y : _y;
        _transform.localPosition = v;
    }
    public static void E_SetLocalZ(this Transform _transform, float _z, bool _overly = false)
    {
        Vector3 v = _transform.localPosition;
        v.z = _overly ? v.z + _z : _z;
        _transform.localPosition = v;
    }
    public static void E_SetX(this Transform _transform, float _x, bool _overly = false)
    {
        Vector3 v = _transform.position;
        v.x = _overly ? v.x + _x : _x;
        _transform.position = v;
    }
    public static void E_SetY(this Transform _transform, float _y, bool _overly = false)
    {
        Vector3 v = _transform.position;
        v.y = _overly ? v.y + _y : _y;
        _transform.position = v;
    }
    public static void E_SetZ(this Transform _transform, float _z, bool _overly = false)
    {
        Vector3 v = _transform.position;
        v.z = _overly ? v.z + _z : _z;
        _transform.position = v;
    }
    public static void E_SetLocalRX(this Transform _transform,float _angle)
    {
        Vector3 v = _transform.localEulerAngles;
        v.x = _angle;
        _transform.localEulerAngles = v;
    }
    public static void E_SetLocalRY(this Transform _transform, float _angle)
    {
        Vector3 v = _transform.localEulerAngles;
        v.y = _angle;
        _transform.localEulerAngles = v;
    }
    public static void E_SetLocalRZ(this Transform _transform, float _angle)
    {
        Vector3 v = _transform.localEulerAngles;
        v.z = _angle;
        _transform.localEulerAngles = v;
    }
    public static void E_Reset(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
        RectTransform rect = trans.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
        }
    }
    #endregion
    #region GameObject
    //------------------------------------------------------GameObject
    public static void E_Reset(this GameObject _gameObject)
    {
        Transform t = _gameObject.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        RectTransform rect = _gameObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
        }
    }
    //设置角度
    public static float E_GetAngle(this GameObject _gameObject, Vector3 point)
    {
        float a = Vector3.Angle(Vector3.up, point);
        if (point.x > 0)
        {
            a = -a;
        }
        return (float)a;
    }
    //设置角度
    public static float E_SetAngle(this GameObject _gameObject, Vector3 point, bool isAngle = false)
    {
        if (isAngle == true)
        {
            _gameObject.transform.localEulerAngles = point;
            return (float)point.z;
        }
        float a = E_GetAngle(_gameObject, point);
        _gameObject.transform.localEulerAngles = new Vector3(0, 0, a);
        return (float)a;
    }
    public static void E_SetSizeDelta(this GameObject _gameObject, float sizeX, float sizeY)
    {
        RectTransform t = _gameObject.transform.GetComponent<RectTransform>();
        t.sizeDelta = new Vector2(sizeX, sizeY);
    }
    #endregion
    #region Vector3
    //------------------------------------------------------Vector3
    public static void E_SetX(this Vector3 _v, float _x, bool _overly = false)
    {
        _v.Set(_overly ? _v.x + _x : _x, _v.y, _v.z);
    }
    public static void E_SetY(this Vector3 _v, float _y, bool _overly = false)
    {
        _v.Set(_v.x, _overly ? _v.y + _y : _y, _v.z);
    }
    public static void E_SetZ(this Vector3 _v, float _z, bool _overly = false)
    {
        _v.Set(_v.x, _v.y, _overly ? _v.z + _z : _z);
    }
    //对比内容
    public static bool E_Equal(this Vector3 _v1, Vector3 _v2)
    {
        if (_v1.x != _v2.x)
            return false;
        if (_v1.y != _v2.y)
            return false;
        if (_v1.z != _v2.z)
            return false;
        return true;
    }
    public static void E_CopyFrom(this Vector3 _old, Vector3 _new)
    {
        _old.x = _new.x;
        _old.y = _new.y;
        _old.z = _new.z;
    }
    #endregion
    #region Other
    //------------------------------------------------------Other
    public static bool IsEmpty<T>(this ICollection<T> list)
    {
        if (list != null && list.Count > 0)
        {
            return false;
        }
        return true;
    }
    public static T GetOrAddComponent<T>(this MonoBehaviour script) where T : Component
    {
        var result = script.GetComponent<T>();
        if (result == null)
        {
            result = script.gameObject.AddComponent<T>();
        }
        return result;
    }
    public static string GetLibiiDateString(this DateTime time)
    {
        return string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}",
            time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
    }
    #endregion
    #region Texture
    public static Texture2D CreateTextureFromAtlas(this Texture2D mainTexture, Rect rect, int fullWidth,
    int fullHeight)
    {
        var fullPixels = mainTexture.GetPixels32();
        int spriteX = (int)rect.x;
        int spriteY = (int)rect.y;
        int spriteWidth = (int)rect.width;
        int spriteHeight = (int)rect.height;

        int xmin = Mathf.Clamp(spriteX, 0, fullWidth);
        int ymin = Mathf.Clamp(spriteY, 0, fullHeight);
        int xmax = Mathf.Min(xmin + spriteWidth, fullWidth - 1);
        int ymax = Mathf.Min(ymin + spriteHeight, fullHeight - 1);
        int newWidth = Mathf.Clamp(spriteWidth, 0, fullWidth);
        int newHeight = Mathf.Clamp(spriteHeight, 0, fullHeight);

        if (newWidth == 0 || newHeight == 0) return null;

        Color32[] newPixels = new Color32[newWidth * newHeight];

        for (int y = 0; y < newHeight; ++y)
        {
            int cy = ymin + y;
            if (cy > ymax) cy = ymax;

            for (int x = 0; x < newWidth; ++x)
            {
                int cx = xmin + x;
                if (cx > xmax) cx = xmax;

                int newIndex = y * newWidth + x;
                int oldIndex = cy * fullWidth + cx;

                newPixels[newIndex] = fullPixels[oldIndex];
            }
        }

        var tex = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false);
        tex.SetPixels32(newPixels);

        return tex;
    }
    #endregion
}
