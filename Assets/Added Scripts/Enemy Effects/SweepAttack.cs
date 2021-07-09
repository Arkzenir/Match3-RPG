using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepAttack : Skill
{
    public int damage = 10;
    public override void Effect(Entity target)
    {
        target.TakeDamage(damage);
    }

    public override void UseEffect(Entity target, EntitySO.TargetTypes targetType)
    {
        for (int i = 0; i < Heroes.instance.heroes.Length; i++)
        {
            if (Heroes.instance.heroes[i] != null)
            {
                Effect(Heroes.instance.heroes[i]);
            }
        }
    }
}
