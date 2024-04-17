using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Computer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TileGrid grid;
    [SerializeField] private TileBoard board;

    private List<TileCell> listCanSelectCells;
    private List<List<TileCell>> listCanTargetCells;

    private void OnEnable()
    {
        GameManager.OnComputerTurn += OnComputerTurn;
        listCanSelectCells = new List<TileCell>();
        listCanTargetCells = new List<List<TileCell>>();
    }

    private void OnComputerTurn()
    {
        int maxScore = 0;
        int selectCellId = 0;
        int targetCellId = 0;

        FindAllAvailableMoves();

        // Easy
        if(GameSetting.Instance.playerLevel == GameSetting.Level.Easy)
        {
            System.Random random = new System.Random();
            selectCellId = random.Next(listCanSelectCells.Count-1);
            targetCellId = random.Next(listCanTargetCells[selectCellId].Count - 1);
        }
        // Hard
        else
        {
            if(gameManager.turn == GameManager.Turn.Sun)
            {
                for(int i=0; i<listCanSelectCells.Count; i++)
                {
                    for(int j=0; j<listCanTargetCells[i].Count; j++)
                    {
                        Debug.Log("CheckSunScore: " + listCanSelectCells[i].coordinates + " || " + listCanTargetCells[i][j].coordinates);
                        int newScore = CheckSunScore(listCanSelectCells[i], listCanTargetCells[i][j]);
                        System.Random random = new System.Random();
                        if (newScore > maxScore || (newScore == maxScore && random.Next(2) == 1))
                        {
                            maxScore = newScore;
                            selectCellId = i;
                            targetCellId = j;
                        }
                    }
                }
            }
            else
            {
                for(int i=0; i<listCanSelectCells.Count; i++)
                {
                    for(int j=0; j<listCanTargetCells[i].Count; j++)
                    {
                        int newScore = CheckMoonScore(listCanSelectCells[i], listCanTargetCells[i][j]);
                        if(newScore > maxScore)
                        {
                            maxScore = newScore;
                            selectCellId = i;
                            targetCellId = j;
                        }
                    }
                }
            }

            Debug.Log("maxScore: " + maxScore);
            Debug.Log(listCanSelectCells[selectCellId].coordinates + " || " + listCanTargetCells[selectCellId][targetCellId].coordinates);
        }

        if(gameManager.turn == GameManager.Turn.Sun)
        {
            listCanSelectCells[selectCellId].tile.SetSun();
        }
        else if(gameManager.turn == GameManager.Turn.Moon)
        {
            listCanSelectCells[selectCellId].tile.SetMoon();
        }

        board.Swap(listCanSelectCells[selectCellId], listCanTargetCells[selectCellId][targetCellId]);
    }

    private int CheckSunScore(TileCell cellSelect, TileCell cellTarget)
    {
        int maxScore = 0;
        string text = "";
        for(int i = 0; i < 5; i++)
        {
            int rowScore = 0;
            int colScore = 0;

            for(int j = 0; j < 5; j++)
            {
                // row
                // attack
                if(grid.GetCell(cellTarget.coordinates.x, j).tile.isSun)
                {
                    rowScore += 3;
                    if(cellTarget.coordinates.x != cellSelect.coordinates.x) rowScore += 2;

                    for(int k = 0; k < 5; k++)
                    {
                        if(cellTarget.coordinates.x == 0 && grid.GetCell(cellTarget.coordinates.x+1, k).tile.isSun) 
                        {
                            rowScore += 3;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                        else if(cellTarget.coordinates.x == 4 && grid.GetCell(cellTarget.coordinates.x-1, k).tile.isSun) 
                        {
                            rowScore += 3;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                    }
                }
                else if(cellTarget.coordinates.x == 0 && grid.GetCell(cellTarget.coordinates.x+1, j).tile.isSun)
                {
                    for(int k = 0; k < 5; k++)
                    {
                        if(grid.GetCell(3, k).tile.isSun) 
                        {
                            rowScore += 2;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                    }
                }
                else if(cellTarget.coordinates.x == 4 && grid.GetCell(cellTarget.coordinates.x-1, j).tile.isSun)
                {
                    for(int k = 0; k < 5; k++)
                    {
                        if(grid.GetCell(3, k).tile.isSun) 
                        {
                            rowScore += 2;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                    }
                }
                // defense
                else
                {
                    if(grid.GetCell(cellTarget.coordinates.x, j).tile.isMoon) rowScore += 3;
                    if(j == cellTarget.coordinates.y) rowScore++;
                    if(cellTarget.coordinates.x != cellSelect.coordinates.x) rowScore++;

                    for(int k = 0; k < 5; k++)
                    {
                        if(cellTarget.coordinates.x == 0 && grid.GetCell(cellTarget.coordinates.x+1, k).tile.isMoon) 
                        {
                            rowScore -= 2;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                        else if(cellTarget.coordinates.x == 4 && grid.GetCell(cellTarget.coordinates.x-1, k).tile.isMoon) 
                        {
                            rowScore -= 2;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                    }
                }

                // col
                // attack
                if(grid.GetCell(j, cellTarget.coordinates.y).tile.isSun)
                {
                    colScore += 3;
                    if(cellTarget.coordinates.y != cellSelect.coordinates.y) colScore += 2;

                    for(int k = 0; k < 5; k++)
                    {
                        if(cellTarget.coordinates.y == 0 && grid.GetCell(k, cellTarget.coordinates.y+1).tile.isSun) 
                        {
                            colScore += 3;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                        else if(cellTarget.coordinates.y == 4 && grid.GetCell(k, cellTarget.coordinates.y-1).tile.isSun) 
                        {
                            colScore += 3;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                    }
                }
                else if(cellTarget.coordinates.y == 0 && grid.GetCell(j, cellTarget.coordinates.y+1).tile.isSun)
                {
                    for(int k = 0; k < 5; k++)
                    {
                        if(grid.GetCell(k, 3).tile.isSun) 
                        {
                            colScore += 2;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                    }
                }
                else if(cellTarget.coordinates.y == 4 && grid.GetCell(j, cellTarget.coordinates.y-1).tile.isSun)
                {
                    for(int k = 0; k < 5; k++)
                    {
                        if(grid.GetCell(k, 3).tile.isSun) 
                        {
                            colScore += 2;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                    }
                }
                // defense
                else
                {
                    if(grid.GetCell(j, cellTarget.coordinates.y).tile.isMoon) colScore += 3;
                    if(j == cellTarget.coordinates.x) colScore++;
                    if(cellTarget.coordinates.y != cellSelect.coordinates.y) colScore++;

                    for(int k = 0; k < 5; k++)
                    {
                        if(cellTarget.coordinates.y == 0 && grid.GetCell(k, cellTarget.coordinates.y+1).tile.isMoon) 
                        {
                            colScore -= 2;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                        else if(cellTarget.coordinates.y == 4 && grid.GetCell(k, cellTarget.coordinates.y-1).tile.isMoon) 
                        {
                            colScore -= 2;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                    }
                }
            }
            if(rowScore >= colScore) 
            {
                maxScore = rowScore;
                text = "rowScore";
            }
            else 
            {
                maxScore = colScore;
                text = "colScore";
            }
        }

        int mainDiagonalScore = 0;
        for(int i = 0; i < 5; i++)
        {
            if(grid.GetCell(i, i).tile.isSun)
            {
                mainDiagonalScore++;
            }
            else if(grid.GetCell(i, i).tile.isMoon)
            {
                mainDiagonalScore++;
                if(i == cellTarget.coordinates.x && i == cellTarget.coordinates.y) mainDiagonalScore++;
            }
            else
            {
                if(i == cellTarget.coordinates.x && i == cellTarget.coordinates.y) mainDiagonalScore++;
            }
        }
        if(mainDiagonalScore > maxScore) 
        {
            maxScore = mainDiagonalScore;
            text = "mainDiagonalScore";
        }

        int antiDiagonalScore = 0;
        for(int i = 0; i < 5; i++)
        {
            if(grid.GetCell(i, 4 - i).tile.isSun)
            {
                antiDiagonalScore++;
            }
            else if(grid.GetCell(i, 4 - i).tile.isMoon)
            {
                antiDiagonalScore++;
                if(i == cellTarget.coordinates.y && 4-i == cellTarget.coordinates.x) antiDiagonalScore++;
            }
            else
            {
                if(i == cellTarget.coordinates.y && 4-i == cellTarget.coordinates.x) antiDiagonalScore++;
            }
        }
        if(antiDiagonalScore > maxScore) 
        {
            maxScore = antiDiagonalScore;
            text = "mainDiagonalScore";
        }

        Debug.Log("CheckSunScore: " + cellSelect.coordinates + " || " + cellTarget.coordinates + " || " + maxScore);
        Debug.Log(text);
        return maxScore;
    }

    private int CheckMoonScore(TileCell cellSelect, TileCell cellTarget)
    {
        int maxScore = 0;
        string text = "";
        for(int i = 0; i < 5; i++)
        {
            int rowScore = 0;
            int colScore = 0;

            for(int j = 0; j < 5; j++)
            {
                // row
                // attack
                if(grid.GetCell(cellTarget.coordinates.x, j).tile.isMoon)
                {
                    rowScore += 3;
                    if(cellTarget.coordinates.x != cellSelect.coordinates.x) rowScore += 2;

                    for(int k = 0; k < 5; k++)
                    {
                        if(cellTarget.coordinates.x == 0 && grid.GetCell(cellTarget.coordinates.x+1, k).tile.isMoon) 
                        {
                            rowScore += 3;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                        else if(cellTarget.coordinates.x == 4 && grid.GetCell(cellTarget.coordinates.x-1, k).tile.isMoon) 
                        {
                            rowScore += 3;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                    }
                }
                else if(cellTarget.coordinates.x == 0 && grid.GetCell(cellTarget.coordinates.x+1, j).tile.isMoon)
                {
                    for(int k = 0; k < 5; k++)
                    {
                        if(grid.GetCell(3, k).tile.isMoon) 
                        {
                            rowScore += 2;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                    }
                }
                else if(cellTarget.coordinates.x == 4 && grid.GetCell(cellTarget.coordinates.x-1, j).tile.isMoon)
                {
                    for(int k = 0; k < 5; k++)
                    {
                        if(grid.GetCell(3, k).tile.isMoon) 
                        {
                            rowScore += 2;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                    }
                }
                // defense
                else
                {
                    if(grid.GetCell(cellTarget.coordinates.x, j).tile.isSun) rowScore += 3;
                    if(j == cellTarget.coordinates.y) rowScore++;
                    if(cellTarget.coordinates.x != cellSelect.coordinates.x) rowScore++;

                    for(int k = 0; k < 5; k++)
                    {
                        if(cellTarget.coordinates.x == 0 && grid.GetCell(cellTarget.coordinates.x+1, k).tile.isMoon) 
                        {
                            rowScore -= 2;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                        else if(cellTarget.coordinates.x == 4 && grid.GetCell(cellTarget.coordinates.x-1, k).tile.isMoon) 
                        {
                            rowScore -= 2;
                            if(cellTarget.coordinates.y != k) rowScore++;
                        }
                    }
                }

                // col
                // attack
                if(grid.GetCell(j, cellTarget.coordinates.y).tile.isMoon)
                {
                    colScore += 3;
                    if(cellTarget.coordinates.y != cellSelect.coordinates.y) colScore += 2;

                    for(int k = 0; k < 5; k++)
                    {
                        if(cellTarget.coordinates.y == 0 && grid.GetCell(k, cellTarget.coordinates.y+1).tile.isMoon) 
                        {
                            colScore += 3;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                        else if(cellTarget.coordinates.y == 4 && grid.GetCell(k, cellTarget.coordinates.y-1).tile.isMoon) 
                        {
                            colScore += 3;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                    }
                }
                else if(cellTarget.coordinates.y == 0 && grid.GetCell(j, cellTarget.coordinates.y+1).tile.isMoon)
                {
                    for(int k = 0; k < 5; k++)
                    {
                        if(grid.GetCell(k, 3).tile.isMoon) 
                        {
                            colScore += 2;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                    }
                }
                else if(cellTarget.coordinates.y == 4 && grid.GetCell(j, cellTarget.coordinates.y-1).tile.isMoon)
                {
                    for(int k = 0; k < 5; k++)
                    {
                        if(grid.GetCell(k, 3).tile.isMoon) 
                        {
                            colScore += 2;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                    }
                }
                // defense
                else
                {
                    if(grid.GetCell(j, cellTarget.coordinates.y).tile.isSun) colScore += 3;
                    if(j == cellTarget.coordinates.x) colScore++;
                    if(cellTarget.coordinates.y != cellSelect.coordinates.y) colScore++;

                    for(int k = 0; k < 5; k++)
                    {
                        if(cellTarget.coordinates.y == 0 && grid.GetCell(k, cellTarget.coordinates.y+1).tile.isSun) 
                        {
                            colScore -= 2;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                        else if(cellTarget.coordinates.y == 4 && grid.GetCell(k, cellTarget.coordinates.y-1).tile.isSun) 
                        {
                            colScore -= 2;
                            if(cellTarget.coordinates.x != k) colScore++;
                        }
                    }
                }
            }
            if(rowScore >= colScore) 
            {
                maxScore = rowScore;
                text = "rowScore";
            }
            else 
            {
                maxScore = colScore;
                text = "colScore";
            }
        }

        int mainDiagonalScore = 0;
        for(int i = 0; i < 5; i++)
        {
            if(grid.GetCell(i, i).tile.isMoon)
            {
                mainDiagonalScore++;
            }
            else if(grid.GetCell(i, i).tile.isSun)
            {
                mainDiagonalScore++;
                if(i == cellTarget.coordinates.x && i == cellTarget.coordinates.y) mainDiagonalScore++;
            }
            else
            {
                if(i == cellTarget.coordinates.x && i == cellTarget.coordinates.y) mainDiagonalScore++;
            }
        }
        if(mainDiagonalScore > maxScore) 
        {
            maxScore = mainDiagonalScore;
            text = "mainDiagonalScore";
        }

        int antiDiagonalScore = 0;
        for(int i = 0; i < 5; i++)
        {
            if(grid.GetCell(i, 4 - i).tile.isMoon)
            {
                antiDiagonalScore++;
            }
            else if(grid.GetCell(i, 4 - i).tile.isSun)
            {
                antiDiagonalScore++;
                if(i == cellTarget.coordinates.y && 4-i == cellTarget.coordinates.x) antiDiagonalScore++;
            }
            else
            {
                if(i == cellTarget.coordinates.y && 4-i == cellTarget.coordinates.x) antiDiagonalScore++;
            }
        }
        if(antiDiagonalScore > maxScore) 
        {
            maxScore = antiDiagonalScore;
            text = "antiDiagonalScore";
        }

        Debug.Log("CheckMoonScore: " + cellSelect.coordinates + " || " + cellTarget.coordinates + " || " + maxScore);
        Debug.Log(text);
        return maxScore;
    }

    private void FindAllAvailableMoves()
    {
        listCanSelectCells.Clear();
        listCanTargetCells.Clear();

        foreach(TileCell cell in grid.cells)
        {
            if(gameManager.turn == GameManager.Turn.Sun && (cell.Empty || cell.tile.isSun) && grid.CanSelected(cell))
            {
                listCanSelectCells.Add(cell);
            }
            else if(gameManager.turn == GameManager.Turn.Moon && (cell.Empty || cell.tile.isMoon) && grid.CanSelected(cell))
            {
                listCanSelectCells.Add(cell);
            }
        }
        for(int i=0; i<listCanSelectCells.Count; i++)
        {
            List<TileCell> listCells = new List<TileCell>();
            foreach(TileCell cellTarget in grid.cells)
            {
                if(grid.CanSwap(listCanSelectCells[i], cellTarget))
                {
                    listCells.Add(cellTarget);
                }
            }
            listCanTargetCells.Add(listCells);
        }
    }
    
    private void OnDisable()
    {
        GameManager.OnComputerTurn -= OnComputerTurn;
    }
}
