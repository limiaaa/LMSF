using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class FlyCoinHelper : MonoBehaviour
{
    [Header("[Boomb Param]")]
    public float range; //炸出范围
    public float timeToBoom;  //飞出时间
    public float timeToStop;  //飞出后停滞的时间
    public float timeToCollect; //飞去固定位置的时间
    [Header("[UnityObj]")]

   GameObject prefab; //复制的预设

    //--------------- status --------------
    public bool isPlaying { get { return ani != null; } }
    public GameObject sprite
    {
        get { return prefab.gameObject; }
        set { prefab = value; }
    }
    public Vector3 startPos
    {
        get { return _startPos; }
        set { this._startPos = value; }
    }
    //--------------- private -------------
    Coroutine ani;
    Transform parentOfCurrency;
    Vector3 _startPos;
    Action FlyEndFunc;
    //--------------- function ------------
    public void BoombToCollectCurrency(int count, Vector3 to,Action flyendfunc)
    {
        _startPos = this.transform.position;
        prefab = this.transform.GetChild(0).gameObject;
        prefab.gameObject.SetActive(false);
        FlyEndFunc = flyendfunc;
        if (isPlaying) return;
        CreateParent();
        int coinCount = count;
        ani = StartCoroutine(_BoombToCollectCurrency(coinCount, to));
    }

    public void StopAni()
    {
        if (!isPlaying) return;
        StopAllCoroutines();
        ani = null;
        DestroyImmediate(parentOfCurrency.gameObject);
        DestroyImmediate(this.gameObject);
    }

    //----------- boomb ani -----------------

    IEnumerator _BoombToCollectCurrency(int count, Vector3 to)
    {
        List<Transform> currencyObj = new List<Transform>();
        for (int i = 0; i < count; i++)
        {
            currencyObj.Add(CreateCurrencyObj());
        }
        yield return StartCoroutine(ExporeOut(currencyObj));
        yield return new WaitForSeconds(timeToStop);
        yield return StartCoroutine(FlyToPos(currencyObj, to));
        Destroy(parentOfCurrency.gameObject);
        ani = null;
        Destroy(this.gameObject);
    }

    IEnumerator ExporeOut(List<Transform> currencyObj)
    {
        List<Vector3> toPos = new List<Vector3>();
        List<Vector3> fromPos = new List<Vector3>();
        foreach (Transform obj in currencyObj)
        {
            Vector3 pos = Random.insideUnitCircle;
            pos = pos * range + obj.transform.position;
            toPos.Add(pos);
            fromPos.Add(obj.transform.position);
        }
        float DelayTimerHelper = 0;
        while (DelayTimerHelper < timeToBoom)
        {
            DelayTimerHelper += Time.deltaTime;
            for (int i = 0; i < currencyObj.Count; i++)
            {
                Vector3 from = fromPos[i];
                Vector3 to = toPos[i];
                Transform obj = currencyObj[i];
                Vector3 pos = Vector3.Lerp(currencyObj[i].transform.position, to, DelayTimerHelper / timeToBoom);
                obj.transform.position = pos;
            }
            yield return null;
        }
    }
    IEnumerator FlyToPos(List<Transform> currencyObj, Vector3 to)
    {
        Vector3 toPos = to;
        List<Vector3> fromPos = new List<Vector3>();
        foreach (Transform obj in currencyObj)
            fromPos.Add(obj.transform.position);
        float DelayTimerHelper = 0;
        while (DelayTimerHelper < timeToCollect)
        {
            DelayTimerHelper += Time.deltaTime;
            for (int i = 0; i < currencyObj.Count; i++)
            {
                Vector3 from = fromPos[i];
                Transform obj = currencyObj[i];
                Vector3 pos = Vector3.Lerp(from, to, DelayTimerHelper / timeToCollect);
                obj.transform.position = pos;
            }
            yield return null;
        }
        //SoundMgr.Instance.PlaySoundEffect(SoundTypes.coin_get.ToString());
        if (FlyEndFunc!=null)
        {
            FlyEndFunc();
        }
    }



    Transform CreateCurrencyObj()
    {
        GameObject obj = GameObject.Instantiate(prefab.gameObject);
        obj.SetActive(true);
        obj.transform.SetParent(parentOfCurrency);
        //obj.GetComponent<Image>().sprite = sprite;
        obj.transform.position = parentOfCurrency.transform.position;
        obj.transform.localScale = Vector3.one;
        return obj.transform;
    }

    void CreateParent()
    {
        GameObject obj = new GameObject("parentOfCurrency");
        parentOfCurrency = obj.transform;
        parentOfCurrency.SetParent(this.transform);
        parentOfCurrency.localScale = Vector3.one;
        parentOfCurrency.position =new Vector3(startPos.x, startPos.y,-10);
        //GameUtils.SetGameObjectLayer(parentOfCurrency.transform, LayerMask.NameToLayer(Global.Layer.UI));
        parentOfCurrency.position = new Vector3(startPos.x, startPos.y, -100);
        parentOfCurrency.localPosition = new Vector3(startPos.x, startPos.y, -100);

    }

}
