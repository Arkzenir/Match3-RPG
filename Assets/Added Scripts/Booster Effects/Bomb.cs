using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                if (x + i < 0 || x + i >= xMax || y + j < 0 || y + j >= yMax) continue;
                if (Match3.instance.GetGridAtXY(x+i,y+j).HasGemGrid())
                {
                    Match3.instance.GetGridAtXY(x + i,y + j).FlyGem();
                }
            }
        }
    }
}
