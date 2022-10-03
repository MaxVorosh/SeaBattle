namespace GameClasses;

public class HideBoard
{
    public TileCondition[,] board;
    public Board linkedBoard;

    public HideBoard(Board linkedBoard)
    {
        board = new TileCondition[10, 10];
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                board[i, j] = TileCondition.Unknown;
            }
        }

        this.linkedBoard = linkedBoard;
    }

    public ShootResult OpenTile(int x, int y)
    {
        if (linkedBoard.board[x, y])
        {
            board[x, y] = TileCondition.Hit;
        }
        else
        {
            board[x, y] = TileCondition.Missed;
            return ShootResult.Missed;
        }

        if (IsShipKilled(x, y))
        {
            return ShootResult.Killed;
        }

        return ShootResult.Injured;
    }

    public bool IsShipKilled(int x, int y)
    {
        int[,] coords = { {x, y}, {x, y}, {x, y}, {x, y}}; 
        bool[] isMissed = new bool[4];
        Tuple<int, int>[] moves =
        {
            new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(0, -1), new Tuple<int, int>(0, 1)
        };
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; ++j)
            {
                coords[j, 0] += moves[j].Item1;
                coords[j, 1] += moves[j].Item2;
                if (!isMissed[j] || coords[j, 0] < 0 || coords[j, 1] < 0 || coords[j, 0] >= 10 || coords[j, 1] >= 10)
                    continue;
                if (linkedBoard.board[coords[j, 0], coords[j, 1]] &&
                    board[coords[j, 0], coords[j, 1]] == TileCondition.Unknown)
                {
                    return false;
                }
                if (!linkedBoard.board[coords[j, 0], coords[j, 1]])
                    isMissed[j] = true;
            }
        }
        return true;
    }

    public bool IsShooted(int x, int y)
    {
        return (board[x, y] != TileCondition.Unknown);
    }
}