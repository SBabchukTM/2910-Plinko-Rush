using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

[ExecuteAlways]
public class GridLayoutPercentageGroup : LayoutGroup
{
    public enum ConstraintMode
    {
        FixedColumns,
        FixedRows
    }

    public enum AlignmentMode
    {
        Start,  // Left or Top
        Center,
        End     // Right or Bottom
    }

    [Header("Grid Constraint")]
    [SerializeField] private ConstraintMode constraintMode = ConstraintMode.FixedColumns;
    [SerializeField, Min(1)] private int fixedCount = 2;

    [Header("Spacing")]
    [SerializeField, Range(0f, 0.2f)] private float spacingPercent = 0.02f;

    [Header("Padding")]
    [SerializeField, Range(0f, 0.5f)] private float leftOffsetPercent = 0.05f;
    [SerializeField, Range(0f, 0.5f)] private float rightOffsetPercent = 0.05f;
    [SerializeField, Range(0f, 0.5f)] private float topOffsetPercent = 0.05f;
    [SerializeField, Range(0f, 0.5f)] private float bottomOffsetPercent = 0.05f;

    [Header("Alignment for Incomplete Rows/Columns")]
    [SerializeField] private AlignmentMode incompleteAlignment = AlignmentMode.Center;

    private bool IsFixedColumns => constraintMode == ConstraintMode.FixedColumns;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        ArrangeElements();
    }

    public override void CalculateLayoutInputVertical()
    {
        ArrangeElements();
    }

    public override void SetLayoutHorizontal()
    {
        ArrangeElements();
    }

    public override void SetLayoutVertical()
    {
        ArrangeElements();
    }

    private void ArrangeElements()
    {
        int childCount = rectChildren.Count;
        if (childCount == 0) return;

        ConfigureRectTransform();

        int columns = CalculateColumns(childCount);
        int rows = CalculateRows(childCount, columns);

        float spacing = rectTransform.rect.width * spacingPercent;

        float[] columnWidths = CalculateColumnWidths(columns, spacing);
        float[] rowHeights = CalculateRowHeights(rows, spacing);

        PositionChildren(columns, rows, columnWidths, rowHeights, spacing);
    }

    private void ConfigureRectTransform()
    {
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
    }

    private int CalculateColumns(int childCount)
    {
        return IsFixedColumns ? fixedCount : Mathf.CeilToInt((float)childCount / fixedCount);
    }

    private int CalculateRows(int childCount, int columns)
    {
        return IsFixedColumns ? Mathf.CeilToInt((float)childCount / columns) : fixedCount;
    }

    private float[] CalculateColumnWidths(int columns, float spacing)
    {
        float parentWidth = rectTransform.rect.width;
        float totalHorizontalPadding = parentWidth * (leftOffsetPercent + rightOffsetPercent);
        float availableWidth = parentWidth - totalHorizontalPadding - (columns - 1) * spacing;

        float columnWidth = availableWidth / columns;
        return Enumerable.Repeat(columnWidth, columns).ToArray();
    }

    private float[] CalculateRowHeights(int rows, float spacing)
    {
        float parentHeight = rectTransform.rect.height;
        float totalVerticalPadding = parentHeight * (topOffsetPercent + bottomOffsetPercent);
        float availableHeight = parentHeight - totalVerticalPadding - (rows - 1) * spacing;

        float rowHeight = availableHeight / rows;
        return Enumerable.Repeat(rowHeight, rows).ToArray();
    }

    private void PositionChildren(int columns, int rows, float[] columnWidths, float[] rowHeights, float spacing)
    {
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float xStart = padding.left + parentWidth * leftOffsetPercent;
        float yStart = -padding.top - parentHeight * topOffsetPercent;

        if (IsFixedColumns)
        {
            for (int row = 0; row < rows; row++)
            {
                int itemsInThisRow = (row == rows - 1) ? Mathf.Min(rectChildren.Count - row * columns, columns) : columns;

                float rowWidth = columnWidths.Take(itemsInThisRow).Sum() + spacing * (itemsInThisRow - 1);
                float alignmentOffset = GetAlignmentOffset(columns, itemsInThisRow, columnWidths[0] + spacing, incompleteAlignment);

                for (int col = 0; col < itemsInThisRow; col++)
                {
                    int index = row * columns + col;
                    if (index >= rectChildren.Count) break;

                    RectTransform child = rectChildren[index];

                    float xPos = xStart + alignmentOffset + columnWidths.Take(col).Sum() + col * spacing;
                    float yPos = yStart - row * (rowHeights[row] + spacing);

                    SetChildAlongAxis(child, 0, xPos, columnWidths[col]);
                    SetChildAlongAxis(child, 1, -yPos, rowHeights[row]);
                }
            }
        }
        else
        {
            for (int col = 0; col < columns; col++)
            {
                int itemsInThisCol = (col == columns - 1) ? Mathf.Min(rectChildren.Count - col * rows, rows) : rows;

                float colHeight = rowHeights.Take(itemsInThisCol).Sum() + spacing * (itemsInThisCol - 1);
                float alignmentOffset = GetAlignmentOffset(rows, itemsInThisCol, rowHeights[0] + spacing, incompleteAlignment);

                for (int row = 0; row < itemsInThisCol; row++)
                {
                    int index = col * rows + row;
                    if (index >= rectChildren.Count) break;

                    RectTransform child = rectChildren[index];

                    float xPos = xStart + col * (columnWidths[col] + spacing);
                    float yPos = yStart - alignmentOffset - row * (rowHeights[row] + spacing);

                    SetChildAlongAxis(child, 0, xPos, columnWidths[col]);
                    SetChildAlongAxis(child, 1, -yPos, rowHeights[row]);
                }
            }
        }
    }

    private float GetAlignmentOffset(int fullCount, int actualCount, float cellWithSpacing, AlignmentMode mode)
    {
        int missing = fullCount - actualCount;

        switch (mode)
        {
            case AlignmentMode.Center:
                return missing * cellWithSpacing / 2f;
            case AlignmentMode.End:
                return missing * cellWithSpacing;
            case AlignmentMode.Start:
            default:
                return 0f;
        }
    }
}