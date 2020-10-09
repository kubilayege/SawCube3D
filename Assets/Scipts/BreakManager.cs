using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakManager : MonoBehaviour
{
    public static BreakManager instance;
    Queue<Block> blocksToDestroy;
    Coroutine breakingCor;    
    
    void Awake()
    {
        instance = this;
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
                //Find curBlocks neighbors
                //Destroy curBlock
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
}


