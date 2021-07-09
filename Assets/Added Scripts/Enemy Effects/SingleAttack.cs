using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAttack : Skill
{
    public int damage = 20;
    public override void Effect(Entity target)
    {
        target.TakeDamage(damage);
    }
}
