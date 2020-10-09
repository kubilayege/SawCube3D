using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    public int rowIndex;
    public int colIndex;

    public void Init(int _row, int _col)
    {
        rowIndex = _row;
        colIndex = _col;
        BreakManager.blocks[rowIndex, colIndex] = this;
    }

    public abstract void Break();
}
