using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    private int maxHealth;
    private int health;
    private bool isDead;
    private int poisonCounter;
    private int poisonDamage;
    protected Skill skill;
    protected EntitySO.TargetTypes targetType;
    
    
    public Entity(int maxHealth, Skill skill, EntitySO.TargetTypes targetType)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
        this.skill = skill;
        this.targetType = targetType;
        isDead = false;
        poisonCounter = 0;
        poisonDamage = 0;
    }

    public bool TakeDamage(int val)
    {
        if (val < health)
        {
            health -= val;
            return true;
        }
        else
        {
            Die();
            return false;
        }
    }

    public virtual void Die()
    {
        health = 0;
        isDead = true;
    }

    public void GetHealed(int val)
    {
        health += val;
        if (health > maxHealth)
            health = maxHealth;
    }
    
    public void GetPoisoned(int rounds, int damage)
    {
        poisonCounter = rounds;
        poisonDamage = damage;
    }

    public void UpdateStatus()
    {
        if (poisonCounter > 0)
        {
            poisonCounter--;
            TakeDamage(poisonDamage);
        }
    }

    public int Health
    {
        get => health;
        set => health = value;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
}
