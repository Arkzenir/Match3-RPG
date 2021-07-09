using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Entity
{
    //Visual Properties
    private float size;
    private Transform healthBar;
    private TextMeshPro healthBarText;
    protected Transform skillBar;
    protected TextMeshPro skillBarText;
    protected GameObject visual;
    
    private int maxHealth;
    private int health;
    private bool isDead;
    private int poisonCounter;
    private int poisonDamage;
    protected Skill skill;
    protected Sprite sprite;
    protected EntitySO.TargetTypes targetType;

    protected GameObject prefab;
    
    public Entity(int maxHealth, Skill skill, EntitySO.TargetTypes targetType, GameObject pf, Sprite sprite, float size)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
        this.skill = skill;
        this.targetType = targetType;
        isDead = false;
        poisonCounter = 0;
        poisonDamage = 0;
        prefab = pf;
        this.sprite = sprite;
        this.size = size;
    }

    public void HealthSetup(Transform hBar, TextMeshPro hText)
    {
        healthBar = hBar;
        healthBarText = hText;
        healthBarText.text = health + " / " + maxHealth;
    }

    public virtual void SkillSetup(Transform sBar, TextMeshPro sText) { }
    
    public GameObject GetPrefab() { return prefab;}
    public Sprite GetSprite() { return sprite;}
    public void SetVisual(GameObject v) { visual = v;}
    
    
    public void TakeDamage(int val)
    {
        if (val < health)
        {
            health -= val;
            healthBarText.text = health + " / " + maxHealth;
            float percentHealth = (float) health / maxHealth;
            healthBar.transform.localScale = new Vector3(percentHealth, 1);
        }
        else if (val >= health)
        {
            Die();
        }
    }

    public virtual void UseSkill()
    {
        skill.UseEffect(this, targetType);
    }
    
    public virtual void Die()
    {
        health = 0;
        healthBar.transform.localScale = new Vector3(health, 1);
        isDead = true;
    }

    public void GetHealed(int val)
    {
        health += val;
        if (health > maxHealth)
            health = maxHealth;
        
        healthBarText.text = health + " / " + maxHealth;
        float percentHealth = (float) health / maxHealth;
        healthBar.transform.localScale = new Vector3(percentHealth, 1);
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

    public virtual void ChargeSkill(int times) {}

    public int Health
    {
        get => health;
        set => health = value;
    }
    
    public float Size
    {
        get => size;
        set => size = value;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
