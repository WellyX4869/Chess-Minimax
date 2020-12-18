using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    [HideInInspector]
    public bool mIsKingAlive = true;

    public Board mBoard = null;
    public GameObject mPiecePrefab;

    [Header("DIFFICULTY")]
    public int difficulty = 0;
    Heuristic weight = new Heuristic();

    private List<BasePiece> mWhitePieces = null;
    private List<BasePiece> mBlackPieces = null;
    private List<BasePiece> mPromotedPieces = new List<BasePiece>();

    private string[] mPieceOrder = new string[16]
    {
        "P", "P", "P", "P", "P", "P", "P", "P",
        "R", "KN", "B", "Q", "K", "B", "KN", "R"
    };

    private Dictionary<string, Type> mPieceLibrary = new Dictionary<string, Type>()
    {
        {"P",  typeof(Pawn)},
        {"R",  typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B",  typeof(Bishop)},
        {"K",  typeof(King)},
        {"Q",  typeof(Queen)}
    };

    public void Setup(Board board)
    {
        // Create white pieces
        //mWhitePieces = CreatePieces(Color.white, new Color32(80, 124, 159, 255)); //white
        mWhitePieces = CreatePieces(Color.white, new Color32(248, 248, 248, 255)); //white
        
        // Create black pieces 
        //mBlackPieces = CreatePieces(Color.black, new Color32(210, 95, 64, 255)); //black
        mBlackPieces = CreatePieces(Color.black, new Color32(40, 40, 40, 255)); //black

        // Place pieces
        PlacePieces(1, 0, mWhitePieces, board);
        PlacePieces(6, 7, mBlackPieces, board);

        mBoard = board;
        // White goes first
        SwitchSides(Color.black);
    }

    private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor)
    {
        List<BasePiece> newPieces = new List<BasePiece>();

        for (int i = 0; i < mPieceOrder.Length; i++)
        {
            // Get the type
            string key = mPieceOrder[i];
            Type pieceType = mPieceLibrary[key];

            // Create
            BasePiece newPiece = CreatePiece(pieceType);
            newPieces.Add(newPiece);

            // Setup
            newPiece.Setup(teamColor, spriteColor, this);
        }

        return newPieces;
    }

    private BasePiece CreatePiece(Type pieceType)
    {
        // Create new object
        GameObject newPieceObject = Instantiate(mPiecePrefab);
        newPieceObject.transform.SetParent(transform);

        // Set scale and position
        newPieceObject.transform.localScale = new Vector3(1, 1, 1);
        newPieceObject.transform.localRotation = Quaternion.identity;

        // Store new piece
        BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);

        return newPiece;
    }

    private void PlacePieces(int pawnRow, int royaltyRow, List<BasePiece> pieces, Board board)
    {
        for (int i = 0; i < 8; i++)
        {
            // Place pawns    
            pieces[i].Place(board.mAllCells[i, pawnRow]);

            // Place royalty
            pieces[i + 8].Place(board.mAllCells[i, royaltyRow]);
        }
    }

    private void SetInteractive(List<BasePiece> allPieces, bool value)
    {
        foreach (BasePiece piece in allPieces)
            piece.enabled = value;
    }

    ////BEST MOVE
    public IEnumerator bestMove()
    {
        yield return null;
        int bestScore = int.MinValue;
        Cell AICell = null;
        Cell bestCell = null;
        Board board = mBoard;

        for (int y = 0; y<8; y++)
        {
            for(int x = 0; x<8; x++)
            {
                Cell startCell = board.mAllCells[x, y];
                if (startCell.mCurrentPiece != null)
                {
                    if (startCell.mCurrentPiece.mColor == Color.black)
                    {
                        BasePiece piece = startCell.mCurrentPiece;
                        if (piece.HasMove())
                        {
                            Vector2Int startPos = new Vector2Int(piece.mCurrentCell.mBoardPosition.x, piece.mCurrentCell.mBoardPosition.y);
                            List<Cell> targetCells = new List<Cell>();
                            
                            for (int i = 0; i < piece.mHighlightedCells.Count; i++)
                            {
                                int x1 = piece.mHighlightedCells[i].mBoardPosition.x; int y1 = piece.mHighlightedCells[i].mBoardPosition.y;
                                //target += "(" + x1 + ", " + y1 + ");";
                                targetCells.Add(board.mAllCells[x1, y1]);
                            }

                            for (int i = 0; i < targetCells.Count; i++)
                            {
                                Vector2Int pos = new Vector2Int(targetCells[i].mBoardPosition.x, targetCells[i].mBoardPosition.y);
                                BasePiece killedPiece = board.mAllCells[pos.x, pos.y].mCurrentPiece;

                                Cell tempCell = targetCells[i];
                                
                                //Debug.Log(startPos + ";" + pos);
                                board.DoFakeMove(startPos, pos, piece);
                                //Debug.Log(board.display());

                                int score = minimax(board, difficulty, false, int.MinValue, int.MaxValue);
                                board.UndoFakeMove(startPos, pos, piece, killedPiece);

                                if (score > bestScore)
                                {
                                    bestScore = score;
                                    AICell = startCell;
                                    bestCell = tempCell;
                                }
                            }
                            piece.ClearMove();
                        }
                    }
                }
            }
        }
        Debug.Log("BEST SCORE is " + bestScore + " and target cell is " + bestCell.mBoardPosition);
        AICell.mCurrentPiece.Moving(bestCell);
    }

  
    //EVALUATE CHESS BOARD
    private int evaluate(Board board)
    {
        int total = 0;
        for(int y = 0; y<8; y++)
        {
            for(int x = 0; x<8; x++)
            {
                BasePiece piece = board.mAllCells[x, y].mCurrentPiece;
                if(piece != null)
                {   
                    if (piece.mColor == Color.white)
                        total -= piece.mValue;
                    else if(piece.mColor == Color.black)
                        total += piece.mValue;
                }
            }
        }

        return total;
    }

    private int evaluate2(Board board)
    {
        float pieceDifference = 0;
        float whiteWeight = 0;
        float blackWeight = 0;
        int blackScore = 0;
        int whiteScore = 0;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Cell cell = board.mAllCells[x, y];
                if (cell.mCurrentPiece == null)
                {
                    continue;
                }
                else
                {
                    BasePiece piece = cell.mCurrentPiece;
                    if (piece.mColor == Color.white)
                    {
                        whiteScore += piece.mValue;
                        whiteWeight += weight.GetBoardWeight(piece.role, piece.mCurrentCell.mBoardPosition, piece.mColor);
                    }
                    else
                    {
                        blackScore += piece.mValue;
                        blackWeight += weight.GetBoardWeight(piece.role, piece.mCurrentCell.mBoardPosition, piece.mColor); ;
                    }
                }
            }
        }
       
        pieceDifference = (blackScore + (blackWeight / 100)) - (whiteScore + (whiteWeight / 100));
        return Mathf.RoundToInt(pieceDifference * 100);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// <summary>
    private int minimax(Board board, int depth, bool isMaximizing, int alpha, int beta)
    {
        if (depth <= 0)
        {
            //if (difficulty > 1)
            //{
            //    res = evaluate2(board);
            //}
            //else evaluate(board);
            int res = evaluate2(board);
            //if(res != 0)
            //Debug.Log(res + "\n" + board.display());
            return res;
        }

        if (isMaximizing) // gets next best move
        {
            int bestScore = int.MinValue;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Cell startCell = board.mAllCells[x, y];
                    if (startCell.mCurrentPiece != null)
                    {
                        if (startCell.mCurrentPiece.mColor == Color.black)
                        {
                            BasePiece piece2 = startCell.mCurrentPiece;
                            if (piece2.HasMove())
                            {
                                Vector2Int startPos2 = new Vector2Int(piece2.mCurrentCell.mBoardPosition.x, piece2.mCurrentCell.mBoardPosition.y);
                                List<Cell> targetCells = new List<Cell>();

                                for (int i = 0; i < piece2.mHighlightedCells.Count; i++)
                                {
                                    int x1 = piece2.mHighlightedCells[i].mBoardPosition.x; int y1 = piece2.mHighlightedCells[i].mBoardPosition.y;
                                    targetCells.Add(board.mAllCells[x1, y1]);
                                }

                                for (int i = 0; i < targetCells.Count; i++)
                                {
                                    Vector2Int pos2 = new Vector2Int(targetCells[i].mBoardPosition.x, targetCells[i].mBoardPosition.y);
                                    BasePiece killedPiece2 = board.mAllCells[pos2.x, pos2.y].mCurrentPiece;

                                    board.DoFakeMove(startPos2, pos2, piece2);
                                    //Debug.Log(board.display());

                                    int score = minimax(board, depth - 1, false, alpha, beta);

                                    board.UndoFakeMove(startPos2, pos2, piece2, killedPiece2);
                                    bestScore = Math.Max(score, bestScore);
                                    alpha = Math.Max(alpha, bestScore);

                                    if (beta <= alpha && (piece2.role == "P" || difficulty < 3))
                                    {
                                        break;
                                    }
                                }
                                piece2.ClearMove();
                            }
                        }
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Cell startCell = board.mAllCells[x, y];
                    if (startCell.mCurrentPiece != null)
                    {
                        if (startCell.mCurrentPiece.mColor == Color.white)
                        {
                            BasePiece piece3 = startCell.mCurrentPiece;
                            if (piece3.HasMove())
                            {
                                Vector2Int startPos3 = new Vector2Int(piece3.mCurrentCell.mBoardPosition.x, piece3.mCurrentCell.mBoardPosition.y);
                                List<Cell> targetCells = new List<Cell>();

                                for (int i = 0; i < piece3.mHighlightedCells.Count; i++)
                                {
                                    int x1 = piece3.mHighlightedCells[i].mBoardPosition.x; int y1 = piece3.mHighlightedCells[i].mBoardPosition.y;
                                    targetCells.Add(board.mAllCells[x1, y1]);
                                }

                                //if (depth == 3 && piece3.role == "Q") Debug.Log(target);
                                for (int i = 0; i < targetCells.Count; i++)
                                {
                                    Vector2Int pos3 = new Vector2Int(targetCells[i].mBoardPosition.x, targetCells[i].mBoardPosition.y);
                                    BasePiece killedPiece3 = board.mAllCells[pos3.x, pos3.y].mCurrentPiece;

                                    board.DoFakeMove(startPos3, pos3, piece3);
                                    //Debug.Log(board.display());

                                    int score = minimax(board, depth-1, true, alpha, beta);

                                    board.UndoFakeMove(startPos3, pos3, piece3, killedPiece3);
                                   
                                    bestScore = Math.Min(score, bestScore);
                                    beta = Math.Min(alpha, bestScore);
                                    if (beta <= alpha && piece3.role == "P")
                                    {
                                        break;
                                    }
                                }
                                piece3.ClearMove();
                            }
                        }
                    }
                }
            }
            return bestScore;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// </summary>
   
    private void MoveRandomPiece()
    {
        BasePiece finalPiece = null;

        while (!finalPiece)
        {
            // Get piece
            int i = UnityEngine.Random.Range(0, mBlackPieces.Count);
            BasePiece newPiece = mBlackPieces[i];

            // Does this piece have any moves?
            if (!newPiece.HasMove())
                continue;

            // Is piece active?
            if (newPiece.gameObject.activeInHierarchy)
                finalPiece = newPiece;
        }

        finalPiece.ComputerMove();
    }

    public void SwitchSides(Color color)
    {
        if (!mIsKingAlive)
        {
            // Reset pieces
            ResetPieces();

            // King has risen from the dead
            mIsKingAlive = true;

            // Change color to black, so white can go first again
            color = Color.black;
        }

        bool isBlackTurn = color == Color.white ? true : false;

        // Set team interactivity
        SetInteractive(mWhitePieces, !isBlackTurn);

        // Disable this so player can't move pieces
        SetInteractive(mBlackPieces, isBlackTurn);

        // Set promoted interactivity
        foreach (BasePiece piece in mPromotedPieces)
        {
            bool isBlackPiece = piece.mColor != Color.white ? true : false;
            bool isPartOfTeam = isBlackPiece == true ? isBlackTurn : !isBlackTurn;

            piece.enabled = isPartOfTeam;
        }

        // ADDED: Move random piece
        if (isBlackTurn)
            StartCoroutine(bestMove());

    }

    public void ResetPieces()
    {
        foreach (BasePiece piece in mPromotedPieces)
        {
            piece.Kill();
            Destroy(piece.gameObject);
        }

        mPromotedPieces.Clear();

        //foreach (BasePiece piece in mWhitePieces)
        //    piece.Reset();

        //foreach (BasePiece piece in mBlackPieces)
        //    piece.Reset();
        Application.LoadLevel(0);
    }

    public void PromotePiece(Pawn pawn, Cell cell, Color teamColor, Color spriteColor)
    {
        // Kill Pawn
        pawn.Kill();

        // Create
        BasePiece promotedPiece = CreatePiece(typeof(Queen));
        promotedPiece.Setup(teamColor, spriteColor, this);

        // Place piece
        promotedPiece.Place(cell);

        // Add
        mPromotedPieces.Add(promotedPiece);
    }
}
