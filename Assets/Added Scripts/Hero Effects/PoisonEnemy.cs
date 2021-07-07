using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEnemy : Skill
{
    public int damage = 5;
    public int rounds = 3;
    public override void Effect(Entity target)
    {
        target.GetPoisoned(rounds,damage);
    }
}
