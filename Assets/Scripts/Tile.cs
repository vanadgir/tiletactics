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

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        transform.localScale = Vector3.one * GridManager.Instance.gridScale;

        worldPos.x = (GridManager.Instance.gridScale * gridPos.x);
        worldPos.y = (GridManager.Instance.gridScale * gridPos.y);
        worldPos.z = depth;

        transform.localPosition = worldPos;

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
        validOptions = new List<Sprite>(GridManager.Instance.allSprites);
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
    }

    private Sprite PickRandomSprite(List<Sprite> sprites)
    {
        return sprites[Random.Range(0, sprites.Count)];
    }

}