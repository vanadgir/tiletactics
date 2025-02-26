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
    public Tilemap tilemap;
    public TileBase emptyTile;
    public List<TileBase> allTiles; // drag all the Tiles into editor here

    private TileData[,] grid;

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
    private void InitializeDataGrid()
    {
        grid = new TileData[gridWidth, gridHeight];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                TileData tileData = new TileData(x, y);

                grid[x, y] = tileData;
            }
        }
    }

    // this is Unity's tilemap and renderer
    // primarily for visuals
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
        InitializeDataGrid();
        InitializeTilemap();

        cc.SetResetPosition(gridWidth / 2f, gridHeight / 2f);
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
    }
}