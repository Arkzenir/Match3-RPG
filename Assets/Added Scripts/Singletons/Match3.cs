using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Represents the underlying Grid logic
 * */
public class Match3 : MonoBehaviour
{
    public static Match3 instance;
    
    public event EventHandler OnGemGridPositionDestroyed;
    public event EventHandler<OnNewGemGridPositionFlyEventArgs> OnGemGridPositionFly;
    public event EventHandler<OnNewGemGridSpawnedEventArgs> OnNewGemGridSpawned;
    public event EventHandler<OnLevelSetEventArgs> OnLevelSet;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnWin;
    public event EventHandler OnLose;
    public class OnNewGemGridSpawnedEventArgs : EventArgs {
        public GemGrid gemGrid;
        public GemGridPosition gemGridPosition;
    }

    public class OnLevelSetEventArgs : EventArgs {
        public LevelSO levelSO;
        public Grid<GemGridPosition> grid;
    }

    public class OnNewGemGridPositionFlyEventArgs : EventArgs
    {
        public int x;
        public int y;
        public GemSO gemType;
    }

    public List<Vector2> lastModifiedPositions = new List<Vector2>();
    
    [SerializeField] private LevelSO levelSO;
    [SerializeField] private bool autoLoadLevel;
    [SerializeField] private bool match4Explosions; // Explode neighbour nodes on 4 match

    private int gridWidth;
    private int gridHeight;
    private Grid<GemGridPosition> grid;
    private int score;

