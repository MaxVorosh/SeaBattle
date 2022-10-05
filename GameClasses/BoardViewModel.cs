namespace GameClasses;

public class BoardViewModel
{
    private Board board;
    private int currentLength;
    private bool isHorizontal;
    private bool prepareDelete;
    private int lastDeleteResult;
    private int lastLength;

    public BoardViewModel(Board board)
    {
        this.board = board;
        currentLength = -1;
        isHorizontal = false;
        prepareDelete = false;
        lastDeleteResult = -1;
        lastLength = -1;
    }

    public void ChangeRotation()
    {
        isHorizontal = !isHorizontal;
    }

    public void SetShip(int length)
    {
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
            if (0 <= x && x < 10 && 0 <= y && y < 10)
            {
                occupiedTiles.Add(new Tuple<int, int>(x, y));
                if (i != currentLength - 1)
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
        return new Tuple<int, int>(xCoord / 30, yCoord / 30);
    }

    public ClickResult PutShip(int x, int y)
    {
        var canPut = CanPutShip(x, y);
        if (!canPut.Item1)
        {
            lastLength = currentLength;
            currentLength = -1;
            return ClickResult.Sleep;
        }

        bool result;
        if (isHorizontal)
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
            prepareDelete = false;
            clickResult = DeleteShip(tile.Item2, tile.Item1);
        }
        if (currentLength != -1)
        {
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
}