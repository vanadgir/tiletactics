using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTile : Tile
{
    public Vector2Int gridPos;

    // WFC variables
    public List<Sprite> validOptions;
    public int Entropy => validOptions.Count;

    public bool isCollapsed;

    // used to pass terrain info down to subgrid
    public string[,] quadrants = new string[,] { { "", "" }, { "", "" } };

    public void Initialize(Vector2Int position)
    {
        gridPos = position;
        ResetData();
    }

    public void ResetData()
    {
        sprite = GameManager.Instance.emptySprite;
        validOptions = GameManager.Instance.allSprites;
        isCollapsed = false;
    }

    public Sprite PickRandomSprite()
    {
        return validOptions[Random.Range(0, validOptions.Count)];
    }

    // this makes the tile unable to be picked as a candidate again
    public void CollapseTile()
    {
        if (isCollapsed) return;

        validOptions = new List<Sprite>(){ sprite };
        name = sprite.name;
        isCollapsed = true;
    }

    // this works because the adjacency rules are already based off the terrain
    public string[,] GetTerrain()
    {
        string top = TileRuleLookup.TileRulesDictionary[name].NORTH;
        string bottom = TileRuleLookup.TileRulesDictionary[name].SOUTH;

        quadrants[0, 0] = top != "" ? top[0].ToString(): "";
        quadrants[0, 1] = top != "" ? top[1].ToString() : "";
        quadrants[1, 0] = bottom != "" ? bottom[0].ToString(): "";
        quadrants[1, 1] = bottom != "" ? bottom[1].ToString() : "";

        return quadrants;
    }

}
