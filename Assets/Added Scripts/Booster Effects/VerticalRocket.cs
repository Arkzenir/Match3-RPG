using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalRocket : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        for (int i = 0; i < Match3.instance.GetGridHeight(); i++)
        {
            if (Match3.instance.GetGridAtXY(x,i).HasGemGrid())
            {
                Match3.instance.GetGridAtXY(x,i).FlyGem();
            }
        }
    }
}
