﻿using System;
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

                gemGridDictionary[gemGrid] = gemGridVisual;
                gemGridVisual.MoveSequence(grid.GetWorldPosition(x,y),0.3f);

                // Background Grid Visual
                Instantiate(pfBackgroundGridVisual, grid.GetWorldPosition(x, y), Quaternion.identity, transform);

            }
        }

        SetBusyState(.5f, () => SetState(State.TryFindMatches));
        busyCount = 0;
        DESTROY_THRESHOLD = EntityVisual.ENEMY_GRID_OFFSET + Match3.instance.GetGridHeight() + 1;
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

        
        if (gemGridPosition != null)
        {
            if (gemGridPosition.GetGemGrid().GetGem().type == GemSO.GemType.Standard)
                gemGridVisual.MoveSequence(e.gemGridPosition.GetWorldPosition(), 0.3f);
            else
                gemGridVisual.BoosterSpawnSequence(0f,0.3f);
        }
        
        
    }

    private void Match3_OnOnGemGridPositionFly(object sender, Match3.OnNewGemGridPositionFlyEventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        if (gemGridPosition != null && gemGridPosition.GetGemGrid() != null)
        {
            gemGridDictionary[gemGridPosition.GetGemGrid()].FlySequence(0.3f,0.3f,new Vector3(e.x,e.y));
            //gemGridDictionary[gemGridPosition.GetGemGrid()].DestroyVisual(0f);
            //gemGridDictionary.Remove(gemGridPosition.GetGemGrid());
        }
    }

    private void Match3_OnGemGridPositionDestroyed(object sender, System.EventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        if (gemGridPosition != null && gemGridPosition.GetGemGrid() != null)
        {
            gemGridDictionary[gemGridPosition.GetGemGrid()].DestroyVisual(0f);
            gemGridDictionary.Remove(gemGridPosition.GetGemGrid());
        }
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
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                    grid.GetXY(mouseWorldPosition, out touchX, out touchY);
                    if (touchY >= 0 && touchY <= grid.GetHeight() - 1 && touchX >= 0 && touchX <= grid.GetWidth() - 1)
                        RemoveGridPosition(touchX, touchY);
                }

                break;
            case State.TryFindMatches:
                if (busyCount < grid.GetHeight())
                {
                    SetBusyState(0.1f, () => { match3.FallGemsIntoEmptyPositions(); });
                    busyCount++;
                }
                else
                {
                    match3.SpawnNewMissingGridPositions();
                    busyCount = 0;
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
            if (gemGrid != null) {
                gemGridDictionary[gemGrid].Update();
                if (gemGridDictionary[gemGrid].GetWorldPos().y >= DESTROY_THRESHOLD)
                {
                    gemGridDictionary[gemGrid].DestroyVisual(0.2f);
                    gemGridDictionary.Remove(gemGrid);
                    break;
                }
            }
        }
    }

    public void RemoveGridPosition(int x, int y)
    {
        match3.TryGemGridPositionFly(x, y);
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
            if (currState == State.TryFindMatches && numOfMatched > 0)
            {
                SetState(State.EnemyTurn);
            }
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

    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {state = state});
    }

    public Dictionary<Match3.GemGrid, GemGridVisual> GetDict()
    {
        return gemGridDictionary;
    }

    public State GetState()
    {
        return state;
    }

    public class GemGridVisual
    {

        private Transform transform;
        private Match3.GemGrid gemGrid;
        private GameObject defaultParticles;
        private GameObject boosterParticles;
        private Sequence fly;
        private Sequence boosterSpawn;

        public GemGridVisual(Transform transform, Match3.GemGrid gemGrid)
        {
            this.transform = transform;
            this.gemGrid = gemGrid;
            defaultParticles = this.transform.Find("defaultParticles").gameObject;
            boosterParticles = this.transform.Find("boosterParticles").gameObject;
        }

        public void DestroyVisual(float delay)
        {
            GemPool.instance.PutInPool(transform.gameObject, delay);
        }

        public void FlySequence(float delayBeforeFly, float flyDuration, Vector3 targetPos)
        {
            fly = DOTween.Sequence();
            defaultParticles.SetActive(true);
            fly.Append(transform.DOMove(targetPos, flyDuration));
            fly.PrependInterval(delayBeforeFly);
        }

        public void BoosterSpawnSequence(float delayBeforeSpawn, float scaleDuration)
        {
            boosterSpawn = DOTween.Sequence();
            boosterParticles.SetActive(true);
            boosterSpawn.PrependInterval(delayBeforeSpawn);
            transform.localScale = new Vector3(0.3f, 0.3f);
            boosterSpawn.Append(transform.DOScale(new Vector3(1f, 1f), scaleDuration));
        }

        public void MoveSequence(Vector3 targetPos, float duration)
        {
            transform.DOMove(targetPos, duration);
        }

        public Vector3 GetWorldPos()
        {
            return transform.position;
        }

        public void Update()
        {
            /*
            Vector3 targetPosition = gemGrid.GetWorldPosition();
            Vector3 moveDir = (targetPosition - transform.position);
            float moveSpeed = 10f;
            
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            */
        }
    }
}
