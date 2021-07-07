using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealRandom : Skill
{
    public int heal = 15;
    public override void Effect(Entity target)
    {
        target.GetHealed(heal);
    }
    
}
