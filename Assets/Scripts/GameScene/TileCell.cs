using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates;
    public Tile tile;

    public bool Empty => tile.isDefault == true;
    public bool Occupied => tile.isDefault == false;
}
