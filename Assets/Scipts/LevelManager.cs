using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

public enum NextLevelState
{
    Restart = 0,
    Pass = 1
}

public class LevelManager : MonoBehaviour
{
    public Level[] levels;
    public Level currentLevelData;
    public GameObject currentLevel;

    public GameObject currentLevelMissionObject;

    public int currentLevelIndex;
    public int currentLevelProgress;
    private Vector3 currentLevelMissionDestination;



    public int xMin;
    public int xMax;
    public int zMin;
    public int zMax;

    [SerializeField]
    public MissionObjectData currentMissionObjects;


    public Material centerMat;
    public Material missionMat;

    public Coroutine progressCoroutine;
    public static LevelManager instance;

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangeLevel(NextLevelState.Restart);
    }
    public void ChangeLevel(NextLevelState val)
    {
        Debug.Log(val);
        currentLevelProgress = 0;
        currentLevelMissionObject = null;
        currentLevelIndex += (int)val;
        progressCoroutine = null;
        if (currentLevel != null) Destroy(currentLevel);

        currentLevelData = levels[currentLevelIndex % levels.Length];
        currentLevel = LevelConstructor.instance.InitLevel(levels[currentLevelIndex % levels.Length]);
        currentMissionObjects = JsonConvert.DeserializeObject<MissionObjectData>(currentLevelData.missionObjectsDataFile.text);

        //SpawnTraps();

        ProgressLevel();
    }

    private void SpawnTraps()
    {
        Debug.Log("TrapSpawn");
        for (int i = 0; i < currentLevelData.traps.Length; i++)
        {
            Instantiate(currentLevelData.traps[i], currentLevelData.trapPositions[i], currentLevelData.traps[i].transform.rotation, currentLevel.transform);
        }
    }

    public void ProgressLevel()
    {
        if(progressCoroutine == null)
        {
            if (currentLevelMissionObject != null)
                progressCoroutine = StartCoroutine(ProgressAnimation());
            else
                SpawnNextMissionPart();
        }

    }

    public IEnumerator ProgressAnimation()
    {
        Vector3 from;
        Vector3 to = currentLevelMissionObject.transform.position + Vector3.up*3f;
        float verticalAnimDuration = 1f;
        float t = 0;
        while (t < verticalAnimDuration)
        {
            from = currentLevelMissionObject.transform.position;
            t += Time.unscaledDeltaTime;
            currentLevelMissionObject.transform.position = Vector3.Lerp(from, to, t / verticalAnimDuration);

            yield return null;
        }

        to = currentLevelMissionDestination + Vector3.up*3f;
        float horizontalAnimDuration = 1f;
        t = 0;
        while (t < horizontalAnimDuration)
        {
            from = currentLevelMissionObject.transform.position;
            t += Time.unscaledDeltaTime;
            currentLevelMissionObject.transform.position = Vector3.Lerp(from, to, t / horizontalAnimDuration);

            yield return null;
        }

        to = currentLevelMissionDestination;
        t = 0;
        while (t < verticalAnimDuration)
        {
            from = currentLevelMissionObject.transform.position;
            t += Time.unscaledDeltaTime;
            currentLevelMissionObject.transform.position = Vector3.Lerp(from, to, t / verticalAnimDuration);

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        currentLevelMissionObject.GetComponent<Player>().enabled = false;

        //FinishLevel
        if (++currentLevelProgress >= currentMissionObjects.data.Count)
            ChangeLevel(NextLevelState.Pass);
        else
            SpawnNextMissionPart();

    }

    private void SpawnNextMissionPart()
    {
        currentLevelMissionObject = new GameObject();
        currentLevelMissionObject.AddComponent<Rigidbody>().useGravity =false;
        currentLevelMissionObject.AddComponent<Player>();
        currentLevelMissionObject.name = "missionObject";
        currentLevelMissionObject.transform.position = Vector3.zero;
        currentLevelMissionObject.transform.parent = currentLevel.transform;
        List<List<int>> objectData = currentMissionObjects.data[currentLevelProgress];
        BreakManager.instance.InitBlocks(objectData.Count, objectData[0].Count);

        for (int i = 0; i < objectData.Count; i++)
        {
            for (int j = 0; j < objectData[i].Count; j++)
            {
                switch (objectData[i][j])
                {
                    //empty
                    case 0:
                        
                        break;

                    //center pieces
                    case 1:
                        GameObject centerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        centerCube.transform.position = new Vector3(j, 0, i);
                        centerCube.GetComponent<MeshRenderer>().material = centerMat;
                        centerCube.transform.parent = currentLevelMissionObject.transform;
                        centerCube.AddComponent<Center>().Init(i,j);
                        break;

                    //mission pieces
                    default:
                        GameObject missionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        missionCube.transform.position = new Vector3(j, 0, i);
                        missionCube.GetComponent<MeshRenderer>().material = missionMat;
                        missionCube.transform.parent = currentLevelMissionObject.transform;
                        missionCube.AddComponent<Mission>().Init(i, j);
                        BreakManager.instance.missionCount++;
                        break;
                }
            }
        }

        progressCoroutine = null;
        currentLevelMissionDestination = currentLevelData.missionObjectDestinations[currentLevelProgress];

    }
}
