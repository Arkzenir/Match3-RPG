using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRandom : Skill
{
    public int damage = 25;
    public override void Effect(Entity target)
    {
        target.TakeDamage(damage);
    }
}
