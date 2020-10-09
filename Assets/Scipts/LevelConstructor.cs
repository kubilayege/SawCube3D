using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelConstructor : MonoBehaviour
{
    public static LevelConstructor instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject InitLevel(Level level)
    {
        GameObject nextLevel = new GameObject();
        nextLevel.transform.position = Vector3.zero;
        nextLevel.name = "Level";

        foreach (var trap in Helper.ToDic(level.trapPositions,level.traps))
        {
            Instantiate(trap.Value, trap.Key, trap.Value.transform.rotation, nextLevel.transform);
        }

        return nextLevel;
    }
}
