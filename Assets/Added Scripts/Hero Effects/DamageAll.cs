using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAll : Skill
{
    public int damage = 10;
    public override void Effect(Entity target)
    {
        target.TakeDamage(damage);
    }
    public override void UseEffect(Entity target, EntitySO.TargetTypes targetType)
    {
        for (int i = 0; i < Enemies.instance.activeEnemies.Count; i++)
        {
            if (Enemies.instance.activeEnemies[i] != null)
            {
                Effect(Enemies.instance.activeEnemies[i]);
            }
        }
    }
}
