using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakManager : MonoBehaviour
{
    public static BreakManager instance;

    private static int _row;
    private static int _col;
    public static Block[,] blocks;
    public int missionCount;
    public Queue<Block> blocksToDestroy;


    Coroutine breakingCor;    
    void Awake()
    {
        instance = this;
    }

    public void InitBlocks(int row,int col)
    {
        if(breakingCor!=null)
            StopCoroutine(breakingCor);
        missionCount = 0;
        blocks = new Block[row,col];
        _row = row;
        _col = col;
    }

    public void ProcessBlock(Block block)
    {

        if (blocksToDestroy?.Count == 0)
            breakingCor = null;

        if (breakingCor == null)
            breakingCor = StartCoroutine(BreakingCor(block));
        else
            blocksToDestroy.Enqueue(block);

    }

    private IEnumerator BreakingCor(Block block)
    {
        blocksToDestroy = new Queue<Block>();
        blocksToDestroy.Enqueue(block);
        while (blocksToDestroy.Count > 0)
        {
            Block curBlock = blocksToDestroy.Dequeue();
            if (curBlock.Is(typeof(Mission)))
            {
                List<Block> neighbours = new List<Block>();
                blocks[block.rowIndex, block.colIndex] = null;

                GetNeighbours(neighbours, curBlock, typeof(Center));
                Destroy(curBlock.gameObject);

                CountMissionBlocks();

                List<List<Block>> blockListsToDestroy = new List<List<Block>>();
                foreach (var _block in neighbours)
                {
                    List<Block> blocksToDestroy = new List<Block>();
                    List<Block> centerBlocks = new List<Block>();

                    CheckNeighbours(_block, blocksToDestroy, centerBlocks);

                    if (centerBlocks.IsEmpty())
                    {
                        blockListsToDestroy.Add(blocksToDestroy);
                    }
                }


                var blocks2Destroy = new List<Block>();

                foreach(var list in blockListsToDestroy)
                {
                    blocks2Destroy.Join(list);
                    //join same lists
                }
                StartCoroutine(BreakAsync(blocks2Destroy));
            }
            else
            {
                //Fail Level
                LevelManager.instance.ChangeLevel(NextLevelState.Restart);
            }
            yield return null;
        }
    }

    private IEnumerator BreakAsync(List<Block> list)
    {
        foreach(var block in list)
        {
            if (block == null)
                continue;

            if(blocks[block.rowIndex, block.colIndex] != null)
            {
                Destroy(blocks[block.rowIndex, block.colIndex]) ;
                Destroy(block.gameObject);
            }
            blocks[block.rowIndex, block.colIndex] = null;

            yield return null;
        }

        CountMissionBlocks();
    }

    private void CountMissionBlocks()
    {
        int currentBlockCount=0;
        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _col; j++)
            {
                if (blocks[i, j] != null && blocks[i, j].Is(typeof(Mission)))
                    currentBlockCount++;
            }
        }

        if (currentBlockCount == 0)
            LevelManager.instance.ProgressLevel();

    }

    private static bool CheckNeighbours(Block currentBlock, List<Block> checkedSoFar, List<Block> centerBlocks)
    {
        if (currentBlock.Is(typeof(Mission)))
            checkedSoFar.SmartAdd(currentBlock);
        else
        {
            centerBlocks.SmartAdd(currentBlock);
            if (checkedSoFar.Count == 0)
                return false;
        }

        var currentMissionNeighbours = new List<Block>();
        GetNeighbours(currentMissionNeighbours,currentBlock, typeof(Center));
        var currentCenterNeighbours = new List<Block>();
        GetNeighbours(currentCenterNeighbours,currentBlock, typeof(Mission));



        if (currentCenterNeighbours.Count > 0)
        {
            foreach (var block in currentCenterNeighbours)
            {
                centerBlocks.SmartAdd(block);
            }
            return false;
        }

        if (currentMissionNeighbours.IsEmpty())
            return true;


        var nextNeighbours = new List<Block>();
        foreach(var block in currentMissionNeighbours)
        {
            if (checkedSoFar.SmartAdd(block))
            { nextNeighbours.Add(block);
                //Debug.Log(block.rowIndex + "," + block.colIndex);
            }
        }


        bool stop = true ;
        foreach (var block in nextNeighbours)
        {
            stop = CheckNeighbours(block, checkedSoFar, centerBlocks);
            if (!stop)
                break;
            else
                continue;
        }
        if (!stop)
            return false;
        return true;

    }

    private static void GetNeighbours(List<Block> neighbours, Block curBlock,Type exclude)
    {
        List<Block> currentList = new List<Block>();
        if(_row > curBlock.rowIndex + 1)
            neighbours.SmartAdd(blocks?[curBlock.rowIndex + 1, curBlock.colIndex], exclude);

        if (curBlock.rowIndex - 1 >= 0)
            neighbours.SmartAdd(blocks?[curBlock.rowIndex - 1, curBlock.colIndex], exclude);

        if(_col > curBlock.colIndex + 1)
            neighbours.SmartAdd(blocks?[curBlock.rowIndex, curBlock.colIndex + 1], exclude);

        if (curBlock.colIndex - 1 >= 0)
            neighbours.SmartAdd(blocks?[curBlock.rowIndex, curBlock.colIndex - 1], exclude);
    }
}


