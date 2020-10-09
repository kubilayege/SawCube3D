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
                List<Block> neighbours = new List<Block>();
                GetNeighbours(curBlock, neighbours);
                Destroy(curBlock.gameObject);
                
                //Check neighbours for possible splitup
                //Destroy neighbours and its connecting blocks neighbours
            }
            else
            {
                //Fail Level
            }
            yield return null;
        }
    }

    private static void GetNeighbours(Block curBlock, List<Block> neighbours)
    {
        neighbours.SmartAdd(blocks[curBlock.rowIndex + 1, curBlock.colIndex]);
        neighbours.SmartAdd(blocks[curBlock.rowIndex, curBlock.colIndex + 1]);
        neighbours.SmartAdd(blocks[curBlock.rowIndex - 1, curBlock.colIndex]);
        neighbours.SmartAdd(blocks[curBlock.rowIndex, curBlock.colIndex - 1]);
    }
}


