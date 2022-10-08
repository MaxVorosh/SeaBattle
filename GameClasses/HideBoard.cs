namespace GameClasses;

public class HideBoard
{
    /// <summary>
    /// Class of opponent's board. Include his board and our information about his board
    /// Also include count of alive tiles (tiles with ships that not hit)
    /// IsValidTile(int x, int y) - returns true if (x, y) tile is on the board
    /// ShootResult OpenTile(int x, int y) - make shoot (x, y) tile
    /// bool IsShipKilled(int x, int y) - check if ship killed
    /// Return true if ship with (x, y) tile will be killed if we shoot (x, y)
    /// bool IsShooted(int x, int y) - return true if we were shoot this tile before
    /// </summary>
    public TileCondition[,] board;
    public Board linkedBoard;
    public int aliveTiles;

    private int boardSize;

    public HideBoard(Board linkedBoard)
    {
        aliveTiles = 0;
        boardSize = linkedBoard.GetBoardSize();
        board = new TileCondition[boardSize, boardSize];
        for (int i = 0; i < boardSize; ++i)
        {
            for (int j = 0; j < boardSize; ++j)
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
            board[x, y] = TileCondition.Hit; // If there's a ship
            aliveTiles--;
        }
        else
        {
            board[x, y] = TileCondition.Missed; // If we missed
            return ShootResult.Missed;
        }
        // Returns shoot result
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
        }; // Neighbours of the tile
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; ++j)
            {
                int newX = x + moves[j].Item1;
                int newY = y + moves[j].Item2; // Coords that runs on particular direction
                if (isMissed[j] || !IsValidTile(newX, newY))
                    continue; // If we already have missed shot on this direction or (newX, newY) not valid - continue
                while (IsValidTile(newX, newY) && board[newX, newY] == TileCondition.Hit)
                {
                    newX += moves[j].Item1;
                    newY += moves[j].Item2; // Run before we are not valid or tile is not hit
                }
                if (!IsValidTile(newX, newY))
                    continue;

                if (linkedBoard.board[newX, newY] &&
                    board[newX, newY] == TileCondition.Unknown)
                {
                    return false; // If we see a tile of this ship that have a ship but we don't shoot on it, ship is not killed
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