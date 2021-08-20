using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearGrid : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();

        for (int i = 0; i < xMax ; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                if (Match3.instance.GetGridAtXY(i,j).HasGemGrid())
                {
                    Match3.instance.GetGridAtXY(i,j).FlyGem();
                }
            }
        }
    }
}
