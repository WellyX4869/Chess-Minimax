using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// New
public enum CellState
{
    None,
    Friendly,
    Enemy,
    Free,
    OutOfBounds
}

public class Board : MonoBehaviour
{
    public GameObject mCellPrefab;

    [HideInInspector]
    public Cell[,] mAllCells = new Cell[8, 8];
    public Cell[,] mComCells = new Cell[8, 8];

    // We create the board here, no surprise
    public void Create()
    {
        #region Create
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                // Create the cell
                GameObject newCell = Instantiate(mCellPrefab, transform);

                // Position
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((x * 100) + 50, (y * 100) + 50);

                // Setup
                mAllCells[x, y] = newCell.GetComponent<Cell>();
                mAllCells[x, y].Setup(new Vector2Int(x, y), this);
                mComCells[x, y] = newCell.GetComponent<Cell>();
                mComCells[x, y].Setup(new Vector2Int(x, y), this);
            }
        }
        #endregion

        #region Color
        for (int x = 0; x < 8; x += 2)
        {
            for (int y = 0; y < 8; y++)
            {
                // Offset for every other line
                int offset = (y % 2 != 0) ? 0 : 1;
                int finalX = x + offset;

                //0-255 color dalam bentuk RGB dan alpha
                byte Red = 185; 
                byte Green = 185;
                byte Blue = 185;
                byte Alpha = 255;
                // Color
                mAllCells[finalX, y].GetComponent<Image>().color = new Color32(Red, Green, Blue, Alpha); //board pieces white
                mComCells[finalX, y].GetComponent<Image>().color = new Color32(Red, Green, Blue, Alpha); //board pieces white
            }
        }
        #endregion
    }

    public void DoFakeMove(Vector2Int startPos, Vector2Int endPos, BasePiece piece)
    {
        piece.mCurrentCell = mAllCells[endPos.x, endPos.y];
        
        mAllCells[endPos.x, endPos.y].mCurrentPiece = piece;
        mAllCells[startPos.x, startPos.y].mCurrentPiece = null;
    }

    public void UndoFakeMove(Vector2Int startPos, Vector2Int endPos, BasePiece piece, BasePiece killedPiece)
    {
        piece.mCurrentCell = mAllCells[startPos.x, startPos.y];
        mAllCells[startPos.x, startPos.y].mCurrentPiece = piece;
        if (killedPiece != null)
        {
            mAllCells[endPos.x, endPos.y].mCurrentPiece = killedPiece;
        }
        else
        {
            mAllCells[endPos.x, endPos.y].mCurrentPiece = null;
        }
    }

    // New
    public CellState ValidateCell(int targetX, int targetY, BasePiece checkingPiece)
    {
        // Bounds check
        if (targetX < 0 || targetX > 7)
            return CellState.OutOfBounds;

        if (targetY < 0 || targetY > 7)
            return CellState.OutOfBounds;

        // Get cell
        Cell targetCell = mAllCells[targetX, targetY];

        // If the cell has a piece
        if (targetCell.mCurrentPiece != null)
        {
            // If friendly
            if (checkingPiece.mColor == targetCell.mCurrentPiece.mColor)
                return CellState.Friendly;

            // If enemy
            if (checkingPiece.mColor != targetCell.mCurrentPiece.mColor)
                return CellState.Enemy;
        }

        return CellState.Free;
    }

    public string display()
    {
        string log = "";
        for(int i = 7; i>=0; i--)
        {
            for(int j = 0; j<8; j++)
            {
                Cell targetCell = mAllCells[j,i];
                if(targetCell.mCurrentPiece == null)
                {
                    log += "0";
                }
                else
                { 
                //    if (targetCell.mCurrentPiece.mColor == Color.white) log += "P";
                //    else log += "B";
                    log += targetCell.mCurrentPiece.role;
                }
            }
            log += '\n';
        }
        return log;
    }
}
