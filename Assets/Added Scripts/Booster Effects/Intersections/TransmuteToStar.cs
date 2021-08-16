using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmuteToStar : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();

        Star s = GetComponent<Star>();
        
        for (int i = 0; i < xMax ; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                if (Match3.instance.GetGridAtXY(i,j).HasGemGrid() 
                    && Match3.instance.GetGridAtXY(i,j).GetGemGrid().GetGem().color == caller.GetGemGrid().GetGem().color)
                {
                    s.BoosterEffect(i,j,caller);
                    
                    //TODO: Change the sprite
                    
                    //TODO: Add delay? (Might be better in the tween animation phase)
                }
            }
        }
    }
}
