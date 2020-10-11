using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakManager : MonoBehaviour
{
    public static BreakManager instance;

    public static Block[,] blocks;

    public Queue<Block> blocksToDestroy;
    Coroutine breakingCor;    
    void Awake()
    {
        instance = this;
    }

    public void InitBlocks(int row,int col)
    {
        blocks = new Block[row,col];
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
            Debug.Log(blocksToDestroy.Count);
            Block curBlock = blocksToDestroy.Dequeue();
            if (curBlock.Is(typeof(Mission)))
            {
                List<Block> neighbours = new List<Block>();
                Debug.Log(neighbours.Count);

                blocks[block.rowIndex, block.colIndex] = null;

                GetNeighbours(neighbours, curBlock, typeof(Center));
                Destroy(curBlock.gameObject);

                Debug.Log(neighbours.Count);
                List<List<Block>> blockListsToDestroy = new List<List<Block>>();
                foreach (var _block in neighbours)
                {
                    List<Block> blocksToDestroy = new List<Block>();
                    List<Block> centerBlocks = new List<Block>();

                    CheckNeighbours(_block, blocksToDestroy, centerBlocks);

                    if (centerBlocks.IsEmpty())
                    {
                        blockListsToDestroy.Add(blocksToDestroy);
                        Debug.Log("Destroy");
                    }

                }
                


                foreach(var list in blockListsToDestroy)
                {
                    //join same lists
                    StartCoroutine(BreakAsync(list));
                }
            }
            else
            {
                //Fail Level
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
            blocks[block.rowIndex, block.colIndex] = null;
            Destroy(block.gameObject);
            yield return null;
        }
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
        neighbours.SmartAdd(blocks[curBlock.rowIndex + 1, curBlock.colIndex], exclude);
        neighbours.SmartAdd(blocks[curBlock.rowIndex, curBlock.colIndex + 1], exclude);
        neighbours.SmartAdd(blocks[curBlock.rowIndex - 1, curBlock.colIndex], exclude);
        neighbours.SmartAdd(blocks[curBlock.rowIndex, curBlock.colIndex - 1], exclude);
    }
}


