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

    public void RoundMissedShots(int x, int y)
    {
        int[] movesX = { 1, -1, 0, 0 };
        int[] movesY = { 0, 0, 1, -1 };
        for (int i = 0; i < 4; ++i)
        {
            int currentX = x;
            int currentY = y;
            while (IsValidTile(currentX, currentY) &&
                   linkedBoard.board[currentX, currentY])
            {
                for (int j = 0; j < 4; ++j)
                {
                    int neighbourX = currentX + movesX[j];
                    int neighbourY = currentY + movesY[j];
                    if (IsValidTile(neighbourX, neighbourY) && board[neighbourX, neighbourY] != TileCondition.Hit)
                    {
                        board[neighbourX, neighbourY] = TileCondition.Missed;
                    }
                }
            }
            currentX += movesX[i];
            currentY += movesY[i];
        }
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
            RoundMissedShots(x, y);
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
                if (!isMissed[j] || !IsValidTile(coords[j, 0], coords[j, 1]))
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