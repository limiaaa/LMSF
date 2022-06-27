using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public class PoolCtrl
    {
        string name;
        string abname;
        int Length=0;
        Dictionary<string, GameObject> objPool = new Dictionary<string, GameObject>();
        public GameObject Pool;
        public GameObject BaseObj;

        public PoolCtrl(string abName,string assetname,GameObject pool)
        {
            name = assetname;
            abname = abName;
            Pool= pool;
            Pool.transform.position = new Vector3(999, 999, 999);
            Pool.transform.eulerAngles = Vector3.zero;
            Pool.gameObject.SetActive(false);
            //BaseObj= ResourceManager.Instance.LoadPrefeb(abname, name);
        }
        public GameObject GetBase()
        {
            if (BaseObj == null)
            {
                //BaseObj = ResourceManager.Instance.LoadPrefeb(abname, name);
            }
            return BaseObj;
        }

        public void Despawn(GameObject obj)
        {
            Length++;

            objPool[Length.ToString()]=obj;
            obj.transform.SetParent(Pool.transform, true);
        }
        public GameObject Spawn()
        {
            if (Length == 0)
            {
                GameObject SpawnObj = GameObject.Instantiate(GetBase());
                SpawnObj.transform.position = Vector3.zero;
                SpawnObj.transform.localScale = Vector3.one;
                SpawnObj.transform.eulerAngles = Vector3.zero;

                return SpawnObj;
            }
            else
            {
                GameObject obj = objPool[Length.ToString()];
                objPool[Length.ToString()] = null;
                Length--;
                return obj;
            }
        }
    }
    public GameObject Pool;
    public Dictionary<string, PoolCtrl> ObjPoolDic = new Dictionary<string, PoolCtrl>();

    public ObjectManager(string name= "Pool")
    {
        Pool = new GameObject(name);
        
    }

    public void InitObjectManager()
    {
        
    }
    public void Preload(string abname,string assetname)
    {
        if (!ObjPoolDic.ContainsKey(assetname))
        {
            ObjPoolDic[assetname] = new PoolCtrl(abname, assetname, Pool);
        }
    }

    public GameObject Spwan(string abname,string name)
    {
        if (!ObjPoolDic.ContainsKey(name))
        {
            ObjPoolDic[name] = new PoolCtrl(abname, name,Pool);
        }
        GameObject obj = ObjPoolDic[name].Spawn();
        obj.transform.position = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.eulerAngles = Vector3.zero;

        obj.gameObject.SetActive(true);
        return obj;
    }
    public void DesSpwan(string name,GameObject obj)
    {
        if (!ObjPoolDic.ContainsKey(name))
        {
            DebugUtils.Log("无此对象池", "red");
            return;
        }
        Debug.Log(name+"---");
        foreach(var i in ObjPoolDic.Keys)
        {
            Debug.Log(i);
        }
        Debug.Log(ObjPoolDic[name]);
        ObjPoolDic[name].Despawn(obj);
    }

    public void ClearPool()
    {
        ObjPoolDic.Clear();
        GameObject.Destroy(Pool);
    }
}