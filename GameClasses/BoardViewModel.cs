namespace GameClasses;

public class BoardViewModel
{
    private Board board;
    private int currentLength;
    private bool isHorizontal;

    public BoardViewModel(Board board)
    {
        this.board = board;
        currentLength = -1;
        isHorizontal = false;
    }

    public void ChangeRotation()
    {
        isHorizontal = !isHorizontal;
    }

    public void SetShip(int length)
    {
        currentLength = length;
    }

    public Tuple<bool, List<Tuple<int, int>>> CanPutShip(Tuple<int, int> startTile)
    {
        int x = startTile.Item1;
        int y = startTile.Item2;
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
            }
            else
            {
                break;
            }
        }

        return new Tuple<bool, List<Tuple<int, int>>>(canPut, occupiedTiles);
    }
}