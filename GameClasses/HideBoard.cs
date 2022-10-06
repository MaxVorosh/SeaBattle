namespace GameClasses;

public class HideBoard
{
    public TileCondition[,] board;
    public Board linkedBoard;
    public int aliveTiles = 0;

    public HideBoard(Board linkedBoard)
    {
        board = new TileCondition[10, 10];
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                board[i, j] = TileCondition.Unknown;
                aliveTiles += Convert.ToInt32(linkedBoard.board[i, j]);
            }
        }

        this.linkedBoard = linkedBoard;
    }

    private bool IsValidTile(int x, int y)
    {
        return (x >= 0 && x < 10 && y >= 0 && y < 10);
    }

    public ShootResult OpenTile(int x, int y)
    {
        if (linkedBoard.board[x, y])
        {
            board[x, y] = TileCondition.Hit;
            aliveTiles--;
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
        bool[] isMissed = new bool[4];
        Tuple<int, int>[] moves =
        {
            new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(0, -1), new Tuple<int, int>(0, 1)
        };
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; ++j)
            {
                int newX = x + moves[j].Item1;
                int newY = y + moves[j].Item2;
                if (isMissed[j] || !IsValidTile(newX, newY))
                    continue;
                while (IsValidTile(newX, newY) && board[newX, newY] == TileCondition.Hit)
                {
                    newX += moves[j].Item1;
                    newY += moves[j].Item2;
                }
                if (!IsValidTile(newX, newY))
                    continue;

                if (linkedBoard.board[newX, newY] &&
                    board[newX, newY] == TileCondition.Unknown)
                {
                    return false;
                }
                if (!linkedBoard.board[newX, newY])
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