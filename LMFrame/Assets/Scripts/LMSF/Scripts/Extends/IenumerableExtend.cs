using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IenumerableExtend
{
    public static void E_ForEach<TValue>(this IEnumerable<TValue> dict, Action<TValue> action)
    {
        if (action == null) return;
        var it = dict.GetEnumerator();
        while (it.MoveNext())
        {
            action(it.Current);
        }
        it.Dispose();
    }
    public static void E_ForEach<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, Action<TKey, TValue> action)
    {
        if (action == null) return;
        var it = dict.GetEnumerator();
        while (it.MoveNext())
        {
            action(it.Current.Key, it.Current.Value);
        }
        it.Dispose();
    }
    public static TValue E_Find<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, Predicate<TValue> match)
    {
        if (match != null)
        {
            var it = dict.GetEnumerator();
            while (it.MoveNext())
            {
                if (match(it.Current.Value))
                    return it.Current.Value;
            }
            it.Dispose();
        }
        return default(TValue);
    }
    public static List<TValue> E_FindAll<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, Predicate<TValue> match)
    {
        if (match != null)
        {
            List<TValue> retList = new List<TValue>();
            var it = dict.GetEnumerator();
            while (it.MoveNext())
            {
                if (match(it.Current.Value))
                    retList.Add(it.Current.Value);
            }
            it.Dispose();
            return retList;
        }
        return null;
    }
}