    private List<GemGrid> toBeFlown = new List<GemGrid>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(this);
    }
    
    private void Start() {
        if (autoLoadLevel) {
            SetLevelSO(levelSO);
        }
    }

    public LevelSO GetLevelSO() {
        return levelSO;
    }

    public void SetLevelSO(LevelSO levelSO) {
        this.levelSO = levelSO;

        gridWidth = levelSO.width;
        gridHeight = levelSO.height;
        grid = new Grid<GemGridPosition>(gridWidth, gridHeight, 1f, Vector3.zero, (Grid<GemGridPosition> g, int x, int y) => new GemGridPosition(g, x, y));

        // Initialize Grid
        for (int x = 0; x < gridWidth; x++)
        {
                for (int y = 0; y < gridHeight; y++)
                {
                    GemSO gem = levelSO.gemList[UnityEngine.Random.Range(0, levelSO.gemList.Count)]; 
                    GemGrid gemGrid = new GemGrid(gem, x, y);
                    grid.GetGridObject(x, y).SetGemGrid(gemGrid);
                }
        }


        score = 0;
        OnLevelSet?.Invoke(this, new OnLevelSetEventArgs { levelSO = levelSO, grid = grid });
    }

    public int GetScore() {
        return score;
    }


    public bool TryFindMatchesAndDestroyThem() {
        List<GemGridPosition[,]> allLinkedGemGridPositionList = GetAllMatch3Links();

        bool foundMatch = false;

        foreach (GemGridPosition[,] linkedGemGridPositionList in allLinkedGemGridPositionList)
        {
            int matchedGemCount = 0;
            for (int i = 0; i < linkedGemGridPositionList.GetLength(0); i++)
            {
                for (int j = 0; j < linkedGemGridPositionList.GetLength(1); j++)
                {
                    if (linkedGemGridPositionList[i,j] != null)
                    {
                        matchedGemCount++;
                    }
                }
            }
            
            var color = linkedGemGridPositionList[0, 0].GetGemGrid().GetGem().color;
            
            if (matchedGemCount == 3)
            {
                foreach (GemGridPosition gemGridPosition in linkedGemGridPositionList)
                {
                    if (gemGridPosition != null) TryGemGridPositionFly(gemGridPosition);
                }
                foundMatch = true;
                break;
            }
            
            //Booster in effect
            if (matchedGemCount >= 4) {
                //Match the shape of matrix to reference matrix and spawn booster
                bool boosterNotFound = true;
                int typeIndex = 0;
                List<GemGridPosition> toBeDestroyed = new List<GemGridPosition>();
                int[,] shapeMatrix = new int[5,5];
                
                for (int k = Utils.GetReferenceMatrixList().Count - 1; k >= 0; k--)
                {
                    var refList = Utils.GetReferenceMatrixList()[k];
                    typeIndex = k;
                    foreach (var matrix in refList)
                    {
                        toBeDestroyed.Clear();
                        shapeMatrix = new int[5, 5];
                        boosterNotFound = false;
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if ((linkedGemGridPositionList[i, j] != null) && (matrix[i, j] == 1))
                                {
                                    toBeDestroyed.Add(linkedGemGridPositionList[i,j]);
                                    shapeMatrix[i, j] = 1;
                                }
                            }
                        }
                        

                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if (shapeMatrix[i,j] != matrix[i,j])
                                {
                                    boosterNotFound = true;
                                }
                            }
                        }
                        
                        if (!boosterNotFound)
                            break;
                    }

                    if (!boosterNotFound)
                        break;    
                }

                
                if (!boosterNotFound)
                {
                    Debug.Log("Booster Match");
                    foreach (GemGridPosition gemGridPosition in toBeDestroyed)
                    {
                        if (gemGridPosition != null) TryGemGridPositionFly(gemGridPosition);
                    }
                    
                    //Choose random spot from last modified position list and spawn booster
                    if (Utils.GetLastPosList().Count > 0)
                    {
                        Vector2 chosenPos =
                            Utils.GetLastPosList()[UnityEngine.Random.Range(0, Utils.GetLastPosList().Count - 1)];
                        SpawnNewBoosterGem((int) chosenPos.x, (int) chosenPos.y, typeIndex, color);
                    }

                    foundMatch = true;
                    break;
                }
                
                /*
                bool skip = true;
                int[,] ignoreShapeMatrix = new int[5, 5];
                foreach (var e in Utils.GetExclusionList())
                {
                    skip = true;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if ((linkedGemGridPositionList[i, j] != null) && (e[i, j] == 1))
                            {
                                ignoreShapeMatrix[i, j] = 1;
                            }
                        }
                    }
                
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (ignoreShapeMatrix[i,j] != e[i,j])
                            {
                                skip = false;
                            }
                        }
                    }
                }
            
                if (skip)
                {
                    Debug.Log("Excluded shape");
                    foundMatch = false;
                    break;
                }
                */
            }
            
            
        }
        

        OnScoreChanged?.Invoke(this, EventArgs.Empty);
        Utils.ResetSwitchLists();
        return foundMatch;
    }

    public void TryDestroyGemGridPosition(GemGridPosition gemGridPosition) {
        if (gemGridPosition.HasGemGrid()) {
            gemGridPosition.DestroyGem();
            OnGemGridPositionDestroyed?.Invoke(gemGridPosition, EventArgs.Empty);
            gemGridPosition.ClearGemGrid();
        }
    }

    public void TryGemGridPositionFly(GemGridPosition gemGridPosition)
    {
        if (gemGridPosition.HasGemGrid())
        {
            Utils.AddToPosList(new Vector2(gemGridPosition.GetX(),gemGridPosition.GetY()));
            score += 10;
            if (gemGridPosition.GetGemGrid().GetGem().type == GemSO.GemType.Standard)
            {
                gemGridPosition.FlyGem();
            }
            else
            {
                gemGridPosition.FlyAndBoostGem();
            }
        }
    }

    public void TryGemGridPositionFly(int x, int y)
    {
        GemGridPosition pos = GetGridAtXY(x,y);
        if (pos != null && pos.HasGemGrid())
        {
            score += 10;
            Utils.AddToPosList(new Vector2(pos.GetX(),pos.GetY()));
            if (pos.GetGemGrid().GetGem().type == GemSO.GemType.Standard)
            {
                pos.FlyGem();
            }
            else
            {
                pos.FlyAndBoostGem();
                //OnGemGridPositionDestroyed?.Invoke(pos, EventArgs.Empty);
            }
        }
    }

    public void SpawnNewBoosterGem(int x,int y, int type, GemSO.GemColor color)
    {
        GemGridPosition gemGridPosition = grid.GetGridObject(x, y);
        
        Debug.Log("spawn booster of type " + type + " and color " + color);
        
        GemSO gem = levelSO.boosterList[type];
        gem.booster = levelSO.boosterList[type].booster;
        gem.color = color;
        GemGrid gemGrid = new GemGrid(gem, x, y);

        gemGridPosition.SetGemGrid(gemGrid);

        OnNewGemGridSpawned?.Invoke(gemGrid, new OnNewGemGridSpawnedEventArgs {
            gemGrid = gemGrid,
            gemGridPosition = gemGridPosition,
        });
    }
    
    public void SpawnNewMissingGridPositions() {
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                GemGridPosition gemGridPosition = grid.GetGridObject(x, y);

                if (gemGridPosition.IsEmpty()) {
                    GemSO gem = levelSO.gemList[UnityEngine.Random.Range(0, levelSO.gemList.Count)];
                    GemGrid gemGrid = new GemGrid(gem, x, y);

                    gemGridPosition.SetGemGrid(gemGrid);

                    OnNewGemGridSpawned?.Invoke(gemGrid, new OnNewGemGridSpawnedEventArgs {
                        gemGrid = gemGrid,
                        gemGridPosition = gemGridPosition,
                    });
                }
            }
        }
    }

    public bool FallGemsIntoEmptyPositions()
    {
        bool finished = true;
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                GemGridPosition gemGridPosition = grid.GetGridObject(x, y);

                if (!gemGridPosition.IsEmpty()) {
                    // Grid Position has Gem
                    for (int i = y + 1; i < gridHeight; i++) {
                        GemGridPosition nextGemGridPosition = grid.GetGridObject(x, i);
                        if (nextGemGridPosition.IsEmpty())
                        {
                            finished = false;
                            gemGridPosition.GetGemGrid().SetGemXY(x, i);
                            nextGemGridPosition.SetGemGrid(gemGridPosition.GetGemGrid());
                            gemGridPosition.ClearGemGrid();

                            gemGridPosition = nextGemGridPosition;
                        } else {
                            // Next Grid Position is not empty, stop looking
                            break;
                        }
                    }
                }
            }
        }
        return finished;
    }

    public bool HasMatch3Link(int x, int y) {
        GemGridPosition[,] linkedGemGridPositionList = GetMatch3Links(x, y);

        int matchedGemCount = 0;
        if (linkedGemGridPositionList != null)
        {
            for (int i = 0; i < linkedGemGridPositionList.GetLength(0); i++)
            {
                for (int j = 0; j < linkedGemGridPositionList.GetLength(1); j++)
                {
                    if (linkedGemGridPositionList[i, j] != null)
                    {
                        matchedGemCount++;
                    }
                }
            }

            return matchedGemCount >= 3;
        }
        else
        {
            return false;
        }
    }

    public GemGridPosition[,] GetMatch3Links(int x, int y) {
        GemSO gemSO = GetGemSO(x, y);

        if (gemSO == null) return null;

        GemGridPosition[,] matchArray = new GemGridPosition[5, 5];

        int matchedGemCount = 0;

        for (int i = 0; i < matchArray.GetLength(0); i++)
        {
            for (int j = 0; j < matchArray.GetLength(1); j++)
            {
                if (IsValidPosition(x + i, y + j))
                {
                    GemSO nextGemSO = GetGemSO(x + i, y + j);
                    if (nextGemSO == gemSO)
                    {
                        if (i != 0 || j != 0)
                        {
                            bool cont = true;
                            foreach (var pos in matchArray)
                            {
                                if (pos != null && (
                                    (x + i - pos.GetX() == 1 && y + j - pos.GetY() == 0) ||
                                    (x + i - pos.GetX() == 0 && y + j - pos.GetY() == 1)))
                                    cont = false;
                            }
                            
                            if (cont) 
                                continue;
                        }
                        
                        matchArray[i, j] = grid.GetGridObject(x + i, y + j);
                        matchedGemCount++;
                    }
                }
            }
        }

        if (matchedGemCount > 3)
        {
            int[,] shapeMatrix = new int[5, 5];
            bool falseFlag = true;
            for (int k = Utils.GetReferenceMatrixList().Count - 1; k >= 0; k--)
            {
                var refList = Utils.GetReferenceMatrixList()[k];
                foreach (var matrix in refList)
                {
                    shapeMatrix = new int[5, 5];
                    falseFlag = false;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if ((matchArray[i, j] != null) && (matrix[i, j] == 1))
                            {
                                shapeMatrix[i, j] = 1;
                            }
                        }
                    }
                        

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (shapeMatrix[i,j] != matrix[i,j])
                            {
                                falseFlag = true;
                            }
                        }
                    }
                        
                    if (!falseFlag)
                        break;
                }

                if (!falseFlag)
                    break;    
            }
            
            if(!falseFlag)
                return matchArray;
        }
        
        //There are no boosters, check for regular 3 match
        matchArray = new GemGridPosition[5, 5];
        
        int rightLinkAmount = 0;
        for (int i = 1; i < gridWidth; i++) {
            if (IsValidPosition(x + i, y)) {
                GemSO nextGemSO = GetGemSO(x + i, y);
                if (nextGemSO == gemSO) {
                    // Same Gem
                    rightLinkAmount++;
                } else {
                    // Not same Gem
                    break;
                }
            } else {
                // Invalid position
                break;
            }
        }

        int leftLinkAmount = 0;
        for (int i = 1; i < gridWidth; i++) {
            if (IsValidPosition(x - i, y)) {
                GemSO nextGemSO = GetGemSO(x - i, y);
                if (nextGemSO == gemSO) {
                    // Same Gem
                    leftLinkAmount++;
                } else {
                    // Not same Gem
                    break;
                }
            } else {
                // Invalid position
                break;
            }
        }

        int horizontalLinkAmount = 1 + leftLinkAmount + rightLinkAmount; // This Gem + left + right

        if (horizontalLinkAmount == 3) {
            // Has 3 horizontal linked gems
            //List<GemGridPosition> linkedGemGridPositionList = new List<GemGridPosition>();
            int leftMostX = x - leftLinkAmount;
            for (int i = 0; i < horizontalLinkAmount; i++)
            {
                matchArray[0, i] = grid.GetGridObject(leftMostX + i, y);
                //linkedGemGridPositionList.Add(grid.GetGridObject(leftMostX + i, y));
            }
            return matchArray;
        }


        int upLinkAmount = 0;
        for (int i = 1; i < gridHeight; i++) {
            if (IsValidPosition(x, y + i)) {
                GemSO nextGemSO = GetGemSO(x, y + i);
                if (nextGemSO == gemSO) {
                    // Same Gem
                    upLinkAmount++;
                } else {
                    // Not same Gem
                    break;
                }
            } else {
                // Invalid position
                break;
            }
        }

        int downLinkAmount = 0;
        for (int i = 1; i < gridHeight; i++) {
            if (IsValidPosition(x, y - i)) {
                GemSO nextGemSO = GetGemSO(x, y - i);
                if (nextGemSO == gemSO) {
                    // Same Gem
                    downLinkAmount++;
                } else {
                    // Not same Gem
                    break;
                }
            } else {
                // Invalid position
                break;
            }
        }

        int verticalLinkAmount = 1 + downLinkAmount + upLinkAmount; // This Gem + down + up

        if (verticalLinkAmount == 3) {
            // Has 3 vertical linked gems
            //List<GemGridPosition> linkedGemGridPositionList = new List<GemGridPosition>();
            int downMostY = y - downLinkAmount;
            for (int i = 0; i < verticalLinkAmount; i++)
            {
                matchArray[i, 0] = grid.GetGridObject(x, downMostY + i);
                //linkedGemGridPositionList.Add(grid.GetGridObject(x, downMostY + i));
            }
            return matchArray;
        }
        
        // No links
        return null;
    }

    public List<GemGridPosition[,]> GetAllMatch3Links() {
        // Finds all the links with the current grid
        List<GemGridPosition[,]> allLinkedGemGridPositionList = new List<GemGridPosition[,]>();

        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                if (HasMatch3Link(x, y)) {
                    GemGridPosition[,] linkedGemGridPositionList = GetMatch3Links(x, y);

                    if (allLinkedGemGridPositionList.Count == 0) {
                        // First one
                        allLinkedGemGridPositionList.Add(linkedGemGridPositionList);
                    } else {
                        bool uniqueNewLink = true;

                        foreach (GemGridPosition[,]  tmpLinkedGemGridPositionList in allLinkedGemGridPositionList) {
                            
                            int tmpMatchedCount = 0;
                            int linkMatchedCount = 0;
                            
                            for (int i = 0; i < tmpLinkedGemGridPositionList.GetLength(0); i++)
                            {
                                for (int j = 0; j < tmpLinkedGemGridPositionList.GetLength(1); j++)
                                {
                                    if (tmpLinkedGemGridPositionList[i,j] != null)
                                    {
                                        tmpMatchedCount++;
                                    }
                                }
                            }
        
                            for (int i = 0; i < linkedGemGridPositionList.GetLength(0); i++)
                            {
                                for (int j = 0; j < linkedGemGridPositionList.GetLength(1); j++)
                                {
                                    if (linkedGemGridPositionList[i,j] != null)
                                    {
                                        linkMatchedCount++;
                                    }
                                }
                            }
                            
                            
                            if (linkMatchedCount == tmpMatchedCount) {
                                // Same number of links
                                // Are they all the same?
                                bool allTheSame = true;
                                for (int i = 0; i < linkedGemGridPositionList.GetLength(0); i++)
                                {
                                    for (int j = 0; j < linkedGemGridPositionList.GetLength(1); j++)
                                    {
                                        if (linkedGemGridPositionList[i,j] == tmpLinkedGemGridPositionList[i,j]) {
                                            // This one is the same, link is not unique
                                        } else {
                                            // These don't match
                                            allTheSame = false;
                                            break;
                                        }
                                    }
                                }

                                if (allTheSame) {
                                    // Nodes are all the same, not a new unique link
                                    uniqueNewLink = false;
                                }
                            }
                        }

                        // Add to the total list if it's a unique link
                        if (uniqueNewLink) {
                            allLinkedGemGridPositionList.Add(linkedGemGridPositionList);
                        }
                    }
                }
            }
        }

        return allLinkedGemGridPositionList;
    }
    
    private GemSO GetGemSO(int x, int y) {
        if (!IsValidPosition(x, y)) return null;

        GemGridPosition gemGridPosition = grid.GetGridObject(x, y);

        if (gemGridPosition.GetGemGrid() == null) return null;

        return gemGridPosition.GetGemGrid().GetGem();
    }

    public GemGridPosition GetGridAtXY(int x, int y)
    {
        return grid.GetGridObject(x,y);
    }

    private bool IsValidPosition(int x, int y) {
        if (x < 0 || y < 0 ||
            x >= gridWidth || y >= gridHeight) {
            // Invalid position
            return false;
        } else {
            return true;
        }
    }

    public bool TryIsGameOver()
    {
        bool result = true;
        foreach (var h in Heroes.instance.heroes)
        {
            if (!h.IsDead())
                result = false;
        }

        if (result)
            OnLose?.Invoke(this,EventArgs.Empty);

        if (Enemies.instance.activeEnemies.Count > 0)
        {
            result = false;
            return result;
        }
        OnWin?.Invoke(this, EventArgs.Empty);
        return result;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetGridWidth()
    {
        return levelSO.width;
    }

    public int GetGridHeight()
    {
        return levelSO.height;
    }

    /*
     * Represents a single Grid Position
     * Only the Grid Position which may or may not have an actual Gem on it
     * */
    public class GemGridPosition {
        
        private GemGrid gemGrid;

        private Grid<GemGridPosition> grid;
        private int x;
        private int y;

        public GemGridPosition(Grid<GemGridPosition> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetGemGrid(GemGrid gemGrid) {
            this.gemGrid = gemGrid;
            grid.TriggerGridObjectChanged(x, y);
        }

        public int GetX() {
            return x;
        }

        public int GetY() {
            return y;
        }
        
        
        public Vector3 GetWorldPosition() {
            return grid.GetWorldPosition(x, y);
        }

        public GemGrid GetGemGrid() {
            return gemGrid;
        }

        public void ClearGemGrid() {
            gemGrid = null;
        }

        public void DestroyGemGrid() {
            gemGrid?.Destroy();
        }

        public void FlyGem()
        {
            instance.OnGemGridPositionFly?.Invoke(this, 
                new OnNewGemGridPositionFlyEventArgs{x = x,y = (int)Match3Visual.instance.DESTROY_THRESHOLD + 1, gemType = GetGemGrid().GetGem()});
            RemoveGem();
        }

        public void FlyAndBoostGem()
        {
            gemGrid.GetGem().booster.UseBooster(x,y,this);
            instance.OnGemGridPositionDestroyed?.Invoke(this, EventArgs.Empty);
            RemoveGem();
        }

        public void DestroyGem()
        {
            instance.OnGemGridPositionDestroyed?.Invoke(this, EventArgs.Empty);
            RemoveGem();
        }

        private void RemoveGem()
        {
            DestroyGemGrid();
            ClearGemGrid();
            grid.TriggerGridObjectChanged(x,y);
        }
        
        public bool HasGemGrid() {
            return gemGrid != null;
        }

        public bool IsEmpty() {
            return gemGrid == null;
        }

        public override string ToString() {
            return gemGrid?.ToString();
        }
    }

    /*
     * Represents a Gem Object in the Grid
     * */
    public class GemGrid {
        
        private GemSO gem;
        private int x;
        private int y;
        private bool isDestroyed;

        public GemGrid(GemSO gem, int x, int y) {
            this.gem = gem;
            this.x = x;
            this.y = y;
            isDestroyed = false;
        }

        public GemSO GetGem() {
            return gem;
        }

        public Vector3 GetWorldPosition() {
            return new Vector3(x, y);
        }
        
        public void SetGemXY(int x, int y) {
            this.x = x;
            this.y = y;
            Match3Visual.instance.GetDict()[this].MoveSequence(new Vector3(x,y), 0.3f);
        }

        public void Destroy() {
            isDestroyed = true;
        }

        public bool IsDestroyed() { return isDestroyed;}
        
        public override string ToString() {
            return isDestroyed.ToString();
        }

    }

}
