using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : Block
{
    public override void Break()
    {
        BreakManager.instance.ProcessBlock(this);
    }
}
