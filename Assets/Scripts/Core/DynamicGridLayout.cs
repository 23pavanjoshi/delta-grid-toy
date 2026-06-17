using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridLayout : MonoBehaviour
{
    private GridLayoutGroup _grid;
    private RectTransform _rt;

    private void Awake()
    {
        _grid = GetComponent<GridLayoutGroup>();
        _rt = GetComponent<RectTransform>();
    }

    public void SetupGrid(int columns, int rows, float spacing)
    {
        var containerWidth = _rt.rect.width;
        var containerHeight = _rt.rect.height;

        var cardW = (containerWidth - (columns + 1) * spacing) / columns;
        var cardH = (containerHeight - (rows + 1) * spacing) / rows;
        var cardSize = Mathf.Min(cardW, cardH);
        var roundSpacing = Mathf.RoundToInt(spacing);
        
        _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _grid.constraintCount = columns;
        _grid.cellSize = new Vector2(cardSize, cardSize);
        _grid.spacing = new Vector2(spacing, spacing);
        _grid.padding = new RectOffset(roundSpacing, roundSpacing, roundSpacing, roundSpacing);
    }
}