using System;
using UnityEngine;
using UnityEngine.EventSystems;
public static class ExtendGameObject
{
    public static bool M_IsNull(this UnityEngine.Object _obj)
    {
        return _obj == null;
    }
    public static void M_AddChild(this GameObject _gameObject, GameObject _child, bool _reset = true)
    {
        _child.M_SetParent(_gameObject, _reset); 
    }

    public static void M_SetParent(this GameObject _gameObject, GameObject _parent, bool _reset = true)
    {
        Transform t = _gameObject.transform;
        GameObject obj = new GameObject();
        t.SetParent(_parent.transform, true);            
        if (_reset)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

                RectTransform rect = _gameObject.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                }
        }
    }

    public static T M_FindChild<T>(this GameObject _gameObject, string _childname, bool _includeinactive = true)
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

    public static GameObject M_FindChild(this GameObject _gameObject, string _childname, bool _includeinactive = true)
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

    public static GameObject M_FindChildByPath(this GameObject _gameObject, string _path)
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


    public static T[] M_FindChildren<T>(this GameObject _gameObject, bool _includeinactive = true)
        where T : MonoBehaviour
    {
        T[] ts = _gameObject.GetComponentsInChildren<T>(_includeinactive);
        return ts;
    }
    //用于滚动条2017.3 滚动条bug 设置宽 or 高
    public static void M_ScrollBar(this GameObject _gameObject, bool isbool = true, int rows = 1, int interval = 0)
    {
        RectTransform t = _gameObject.transform.GetComponent<RectTransform>();
        Vector2 wh = Vector2.zero;
        Vector2 m_wh = t.sizeDelta;
        int childCount = t.childCount;
        childCount =(int) Math.Ceiling((double)childCount / rows);
        RectTransform child = null;
        for (int i = childCount - 1; i >= 0; i--)
        {
            child = t.GetChild(i).gameObject.transform.GetComponent<RectTransform>();
            wh += child.sizeDelta;
            wh += new Vector2(interval,interval);
        }
        if (isbool){m_wh.x = wh.x; }else{ m_wh.y = wh.y; }
        t.sizeDelta = m_wh;
    }
    public static Vector3 M_TouchPosition(this GameObject _gameObject)
    {
        return Input.mousePosition;
    }
    //设置角度
    public static float M_GetAngle(this GameObject _gameObject, Vector3 point)
    {
        float a = Vector3.Angle(Vector3.up, point);
        if (point.x > 0)
        {
            a = -a;
        }
        return (float)a;
    }
    //设置角度
    public static float M_SetAngle(this GameObject _gameObject ,Vector3 point,bool isAngle = false)
    {
        if (isAngle == true) {
            _gameObject.transform.localEulerAngles = point;
            return (float)point.z;
        }
        float a = Vector3.Angle(Vector3.up, point);
        if (point.x > 0)
        {
            a = -a;
        }
        _gameObject.transform.localEulerAngles = new Vector3(0, 0, a);
        return (float)a;
    }
    public static void M_SetSize(this GameObject _gameObject, float size)
    {
        RectTransform t = _gameObject.transform.GetComponent<RectTransform>();
        t.sizeDelta = new Vector2(size, size); 
    }
    //当前坐标 转换 为父层级坐标
    public static Vector3 M_GetToParentPoint(this GameObject _gameObject , GameObject parent)
    {
        return parent.transform.InverseTransformPoint(_gameObject.transform.position);
    }   
    public static void M_RemoveAll(this GameObject _gameObject)
    {
        Transform t = _gameObject.transform;
        int childCount = t.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(t.GetChild(i).gameObject);
        }
    }
}
