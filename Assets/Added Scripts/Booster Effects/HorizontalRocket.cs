using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalRocket : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        Debug.Log("H.Rocket used at x: " + x + " y: " + y );
        for (int i = 0; i < Match3.instance.GetGridWidth(); i++)
        {
            if (i == x) continue;
            if (Match3.instance.GetGridAtXY(i,y).HasGemGrid())
            {
                Debug.Log("Exists at x: " + i + " y: " + y );
                Match3.instance.GetGridAtXY(i,y).FlyGem();
            }
        }
    }
}
