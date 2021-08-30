using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using DG.Tweening;
using UnityEditorInternal;
using Random = System.Random;

/*
 * Visual Representation of the underlying Match3 Grid
 * */
public class Match3Visual : MonoBehaviour
{

    public static Match3Visual instance;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Busy,
        WaitingForUser,
        TryFindMatches,
        EnemyTurn,
        GameOver,
    }

    [SerializeField] private Transform pfBackgroundGridVisual;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Match3 match3;

    private Grid<Match3.GemGridPosition> grid;

    private Dictionary<Match3.GemGrid, GemGridVisual> gemGridDictionary;

    private bool isSetup;
    private State state;
    private float busyTimer;
    private int busyCount;
    public float DESTROY_THRESHOLD;
    public static float MOVE_TIMER;
    private Action onBusyTimerElapsedAction;

    private int numOfMatched = 0;

    private int touchX;
    private int touchY;
    private Vector3 MouseWorldPosition;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            state = State.Busy;
            isSetup = false;
            match3.OnLevelSet += Match3_OnLevelSet;
        }
        else
            DestroyImmediate(this);
    }

    private void Match3_OnLevelSet(object sender, Match3.OnLevelSetEventArgs e)
    {
        FunctionTimer.Create(() => { Setup(sender as Match3, e.grid); }, .1f);
    }

    public void Setup(Match3 match3, Grid<Match3.GemGridPosition> grid)
    {
        this.match3 = match3;
        this.grid = grid;

        float cameraYOffset = 1f;
        cameraTransform.position = new Vector3(grid.GetWidth() * .5f, grid.GetHeight() * .5f + cameraYOffset,
            cameraTransform.position.z);

        match3.OnGemGridPositionDestroyed += Match3_OnGemGridPositionDestroyed;
        match3.OnGemGridPositionFly += Match3_OnOnGemGridPositionFly;
        match3.OnNewGemGridSpawned += Match3_OnNewGemGridSpawned;

        // Initialize Visual
        gemGridDictionary = new Dictionary<Match3.GemGrid, GemGridVisual>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Match3.GemGridPosition gemGridPosition = grid.GetGridObject(x, y);
                Match3.GemGrid gemGrid = gemGridPosition.GetGemGrid();

                Vector3 position = grid.GetWorldPosition(x, y);
                position = new Vector3(position.x, -8);

                // Visual Transform
                //Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, position, Quaternion.identity);
                Transform gemGridVisualTransform = GemPool.instance
                    .SpawnFromPool(position, Quaternion.identity, gemGrid.GetGem()).transform;
                //gemGridVisualTransform.Find("sprite").GetComponent<SpriteRenderer>().sprite = gemGrid.GetGem().sprite;

                GemGridVisual gemGridVisual = new GemGridVisual(gemGridVisualTransform, gemGrid);
                gemGrid.SetVisual(gemGridVisual);

                gemGridDictionary[gemGrid] = gemGridVisual;
                gemGridVisual.MoveSequence(grid.GetWorldPosition(x,y),0.3f);

                // Background Grid Visual
                Instantiate(pfBackgroundGridVisual, grid.GetWorldPosition(x, y), Quaternion.identity, transform);

            }
        }

        SetBusyState(.5f, () => SetState(State.TryFindMatches));
        busyCount = 0;
        DESTROY_THRESHOLD = EntityVisual.ENEMY_GRID_OFFSET + Match3.instance.GetGridHeight() + 1;
        MOVE_TIMER = 0.15f;
        isSetup = true;
    }



    private void Match3_OnNewGemGridSpawned(object sender, Match3.OnNewGemGridSpawnedEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = e.gemGridPosition;
        Vector3 position = e.gemGridPosition.GetWorldPosition();
        
        //Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, position, Quaternion.identity);

        if (gemGridPosition != null)
        {
            if (gemGridPosition.GetGemGrid().GetGem().type == GemSO.GemType.Standard)
                position = new Vector3(position.x, -8);
            else
                position = new Vector3(position.x, position.y);
        }
        
        
        var gemGridVisualTransform = GemPool.instance.SpawnFromPool(position, Quaternion.identity, e.gemGrid.GetGem()).transform;
        GemGridVisual gemGridVisual = new GemGridVisual(gemGridVisualTransform, e.gemGrid);
        gemGridDictionary[e.gemGrid] = gemGridVisual;
        e.gemGrid.SetVisual(gemGridVisual);

        
        if (gemGridPosition != null)
        {
            if (gemGridPosition.GetGemGrid().GetGem().type == GemSO.GemType.Standard)
                gemGridVisual.MoveSequence(e.gemGridPosition.GetWorldPosition(), 0.3f);
            else
                gemGridVisual.BoosterSpawnSequence(0.3f,0.3f);
        }
        
        
    }

    private void Match3_OnOnGemGridPositionFly(object sender, Match3.OnNewGemGridPositionFlyEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        if (gemGridPosition != null && gemGridPosition.GetGemGrid() != null)
        {
            gemGridDictionary[gemGridPosition.GetGemGrid()].FlySequence(0.3f,0.3f,new Vector3(e.x,e.y));
        }
    }

    private void Match3_OnGemGridPositionDestroyed(object sender, Match3.OnGemGridPositionDestroyedEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        if (gemGridPosition != null && gemGridPosition.GetGemGrid() != null)
        {
            if (e.intersect)
                gemGridDictionary[gemGridPosition.GetGemGrid()].BoosterIntersectSequence(0.2f);
            else
                gemGridDictionary[gemGridPosition.GetGemGrid()].BoosterUseSequence(0.3f, 0.3f, 0.2f);
        }

        SetState(State.TryFindMatches);
    }



    private void Update()
    {
        if (!isSetup) return;
        
        switch (state)
        {
            case State.Busy:
                busyTimer -= Time.deltaTime;
                if (busyTimer <= 0f)
                {
                    onBusyTimerElapsedAction();
                    SetState(State.TryFindMatches);
                }

                break;
            case State.EnemyTurn:
                if (!match3.TryFindMatchesAndDestroyThem())
                {
                    TrySetStateWaitingForUser(state);
                }
                else
                    numOfMatched++;

                break;
            case State.WaitingForUser:
                if (Input.GetMouseButtonDown(0) && state != State.GameOver)
                {
                    Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                    grid.GetXY(mouseWorldPosition, out touchX, out touchY);
                    if (touchY >= 0 && touchY <= grid.GetHeight() - 1 && touchX >= 0 && touchX <= grid.GetWidth() - 1)
                        RemoveGridPosition(touchX, touchY);
                }

                break;
            case State.TryFindMatches:
                //Move gems into places step by step
                if (busyCount < grid.GetHeight())
                {
                    SetBusyState(MOVE_TIMER, () => { match3.FallGemsIntoEmptyPositions(); });
                    busyCount++;
                }
                else
                {
                    match3.SpawnNewMissingGridPositions();
                    busyCount = 0;
                    //Keep repeating until all gems in positions
                    if (busyCount != 0 && !match3.FallGemsIntoEmptyPositions())
                    {
                        SetBusyState(MOVE_TIMER / 2, () => { SetState(State.TryFindMatches);  });
                    }
                    if (match3.TryFindMatchesAndDestroyThem())
                    {
                        numOfMatched++;
                        SetBusyState(.2f, () => { SetState(State.TryFindMatches); });
                    }
                    else
                    {
                        TrySetStateWaitingForUser(state);
                    }
                }

                break;
            case State.GameOver:
                break;
        }
    }

    private void FixedUpdate()
    {
        UpdateVisual();
    }

    private IEnumerator DelayEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetState(State.Busy);
    }

    private void UpdateVisual()
    {
        foreach (Match3.GemGrid gemGrid in gemGridDictionary.Keys)
        {
            if (gemGrid == null) continue;
            if (gemGridDictionary[gemGrid].GetWorldPos().y >= DESTROY_THRESHOLD)
            {
                gemGridDictionary[gemGrid].UpdateEntitiesAndDisappear(0.2f);
                gemGridDictionary.Remove(gemGrid);
                break;
            }
        }
    }

    public void RemoveGridPosition(int x, int y)
    {
        match3.TryGemGridPositionFly(x, y);
        Utils.AddToPosList(new Vector2(x,y));
        SetBusyState(0.2f, () => SetState(State.TryFindMatches));
    }

    private void SetBusyState(float busyTimer, Action onBusyTimerElapsedAction)
    {
        SetState(State.Busy);
        this.busyTimer = busyTimer;
        this.onBusyTimerElapsedAction = onBusyTimerElapsedAction;
    }

    private void TrySetStateWaitingForUser(State currState)
    {
        Utils.ResetSwitchLists();
        if (match3.TryIsGameOver())
        {
            // Game Over!
            Debug.Log("Game Over!");
            SetState(State.GameOver);
        }
        else
        {
            //You had a match, turn goes to the enemy
            if (currState == State.TryFindMatches && numOfMatched > 0)
            {
                SetState(State.EnemyTurn);
            }
            //No match after click, turn is yours
            else if ((currState == State.EnemyTurn || currState == State.TryFindMatches) && numOfMatched == 0)
            {
                SetState(State.WaitingForUser);
            }
            else
            {
                SetState(State.TryFindMatches);
            }
        }

        numOfMatched = 0;
    }

    private void SetState(State s)
    {
        state = s;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {state = s});
    }

    public Dictionary<Match3.GemGrid, GemGridVisual> GetDict()
    {
        return gemGridDictionary;
    }
    
    public State GetState()
    {
        return state;
    }

    //These are here for utility, for now they are unused
    private IEnumerator WaitSwitchState(State s, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetState(s);
    }

    private IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
    
    
    public class GemGridVisual
    {

        private Transform transform;
        private Match3.GemGrid gemGrid;
        private GameObject defaultParticles;
        private GameObject boosterSpawnParticles;
        private GameObject boosterUseParticles;
        private GameObject boosterIntersectParticles;
        private Sequence fly;
        private Sequence boosterSpawn;
        private Sequence boosterUse;
        private Sequence move;

        public GemGridVisual(Transform transform, Match3.GemGrid gemGrid)
        {
            this.transform = transform;
            this.gemGrid = gemGrid;
            defaultParticles = this.transform.Find("defaultParticles").gameObject;
            boosterSpawnParticles = this.transform.Find("boosterSpawnParticles").gameObject;
            boosterUseParticles = this.transform.Find("boosterUseParticles").gameObject;
            boosterIntersectParticles = this.transform.Find("boosterIntersectParticles").gameObject;
        }

        public void DestroyVisual(float delay)
        {
            GemPool.instance.PutInPool(transform.gameObject, delay);
        }

        public void FlySequence(float delayBeforeFly, float flyDuration, Vector3 targetPos)
        {
            fly = DOTween.Sequence();
            defaultParticles.SetActive(true);
            fly.PrependInterval(delayBeforeFly);
            fly.Append(transform.DOMove(targetPos, flyDuration));
            
        }

        public void BoosterSpawnSequence(float delayBeforeSpawn, float scaleDuration)
        {
            boosterSpawn = DOTween.Sequence();
            boosterSpawn.PrependInterval(delayBeforeSpawn);
            transform.localScale = new Vector3(0f, 0f);
            boosterSpawn.Append(transform.DOScale(0.3f, 0.001f))
                .OnComplete(() => boosterSpawnParticles.SetActive(true));
            
            boosterSpawn.Append(transform.DOScale(new Vector3(1f, 1f), scaleDuration));
        }

        public void BoosterUseSequence(float delayBeforeUse, float useDuration, float scaleDownDuration)
        {
            boosterUse = DOTween.Sequence();
            boosterUseParticles.SetActive(true);
            boosterUse.SetEase(Ease.InOutBack);
            boosterUse.PrependInterval(delayBeforeUse);
            boosterUse.Append(transform.DOScale(new Vector3(1.3f, 1.3f), useDuration));

            Match3.GemGridPosition b =  Match3.instance.GetGridAtXY(gemGrid.GetGemX(), gemGrid.GetGemY());

            boosterUse.Append(transform.DOScale(new Vector3(0f, 0f), scaleDownDuration).OnComplete(b.CallBoosterOnSelf));
            DestroyVisual(delayBeforeUse + useDuration + scaleDownDuration + 0.5f);
        }

        public void BoosterIntersectSequence(float delayBeforeDestroy)
        {
            boosterIntersectParticles.SetActive(true);
            DestroyVisual(delayBeforeDestroy);
        }

        public void MoveSequence(Vector3 targetPos, float duration)
        {
            move = DOTween.Sequence();
            //This value is tied to the delay for gems falling into positions, and therefore must be adjusted through the const
            move.PrependInterval(MOVE_TIMER * 2); 
            move.Append(transform.DOMove(targetPos, duration));
        }

        public void UpdateEntitiesAndDisappear(float delay)
        {
            Enemies.instance.EnemyTakeDamage(gemGrid.GetGemX(),gemGrid.GetGem().damage);
            Heroes.instance.ChargeHeroSkillFromGem(gemGrid.GetGem());
            DestroyVisual(delay);
        }
        
        public Vector3 GetWorldPos()
        {
            return transform.position;
        }

        
    }
}
