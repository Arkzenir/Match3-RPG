using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.IK;
using UnityEngine;

public class Heroes : MonoBehaviour
{
    public static Heroes instance;
    
    private Match3Visual visual;
    private Enemies enemies;
    private List<HeroSO> heroList;
    public Hero[] heroes;
    public const int HERO_COUNT = 5;

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
        enemies = Enemies.instance;
        visual = Match3Visual.instance;
        visual.OnStateChanged += Visual_OnOnStateChanged;
        
        heroes = new Hero[HERO_COUNT];

        heroList = Match3.instance.GetLevelSO().heroList;
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
                Hero add = new Hero(
                    heroList[i].health, heroList[i].associatedGem, heroList[i].skill, heroList[i].targets, heroList[i].maxCharge, heroList[i].chargeIncrease,
                    heroList[i].visualPrefab, heroList[i].sprite, (float)Match3.instance.GetGridWidth() / HERO_COUNT);
                heroes[i] = add;
            }
            else
                heroes[i] = null;
        }
        return true;
    }
    
    private void Visual_OnOnStateChanged(object sender, Match3Visual.OnStateChangedEventArgs e)
    {
        if (e.state == Match3Visual.State.WaitingForUser)
        {
            foreach (var hero in heroes)
            {
                hero.UpdateStatus();
            }
        }
    }

    private void Match3_OnOnGemGridPositionFly(object sender, Match3.OnNewGemGridPositionFlyEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        GemSO gemType = gemGridPosition.GetGemGrid().GetGem(); 

        foreach (var hero in heroes)
        {
            if (hero != null && hero.GetAssociatedGem() == gemType.color)
            {
                hero.ChargeSkill(1);
            }
        }
    }

    public class Hero : Entity
    {
        private GemSO.GemColor associatedGem;
        private int charge;
        private int maxCharge;
        private int chargeIncrease;
        private bool ready;
        
        public Hero(int maxHealth, GemSO.GemColor associatedGem, Skill skill, EntitySO.TargetTypes targetType, int maxCharge, int chargeIncrease, GameObject pf, Sprite sprite, float size)
            : base(maxHealth, skill, targetType, pf, sprite, size)
        {
            this.associatedGem = associatedGem;
            this.maxCharge = maxCharge;
            this.chargeIncrease = chargeIncrease;
            ready = false;
        }
        
        public bool IsReady()
        {
            return ready;
        }

        public override void UseSkill()
        {
            if (ready)
            {
                charge = 0;
                ready = false;
                float percentSkill = (float) charge / maxCharge;
                skillBar.transform.localScale = new Vector3(percentSkill, 1);
                base.UseSkill();
            }
        }

        public override void SkillSetup(Transform sBar, TextMeshPro sText)
        {
            skillBar = sBar;
            skillBarText = sText;
            skillBarText.text = charge + " / " + maxCharge;
            float percentSkill = (float) charge / maxCharge;
            skillBar.transform.localScale = new Vector3(percentSkill, 1);
        }

        public override void ChargeSkill(int times)
        {
            charge = charge + chargeIncrease * times;
            if (charge <= maxCharge)
            {
                skillBarText.text = charge + " / " + maxCharge;
                float percentSkill = (float) charge / maxCharge;
                skillBar.transform.localScale = new Vector3(percentSkill, 1);
            }
            
            if (charge >= maxCharge)
                ready = true;
        }
        
        public GemSO.GemColor GetAssociatedGem()
        {
            return associatedGem;
        }

        public override void Die()
        {
            for (int i = 0; i < instance.heroes.Length; i++)
            {
                if (instance.heroes[i] == this)
                {
                    instance.heroes[i] = null;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return "Type: " + associatedGem + "\n"
                   +"Health: " + Health + "\n"
                   +"Charge " + charge + "\n"
                   +"isDead: " + IsDead() + "\n";
        }

        
    }
}
