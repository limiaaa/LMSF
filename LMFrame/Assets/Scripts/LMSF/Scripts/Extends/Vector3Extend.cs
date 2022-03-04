using System;
using UnityEngine;
public static class Vector3Extend
{
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
    //¶Ô±ÈÄÚÈÝ
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
}
