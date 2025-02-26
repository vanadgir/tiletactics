using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    [SerializeField] public Vector2Int gridPos;
    [SerializeField] public Vector3 worldPos;
    public bool isOnScreen;

    [SerializeField] private bool selected;
    [SerializeField] private int depth;

    public List<Sprite> validOptions;
    public int entropy;
    public bool isCollapsed;

    [SerializeField] private Sprite emptySprite;
    private SpriteRenderer sr;

    // not sure which will be better so here's both
    private char[,] quadrants = new char[2, 2];
    [SerializeField] List<char> quads = new List<char>();

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
/*        transform.localScale = Vector3.one * GridManager.Instance.gridScale;

        worldPos.x = (GridManager.Instance.gridScale * gridPos.x);
        worldPos.y = (GridManager.Instance.gridScale * gridPos.y);
        worldPos.z = depth;

        transform.localPosition = worldPos;*/

        ResetTile();
    }

    private void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);
        isOnScreen = viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1;
    }

    public void ResetTile()
    {
        GetComponent<SpriteRenderer>().sprite = emptySprite;
        // validOptions = new List<Sprite>(GridManager.Instance.allSprites);
        entropy = validOptions.Count;
        isCollapsed = false;
    }

    public void CollapseTile()
    {
        if (isCollapsed) return;

        Sprite tileSprite = PickRandomSprite(validOptions);

        sr.sprite = tileSprite;
        validOptions = new List<Sprite>() { tileSprite };
        isCollapsed = true;
        entropy = 1;

        GetQuadrantsFromSprite();
    }

    private Sprite PickRandomSprite(List<Sprite> sprites)
    {
        return sprites[Random.Range(0, sprites.Count)];
    }

    private void GetQuadrantsFromSprite()
    {
        if (sr.sprite == null) return;

        string top = TileRuleLookup.TileRulesDictionary[sr.sprite.name].NORTH;
        string bottom = TileRuleLookup.TileRulesDictionary[sr.sprite.name].SOUTH;

        quadrants[0, 0] = top[0];
        quadrants[0, 1] = top[1];
        quadrants[1, 0] = bottom[0];
        quadrants[1, 1] = bottom[1];

        quads.Clear();
        quads.Add(top[0]);
        quads.Add(top[1]);
        quads.Add(bottom[0]);
        quads.Add(bottom[1]);
    }

}