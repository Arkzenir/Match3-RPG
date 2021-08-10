using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideFullStar : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();

        for (int k = -1; k < 1; k++)
        {
            for (int i = 0; i < xMax; i++)
            {
                if (i == x && k == 0) continue;
                if (Match3.instance.GetGridAtXY(i + k, y) != null && Match3.instance.GetGridAtXY(i + k, y).HasGemGrid())
                {
                    Match3.instance.GetGridAtXY(i + k, y).FlyGem();
                }
            }
        }

        for (int k = -1; k < 1; k++)
        {
            for (int i = 0; i < yMax; i++)
            {
                if (i == y && k == 0) continue;
                if (Match3.instance.GetGridAtXY(x, i + k) != null && Match3.instance.GetGridAtXY(x, i + k).HasGemGrid())
                {
                    Match3.instance.GetGridAtXY(x, i + k).FlyGem();
                }
            }
        }
    }
}
