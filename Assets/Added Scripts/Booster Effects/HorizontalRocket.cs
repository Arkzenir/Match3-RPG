using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalRocket : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        for (int i = 0; i < Match3.instance.GetGridWidth(); i++)
        {
            if (i == x) continue;
            if (Match3.instance.GetGridAtXY(i,y).HasGemGrid())
            {
                Match3.instance.GetGridAtXY(i,y).FlyGem();
            }
        }
    }
}
