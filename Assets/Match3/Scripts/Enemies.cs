using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    private Match3 match3;
    private Match3Visual visual;
    private List<EnemySO> enemyList;
    public const int MAX_ENEMY_AMOUNT = 5;
    
    //Reference by position to enemies, remember to update on enemy death
    public Enemy[] ePosArr;
    void Start()
    {
        match3 = GameObject.FindWithTag("match3").GetComponent<Match3>();
        match3.OnGemGridPositionFly += Match3_OnOnGemGridPositionFly;

        visual = GameObject.FindWithTag("visual").GetComponent<Match3Visual>();
        visual.OnStateChanged += Visual_OnOnStateChanged;
        
        ePosArr = new Enemy[match3.GetGridWidth()];

        enemyList = match3.GetLevelSO().enemyList;
        
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
        int maxSize = match3.GetGridWidth();
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
        
        Debug.Log(tempCount);
        Debug.Log(tempSize);
        Debug.Log(ePosArr.Length);
        
        if (tempSize > maxSize || tempCount > MAX_ENEMY_AMOUNT)
            return false;
        
        for (int i = 0; i < ePosArr.Length; i++)
        {
            if (enemyList[i] != null )
            {
                Enemy add = new Enemy {Health = enemyList[i].health, AttackType = enemyList[i].enemyType};

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
            
            Debug.Log(i);
        }
        
        return true;
    }

    public bool enemyTakeDamage(int pos, int dmg)
    {
        if (ePosArr[pos] != null)
        {
            return ePosArr[pos].takeDamage(dmg);
        }
        else
            return false;
    }
    
    public class Enemy
    {
            private int health;
            //Action on attack
            private EnemySO.Type attackType;

            private bool isDead = false;
            private int poisonCounter;

        public bool IsDead()
        {
            return isDead;
        }

        public void getPoisoned(int rounds)
        {
            poisonCounter = rounds;
        }

        public bool takeDamage(int val)
        {
            if (val < health)
            {
                health -= val;
                return true;
            }
            else
            {
                isDead = true;
                return false;
            }
        }

        public int Health
        {
            get => health;
            set => health = value;
        }

        public EnemySO.Type AttackType
        {
            get => attackType;
            set => attackType = value;
        }

        public override string ToString()
        {
            return "Type: " + attackType + "\n"
                    +"Health: " + health + "\n"
                    +"isDead: " + isDead + "\n";
        }
    }
    
}
