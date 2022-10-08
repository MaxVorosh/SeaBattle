namespace GameClasses;

public class BoardViewModel
{
    /// <summary>
    /// MVVM-type model that linked Prepare to game window and Board class
    /// void ChangeRotation() - change rotation of future ships
    /// void SetShip(int length) - sets current ship length to int length
    /// Tuple<bool, List<Tuple<int, int>>> CanPutShip(int x, int y)
    /// returns tuple. First parameter equals true if if we can put ship with memoried length when start tile = (x, y)
    /// Second parameter - list of coords on the board, that ship located in
    /// void PrepareToDelete() - next clicked ship will be delete
    /// Tuple<int, int> GetTile(int xCoord, int yCoord) - returns tile by coords
    /// ClickResult PutShip(int x, int y):
    /// Tries put ship with memoried length in (x, y) coord. If success, put it and returns ClickResult.Put
    /// Else does nothing and returns ClickResult.Sleep
    /// ClickResult DeleteShip(int x, int y):
    /// Tries delete ship one of the tiles of it (x, y). If success, returns ClickResult.Delete and delete ship
    /// Else just returns ClickResult.Sleep
    /// ClickResult Click(int xCoord, int yCoord) - delete or put ship - depends on prepareDelete
    /// bool IsShip(int x, int y) - returns true if board[x, y] - is a part of a ship
    /// </summary>
    private Board board;
    private int currentLength;
    private bool isHorizontal;
    private bool prepareDelete;
    private int lastDeleteResult;
    private int lastLength;
    private int boardSize;
    private const int tileSize = 30;

    public BoardViewModel(Board board)
    {
        this.board = board;
        currentLength = -1;
        isHorizontal = false;
        prepareDelete = false;
        lastDeleteResult = -1;
        lastLength = -1;
        boardSize = board.GetBoardSize();
    }

    public void ChangeRotation()
    {
        isHorizontal = !isHorizontal;
    }

    public void SetShip(int length)
    {
        prepareDelete = false;
        lastLength = currentLength;
        currentLength = length;
    }

    public Tuple<bool, List<Tuple<int, int>>> CanPutShip(int x, int y)
    {
        List<Tuple<int, int>> occupiedTiles = new List<Tuple<int, int>>();
        int oldX = x;
        int oldY = y;
        for (int i = 0; i < currentLength; ++i)
        {
            if (0 <= x && x < boardSize && 0 <= y && y < boardSize)
            {
                occupiedTiles.Add(new Tuple<int, int>(x, y));
                if (i != currentLength - 1) // If it's not end, update coords
                {
                    if (isHorizontal)
                    {
                        x++;
                    }
                    else
                    {
                        y++;
                    }
                }
            }
            else
            {
                break;
            }
        }
        return new Tuple<bool, List<Tuple<int, int>>>(board.CheckShipPosition(oldX, oldY, x, y),
            occupiedTiles);
    }

    public void PrepareToDelete()
    {
        prepareDelete = true;
    }

    public Tuple<int, int> GetTile(int xCoord, int yCoord)
    {
        return new Tuple<int, int>(xCoord / tileSize, yCoord / tileSize);
    }

    public ClickResult PutShip(int x, int y)
    {
        var canPut = CanPutShip(x, y);
        if (!canPut.Item1)
        {
            lastLength = currentLength; // Can't put ship
            currentLength = -1;
            return ClickResult.Sleep;
        }

        bool result;
        if (isHorizontal) // Add ship depends on his orientation
        {
            result = board.AddShip(x, y, x + currentLength - 1, y);
        }
        else
        {
            result = board.AddShip(x, y, x, y + currentLength - 1);
        }

        if (result)
        {
            return ClickResult.Put;
        }

        return ClickResult.Sleep;
    }

    public ClickResult DeleteShip(int x, int y)
    {
        int result = board.DeleteShip(x, y);
        lastDeleteResult = result;
        if (result == 0)
        {
            return ClickResult.Sleep;
        }
        return ClickResult.Delete;
    }

    public int GetLastDeleteLength()
    {
        return lastDeleteResult;
    }

    public int GetLastLength()
    {
        return lastLength;
    }

    public ClickResult Click(int xCoord, int yCoord)
    {
        var tile = GetTile(xCoord, yCoord);
        var clickResult = ClickResult.Sleep;
        if (prepareDelete)
        {
            // Delete ship branch
            prepareDelete = false;
            clickResult = DeleteShip(tile.Item2, tile.Item1);
        }
        if (currentLength != -1)
        {
            // Put ship branch
            clickResult = PutShip(tile.Item2, tile.Item1);
        }

        lastLength = currentLength;
        currentLength = -1;
        return clickResult;
    }

    public bool IsShip(int x, int y)
    {
        return board.board[x, y];
    }

    public Board GetBoard()
    {
        return board;
    }
}