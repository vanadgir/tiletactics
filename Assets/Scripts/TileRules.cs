using System.Collections.Generic;

public class TileRules
{
    public string NORTH { get; set; }
    public string EAST { get; set; }
    public string SOUTH { get; set; }
    public string WEST { get; set; }
}

public class TileRuleLookup
{
    public static readonly Dictionary<string, TileRules> TileRulesDictionary = new()
    {
        // Dirt
        { "Tile 01_0", new TileRules { NORTH = "GG", EAST = "GS", SOUTH = "GS", WEST = "GG" } },
        { "Tile 01_1", new TileRules { NORTH = "GG", EAST = "GS", SOUTH = "SS", WEST = "GS" } },
        { "Tile 01_2", new TileRules { NORTH = "GG", EAST = "GS", SOUTH = "SS", WEST = "GS" } },
        { "Tile 01_3", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "SG", WEST = "GS" } },
        { "Tile 01_4", new TileRules { NORTH = "GS", EAST = "SS", SOUTH = "GS", WEST = "GG" } },
        { "Tile 01_5", new TileRules { NORTH = "SS", EAST = "SS", SOUTH = "SS", WEST = "SS" } },
        { "Tile 01_6", new TileRules { NORTH = "SS", EAST = "SS", SOUTH = "SS", WEST = "SS" } },
        { "Tile 01_7", new TileRules { NORTH = "SG", EAST = "GG", SOUTH = "SG", WEST = "SS" } },
        { "Tile 01_8", new TileRules { NORTH = "GS", EAST = "SS", SOUTH = "GS", WEST = "GG" } },
        { "Tile 01_9", new TileRules { NORTH = "SS", EAST = "SS", SOUTH = "SS", WEST = "SS" } },
        { "Tile 01_10", new TileRules { NORTH = "SS", EAST = "SS", SOUTH = "SS", WEST = "SS" } },
        { "Tile 01_11", new TileRules { NORTH = "SG", EAST = "GG", SOUTH = "SG", WEST = "SS" } },
        { "Tile 01_12", new TileRules { NORTH = "GS", EAST = "SG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 01_13", new TileRules { NORTH = "SS", EAST = "SG", SOUTH = "GG", WEST = "SG" } },
        { "Tile 01_14", new TileRules { NORTH = "SS", EAST = "SG", SOUTH = "GG", WEST = "SG" } },
        { "Tile 01_15", new TileRules { NORTH = "SG", EAST = "GG", SOUTH = "GG", WEST = "SG" } },

        // Sand
        { "Tile 02_0", new TileRules { NORTH = "SS", EAST = "SG", SOUTH = "SG", WEST = "SS" } },
        { "Tile 02_1", new TileRules { NORTH = "SS", EAST = "SG", SOUTH = "GG", WEST = "SG" } },
        { "Tile 02_2", new TileRules { NORTH = "SS", EAST = "SG", SOUTH = "GG", WEST = "SG" } },
        { "Tile 02_3", new TileRules { NORTH = "SS", EAST = "SS", SOUTH = "GS", WEST = "SG" } },
        { "Tile 02_4", new TileRules { NORTH = "SG", EAST = "GG", SOUTH = "SG", WEST = "SS" } },
        { "Tile 02_5", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 02_6", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 02_7", new TileRules { NORTH = "GS", EAST = "SS", SOUTH = "GS", WEST = "GG" } },
        { "Tile 02_8", new TileRules { NORTH = "SG", EAST = "GG", SOUTH = "SG", WEST = "SS" } },
        { "Tile 02_9", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 02_10", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 02_11", new TileRules { NORTH = "GS", EAST = "SS", SOUTH = "GS", WEST = "GG" } },
        { "Tile 02_12", new TileRules { NORTH = "SG", EAST = "GS", SOUTH = "SS", WEST = "SS" } },
        { "Tile 02_13", new TileRules { NORTH = "GG", EAST = "GS", SOUTH = "SS", WEST = "GS" } },
        { "Tile 02_14", new TileRules { NORTH = "GG", EAST = "GS", SOUTH = "SS", WEST = "GS" } },
        { "Tile 02_15", new TileRules { NORTH = "GS", EAST = "SS", SOUTH = "SS", WEST = "GS" } },

        // Brush
        { "Tile 03_0", new TileRules { NORTH = "GG", EAST = "GB", SOUTH = "GB", WEST = "GG" } },
        { "Tile 03_1", new TileRules { NORTH = "GG", EAST = "GB", SOUTH = "BB", WEST = "GB" } },
        { "Tile 03_2", new TileRules { NORTH = "GG", EAST = "GB", SOUTH = "BB", WEST = "GB" } },
        { "Tile 03_3", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "BG", WEST = "GB" } },
        { "Tile 03_4", new TileRules { NORTH = "GB", EAST = "BB", SOUTH = "GB", WEST = "GG" } },
        { "Tile 03_5", new TileRules { NORTH = "BB", EAST = "BB", SOUTH = "BB", WEST = "BB" } },
        { "Tile 03_6", new TileRules { NORTH = "BB", EAST = "BB", SOUTH = "BB", WEST = "BB" } },
        { "Tile 03_7", new TileRules { NORTH = "BG", EAST = "GG", SOUTH = "BG", WEST = "BB" } },
        { "Tile 03_8", new TileRules { NORTH = "GB", EAST = "BB", SOUTH = "GB", WEST = "GG" } },
        { "Tile 03_9", new TileRules { NORTH = "BB", EAST = "BB", SOUTH = "BB", WEST = "BB" } },
        { "Tile 03_10", new TileRules { NORTH = "BB", EAST = "BB", SOUTH = "BB", WEST = "BB" } },
        { "Tile 03_11", new TileRules { NORTH = "BG", EAST = "GG", SOUTH = "BG", WEST = "BB" } },
        { "Tile 03_12", new TileRules { NORTH = "GB", EAST = "BG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 03_13", new TileRules { NORTH = "BB", EAST = "BG", SOUTH = "GG", WEST = "BG" } },
        { "Tile 03_14", new TileRules { NORTH = "BB", EAST = "BG", SOUTH = "GG", WEST = "BG" } },
        { "Tile 03_15", new TileRules { NORTH = "BG", EAST = "GG", SOUTH = "GG", WEST = "BG" } },

        // Grass
        { "Tile 04_0", new TileRules { NORTH = "BB", EAST = "BG", SOUTH = "BG", WEST = "BB" } },
        { "Tile 04_1", new TileRules { NORTH = "BB", EAST = "BG", SOUTH = "GG", WEST = "BG" } },
        { "Tile 04_2", new TileRules { NORTH = "BB", EAST = "BG", SOUTH = "GG", WEST = "BG" } },
        { "Tile 04_3", new TileRules { NORTH = "BB", EAST = "BB", SOUTH = "GB", WEST = "BG" } },
        { "Tile 04_4", new TileRules { NORTH = "BG", EAST = "GG", SOUTH = "BG", WEST = "BB" } },
        { "Tile 04_5", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 04_6", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 04_7", new TileRules { NORTH = "GB", EAST = "BB", SOUTH = "GB", WEST = "GG" } },
        { "Tile 04_8", new TileRules { NORTH = "BG", EAST = "GG", SOUTH = "BG", WEST = "BB" } },
        { "Tile 04_9", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 04_10", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 04_11", new TileRules { NORTH = "GB", EAST = "BB", SOUTH = "GB", WEST = "GG" } },
        { "Tile 04_12", new TileRules { NORTH = "BG", EAST = "GB", SOUTH = "BB", WEST = "BB" } },
        { "Tile 04_13", new TileRules { NORTH = "GG", EAST = "GB", SOUTH = "BB", WEST = "GB" } },
        { "Tile 04_14", new TileRules { NORTH = "GG", EAST = "GB", SOUTH = "BB", WEST = "GB" } },
        { "Tile 04_15", new TileRules { NORTH = "GB", EAST = "BB", SOUTH = "BB", WEST = "GB" } },

        // Pond
        { "Water01_0", new TileRules { NORTH = "GG", EAST = "GW", SOUTH = "GW", WEST = "GG" } },
        { "Water01_1", new TileRules { NORTH = "GG", EAST = "GW", SOUTH = "WW", WEST = "GW" } },
        { "Water01_2", new TileRules { NORTH = "GG", EAST = "GW", SOUTH = "WW", WEST = "GW" } },
        { "Water01_3", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "WG", WEST = "GW" } },
        { "Water01_4", new TileRules { NORTH = "GW", EAST = "WW", SOUTH = "GW", WEST = "GG" } },
        { "Water01_5", new TileRules { NORTH = "WW", EAST = "WW", SOUTH = "WW", WEST = "WW" } },
        { "Water01_6", new TileRules { NORTH = "WW", EAST = "WW", SOUTH = "WW", WEST = "WW" } },
        { "Water01_7", new TileRules { NORTH = "WG", EAST = "GG", SOUTH = "WG", WEST = "WW" } },
        { "Water01_8", new TileRules { NORTH = "GW", EAST = "WW", SOUTH = "GW", WEST = "GG" } },
        { "Water01_9", new TileRules { NORTH = "WW", EAST = "WW", SOUTH = "WW", WEST = "WW" } },
        { "Water01_10", new TileRules { NORTH = "WW", EAST = "WW", SOUTH = "WW", WEST = "WW" } },
        { "Water01_11", new TileRules { NORTH = "WG", EAST = "GG", SOUTH = "WG", WEST = "WW" } },
        { "Water01_12", new TileRules { NORTH = "GW", EAST = "WG", SOUTH = "GG", WEST = "GG" } },
        { "Water01_13", new TileRules { NORTH = "WW", EAST = "WG", SOUTH = "GG", WEST = "WG" } },
        { "Water01_14", new TileRules { NORTH = "WW", EAST = "WG", SOUTH = "GG", WEST = "WG" } },
        { "Water01_15", new TileRules { NORTH = "WG", EAST = "GG", SOUTH = "GG", WEST = "WG" } },

