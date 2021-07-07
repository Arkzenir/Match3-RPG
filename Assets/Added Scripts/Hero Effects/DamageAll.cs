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
        foreach (var enemy in Enemies.instance.activeEnemies)
        {
            if (enemy != null)
                Effect(enemy);
        }
    }
}
