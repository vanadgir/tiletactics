using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }
    #endregion

    [Header("World Size")]
    [SerializeField] public int WorldWidth = 25;
    [SerializeField] public int WorldHeight = 25;

    // these will always be 2x the world
    // because game grid is higher resolution
    public int GameWidth => WorldWidth * 2;
    public int GameHeight => WorldHeight * 2;

    [Space(20)]
    [Header("Sprites")]
    public Sprite emptySprite;
    public Sprite selectedSprite;
    public Sprite redSprite;
    public Sprite greenSprite;
    public List<Sprite> allSprites;

    private Tile selectedTile;
    private Tile notWalkable;
    private Tile walkable;

    [Space(20)]
    [Header("Tilemaps")]
    public Tilemap worldTilemap;
    public Tilemap walkbilityTilemap;
    public Tilemap gridUI;

    private Dictionary<Vector2Int, GameTile> gameGrid = new Dictionary<Vector2Int, GameTile>();

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
    }

    private void Start()
    {
        selectedTile = ScriptableObject.CreateInstance<Tile>();
        selectedTile.sprite = selectedSprite;

        notWalkable = ScriptableObject.CreateInstance<Tile>();
        notWalkable.sprite = redSprite;

        walkable = ScriptableObject.CreateInstance<Tile>();
        walkable.sprite = greenSprite;

        StartBuildMap();
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
        Vector3Int gridPosition = walkbilityTilemap.WorldToCell(worldPosition);

        if (gridPosition.x >= 0 && gridPosition.x < GameWidth && gridPosition.y >= 0 && gridPosition.y < GameHeight)
        {
            gridUI.ClearAllTiles();
            gridUI.SetTile(gridPosition, selectedTile);

            string terrain = gameGrid[new Vector2Int(gridPosition.x, gridPosition.y)].terrain;
            if (terrain != null)
            {
                Debug.Log($"{DateTime.Now} - You clicked on {terrain} at {gridPosition.x}, {gridPosition.y}");
            } else
            {
                Debug.Log($"{DateTime.Now} - There is nothing at {gridPosition.x}, {gridPosition.y}");
            }

        }
    }

    // fill the world tilemap with "empty tile" sprites
    // and initialize some default data for WFC
    private void DrawEmptyWorldTilemap()
    {
        worldTilemap.ClearAllTiles();

        for (int x = 0; x < WorldWidth; x++)
        {
            for (int y = 0; y < WorldHeight; y++)
            {
                Vector2Int gridPos = new(x, y);
                WorldTile tile = ScriptableObject.CreateInstance<WorldTile>();

                tile.Initialize(gridPos);

                worldTilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y, 0), tile);
            }
        }
    }

    // populate the dictionary with initialized game tiles
    // currently only holds each one's terrain type
    // TODO: play around with the GameTile class to see what other stuff we want to store
    private void InitializeGameGrid()
    {
        for (int x = 0; x < GameWidth; x++)
        {
            for (int y = 0; y < GameHeight; y++)
            {
                Vector2Int gridPos = new(x, y);
                GameTile tile = ScriptableObject.CreateInstance<GameTile>();

                gameGrid[gridPos] = tile;
            }
        }
    }

    // this is core to WFC as it filters the valid options for neighbors as nodes collapse
    // it causes entropy of these tiles to change, thus influencing the next candidates
    private void UpdateNeighbors(WorldTile tile)
    {
        string tileName = tile.name;

        // skip everything if this name isn't found in rules
        if (!TileRuleLookup.TileRulesDictionary.ContainsKey(tileName)) return;

        int posX = tile.gridPos.x;
        int posY = tile.gridPos.y;

        WorldTile tileN = worldTilemap.GetTile(new Vector3Int(posX, posY + 1, 0)) as WorldTile;
        WorldTile tileE = worldTilemap.GetTile(new Vector3Int(posX + 1, posY, 0)) as WorldTile;
        WorldTile tileS = worldTilemap.GetTile(new Vector3Int(posX, posY - 1, 0)) as WorldTile;
        WorldTile tileW = worldTilemap.GetTile(new Vector3Int(posX - 1, posY, 0)) as WorldTile;

        var rules = TileRuleLookup.TileRulesDictionary.GetValueOrDefault(tileName);

        // update north neighbor
        if (tileN != null)
        {
            string code = rules.NORTH;
            tileN.validOptions = tileN.validOptions
                .Where(option => TileRuleLookup.TileRulesDictionary[option.name].SOUTH == code)
                .ToList();
        }

        // update east neighbor
        if (tileE != null)
        {
            string code = rules.EAST;
            tileE.validOptions = tileE.validOptions
                .Where(option => TileRuleLookup.TileRulesDictionary[option.name].WEST == code)
                .ToList();
        }

        // update south neighbor
        if (tileS != null)
        {
            string code = rules.SOUTH;
            tileS.validOptions = tileS.validOptions
                .Where(option => TileRuleLookup.TileRulesDictionary[option.name].NORTH == code)
                .ToList();
        }

        // update west neighbor
        if (tileW != null)
        {
            string code = rules.WEST;
            tileW.validOptions = tileW.validOptions
                .Where(option => TileRuleLookup.TileRulesDictionary[option.name].EAST == code)
                .ToList();
        }
    }

    // get a list of candidates for next collapse
    public List<WorldTile> FindLowestEntropy()
    {
        List<WorldTile> candidates = new();
        int lowestEntropy = allSprites.Count;

        for (int x = 0; x < WorldWidth; x++) 
        {
            for (int y = 0; y < WorldHeight; y++)
            {
                Vector3Int gridPos = new(x, y, 0);
                WorldTile tile = worldTilemap.GetTile(gridPos) as WorldTile;

                // only consider tiles that aren't collapsed and don't have entropy 0
                if (tile != null && !tile.isCollapsed && tile.Entropy >= 1)
                {
                    int tileEntropy = tile.Entropy;
                    if (tileEntropy < lowestEntropy)
                    {
                        lowestEntropy = tileEntropy;
                        candidates.Clear();
                        candidates.Add(tile);
                    }
                    else if (tileEntropy == lowestEntropy)
                    {
                        candidates.Add(tile);
                    }
                }

            }
        }

        return candidates;
    }

    public void StartBuildMap()
    {
        StartCoroutine(BuildMapCoroutine());
    }

    // this is part that does the WFC loop
    public IEnumerator BuildMapCoroutine()
    {
        walkbilityTilemap.ClearAllTiles();
        gridUI.ClearAllTiles();

        DrawEmptyWorldTilemap();
        InitializeGameGrid();

        bool gridChanged = true;
        while (gridChanged)
        {
            gridChanged = false;

            // WFC Step 1: pick a tile with lowest entropy 
            List<WorldTile> candidates = FindLowestEntropy();

            if (candidates.Count == 0) break;

            // had to namespace this because of System / UnityEngine both having Random
            // and I'm using System for DateTime elsewhere, but prob won't need forever
            WorldTile tileToCollapse = candidates[UnityEngine.Random.Range(0, candidates.Count)];

            if (!tileToCollapse.isCollapsed)
            {
                // WFC Step 2: collapse the tile to make entropy 1
                tileToCollapse.sprite = tileToCollapse.PickRandomSprite();
                tileToCollapse.CollapseTile();

                worldTilemap.SetTile(new Vector3Int(tileToCollapse.gridPos.x, tileToCollapse.gridPos.y, 1), tileToCollapse);
                AssignGameGridTerrain(tileToCollapse);
                
                // WFC Step 3: propagate changes, loop again
                UpdateNeighbors(tileToCollapse);

                gridChanged = true;
            }

            yield return new WaitForSeconds(0.001f); // change the speed of tile propagation
        }

        DrawWalkabilityTilemap();

        yield return new WaitForSeconds(0.01f);
    }

    // this uses the quadrants of the world grid to label the
    // individual game grid tiles (since they are embedded 2x2)
    private void AssignGameGridTerrain(WorldTile collapsedTile)
    {
        int worldX = collapsedTile.gridPos.x;
        int worldY = collapsedTile.gridPos.y;

        string[,] quadrants = collapsedTile.GetTerrain();

        // these might be confusing but basically we convert larger grid coords to subgrid coords
        // Euclidean coords go from (0, 0) bottom left while 2D array/matrices go from (0, 0) top left
        GameTile bottomLeft = gameGrid[new Vector2Int(worldX * 2, worldY * 2)];
        bottomLeft.terrain = quadrants[1, 0];

        GameTile topLeft = gameGrid[new Vector2Int(worldX * 2, worldY * 2 + 1)];
        topLeft.terrain = quadrants[0, 0];

        GameTile bottomRight = gameGrid[new Vector2Int(worldX * 2 + 1, worldY * 2)];
        bottomRight.terrain = quadrants[1, 1];

        GameTile topRight = gameGrid[new Vector2Int(worldX * 2 + 1, worldY * 2 + 1)];
        topRight.terrain = quadrants[0, 1];
    }
    
    // just assigns red or green tile so we can verify walkability visually
    // no actual data gets saved beyond just knowing the terrain label
    private void DrawWalkabilityTilemap()
    {
        for (int x = 0; x < GameWidth; x++)
        {
            for (int y = 0; y < GameHeight; y++)
            {
                Vector2Int gameGridPos = new(x, y);
                string terrain = gameGrid[gameGridPos].terrain;

                switch (terrain)
                {
                    case "G":         
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), walkable);
                        break;
                    case "S":
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), walkable);
                        break;
                    case "B":
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), notWalkable);
                        break;
                    case "W":
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), notWalkable);
                        break;
                    default:
                        walkbilityTilemap.SetTile(new Vector3Int(x, y, 0), notWalkable);
                        break;
                }
            }
        }
    }
}

// dev hacks for editor
// comment these out when making build
[CustomEditor(typeof(GameManager))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager myScript = (GameManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Map"))
        {
            myScript.StartBuildMap();
        }
        if (GUILayout.Button("Clear Tilemaps"))
        {
            myScript.worldTilemap.ClearAllTiles();
            myScript.walkbilityTilemap.ClearAllTiles();
            myScript.gridUI.ClearAllTiles();
        }
        if (GUILayout.Button("Toggle Walkability Grid"))
        {
            bool visibility = myScript.walkbilityTilemap.gameObject.activeSelf;
            myScript.walkbilityTilemap.gameObject.SetActive(!visibility);
        }
    }
}