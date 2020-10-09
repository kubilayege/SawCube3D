using System.Collections;
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

}
