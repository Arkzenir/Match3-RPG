using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeBomb : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();

        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if (x + i <= 0 || x + i >= xMax || y + j <= 0 || y + j >= yMax) continue;
                if (Match3.instance.GetGridAtXY(x+i,y+j).HasGemGrid())
                {
                    Match3.instance.GetGridAtXY(x + i,y + j).FlyGem();
                }
            }
        }
    }
}
