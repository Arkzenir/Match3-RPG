using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEditorInternal;
using Random = System.Random;

/*
 * Visual Representation of the underlying Match3 Grid
 * */
public class Match3Visual : MonoBehaviour {

    public static Match3Visual instance;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State {
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
    private Action onBusyTimerElapsedAction;

    private int numOfMatched = 0;
    
    private int touchX;
    private int touchY;
    private Vector3 MouseWorldPosition;

    private void Awake() {

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

    private void Match3_OnLevelSet(object sender, Match3.OnLevelSetEventArgs e) {
        FunctionTimer.Create(() => {
            Setup(sender as Match3, e.grid);
        }, .1f);
    }

    public void Setup(Match3 match3, Grid<Match3.GemGridPosition> grid) {
        this.match3 = match3;
        this.grid = grid;

        float cameraYOffset = 1f;
        cameraTransform.position = new Vector3(grid.GetWidth() * .5f, grid.GetHeight() * .5f + cameraYOffset, cameraTransform.position.z);

        match3.OnGemGridPositionDestroyed += Match3_OnGemGridPositionDestroyed;
        match3.OnGemGridPositionFly += Match3_OnOnGemGridPositionFly;
        match3.OnNewGemGridSpawned += Match3_OnNewGemGridSpawned;

        // Initialize Visual
        gemGridDictionary = new Dictionary<Match3.GemGrid, GemGridVisual>();

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                Match3.GemGridPosition gemGridPosition = grid.GetGridObject(x, y);
                Match3.GemGrid gemGrid = gemGridPosition.GetGemGrid();

                Vector3 position = grid.GetWorldPosition(x, y);
                position = new Vector3(position.x, -8);

                // Visual Transform
                //Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, position, Quaternion.identity);
                Transform gemGridVisualTransform = GemPool.instance.SpawnFromPool(position, Quaternion.identity, gemGrid.GetGem()).transform;
                //gemGridVisualTransform.Find("sprite").GetComponent<SpriteRenderer>().sprite = gemGrid.GetGem().sprite;

                GemGridVisual gemGridVisual = new GemGridVisual(gemGridVisualTransform, gemGrid);

                gemGridDictionary[gemGrid] = gemGridVisual;

                // Background Grid Visual
                Instantiate(pfBackgroundGridVisual, grid.GetWorldPosition(x, y), Quaternion.identity, transform);
                
            }
        }
        SetBusyState(.5f, () => SetState(State.TryFindMatches));
        busyCount = grid.GetHeight();
        DESTROY_THRESHOLD = EntityVisual.ENEMY_GRID_OFFSET + Match3.instance.GetGridHeight() + 1;
        isSetup = true;
    }

    

    private void Match3_OnNewGemGridSpawned(object sender, Match3.OnNewGemGridSpawnedEventArgs e) {
        Vector3 position = e.gemGridPosition.GetWorldPosition();
        
        Transform gemGridVisualTransform;
        Transform pfGemGridVisual = GemPool.instance.gemPrefab.transform;
        //Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, position, Quaternion.identity);
        position = new Vector3(position.x, -8);
        gemGridVisualTransform = GemPool.instance.SpawnFromPool(position, Quaternion.identity, e.gemGrid.GetGem()).transform;


        //gemGridVisualTransform.Find("sprite").GetComponent<SpriteRenderer>().sprite = e.gemGrid.GetGem().sprite;

        GemGridVisual gemGridVisual = new GemGridVisual(gemGridVisualTransform, e.gemGrid);

        gemGridDictionary[e.gemGrid] = gemGridVisual;
    }
    
    private void Match3_OnOnGemGridPositionFly(object sender, Match3.OnNewGemGridPositionFlyEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        if (gemGridPosition != null && gemGridPosition.GetGemGrid() != null) {
            gemGridPosition.GetGemGrid().SetGemXY(gemGridPosition.GetX(), e.y);
            
            //gemGridDictionary.Remove(gemGridPosition.GetGemGrid());
        }
    }
    
    private void Match3_OnGemGridPositionDestroyed(object sender, System.EventArgs e) {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        if (gemGridPosition != null && gemGridPosition.GetGemGrid() != null) {
            gemGridDictionary[gemGridPosition.GetGemGrid()].DestroyVisual(0f);
            gemGridDictionary.Remove(gemGridPosition.GetGemGrid());
        }
    }

    

    private void Update()
    {
        if (!isSetup) return;

        UpdateVisual();

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
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                    grid.GetXY(mouseWorldPosition, out touchX, out touchY);
                    if (touchY >= 0)
                        RemoveGridPosition(touchX, touchY);
                }
                break;
            case State.TryFindMatches:
                if (busyCount < match3.GetGridHeight() - 1)
                {
                    busyCount++;
                    SetBusyState(0.1f, () => { match3.FallGemsIntoEmptyPositions(); });
                }
                else
                {
                    busyCount = 0;
                    if (match3.TryFindMatchesAndDestroyThem())
                    {
                        numOfMatched++;
                        SetBusyState(.2f, () =>
                        {
                            SetState(State.TryFindMatches);
                        });
                    }
                    else
                    {
                        match3.SpawnNewMissingGridPositions();
                        TrySetStateWaitingForUser(state);
                    }
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private IEnumerator DelayEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetState(State.Busy);
    }

    private void UpdateVisual() {
        foreach (Match3.GemGrid gemGrid in gemGridDictionary.Keys)
        {
            gemGridDictionary[gemGrid].Update();
            if (gemGridDictionary[gemGrid].GetWorldPos().y >= DESTROY_THRESHOLD)
            {
                gemGridDictionary[gemGrid].DestroyVisual(0.2f);
                gemGridDictionary.Remove(gemGrid);
                break;
            }
        }
    }
    
    public void RemoveGridPosition(int x, int y)
    {
        match3.TryGemGridPositionFly(x,y);
        SetBusyState(0.2f, () => SetState(State.TryFindMatches));
    }
    
    private void SetBusyState(float busyTimer, Action onBusyTimerElapsedAction) {
        SetState(State.Busy);
        this.busyTimer = busyTimer;
        this.onBusyTimerElapsedAction = onBusyTimerElapsedAction;
    }

    private void TrySetStateWaitingForUser(State currState) {
        Utils.ResetSwitchLists();
        if (match3.TryIsGameOver()) {
            // Game Over!
            Debug.Log("Game Over!");
            SetState(State.GameOver);
        } else {
            if (currState == State.TryFindMatches && numOfMatched > 0)
            {
                numOfMatched = 0;
                SetState(State.EnemyTurn);
            }else if ((currState == State.EnemyTurn || currState == State.TryFindMatches) && numOfMatched == 0)
            {
                numOfMatched = 0;
                SetState(State.WaitingForUser);
            }else
            {
                numOfMatched = 0;
                SetState(State.TryFindMatches);
            }
        }
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{state = state});
    }

    public State GetState() {
        return state;
    }

    public class GemGridVisual {

        private Transform transform;
        private Match3.GemGrid gemGrid;

        public GemGridVisual(Transform transform, Match3.GemGrid gemGrid) {
            this.transform = transform;
            this.gemGrid = gemGrid;
        }

        public void DestroyVisual(float delay)
        {
            GemPool.instance.PutInPool(transform.gameObject, delay);
        }

        public Vector3 GetWorldPos()
        {
            return transform.position;
        }
        
        public void Update() {

            Vector3 targetPosition = gemGrid.GetWorldPosition();
            Vector3 moveDir = (targetPosition - transform.position);
            float moveSpeed = 10f;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        
    }
    

}
