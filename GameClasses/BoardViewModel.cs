namespace GameClasses;

public class BoardViewModel
{
    private Board board;
    private int currentLength;
    private bool isHorizontal;
    private bool prepareDelete;

    public BoardViewModel(Board board)
    {
        this.board = board;
        currentLength = -1;
        isHorizontal = false;
        prepareDelete = false;
    }

    public void ChangeRotation()
    {
        isHorizontal = !isHorizontal;
    }

    public void SetShip(int length)
    {
        currentLength = length;
    }

    public Tuple<bool, List<Tuple<int, int>>> CanPutShip(int x, int y)
    {
        bool canPut = true;
        List<Tuple<int, int>> occupiedTiles = new List<Tuple<int, int>>();
        for (int i = 0; i < currentLength; ++i)
        {
            if (0 <= x && x < 10 && 0 <= y && y < 10)
            {
                occupiedTiles.Add(new Tuple<int, int>(x, y));
                if (board.board[x, y])
                {
                    canPut = false;
                }

                if (isHorizontal)
                {
                    x++;
                }
                else
                {
                    y++;
                }
            }
            else
            {
                canPut = false;
                break;
            }
        }

        return new Tuple<bool, List<Tuple<int, int>>>(canPut, occupiedTiles);
    }

    public void PrepareToDelete()
    {
        prepareDelete = true;
    }

    public Tuple<int, int> GetTile(int xCoord, int yCoord)
    {
        return new Tuple<int, int>(xCoord / 30, yCoord / 30);
    }

    public void PutShip(int x, int y)
    {
        var canPut = CanPutShip(x, y);
        if (!canPut.Item1)
        {
            currentLength = -1;
            return;
        }

        if (isHorizontal)
        {
            board.AddShip(x, y, x + currentLength, y);
        }
        else
        {
            board.AddShip(x, y, x, y + currentLength);
        }
    }

    public void DeleteShip(int x, int y)
    {
        board.DeleteShip(x, y);
    }

    public void Click(int xCoord, int yCoord)
    {
        var tile = GetTile(xCoord, yCoord);
        if (prepareDelete)
        {
            DeleteShip(tile.Item1, tile.Item2);
        }
        else if (currentLength != -1)
        {
            PutShip(tile.Item1, tile.Item2);
        }
    }
}