using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAll : Skill
{
    public int heal = 15;
    public override void Effect(Entity target)
    {
        target.GetHealed(heal);
    }

    public override void UseEffect(Entity target, EntitySO.TargetTypes targetType)
    {
        foreach (var h in Heroes.instance.heroes)
        {
            if (h != null)
                Effect(h);
        }
    }
}
