using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionObjectData 
{
    [SerializeField]
    public List<List<List<int>>> data { get; set; }
}