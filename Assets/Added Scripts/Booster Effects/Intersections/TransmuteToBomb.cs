using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmuteToBomb : Booster
{
    public override void BoosterEffect(int x, int y, Match3.GemGridPosition caller)
    {
        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();

        for (int i = 0; i < xMax ; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                if (Match3.instance.GetGridAtXY(i,j).HasGemGrid() 
                && Match3.instance.GetGridAtXY(i,j).GetGemGrid().GetGem().color == caller.GetGemGrid().GetGem().color)
                {
                    GemSO subject = Match3.instance.GetGridAtXY(i, j).GetGemGrid().GetGem();
                    subject.type = GemSO.GemType.Bomb;
                    subject.booster = new Bomb();
                        
                    //TODO: Change the sprite
                    
                    //TODO: Add delay? (Might be better in the tween animation phase)
                    Match3.instance.GetGridAtXY(i,j).FlyAndBoostGem();
                }
            }
        }
    }
}
