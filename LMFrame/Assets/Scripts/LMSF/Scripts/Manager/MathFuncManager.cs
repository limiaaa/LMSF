using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;
using System.Text;
using System.Linq;
using DG.Tweening;

public class MathFuncManager
{
    /// <summary>
    /// 权重算法,传入权重列表与目标总权重,并且传入默认返回值(权重算法有轮空的概率，这样可返回默认返回值)
    /// </summary>
    /// <param name="WeightsList"></param>
    /// <param name="TotalWeights"></param>
    /// <param name="DefalutNumber"></param>
    /// <returns></returns>
    public int GetRandomWeightedIndex(List<int> WeightsList,int TotalWeights=0,int DefalutNumber=1)
    {
        if (TotalWeights == 0)
        {
            foreach(var item in WeightsList)
            {
                TotalWeights += item;
            }
        }
        int index = 0;
        int ListCount = WeightsList.Count;
        int Number = new System.Random().Next(0,TotalWeights);
        int Total = 0;
        while (index <= ListCount - 1)
        {
            Total += WeightsList[index];
            if (Number < Total)
            {
                return index + 1;
            }
            else
            {
                index++;
            }
        }
        return DefalutNumber;
    }
    /// <summary>
    /// 打乱一个列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public List<T> GetRandomListIndex<T>(List<T> list)
    {
        Random random = new Random();
        var newList = new List<T>();
        foreach (var item in list)
        {
            newList.Insert(new Random().Next(0, newList.Count), item);
        }
        return newList;
    }

    /// <summary>
    /// 从一个列表里去重的取出值填充到另外一个列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="CurrentList"></param>
    /// <param name="TargetList"></param>
    /// <returns></returns>
    public List<T> GetRandomNoRepetition<T>(List<T>CurrentList,List<T>TargetList)
    {
        var num = TargetList[new Random().Next(0, TargetList.Count)];
        if (CurrentList.Contains(num))
        {
            return GetRandomNoRepetition(CurrentList, TargetList);
        }
        else
        {
            return CurrentList;
        }
    }
    /// <summary>
    /// 从一个区间内取出一个列表去重的数
    /// </summary>
    /// <param name="CurrentList"></param>
    /// <param name="MinNumber"></param>
    /// <param name="MaxNumber"></param>
    /// <param name="RandomTime"></param>
    /// <returns></returns>
    public int GetRandomNoRepetition(List<int> CurrentList,int MinNumber,int MaxNumber)
    {
            int num = new Random().Next(MinNumber, MaxNumber);
            if (CurrentList.Contains(num))
            {
                return GetRandomNoRepetition(CurrentList, MinNumber, MaxNumber);
            }
            else
            {
                return num;
            }
    }

    /// <summary>
    /// 拼接一个list成员为字符串，用于储存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public string AppendstringFromList<T>(List<T> list)
    {
        if (list.Count <= 0)
        {
            Debug.Log("需要拼接的list长度为0");
            return "";
        }
        StringBuilder Str = new StringBuilder();
        Str.Append(".");
        foreach (var item in list)
        {
            Str.Append(item);
            Str.Append(".");
        }
        return Str.ToString();
    }
    /// <summary>
    /// 通过一个字符串返回字符串列表
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public List<string> GetListFromString(string str)
    {
        if (str == "")
        {
            Debug.Log("需要解析的string为空");
            return null;
        }
        var StringData = str.Split('.');
        var StringList = StringData.ToList();
        var ResultList = new List<string>();
        foreach (var item in StringList)
        {
            if (item != "")
            {
                ResultList.Add(item);
            }
        }
        return ResultList;
    }




}
