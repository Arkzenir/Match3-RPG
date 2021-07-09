using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

public class EntityVisual : MonoBehaviour
{
    public static EntityVisual instance;
    public Grid<EntityGridObject> enemyGrid;
    public Grid<EntityGridObject> heroGrid;
    public const int ENEMY_GRID_OFFSET = 1;
    public const int HERO_GRID_OFFSET = -8;
    
    private int touchX;
    private int touchY;
    private Vector3 MouseWorldPosition;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(this);
    }

    private void Start()
    {
        StartCoroutine(LateStart(0.5f));
    }
    
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SetUpHeroVisuals();
        SetUpEnemyVisuals();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            mouseWorldPosition.y -= heroGrid.GetCellSize();
            heroGrid.GetXY(mouseWorldPosition, out touchX, out touchY);
            heroGrid.GetGridObject(touchX, touchY).Entity.UseSkill();
            Match3.instance.TryIsGameOver();
        }
    }


    private void SetUpEnemyVisuals()
    {
        enemyGrid = new Grid<EntityGridObject>(
            Match3.instance.GetGridWidth(), 1, 1f, new Vector3(0, Match3.instance.GetGridHeight() + ENEMY_GRID_OFFSET),
            (Grid<EntityGridObject>g, int x, int y) => new EntityGridObject(g,x,y) );

        int offset = 0;
        
        var eList = Enemies.instance.ePosArr;
        for (int i = 0; i < eList.Length; i++)
        {
            if (eList[i] != null)
            {
                EntityGridObject obj = enemyGrid.GetGridObject(i, 0);
                obj.SetEntity(eList[i]);
                Vector3 pos = enemyGrid.GetWorldPosition(i, 0);
                
                GameObject gObj = Instantiate(obj.Entity.GetPrefab(), pos, Quaternion.identity);
                obj.Entity.SetVisual(gObj);
                gObj.transform.localScale *= obj.Entity.Size;
                
                gObj.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = obj.Entity.GetSprite();
                
                Transform healthBar = gObj.transform.Find("Healthbar").transform.Find("Bar");
                Transform hBar = gObj.transform.Find("Healthbar");
                hBar.transform.localPosition = new Vector3( hBar.transform.localPosition.x / 2, hBar.transform.localPosition.y);
                TextMeshPro healthText = hBar.Find("HealthText").GetComponent<TextMeshPro>();
                obj.Entity.HealthSetup(healthBar, healthText);
                
                if (eList[i].Size > 1)
                {
                    offset = (int)eList[i].Size;
                }
                else
                {
                    offset = 0;
                }

                i += offset;
                
                
            }
        }
    }

    private void SetUpHeroVisuals()
    {
        heroGrid = new Grid<EntityGridObject>(
            Match3.instance.GetGridWidth(), 1, (float) Match3.instance.GetGridWidth() / Heroes.HERO_COUNT, new Vector3(0, Match3.instance.GetGridHeight() + HERO_GRID_OFFSET),
            (Grid<EntityGridObject>g, int x, int y) => new EntityGridObject(g,x,y) );

        var hList = Heroes.instance.heroes;
        for (int i = 0; i < hList.Length; i++)
        {
            if (hList[i] != null)
            {
                EntityGridObject obj = heroGrid.GetGridObject(i, 0);
                obj.SetEntity(hList[i]);
                Vector3 pos = heroGrid.GetWorldPosition(i, 0);
                
                GameObject gObj = Instantiate(obj.Entity.GetPrefab(), pos, Quaternion.identity);
                gObj.transform.localScale *= obj.Entity.Size;
                
                gObj.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = obj.Entity.GetSprite();
                
                Transform healthBar = gObj.transform.Find("Healthbar").transform.Find("Bar");
                Transform hBar = gObj.transform.Find("Healthbar");
                Transform skillBar = gObj.transform.Find("Skill").transform.Find("Bar");
                Transform sBar = gObj.transform.Find("Skill");
                hBar.transform.localPosition = new Vector3( hBar.transform.localPosition.x / 2, hBar.transform.localPosition.y);
                sBar.transform.localPosition = new Vector3( sBar.transform.localPosition.x / 2, sBar.transform.localPosition.y);
                TextMeshPro healthText = hBar.Find("HealthText").GetComponent<TextMeshPro>();
                TextMeshPro skillText = sBar.Find("SkillText").GetComponent<TextMeshPro>();
                obj.Entity.HealthSetup(healthBar, healthText);
                obj.Entity.SkillSetup(skillBar, skillText);
            }
        }
    }
    
    
    public class EntityGridObject
    {
        private Entity entity;
        public Grid<EntityGridObject> grid;
        public int x;
        public int y;


        public EntityGridObject(Grid<EntityGridObject> g, int x, int y)
        {
            grid = g;
            this.x = x;
            this.y = y;
        }

        public void SetEntity(Entity e) { entity = e; }
        
        public Vector3 GetWorldPosition() {
            return new Vector3(x, y);
        }
        
        public Entity Entity
        {
            get => entity;
            set => entity = value;
        }

        public override string ToString()
        {
            return "X: " + x + "Y: " + y;
        }
    }
}
