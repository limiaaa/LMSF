using System;
using UnityEngine;
public static class ExtendTransform
{
    public static void M_SetLocalX(this Transform _transform, float _x,bool _overly=false)
    {
        Vector3 v = _transform.localPosition;
        v.x = _overly ? v.x + _x : _x;
        _transform.localPosition = v;
    }

    public static void M_SetLocalY(this Transform _transform, float _y, bool _overly = false)
    {
        Vector3 v = _transform.localPosition;
        v.y = _overly ? v.y + _y : _y;
        _transform.localPosition = v;
    }

    public static void M_SetLocalZ(this Transform _transform, float _z, bool _overly = false)
    {
        Vector3 v = _transform.localPosition;
        v.z = _overly ? v.z + _z : _z;
        _transform.localPosition = v;
    }

    public static void M_SetX(this Transform _transform, float _x, bool _overly = false)
    {
        Vector3 v = _transform.position;
        v.x = _overly ? v.x + _x : _x;
        _transform.position = v;
    }

    public static void M_SetY(this Transform _transform, float _y, bool _overly = false)
    {
        Vector3 v = _transform.position;
        v.y = _overly ? v.y + _y : _y;
        _transform.position = v;
    }

    public static void M_SetZ(this Transform _transform, float _z, bool _overly = false)
    {
        Vector3 v = _transform.position;
        v.z = _overly ? v.z + _z : _z;
        _transform.position = v;
    }


    public static void M_SetLocalRX(this Transform _transform,float _angle)
    {
        Vector3 v = _transform.localEulerAngles;
        v.x = _angle;
        _transform.localEulerAngles = v;
    }

    public static void M_SetLocalRY(this Transform _transform, float _angle)
    {
        Vector3 v = _transform.localEulerAngles;
        v.y = _angle;
        _transform.localEulerAngles = v;
    }

    public static void M_SetLocalRZ(this Transform _transform, float _angle)
    {
        Vector3 v = _transform.localEulerAngles;
        v.z = _angle;
        _transform.localEulerAngles = v;
    }
    public static void ResetLocal(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }
    public static Transform ClearChildren(this Transform transform)
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
        return transform;
    }
}
