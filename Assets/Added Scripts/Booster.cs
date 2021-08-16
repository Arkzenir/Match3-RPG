using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Booster : MonoBehaviour
{
    [SerializeReference][SerializeField]
    public List<Booster> intersectionVector;
    public abstract void BoosterEffect(int x, int y, Match3.GemGridPosition caller);

    public virtual void UseBooster(int x, int y, Match3.GemGridPosition caller)
    {
        if (caller.GetGemGrid().GetGem().type == GemSO.GemType.Standard)
            return;

        Booster usedEffect = this;
        
        if (intersectionVector.Count == 0)
        {
            usedEffect.BoosterEffect(x, y, caller);
            return;
        }

        List<Match3.GemGridPosition> nearbyGems = new List<Match3.GemGridPosition>();

        int xMax = Match3.instance.GetGridWidth();
        int yMax = Match3.instance.GetGridHeight();
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (x + i < 0 || x + i >= xMax || y + j < 0 || y + j >= yMax
                || (i == 0 && j == 0)) continue;
                if (Match3.instance.GetGridAtXY(x+i,y+j).HasGemGrid() 
                    && Match3.instance.GetGridAtXY(x+i,y+j).GetGemGrid().GetGem().type != GemSO.GemType.Standard)
                {
                    nearbyGems.Add(Match3.instance.GetGridAtXY(x+i,y+j));
                    Debug.Log("Gem added at x: " + (x+i) + "y: " + (y+j));
                }
            }
        }
        
        if (nearbyGems.Count == 0)
        {
            Debug.Log("Default use");
            usedEffect.BoosterEffect(x, y, caller);
            return;
        }
        
        
        //There is a single booster next to it
        if (nearbyGems.Count == 1)
        {
            Debug.Log("Intersect");
            usedEffect = intersectionVector[(int)nearbyGems[0].GetGemGrid().GetGem().type - 1];
            nearbyGems[0].DestroyGem();
        }
        //There are more than 1 booster in radius, making a total of 3
        else if (nearbyGems.Count > 1)
        {
            int larger = Int32.MinValue;
            int smaller = Int32.MinValue;
            
            int largerIndex = Int32.MinValue;
            int smallerIndex = Int32.MinValue;
            
            //Find max 2 values of GemType in list
            foreach (Match3.GemGridPosition gem in nearbyGems)
            {
                if ((int) gem.GetGemGrid().GetGem().type > larger)
                { 
                    smaller = larger;
                    larger = (int)gem.GetGemGrid().GetGem().type;
                    largerIndex = nearbyGems.IndexOf(gem);
                }
                else if ((int) gem.GetGemGrid().GetGem().type <= larger && (int) gem.GetGemGrid().GetGem().type >= smaller)
                {
                    smaller = (int)gem.GetGemGrid().GetGem().type;
                    smallerIndex = nearbyGems.IndexOf(gem);
                }
            }
            
            

            Debug.Log("Larger: " + larger);
            Debug.Log("Smaller: " + smaller);
            
            //Assign the intersection vector value to usedEffect
            usedEffect = nearbyGems[largerIndex].GetGemGrid().GetGem().booster.intersectionVector[(int)nearbyGems[smallerIndex].GetGemGrid().GetGem().type - 1];
            Debug.Log("Intersection booster assigned to max");
            if (larger > 0)
            {
                nearbyGems[largerIndex].DestroyGem();
            }
            if (smaller > 0)
            {
                nearbyGems[smallerIndex].DestroyGem();
            }

            //For the values not used in intersection vector, call booster effect on their location
            for (int i = 0; i < nearbyGems.Count; i++)
            {
                if (nearbyGems[i] != null && i != largerIndex && i != smallerIndex)
                {
                    BoosterEffect(nearbyGems[i].GetX(),nearbyGems[i].GetY(), caller);
                }            
            }
        }
        
        //Finally, call the booster combined booster effect if it has been assigned
        //(By default, this is a normal effect since it is assigned to "this")
        usedEffect.BoosterEffect(x,y,caller);

        
        //caller.DestroyGem();
        
    }
}
