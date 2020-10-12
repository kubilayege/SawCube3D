using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Helper
{
    public static Dictionary<Vector3,GameObject> ToDic(Vector3[] posList, GameObject[] gameObjectList)
    {
        var dicData = posList.Zip(gameObjectList, (pos, type) => new { pos, type })
                                            .ToDictionary(item => item.pos, item => item.type);
        return dicData;
    }

    public static bool Is<T>(this T obj, Type type)
    {
        return obj.GetType() == type;
    }

    public static bool SmartAdd<T>(this List<T> list,T item, Type exclude)
    {
        if (item != null && !list.Contains(item) && !item.Is(exclude))
        {
            list.Add(item);
            return true;
        }

        return false;
    }

    public static bool SmartAdd<T>(this List<T> list, T item)
    {
        if (item != null && !list.Contains(item))
        {
            list.Add(item);
            return true;
        }

        return false;

    }
    public static void Join<T>(this List<T> list, List<T> append)
    {
        foreach (var item in append)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

    }

    public static bool IsEmpty<T>(this List<T> list)
    {
        if (list.Count == 0)
            return true;

        return false;
    }
}
