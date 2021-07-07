using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public static Enemies instance;
    
    private Match3Visual visual;
    private List<EnemySO> enemyList;
    public const int MAX_ENEMY_AMOUNT = 5;
    //Reference by position to enemies, remember to update on enemy death
    public Enemy[] ePosArr;
    public List<Enemy> activeEnemies;

    private void Awake()
    {
        if (instance == null)
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
    }

    private void Visual_OnOnStateChanged(object sender, EventArgs e)
    {
        
    }

    private void Match3_OnOnGemGridPositionFly(object sender, Match3.OnNewGemGridPositionFlyEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        
//TODO: Link this to UI
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
        
        for (int i = 0; i < ePosArr.Length; i++)
        {
            if (enemyList[i] != null )
            {
                Enemy add = new Enemy (enemyList[i].health, enemyList[i].skill, enemyList[i].targets);
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

    public bool enemyTakeDamage(int pos, int dmg)
    {
        if (ePosArr[pos] != null)
        {
            return ePosArr[pos].TakeDamage(dmg);
        }
        else
            return false;
    }
    
    public class Enemy : Entity
    {
        public Enemy(int maxHealth, Skill skill, EntitySO.TargetTypes targetType) : base(maxHealth, skill, targetType) {}

        public void Attack()
        {
            skill.UseEffect(this, targetType);
        }

        public override void Die()
        {
            for (int i = 0; i < instance.ePosArr.Length; i++)
            {
                if (this == instance.ePosArr[i])
                {
                    instance.ePosArr[i] = null;
                }
            }

            instance.activeEnemies.Remove(this);
        }
        
        
        public override string ToString()
        {
            return "Type: " + skill + "\n"
                    +"Health: " + Health + "\n"
                    +"isDead: " + IsDead() + "\n";
        }



    }
    
}
