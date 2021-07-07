using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepAttack : Skill
{
    public int damage = 10;
    public override void Effect(Entity target)
    {
        target.Health -= damage;
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
