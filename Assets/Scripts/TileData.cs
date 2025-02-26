using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData 
{
    public string name;
    public Vector2Int gridPos;
    public string[,] quadrants;
    public List<string> quadrantsList;

    public List<TileBase> validOptions;
    public int entropy;
    public bool isCollapsed;

    public bool isOnScreen;

    public TileData (int x, int y)
    {
        this.gridPos = new Vector2Int(x, y);
        this.quadrants = new string[2, 2];
        this.quadrantsList = new List<string>();

        ResetData();
    }

    public void ResetData()
    {
        validOptions = GridManager.Instance.allTiles;
        entropy = validOptions.Count;
        isCollapsed = false;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public void SetQuadrantsFromName(string name)
    {
        var tileRule = TileRuleLookup.TileRulesDictionary.GetValueOrDefault(name);

        if (tileRule != null)
        {
            string top = tileRule.NORTH;
            string bottom = tileRule.SOUTH;

            quadrants[0, 0] = top[0].ToString();
            quadrants[0, 1] = top[1].ToString();
            quadrants[1, 0] = bottom[0].ToString();
            quadrants[1, 1] = bottom[1].ToString();

            quadrantsList.Clear();
            quadrantsList.Add(top[0].ToString());
            quadrantsList.Add(top[1].ToString());
            quadrantsList.Add(bottom[0].ToString());
            quadrantsList.Add(bottom[1].ToString());
        }
        else
        {
            SetEmptyQuadrants();
        }

    }

    private void SetEmptyQuadrants()
    {
        quadrants[0, 0] = "";
        quadrants[0, 1] = "";
        quadrants[1, 0] = "";
        quadrants[1, 1] = "";

        quadrantsList.Clear();
        quadrantsList.Add("");
        quadrantsList.Add("");
        quadrantsList.Add("");
        quadrantsList.Add("");
    }

}
