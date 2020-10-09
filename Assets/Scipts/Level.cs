using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Level",menuName ="New Level", order = 0)]
public class Level : ScriptableObject
{
    public Vector3[] trapPositions;
    public GameObject[] traps;

    public TextAsset missionObjectsDataFile;
    public Vector3[] missionObjectDestinations;


}
