using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class EnemySO : ScriptableObject
{
    public enum Type
    {
        AreaAttack,
        SingleAttack,
        Healer,
    }

    public Type enemyType;
    public int health;
    public int size;
    public Sprite sprite;

}
