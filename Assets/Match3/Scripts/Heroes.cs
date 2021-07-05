using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.IK;
using UnityEngine;

public class Heroes : MonoBehaviour
{
    private Match3 match3;
    private Match3Visual visual;
    private Enemies enemies;
    private List<HeroSO> heroList;
    public Hero[] heroes;
    public const int HERO_COUNT = 5;
    void Start()
    {
        match3 = GameObject.FindWithTag("match3").GetComponent<Match3>();
        match3.OnGemGridPositionFly += Match3_OnOnGemGridPositionFly;
        enemies = GameObject.FindWithTag("enemies").GetComponent<Enemies>();
        visual = GameObject.FindWithTag("visual").GetComponent<Match3Visual>();
        visual.OnStateChanged += Visual_OnOnStateChanged;
        
        heroes = new Hero[HERO_COUNT];

        heroList = match3.GetLevelSO().heroList;
        if (!SetUpArray())
            Debug.Log("Hero set up failed at array stage");

    }

    private bool SetUpArray()
    {
        if (heroList.Count > HERO_COUNT)
            return false;
        
        for (int i = 0; i < heroList.Count; i++)
        {
            if (heroList[i] != null)
            {
                Hero add = new Hero(heroList[i].health, heroList[i].associatedGem, heroList[i].maxCharge, heroList[i].chargeIncrease);
                heroes[i] = add;
            }
            else
                heroes[i] = null;
        }

        return true;
    }
    
    private void Visual_OnOnStateChanged(object sender, EventArgs e)
    {
        //For when state = WaitingForUser
    }

    private void Match3_OnOnGemGridPositionFly(object sender, Match3.OnNewGemGridPositionFlyEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        GemSO gemType = gemGridPosition.GetGemGrid().GetGem(); 

        foreach (var hero in heroes)
        {
            if (hero != null && hero.getAssociatedGem() == gemType.color)
            {
                hero.chargeSkill(1);
            }
        }
    }

    public class Hero
    {
        private int health;
        private GemSO.GemColor associatedGem;
        private int charge;
        private int maxCharge;
        private int chargeIncrease;
        private bool isDead;
        private bool ready;


        public Hero(int health, GemSO.GemColor associatedGem, int maxCharge, int chargeIncrease)
        {
            this.health = health;
            this.associatedGem = associatedGem;
            this.maxCharge = maxCharge;
            this.chargeIncrease = chargeIncrease;
            isDead = false;
            ready = false;
        }

        public bool isReady()
        {
            return ready;
        }

        public bool useAbility()
        {
            if (!ready)
                return false;

            charge = 0;
            ready = false;
            //TODO: Add function/event call to ability by gem color
            
            return true;
        }

        public void chargeSkill(int times)
        {
            charge = charge + chargeIncrease * times;

            if (charge >= maxCharge)
                ready = true;
        }
        
        public int Health
        {
            get => health;
            set => health = value;
        }
        
        public GemSO.GemColor getAssociatedGem()
        {
            return associatedGem;
        }

        public bool IsDead
        {
            get => isDead;
            set => isDead = value;
        }

        public override string ToString()
        {
            return "Type: " + associatedGem + "\n"
                   +"Health: " + health + "\n"
                   +"Charge " + charge + "\n"
                   +"isDead: " + isDead + "\n";
        }
        
    }
}
