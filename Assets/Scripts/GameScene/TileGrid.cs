using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows;
    public TileCell[] cells;

    public int Size => cells.Length;
    public int Height => rows.Length;
    public int Width => Size / Height;

    public TileCell canLockCell;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].coordinates = new Vector2Int(i / Width, i % Width);
        }
    }

    public bool CanSelected(TileCell cell)
    {
        return cell.coordinates.x == 0 || cell.coordinates.x == 4 || cell.coordinates.y == 0 || cell.coordinates.y == 4;
    }

    public bool CanSwap(TileCell cell, TileCell target)
    {
        if(cell.coordinates.x == target.coordinates.x && cell != target && !target.tile.isLock)
        {
            return target.coordinates.y == 0 || target.coordinates.y == 4;
        }
        else if(cell.coordinates.y == target.coordinates.y && cell != target && !target.tile.isLock)
        {
            return target.coordinates.x == 0 || target.coordinates.x == 4;
        }
        return false;
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    public TileCell GetCell(int x, int y)
    {
        if (y >= 0 && y < Width && x >= 0 && x < Height) 
        {
            return rows[x].cells[y];
        } 
        else 
        {
            return null;
        }
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }
}
