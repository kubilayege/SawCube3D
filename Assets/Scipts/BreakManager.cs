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
                List<Block> neighbours = GetNeighbours(curBlock,typeof(Center));
                
                Destroy(curBlock.gameObject);
                List<List<Block>> blockListsToDestroy = new List<List<Block>>();
                foreach (var _block in neighbours)
                {
                    List<Block> blocksToDestroy = new List<Block>();
                    List<Block> centerBlocks = new List<Block>();

                    CheckNeighbours(_block, blocksToDestroy, centerBlocks);

                    if (centerBlocks.IsEmpty())
                        blockListsToDestroy.Add(blocksToDestroy);

                }
                
                foreach(var list in blockListsToDestroy)
                {
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
            blocks[block.rowIndex, block.colIndex] = null;
            Destroy(block.gameObject);
            yield return null;
        }
    }

    private static void CheckNeighbours(Block currentBlock, List<Block> checkedSoFar, List<Block> centerBlocks)
    {
        if (currentBlock.Is(typeof(Mission)))
            checkedSoFar.Add(currentBlock);
        else
        {
            centerBlocks.Add(currentBlock);
            if (checkedSoFar.Count == 0)
                return;
        }

        var currentMissionNeighbours = GetNeighbours(currentBlock, typeof(Center));
        var currentCenterNeighbours = GetNeighbours(currentBlock, typeof(Mission));
        

        if(currentCenterNeighbours.Count > 0)
        {
            foreach (var block in currentCenterNeighbours)
            {
                centerBlocks.SmartAdd(block);
            }
            return;
        }

        if (currentMissionNeighbours.IsEmpty())
            return;

        foreach(var block in currentMissionNeighbours)
        {
            checkedSoFar.SmartAdd(block);
        }

        foreach (var block in currentMissionNeighbours)
        {
            CheckNeighbours(block, checkedSoFar, centerBlocks);
        }

    }

    private static List<Block> GetNeighbours(Block curBlock,Type exclude)
    {
        List<Block> neighbours = new List<Block>();
        neighbours.SmartAdd(blocks[curBlock.rowIndex + 1, curBlock.colIndex], exclude);
        neighbours.SmartAdd(blocks[curBlock.rowIndex, curBlock.colIndex + 1], exclude);
        neighbours.SmartAdd(blocks[curBlock.rowIndex - 1, curBlock.colIndex], exclude);
        neighbours.SmartAdd(blocks[curBlock.rowIndex, curBlock.colIndex - 1], exclude);
        return neighbours;
    }
}


