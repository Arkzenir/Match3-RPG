using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmuteToRockets : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();

        VerticalRocket v = GetComponent<VerticalRocket>();
        HorizontalRocket h = GetComponent<HorizontalRocket>();

        for (int i = 0; i < xMax ; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                if (Match3.instance.GetGridAtXY(i,j).HasGemGrid() 
                    && Match3.instance.GetGridAtXY(i,j).GetGemGrid().GetGem().color == caller.GetGemGrid().GetGem().color)
                {
                    if (Random.Range(0,1) == 0)
                    {
                        h.BoosterEffect(i,j,caller);
                    }
                    else
                    {
                        v.BoosterEffect(i,j,caller);
                    }
                    
                    //TODO: Change the sprite

                    //TODO: Add delay? (Might be better in the tween animation phase)
                }
            }
        }
    }
}
