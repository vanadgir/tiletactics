using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    #region Singleton
    private static GridManager _instance;

    public static GridManager Instance { get { return _instance; } }
    #endregion

    [Space(5)]
    [Header("Grid Properties")]
    [SerializeField] public int gridWidth = 10; 
    [SerializeField] public int gridHeight = 10;

    [Space(20)]
    [Header("Tilemap Settings")]
    // assign these in editor
    public Tilemap tilemap;
    public Tilemap gridTilemap;
    public Tilemap walkbilityTilemap;
    public Tilemap gridUI;
    public TileBase emptyTile;
    public TileBase selectedTile;
    public TileBase redTile;
    public TileBase greenTile;
    public List<TileBase> allTiles;

    private TileData[,] grid;
    private string[,] gameGrid;

    CameraController cc;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        cc = Camera.main.GetComponent<CameraController>();
    }

    private void Start()
    {
        StartCoroutine(BuildMapCoroutine());
    }

    // this is the grid of tile metadata
    // needed for WFC, maybe for resources and other gameplay
    private void InitializeTileData()
    {
        grid = new TileData[gridWidth, gridHeight];

        gameGrid = new string[gridWidth * 2, gridHeight * 2];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                TileData tileData = new TileData(x, y);

                grid[x, y] = tileData;
            }
        }
    }

    // this is the world tilemap, primarily for visuals
    private void InitializeTilemap()
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), emptyTile);

                TileData tileData = grid[x, y];
                if (tileData != null)
                {
                    tileData.SetName(emptyTile.name);
                    tileData.SetQuadrantsFromName(emptyTile.name);
                }
            }
        }
    }

    // this is the gameplay tilemap
    // 4 of these inside 1 visual tile
    private void InitializeGameGrid()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                string[,] quadrants = grid[x, y].quadrants;

                gameGrid[2 * x, 2 * y + 1] = quadrants[0, 0] != "" ? quadrants[0, 0]: "0"; // top left
                gameGrid[2 * x, 2 * y] = quadrants[1, 0] != "" ? quadrants[1, 0] : "0"; // bottom left
                gameGrid[2 * x + 1, 2 * y + 1] = quadrants[0, 1] != "" ? quadrants[0, 1] : "0"; // top right
                gameGrid[2 * x + 1, 2 * y] = quadrants[1, 1] != "" ? quadrants[1, 1] : "0"; // bottom right
            }
        }

    }

    private void MakeWalkabilityGrid()
    {
        for (int x = 0; x < gameGrid.GetLength(0); x++)
        {
            for (int y = 0; y < gameGrid.GetLength(1); y++)
            {
                string resource = gameGrid[x, y];

                switch (resource)
                {
                    case "G":
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), greenTile);
                        break;
                    case "S":
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), greenTile);
                        break;
                    case "B":
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), redTile);
                        break;
                    case "W":
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), redTile);
                        break;
                    default:
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), redTile);
                        break;
                }
            }
        }
    }

    // collapsing means assigning a visual Tile
    // and updating some metadata
    private void CollapseTile(TileData tile)
    {
        int posX = tile.gridPos.x;
        int posY = tile.gridPos.y;

        if (tile.isCollapsed) return;

        TileBase tileImg = PickRandomTileBase(tile.validOptions);

        tilemap.SetTile(new Vector3Int(posX, posY, 0), tileImg);

        tile.SetName(tileImg.name);
        tile.SetQuadrantsFromName(tileImg.name);
        tile.isCollapsed = true;
    }

    // if cardinal neighbor(s) exist, update their entropy
    public void UpdateNeighbors(TileData tile)
    {
        int posX = tile.gridPos.x;
        int posY = tile.gridPos.y;

        TileData tileN = posY < grid.GetLength(1) - 1 ? grid[posX, posY + 1] : null;
        TileData tileE = posX < grid.GetLength(0) - 1 ? grid[posX + 1, posY] : null;
        TileData tileS = posY > 0 ? grid[posX, posY - 1] : null;
        TileData tileW = posX > 0 ? grid[posX - 1, posY] : null;

        string tileName = tile.name;

        // skip everything if this name isn't found in rules
        if (!TileRuleLookup.TileRulesDictionary.ContainsKey(tileName)) return;

        var rules = TileRuleLookup.TileRulesDictionary.GetValueOrDefault(tileName);

        // update north neighbor
        if (tileN != null)
        {
            string code = rules.NORTH;
            tileN.validOptions = tileN.validOptions
                .Where(option => TileRuleLookup.TileRulesDictionary[option.name].SOUTH == code)
                .ToList();
            tileN.entropy = tileN.validOptions.Count;
        }

        // update east neighbor
        if (tileE != null)
        {
            string code = rules.EAST;
            tileE.validOptions = tileE.validOptions
                .Where(option => TileRuleLookup.TileRulesDictionary[option.name].WEST == code)
                .ToList();
            tileE.entropy = tileE.validOptions.Count;
        }

        // update south neighbor
        if (tileS != null)
        {
            string code = rules.SOUTH;
            tileS.validOptions = tileS.validOptions
                .Where(option => TileRuleLookup.TileRulesDictionary[option.name].NORTH == code)
                .ToList();
            tileS.entropy = tileS.validOptions.Count;
        }

        // update west neighbor
        if (tileW != null)
        {
            string code = rules.WEST;
            tileW.validOptions = tileW.validOptions
                .Where(option => TileRuleLookup.TileRulesDictionary[option.name].EAST == code)
                .ToList();
            tileW.entropy = tileW.validOptions.Count;
        }

    }

    // get a list of candidates for next collapse
    public List<TileData> FindLowestEntropy()
    {
        List<TileData> candidates = new List<TileData>();
        int lowestEntropy = allTiles.Count;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                TileData tileData = grid[x, y];

                // only consider tiles that aren't collapsed and don't have entropy 0
                if (tileData != null && !tileData.isCollapsed && tileData.entropy >= 1)
                {
                    int tileEntropy = tileData.entropy;
                    if (tileEntropy < lowestEntropy)
                    {
                        lowestEntropy = tileEntropy;
                        candidates.Clear();
                        candidates.Add(tileData);
                    } else if (tileEntropy == lowestEntropy)
                    {
                        candidates.Add(tileData);
                    }
                }
            }
        }

        return candidates;
    }

    private TileBase PickRandomTileBase(List<TileBase> tiles)
    {
        return tiles[Random.Range(0, tiles.Count)];
    }

    public void StartBuildMap()
    {
        StartCoroutine(BuildMapCoroutine());
    }

    public IEnumerator BuildMapCoroutine()
    {
        walkbilityTilemap.ClearAllTiles();

        InitializeTileData();
        InitializeTilemap();

        cc.SetResetPosition(gridWidth, gridHeight);
        cc.ResetCamera();

        bool gridChanged = true;
        while (gridChanged)
        {
            gridChanged = false;

            List<TileData> candidates = FindLowestEntropy();

            if (candidates.Count == 0) break;

            TileData tileToCollapse = candidates[Random.Range(0, candidates.Count)];

            if (!tileToCollapse.isCollapsed)
            {
                CollapseTile(tileToCollapse);
                UpdateNeighbors(tileToCollapse);
                gridChanged = true;
            }

            yield return new WaitForSeconds(0.01f); // change the speed of tile propagation
        }

        InitializeGameGrid();
        MakeWalkabilityGrid();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = gridTilemap.WorldToCell(worldPosition);

        if (gridPosition.x >= 0 && gridPosition.x < gridWidth*2 && gridPosition.y >= 0 && gridPosition.y < gridHeight*2)
        {
            gridUI.ClearAllTiles();
            gridUI.SetTile(gridPosition, selectedTile);

            Debug.Log($"clicked on grid {gridPosition.x}, {gridPosition.y}");

            string resource = gameGrid[gridPosition.x, gridPosition.y];
            if (resource != null)
            {
                Debug.Log(resource);
            }
            
        }
    }

}

// dev hacks for editor
// comment these out when making build
[CustomEditor(typeof(GridManager))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridManager myScript = (GridManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Map"))
        {
            myScript.StartBuildMap();
        }
        if (GUILayout.Button("Clear Tilemap"))
        {
            myScript.tilemap.ClearAllTiles();
        }
        if (GUILayout.Button("Toggle Walkability Grid"))
        {
            bool visibility = myScript.walkbilityTilemap.gameObject.activeSelf;
            myScript.walkbilityTilemap.gameObject.SetActive(!visibility);
        }
    }
}