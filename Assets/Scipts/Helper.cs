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

    public static List<T> SmartAdd<T>(this List<T> list,T item)
    {
        if (item != null)
            list.Add(item);

        return list;
    }

}
