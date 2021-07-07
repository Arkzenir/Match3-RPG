using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public abstract void Effect(Entity target);

    public virtual void UseEffect(Entity caller, EntitySO.TargetTypes target)
    {

        var t = caller;

        switch (target)
        {
            case EntitySO.TargetTypes.Self when caller is Enemies.Enemy:
                t = Enemies.instance.activeEnemies[Random.Range(0, Enemies.instance.activeEnemies.Count)];
                break;
            case EntitySO.TargetTypes.Self when caller is Heroes.Hero:
                t = Heroes.instance.heroes[Random.Range(0, Heroes.instance.heroes.Count)];
                break;
            case EntitySO.TargetTypes.Other when caller is Enemies.Enemy:
                t = Heroes.instance.heroes[Random.Range(0, Heroes.instance.heroes.Count)];
                break;
            case EntitySO.TargetTypes.Other when caller is Heroes.Hero:
                t = Enemies.instance.activeEnemies[Random.Range(0, Enemies.instance.activeEnemies.Count)];
                break;
        }

        Effect(t);
    }
    
}