        // Island
        { "Water02_0", new TileRules { NORTH = "WW", EAST = "WG", SOUTH = "WG", WEST = "WW" } },
        { "Water02_1", new TileRules { NORTH = "WW", EAST = "WG", SOUTH = "GG", WEST = "WG" } },
        { "Water02_2", new TileRules { NORTH = "WW", EAST = "WG", SOUTH = "GG", WEST = "WG" } },
        { "Water02_3", new TileRules { NORTH = "WW", EAST = "WW", SOUTH = "GW", WEST = "WG" } },
        { "Water02_4", new TileRules { NORTH = "WG", EAST = "GG", SOUTH = "WG", WEST = "WW" } },
        { "Water02_5", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Water02_6", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Water02_7", new TileRules { NORTH = "GW", EAST = "WW", SOUTH = "GW", WEST = "GG" } },
        { "Water02_8", new TileRules { NORTH = "WG", EAST = "GG", SOUTH = "WG", WEST = "WW" } },
        { "Water02_9", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Water02_10", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Water02_11", new TileRules { NORTH = "GW", EAST = "WW", SOUTH = "GW", WEST = "GG" } },
        { "Water02_12", new TileRules { NORTH = "WG", EAST = "GW", SOUTH = "WW", WEST = "WW" } },
        { "Water02_13", new TileRules { NORTH = "GG", EAST = "GW", SOUTH = "WW", WEST = "GW" } },
        { "Water02_14", new TileRules { NORTH = "GG", EAST = "GW", SOUTH = "WW", WEST = "GW" } },
        { "Water02_15", new TileRules { NORTH = "GW", EAST = "WW", SOUTH = "WW", WEST = "GW" } },

        // Alt Grass
        { "Tile 06_0", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 06_1", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 06_2", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
        { "Tile 06_3", new TileRules { NORTH = "GG", EAST = "GG", SOUTH = "GG", WEST = "GG" } },
    };
}
