using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullStar : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();

        for (int i = 0; i < xMax ; i++)
        {
            if (Match3.instance.GetGridAtXY(i,y).HasGemGrid())
            {
                Match3.instance.GetGridAtXY(i,y).FlyGem();
            }
        }

        for (int i = 0; i < yMax; i++)
        {
            if (Match3.instance.GetGridAtXY(x,i).HasGemGrid())
            {
                Match3.instance.GetGridAtXY(x,i).FlyGem();
            }
        }
    }
}
