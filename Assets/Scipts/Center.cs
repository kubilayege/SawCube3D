using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : Block
{
    //Called from trap Object which collided with this block
    public override void Break()
    {
        BreakManager.instance.ProcessBlock(this);
        //StopGamePlay
    }
}
