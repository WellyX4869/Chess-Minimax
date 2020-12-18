using UnityEngine;

public class Heuristic 
{
    int[,] PawnWhiteBoardWeight = new int[,]
        {
        { 5,  5,  5,  5,  5,  5,  5,  5},
        {30, 30, 30, 30, 30, 30, 30, 30},
        {10, 10, 20, 30, 30, 20, 10, 10},
        { 5,  5, 10, 25, 25, 10,  5,  5},
        { 0,  0,  0, 20, 20,  0,  0,  0},
        { 5, -5,-10,  0,  0,-10, -5,  5},
        { 5, 10, 10,-20,-20, 10, 10,  5},
        { 5,  5,  5,  5,  5,  5,  5,  5}
        };

    int[,] PawnBlackBoardWeight = new int[,]
        {
        { 0,  0,  0,  0,  0,  0,  0,  0},
        { 5, 10, 10,-20,-20, 10, 10,  5},
        { 5, -5,-10,  0,  0,-10, -5,  5},
        { 0,  0,  0, 20, 20,  0,  0,  0},
        { 5,  5, 10, 25, 25, 10,  5,  5},
        {10, 10, 20, 30, 30, 20, 10, 10},
        {30, 30, 30, 30, 30, 30, 30, 30},
        { 5,  5,  5,  5,  5,  5,  5,  5},
        };

    int[,] BishopWhiteBoardWeight = new int[,]
        {
        {-20,-10,-10,-10,-10,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5, 10, 10,  5,  0,-10},
        {-10,  5,  5, 10, 10,  5,  5,-10},
        {-10,  0, 10, 10, 10, 10,  0,-10},
        {-10, 10, 10, 10, 10, 10, 10,-10},
        {-10,  5,  0,  0,  0,  0,  5,-10},
        {-20,-10,-10,-10,-10,-10,-10,-20}
        };

    int[,] BishopBlackBoardWeight = new int[,]
        {
        {-20,-10,-10,-10,-10,-10,-10,-20},
        {-10,  5,  0,  0,  0,  0,  5,-10},
        {-10, 10, 10, 10, 10, 10, 10,-10},
        {-10,  0, 10, 10, 10, 10,  0,-10},
        {-10,  5,  5, 10, 10,  5,  5,-10},
        {-10,  0,  5, 10, 10,  5,  0,-10},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-20,-10,-10,-10,-10,-10,-10,-20}
        };

    int[,] KnightWhiteBoardWeight = new int[,]
        {
        {-50,-40,-30,-30,-30,-30,-40,-50},
        {-40,-20,  0,  0,  0,  0,-20,-40},
        {-30,  0, 10, 15, 15, 10,  0,-30},
        {-30,  5, 15, 25, 25, 15,  5,-30},
        {-30,  0, 15, 25, 25, 15,  0,-30},
        {-30,  5, 10, 15, 15, 10,  5,-30},
        {-40,-20,  0,  5,  5,  0,-20,-40},
        {-50,-40,-30,-30,-30,-30,-40,-50}
        };

    int[,] KnightBlackBoardWeight = new int[,]
        {
        {-50,-40,-30,-30,-30,-30,-40,-50},
        {-40,-20,  0,  5,  5,  0,-20,-40},
        {-30,  5, 10, 15, 15, 10,  5,-30},
        {-30,  0, 15, 25, 25, 15,  0,-30},
        {-30,  5, 15, 25, 25, 15,  5,-30},
        {-30,  0, 10, 15, 15, 10,  0,-30},
        {-40,-20,  0,  0,  0,  0,-20,-40},
        {-50,-40,-30,-30,-30,-30,-40,-50}
        };

    int[,] RookWhiteBoardWeight = new int[,]
        {
        { 0,  0,  0,  0,  0,  0,  0,  0},
        { 5, 10, 10, 10, 10, 10, 10,  5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        { 0,  0,  0,  5,  5,  0,  0,  0}
        };

    int[,] RookBlackBoardWeight = new int[,]
        {
        { 0,  0,  0,  5,  5,  0,  0,  0},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        {-5,  0,  0,  0,  0,  0,  0, -5},
        { 5, 10, 10, 10, 10, 10, 10,  5},
        { 0,  0,  0,  0,  0,  0,  0,  0}
        };

    int[,] QueenWhiteBoardWeight = new int[,]
        {
        {-20,-10,-10, -5, -5,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5, 10, 10,  5,  0,-10},
        { -5,  0, 10, 15, 15, 10,  0, -5},
        {  0,  0, 10, 15, 15, 10,  0, -5},
        {-10,  5,  5, 10, 10,  5,  0,-10},
        {-10,  0,  5,  0,  0,  0,  0,-10},
        {-20,-10,-10, -5, -5,-10,-10,-20}
        };

    int[,] QueenBlackBoardWeight = new int[,]
        {
        {-20,-10,-10, -5, -5,-10,-10,-20},
        {-10,  0,  5,  0,  0,  0,  0,-10},
        {-10,  5,  5,  5,  5,  5,  0,-10},
        {  0,  0,  5,  5,  5,  5,  0, -5},
        { -5,  0,  5,  5,  5,  5,  0, -5},
        {-10,  0,  5,  5,  5,  5,  0,-10},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-20,-10,-10, -5, -5,-10,-10,-20}
        };

    int[,] KingWhiteBoardWeight =
        {
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-20,-30,-30,-40,-40,-30,-30,-20},
        {-10,-20,-20,-20,-20,-20,-20,-10},
        { 20, 20,  0,  0,  0,  0, 20, 20},
        { 20, 30, 10,  0,  0, 10, 30, 20}
    };

    int[,] KingBlackBoardWeight =
        {
        { 20, 30, 10,  0,  0, 10, 30, 20},
        { 20, 20,  0,  0,  0,  0, 20, 20},
        {-10,-20,-20,-20,-20,-20,-20,-10},
        {-20,-30,-30,-40,-40,-30,-30,-20},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
    };

    public int GetBoardWeight(string type, Vector2 position, Color team)
    {
        switch (type)
        {
            case "P":
                if (team == Color.white)
                {
                    return PawnWhiteBoardWeight[(int)position.x, (int)position.y];
                }
                else
                {
                    return PawnBlackBoardWeight[(int)position.x, (int)position.y];
                }
            case "R":
                if (team == Color.white)
                {
                    return RookWhiteBoardWeight[(int)position.x, (int)position.y];
                }
                else
                {
                    return RookBlackBoardWeight[(int)position.x, (int)position.y];
                }
            case "N":
                if (team == Color.white)
                {
                    return KnightWhiteBoardWeight[(int)position.x, (int)position.y];
                }
                else
                {
                    return KnightBlackBoardWeight[(int)position.x, (int)position.y];
                }
            case "B":
                if (team == Color.white)
                {
                    return BishopWhiteBoardWeight[(int)position.x, (int)position.y];
                }
                else
                {
                    return BishopBlackBoardWeight[(int)position.x, (int)position.y];
                }
            case "Q":
                if (team == Color.white)
                {
                    return QueenWhiteBoardWeight[(int)position.x, (int)position.y];
                }
                else
                {
                    return QueenBlackBoardWeight[(int)position.x, (int)position.y];
                }
            case "K":
                if (team == Color.white)
                {
                    return KingWhiteBoardWeight[(int)position.x, (int)position.y];
                }
                else
                {
                    return KingBlackBoardWeight[(int)position.x, (int)position.y];
                }
            default:
                return -1;
        }
    }
}
