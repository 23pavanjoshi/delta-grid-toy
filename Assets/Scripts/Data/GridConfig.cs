using System.Collections.Generic;

[System.Serializable]
public class GridConfig
{
    public int columns;
    public int rows;

    public int TotalCards  => columns * rows;
    public bool IsOddGrid  => TotalCards % 2 != 0;
    public string Label    => $"{columns}x{rows}";

    public GridConfig(int columns, int rows)
    {
        this.columns = columns;
        this.rows    = rows;
    }
    
    public static readonly List<GridConfig> SupportedLayouts = new()
    {
        new GridConfig(2, 2),
        new GridConfig(2, 3),
        new GridConfig(3, 3),
        new GridConfig(4, 3),
        new GridConfig(4, 4),
        new GridConfig(5, 3),
        new GridConfig(5, 4),
        new GridConfig(5, 5),
    };
}