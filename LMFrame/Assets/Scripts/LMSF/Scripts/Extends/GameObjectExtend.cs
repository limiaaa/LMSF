using System;
using UnityEngine;
using UnityEngine.EventSystems;
public static class GameObjectExtend
{
    public static bool E_IsNull(this UnityEngine.Object _obj)
    {
        return _obj == null;
    }
    public static void E_AddChild(this GameObject _gameObject, GameObject _child, bool _reset = true)
    {
        _child.E_SetParent(_gameObject, _reset); 
    }
    public static void E_SetParent(this GameObject _gameObject, GameObject _parent, bool _reset = true)
    {
        Transform t = _gameObject.transform;
        t.SetParent(_parent.transform, true);
        if (_reset)
        {
            E_Reset(_gameObject);
        }
    }
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
    //_includeinactive包括不活跃的
    public static T E_FindChild<T>(this GameObject _gameObject, string _childname, bool _includeinactive = true)
        where T : MonoBehaviour
    {
        T[] ts = _gameObject.GetComponentsInChildren<T>(_includeinactive);
        for (int i = 0, length = ts.Length; i < length; i++)
        {
            if (ts[i].name.Equals(_childname))
            {
                return ts[i];
            }
        }
        return null;
    }
    public static GameObject E_FindChild(this GameObject _gameObject, string _childname, bool _includeinactive = true)
    {
        Transform[] ts = _gameObject.GetComponentsInChildren<Transform>(_includeinactive);
        for (int i = 0, length = ts.Length; i < length; i++)
        {
            if (ts[i].name.Equals(_childname))
            {
                return ts[i].gameObject;
            }
        }
        return null;
    }
    public static GameObject E_FindChildByPath(this GameObject _gameObject, string _path)
    {
        try
        {
            return _gameObject.transform.Find(_path).gameObject;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    public static T[] E_FindChildren<T>(this GameObject _gameObject, bool _includeinactive = true)
        where T : MonoBehaviour
    {
        T[] ts = _gameObject.GetComponentsInChildren<T>(_includeinactive);
        return ts;
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
    public static float E_SetAngle(this GameObject _gameObject ,Vector3 point,bool isAngle = false)
    {
        if (isAngle == true) {
            _gameObject.transform.localEulerAngles = point;
            return (float)point.z;
        }
        float a = E_GetAngle(_gameObject, point);
        _gameObject.transform.localEulerAngles = new Vector3(0, 0,a);
        return (float)a;
    }
    public static void E_SetSizeDelta(this GameObject _gameObject, float sizeX,float sizeY)
    {
        RectTransform t = _gameObject.transform.GetComponent<RectTransform>();
        t.sizeDelta = new Vector2(sizeX, sizeY); 
    }
}
