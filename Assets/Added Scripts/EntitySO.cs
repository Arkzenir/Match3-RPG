using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySO : ScriptableObject
{
    public enum TargetTypes
    {
        Self,
        Other,
    }
    
    public Skill skill;
    public TargetTypes targets;
    public int health;

}
