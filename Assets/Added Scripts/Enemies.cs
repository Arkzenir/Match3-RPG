using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public static Enemies instance;
    private Match3Visual visual;
    private int eIndex = 0;
    public List<EnemySO> enemyList;
    public const int MAX_ENEMY_AMOUNT = 5;
    //Reference by position to enemies, remember to update on enemy death
    public Enemy[] ePosArr;
    public List<Enemy> activeEnemies;

    
    
    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            DestroyImmediate(this);
    }


    void Start()
    {
        Match3.instance.OnGemGridPositionFly += Match3_OnOnGemGridPositionFly;
        Match3Visual.instance.OnStateChanged += Visual_OnOnStateChanged;
        
        ePosArr = new Enemy[Match3.instance.GetGridWidth()];
        enemyList = Match3.instance.GetLevelSO().enemyList;
        activeEnemies = new List<Enemy>();
        if (!SetUpArray())
            Debug.Log("Enemy set up failed at array stage");
        else
            EntityVisual.instance.SetUpEnemyVisuals();
    }

    private void Visual_OnOnStateChanged(object sender, Match3Visual.OnStateChangedEventArgs e)
    {
        if (e.state == Match3Visual.State.WaitingForUser)
        {
            foreach (var enemy in activeEnemies)
            {
                enemy.UpdateStatus();
            }
        }
        
        if (e.state == Match3Visual.State.EnemyTurn)
        {
            eIndex++;
            if (eIndex >= activeEnemies.Count)
                eIndex = 0;
            Debug.Log("eIndex: " + eIndex);
            activeEnemies[eIndex].UseSkill();
            
        }
    }

    private void Match3_OnOnGemGridPositionFly(object sender, Match3.OnNewGemGridPositionFlyEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        
        if (ePosArr[e.x] != null)
        {
            enemyTakeDamage(e.x, e.gemType.damage);
        }
    }
    

    private bool SetUpArray()
    {
        int maxSize = Match3.instance.GetGridWidth();
        int tempSize = 0;
        int tempCount = 0;

        foreach (var enemySo in enemyList)
        {
            if (enemySo != null)
            {
                tempSize += enemySo.size;
                tempCount++;
            }
        }

        if (tempSize > maxSize || tempCount > MAX_ENEMY_AMOUNT)
            return false;
        
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] != null )
            {
                Enemy add = new Enemy (enemyList[i].health, enemyList[i].skill, enemyList[i].targets, enemyList[i].visualPrefab, enemyList[i].sprite, enemyList[i].size);
                activeEnemies.Add(add);
                if (enemyList[i].size > 0)
                {
                    for (int j = 0; j < enemyList[i].size; j++)
                    {
                        ePosArr[i + j] = add;
                    }
                }
                else
                    return false;
            }
        }
        return true;
    }

    public void enemyTakeDamage(int pos, int dmg)
    {
        if (ePosArr[pos] != null)
        {
            ePosArr[pos].TakeDamage(dmg);
        }
        
    }
    
    public class Enemy : Entity
    {
        public Enemy(int maxHealth, Skill skill, EntitySO.TargetTypes targetType, GameObject pf, Sprite sprite, float size) : base(maxHealth, skill, targetType,  pf, sprite, size) {}
        
        public override void Die()
        {
            for (int i = 0; i < instance.ePosArr.Length; i++)
            {
                if (this == instance.ePosArr[i])
                {
                    instance.ePosArr[i] = null;
                }
            }
            base.Die();
            visual.SetActive(false);
            instance.activeEnemies.Remove(this);
        }
        
        public override string ToString()
        {
            return "Skill: " + skill.name + "\n"
                    +"Health: " + Health + "\n"
                    +"isDead: " + IsDead() + "\n";
        }



    }
    
}
