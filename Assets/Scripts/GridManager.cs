using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Singleton
    private static GridManager _instance;

    public static GridManager Instance { get { return _instance; } }
    #endregion

    [Space(5)]
    [Header("Grid Properties")]
    [SerializeField] public float gridScale;
    [SerializeField] public int gridWidth; 
    [SerializeField] public int gridHeight; 

    [Space(5)]
    [Header("Tile Prefab")]
    [SerializeField] private GameObject tile;

    [Space(5)]
    [Header("Sprites")]
    public List<Sprite> allSprites; // drag the 100 sliced sprites in editor

    private GameObject[,] grid;

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

        InitializeGrid();
    }

    private void Start()
    {
        // map building is a coroutine so it doesn't hang ur whole editor
        StartCoroutine(BuildMapCoroutine());
    }


    private void InitializeGrid()
    {
        grid = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject newTile = Instantiate(tile, transform);
                newTile.name = $"Tile {x}, {y}";
                grid[x, y] = newTile;

                Tile tileComponent = newTile.GetComponent<Tile>();
                tileComponent.gridPos = new Vector2Int(x, y);
                tileComponent.ResetTile();
            }
        }
    }

    public void ResetGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject go = grid[x, y];
                if (go != null)
                {
                    go.GetComponent<Tile>().ResetTile();
                }
            }
        }
    }

    private void CollapseRandom()
    {
        int randomX = Random.Range(0, gridWidth);
        int randomY = Random.Range(0, gridHeight);

        GameObject go = grid[randomX, randomY];

        go.GetComponent<Tile>().CollapseTile();
        UpdateNeighbors(go);
    }

    public void UpdateNeighbors(GameObject go)
    {
        Tile tile = go.GetComponent<Tile>();
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

        // neighbor will be null if at the bounds of grid
        GameObject tileN = tile.gridPos.y < gridHeight - 1 ? grid[tile.gridPos.x, tile.gridPos.y + 1] : null;
        GameObject tileE = tile.gridPos.x < gridWidth - 1 ? grid[tile.gridPos.x + 1, tile.gridPos.y] : null;
        GameObject tileS = tile.gridPos.y > 0 ? grid[tile.gridPos.x, tile.gridPos.y - 1] : null;
        GameObject tileW = tile.gridPos.x > 0 ? grid[tile.gridPos.x - 1, tile.gridPos.y] : null;

        string spriteName = sr.sprite.name;

        // skip whole thing if key not found
        if (!TileRuleLookup.TileRulesDictionary.ContainsKey(spriteName)) return;

        var rules = TileRuleLookup.TileRulesDictionary.GetValueOrDefault(spriteName);

        // update north neighbor
        if (tileN != null)
        {
            Tile tileNComp = tileN.GetComponent<Tile>();
            if (tileNComp != null)
            {
                string code = rules?.NORTH;
                tileNComp.validOptions = tileNComp.validOptions
                    .Where(option => TileRuleLookup.TileRulesDictionary[option.name].SOUTH == code)
                    .ToList();
                tileNComp.entropy = tileNComp.validOptions.Count;
            }
        }

        // update east neighbor
        if (tileE != null)
        {
            Tile tileEComp = tileE.GetComponent<Tile>();
            if (tileEComp != null)
            {
                string code = rules?.EAST;
                tileEComp.validOptions = tileEComp.validOptions
                    .Where(option => TileRuleLookup.TileRulesDictionary[option.name].WEST == code)
                    .ToList();
                tileEComp.entropy = tileEComp.validOptions.Count;
            }
        }

        // update south neighbor
        if (tileS != null)
        {
            Tile tileSComp = tileS.GetComponent<Tile>();
            if (tileSComp != null)
            {
                string code = rules?.SOUTH;
                tileSComp.validOptions = tileSComp.validOptions
                    .Where(option => TileRuleLookup.TileRulesDictionary[option.name].NORTH == code)
                    .ToList();
                tileSComp.entropy = tileSComp.validOptions.Count;
            }
        }

        // update west neighbor
        if (tileW != null)
        {
            Tile tileWComp = tileW.GetComponent<Tile>();
            if (tileWComp != null)
            {
                string code = rules?.WEST;
                tileWComp.validOptions = tileWComp.validOptions
                    .Where(option => TileRuleLookup.TileRulesDictionary[option.name].EAST == code)
                    .ToList();
                tileWComp.entropy = tileWComp.validOptions.Count;
            }
        }
    }

    public List<GameObject> FindLowestEntropy()
    {
        List<GameObject> candidates = new List<GameObject>();
        int lowestEntropy = allSprites.Count;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = grid[x, y].GetComponent<Tile>();
                if (tile != null && !tile.isCollapsed && tile.validOptions.Count >= 1)
                {
                    int tileEntropy = tile.entropy;
                    if (tileEntropy < lowestEntropy)
                    {
                        lowestEntropy = tileEntropy;
                        candidates.Clear();
                        candidates.Add(grid[x, y]);
                    }
                    else if (tileEntropy == lowestEntropy)
                    {
                        candidates.Add(grid[x, y]);
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

    private IEnumerator BuildMapCoroutine()
    {
        ResetGrid();
        CollapseRandom(); 

        bool gridChanged = true;
        while (gridChanged)
        {
            gridChanged = false;

            List<GameObject> candidates = FindLowestEntropy();

            if (candidates.Count == 0)
                break;

            GameObject tileToCollapse = candidates[Random.Range(0, candidates.Count)];
            Tile tileComponent = tileToCollapse.GetComponent<Tile>();

            if (!tileComponent.isCollapsed)
            {
                tileComponent.CollapseTile();
                UpdateNeighbors(tileToCollapse);
                gridChanged = true;
            }

            yield return new WaitForSeconds(0.01f);
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
    }
}