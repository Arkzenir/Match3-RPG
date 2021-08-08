using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        Debug.Log("Crystal used at x: " + x + " y: " + y );
        
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();
        
        for (int i = 0; i < xMax; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                if (i == x && j == y) continue;
                if ( Match3.instance.GetGridAtXY(x+i,y+j) != null && Match3.instance.GetGridAtXY(x+i,y+j).HasGemGrid())
                    //&& Match3.instance.GetGridAtXY(x+i,y+j).GetGemGrid().GetGem().color == caller.GetGemGrid().GetGem().color)
                {
                    Debug.Log("Exists at x: " + x + " y: " + y );
                    Match3.instance.GetGridAtXY(x + i,y + j).FlyGem();
                }
            }
        }
    }
}
