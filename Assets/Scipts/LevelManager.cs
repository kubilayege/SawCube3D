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

    [SerializeField]
    public MissionObjectData currentMissionObjects;


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

        if (currentLevel != null) Destroy(currentLevel);

        currentLevelData = levels[currentLevelIndex % levels.Length];
        currentLevel = LevelConstructor.instance.InitLevel(levels[currentLevelIndex % levels.Length]);
        currentMissionObjects = JsonConvert.DeserializeObject<MissionObjectData>(currentLevelData.missionObjectsDataFile.text);

        ProgressLevel();
    }

    private void ProgressLevel()
    {
        if (currentLevelMissionObject != null)
            StartCoroutine(ProgressAnimation());
        else
            SpawnNextMissionPart();
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



        //FinishLevel
        if (++currentLevelProgress >= currentMissionObjects.data.Count)
            ChangeLevel(NextLevelState.Pass);
        else
            SpawnNextMissionPart();

    }

    private void SpawnNextMissionPart()
    {
        currentLevelMissionObject = new GameObject();
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
                        centerCube.transform.parent = currentLevelMissionObject.transform;
                        centerCube.AddComponent<Center>().Init(i,j);
                        break;

                    //mission pieces
                    default:
                        GameObject missionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        missionCube.transform.position = new Vector3(j, 0, i);
                        missionCube.transform.parent = currentLevelMissionObject.transform;
                        missionCube.AddComponent<Mission>().Init(i, j);
                        break;
                }
            }
        }


        currentLevelMissionDestination = currentLevelData.missionObjectDestinations[currentLevelProgress];

            ProgressLevel();
    }
}
