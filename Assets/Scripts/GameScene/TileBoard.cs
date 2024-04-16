using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public int count;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Tile tilePrefab;

    [SerializeField] private GameObject canTargetPrefab;
    [SerializeField] private GameObject canLockPrefab;
    [SerializeField] private GameObject lockedPrefab;

    private TileGrid grid;
    private List<Tile> tiles;
    private List<GameObject> canTargets;
    private List<GameObject> canLocks;
    private List<GameObject> lockeds;
    public bool waiting;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(count);

        canTargets = new List<GameObject>();
        canLocks = new List<GameObject>();
        lockeds = new List<GameObject>();
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTiles()
    {
        foreach(TileCell cell in grid.cells)
        {
            Tile tile = Instantiate(tilePrefab, grid.transform);
            tile.isDefault = true;
            tile.board = this;
            tile.grid = grid;
            tile.gameManager = gameManager;
            tile.SetDefault();
            tile.Spawn(grid.GetCell(cell.coordinates));
            tiles.Add(tile);
        }
    }

    public void ShowCanLockCell(TileCell cellTarget)
    {
        foreach(GameObject canLock in canLocks)
        {
            Destroy(canLock);
        }
        canLocks.Clear();
        foreach(GameObject locked in lockeds)
        {
            Destroy(locked);
        }
        lockeds.Clear();

        if(cellTarget.coordinates.x == 0)
        {
            GameObject available = Instantiate(canLockPrefab, grid.transform);
            available.transform.position = cellTarget.transform.position + new Vector3(0,1,0);
            canLocks.Add(available);
        }
        else if(cellTarget.coordinates.x == 4)
        {
            GameObject available = Instantiate(canLockPrefab, grid.transform);
            available.transform.position = cellTarget.transform.position - new Vector3(0,1,0);
            canLocks.Add(available);
        }

        if(cellTarget.coordinates.y == 0)
        {
            GameObject available = Instantiate(canLockPrefab, grid.transform);
            available.transform.position = cellTarget.transform.position - new Vector3(1,0,0);
            canLocks.Add(available);
        }
        else if(cellTarget.coordinates.y == 4)
        {
            GameObject available = Instantiate(canLockPrefab, grid.transform);
            available.transform.position = cellTarget.transform.position + new Vector3(1,0,0);
            canLocks.Add(available);
        }
    }

    public void ShowLockedCell(TileCell cellTarget)
    {
        foreach(GameObject canLock in canLocks)
        {
            Destroy(canLock);
        }
        canLocks.Clear();
        foreach(GameObject locked in lockeds)
        {
            Destroy(locked);
        }
        lockeds.Clear();

        if(cellTarget.coordinates.x == 0)
        {
            GameObject available = Instantiate(lockedPrefab, grid.transform);
            available.transform.position = cellTarget.transform.position + new Vector3(0,1,0);
            lockeds.Add(available);
        }
        else if(cellTarget.coordinates.x == 4)
        {
            GameObject available = Instantiate(lockedPrefab, grid.transform);
            available.transform.position = cellTarget.transform.position - new Vector3(0,1,0);
            lockeds.Add(available);
        }

        if(cellTarget.coordinates.y == 0)
        {
            GameObject available = Instantiate(lockedPrefab, grid.transform);
            available.transform.position = cellTarget.transform.position - new Vector3(1,0,0);
            lockeds.Add(available);
        }
        else if(cellTarget.coordinates.y == 4)
        {
            GameObject available = Instantiate(lockedPrefab, grid.transform);
            available.transform.position = cellTarget.transform.position + new Vector3(1,0,0);
            lockeds.Add(available);
        }
    }

    public void ShowCanTargetCell(TileCell cellChoose, bool show)
    {
        if(show)
        {
            foreach(TileCell cell in grid.cells)
            {
                if(grid.CanSwap(cellChoose, cell))
                {
                    if(cell.coordinates.x == cellChoose.coordinates.x)
                    {
                        if(cell.coordinates.y == 0)
                        {
                            GameObject available = Instantiate(canTargetPrefab, grid.transform);
                            available.transform.position = cell.transform.position - new Vector3(1,0,0);
                            canTargets.Add(available);
                        }
                        else if(cell.coordinates.y == 4)
                        {
                            GameObject available = Instantiate(canTargetPrefab, grid.transform);
                            available.transform.position = cell.transform.position + new Vector3(1,0,0);
                            canTargets.Add(available);
                        }
                    }
                    if(cell.coordinates.y == cellChoose.coordinates.y)
                    {
                        if(cell.coordinates.x == 0)
                        {
                            GameObject available = Instantiate(canTargetPrefab, grid.transform);
                            available.transform.position = cell.transform.position + new Vector3(0,1,0);
                            canTargets.Add(available);
                        }
                        else if(cell.coordinates.x == 4)
                        {
                            GameObject available = Instantiate(canTargetPrefab, grid.transform);
                            available.transform.position = cell.transform.position - new Vector3(0,1,0);
                            canTargets.Add(available);
                        }
                    }
                }
            }
        }
        else
        {
            foreach(GameObject target in canTargets)
            {
                Destroy(target);
            }
            canTargets.Clear();
        }
    }

    public void Swap(TileCell cellChoose, TileCell cellTarget)
    {
        // Lock Setting
        waiting = true;
        UnlockAllTiles();

        if(gameManager.turn == GameManager.Turn.Sun && (cellChoose.Empty || cellChoose.tile.isSun) && grid.CanSelected(cellChoose))
        {
            cellChoose.tile.isSun = true;
            cellChoose.tile.isMoon = false;
            cellChoose.tile.isDefault = false;

            if(cellTarget.tile.isMoon && cellTarget == grid.canLockCell)
            {
                cellChoose.tile.isLock = true;
                cellChoose.tile.SetSunLock();
                ShowLockedCell(cellTarget);
            }
            else
            {
                grid.canLockCell = cellTarget;
                ShowCanLockCell(cellTarget);
            }
        }
        else if(gameManager.turn == GameManager.Turn.Moon && (cellChoose.Empty || cellChoose.tile.isMoon) && grid.CanSelected(cellChoose))
        {
            cellChoose.tile.isSun = false;
            cellChoose.tile.isMoon = true;
            cellChoose.tile.isDefault = false;

            if(cellTarget.tile.isSun && cellTarget == grid.canLockCell)
            {
                cellChoose.tile.isLock = true;
                cellChoose.tile.SetMoonLock();
                ShowLockedCell(cellTarget);
            }
            else
            {
                grid.canLockCell = cellTarget;
                ShowCanLockCell(cellTarget);
            }
        }

        // Swap
        cellChoose.tile.transform.position = cellTarget.tile.transform.position;
        Tile tile = cellChoose.tile;

        if(cellChoose.coordinates.y > cellTarget.coordinates.y)
        {
            for(int i=cellChoose.coordinates.y; i>cellTarget.coordinates.y; i--)
            {
                TileCell cella = grid.GetCell(cellChoose.coordinates.x, i);
                TileCell cellb = grid.GetCell(cellChoose.coordinates.x, i-1);
                StartCoroutine(cellb.tile.Animate(cella));
                cellb.tile.cell = cella;
                cella.tile = cellb.tile;
            }
        }
        else if(cellChoose.coordinates.y < cellTarget.coordinates.y)
        {
            for(int i=cellChoose.coordinates.y; i<cellTarget.coordinates.y; i++)
            {
                TileCell cella = grid.GetCell(cellChoose.coordinates.x, i);
                TileCell cellb = grid.GetCell(cellChoose.coordinates.x, i+1);
                cellb.tile.transform.position = cella.transform.position;
                StartCoroutine(cellb.tile.Animate(cella));
                cellb.tile.cell = cella;
                cella.tile = cellb.tile;
            }
        }
        else if(cellChoose.coordinates.x > cellTarget.coordinates.x)
        {
            for(int i=cellChoose.coordinates.x; i>cellTarget.coordinates.x; i--)
            {
                TileCell cella = grid.GetCell(i, cellChoose.coordinates.y);
                TileCell cellb = grid.GetCell(i-1, cellChoose.coordinates.y);
                StartCoroutine(cellb.tile.Animate(cella));
                cellb.tile.cell = cella;
                cella.tile = cellb.tile;
            }
        }
        else if(cellChoose.coordinates.x < cellTarget.coordinates.x)
        {
            for(int i=cellChoose.coordinates.x; i<cellTarget.coordinates.x; i++)
            {
                TileCell cella = grid.GetCell(i, cellChoose.coordinates.y);
                TileCell cellb = grid.GetCell(i+1, cellChoose.coordinates.y);
                StartCoroutine(cellb.tile.Animate(cella));
                cellb.tile.cell = cella;
                cella.tile = cellb.tile;
            }
        }

        cellTarget.tile = tile;
        cellTarget.tile.cell = cellTarget;

        // Check win
        StartCoroutine(WaitForChanges(gameManager.turn));
    }

    public void UnlockAllTiles()
    {
        foreach(Tile tile in tiles)
        {
            tile.Unlock();
        }
    }

    private IEnumerator WaitForChanges(GameManager.Turn turn)
    {
        CheckGameOver(turn);

        yield return new WaitForSeconds(0.1f);

        waiting = false;
    }

    private void CheckGameOver(GameManager.Turn turn)
    {
        if(turn == GameManager.Turn.Sun)
        {
            if(CheckSunWin()) 
            {
                gameManager.GameOver(true);
                return;
            }
            if(CheckMoonWin())
            {
                gameManager.GameOver(false);
                return;
            }
        }
        else if(turn == GameManager.Turn.Moon)
        {
            if(CheckMoonWin())
            {
                gameManager.GameOver(false);
                return;
            }
            if(CheckSunWin()) 
            {
                gameManager.GameOver(true);
                return;
            }
        }

        gameManager.SwapTurn();
    }

    private bool CheckSunWin()
    {
        for (int i = 0; i < 5; i++)
        {
            bool rowWin = true;
            bool colWin = true;
            for (int j = 0; j < 5; j++)
            {
                if (!grid.GetCell(i, j).tile.isSun)
                {
                    rowWin = false;
                }
                if (!grid.GetCell(j, i).tile.isSun)
                {
                    colWin = false;
                }
            }
            if (rowWin || colWin)
            {
                return true;
            }
        }

        bool mainDiagonalWin = true;
        for (int i = 0; i < 5; i++)
        {
            if (!grid.GetCell(i, i).tile.isSun)
            {
                mainDiagonalWin = false;
                break;
            }
        }
        if (mainDiagonalWin)
        {
            return true;
        }

        bool antiDiagonalWin = true;
        for (int i = 0; i < 5; i++)
        {
            if (!grid.GetCell(i, 4 - i).tile.isSun)
            {
                antiDiagonalWin = false;
                break;
            }
        }
        if (antiDiagonalWin)
        {
            return true;
        }

        return false;
    }

    private bool CheckMoonWin()
    {
        for (int i = 0; i < 5; i++)
        {
            bool rowWin = true;
            bool colWin = true;
            for (int j = 0; j < 5; j++)
            {
                if (!grid.GetCell(i, j).tile.isMoon)
                {
                    rowWin = false;
                }
                if (!grid.GetCell(j, i).tile.isMoon)
                {
                    colWin = false;
                }
            }
            if (rowWin || colWin)
            {
                return true;
            }
        }

        bool mainDiagonalWin = true;
        for (int i = 0; i < 5; i++)
        {
            if (!grid.GetCell(i, i).tile.isMoon)
            {
                mainDiagonalWin = false;
                break;
            }
        }
        if (mainDiagonalWin)
        {
            return true;
        }

        bool antiDiagonalWin = true;
        for (int i = 0; i < 5; i++)
        {
            if (!grid.GetCell(i, 4 - i).tile.isMoon)
            {
                antiDiagonalWin = false;
                break;
            }
        }
        if (antiDiagonalWin)
        {
            return true;
        }

        return false;
    }
}
