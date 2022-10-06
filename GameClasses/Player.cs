namespace GameClasses;

public class Player
{
    public bool isComputer;
    public Board playerBoard;
    public HideBoard oppositeBoard;
    public List<Tuple<int, int>> currentShip;
    private Random rnd;
    public bool isMoveStarted;

    public Player(bool isComputer, Board playerBoard, Board oppositeBoard)
    {
        this.isComputer = isComputer;
        this.playerBoard = playerBoard;
        this.oppositeBoard = new HideBoard(oppositeBoard);
        currentShip = new List<Tuple<int, int>>();
        rnd = new Random();
        isMoveStarted = false;
    }

    public Tuple<int, int> GetNextComputerShot(int minX, int maxX, int minY, int maxY)
    {
        int x, y;
        if (minX == maxX)
        {
            minY -= 1;
            maxY += 1;
            x = minX;
            if (minY < 0 || oppositeBoard.board[x, minY] != TileCondition.Unknown || rnd.Next() % 2 == 0)
                y = maxY;
            else
                y = minY;
        }
        else
        {
            y = minY;
            minX -= 1;
            maxX += 1;
            if (minX < 0 || oppositeBoard.board[minX, y] != TileCondition.Unknown || rnd.Next() % 2 == 0)
                x = maxX;
            else
                x = minX;
        }

        return new Tuple<int, int>(x, y);
    }

    public List<Tuple<int, int, bool>> SetMissedShots(int x, int y)
    {
        var result = new List<Tuple<int, int, bool>>();
        int[] movesX = { 1, -1, 0, 0, 1, 1, -1, -1};
        int[] movesY = { 0, 0, 1, -1, 1, -1, 1, -1};
        TileCondition[,] board = oppositeBoard.board;
        for (int i = 0; i < 4; ++i)
        {
            int currentX = x;
            int currentY = y;
            while (currentX >= 0 && currentX < 10 && currentY >= 0 && currentY < 10 &&
                   board[currentX, currentY] == TileCondition.Hit)
            {
                for (int k = 0; k < 8; ++k)
                {
                    int newX = currentX + movesX[k];
                    int newY = currentY + movesY[k];
                    if (newX >= 0 && newX < 10 && newY >= 0 && newY < 10 && board[newX, newY] == TileCondition.Unknown)
                    {
                        board[newX, newY] = TileCondition.Missed;
                        result.Add(new Tuple<int, int, bool>(newX, newY, false));
                    }
                }
                currentX += movesX[i];
                currentY += movesY[i];
            }
        }

        return result;
    }

    public List<Tuple<int, int, bool>> UpdateDataAfterShot(int x, int y)
    {
        var result = oppositeBoard.OpenTile(x, y);
        var tiles = new List<Tuple<int, int, bool>>();
        tiles.Add(new Tuple<int, int, bool>(x, y, true));
        if (result == ShootResult.Killed)
        {
            currentShip = new List<Tuple<int, int>>();
            var missed = SetMissedShots(x, y);
            foreach (var miss in missed)
            {
                tiles.Add(miss);
            }
        }
        else if (result == ShootResult.Missed)
        {
            isMoveStarted = false;
        }
        else
        {
            currentShip.Add(new Tuple<int, int>(x, y));
        }

        return tiles;
    }

    List<Tuple<int, int>> GetCanShootTiles()
    {
        List<Tuple<int, int>> canShoot = new List<Tuple<int, int>>();
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (!oppositeBoard.IsShooted(i, j))
                {
                    canShoot.Add(new Tuple<int, int>(i, j));
                }
            }
        }

        return canShoot;
    }

    public List<Tuple<int, int, bool>> MakeComputerMove()
    {
        if (!isComputer)
            return new List<Tuple<int, int, bool>>();

        Tuple<int, int> newShot = new Tuple<int, int>(-1, -1);
        if (currentShip.Count >= 2)
        {
            int minX = currentShip[0].Item1, maxX = currentShip[0].Item1;
            int minY = currentShip[0].Item2, maxY = currentShip[0].Item2;
            for (int i = 1; i < currentShip.Count; ++i)
            {
                minX = Math.Min(minX, currentShip[i].Item1);
                maxX = Math.Max(maxX, currentShip[i].Item1);
                minY = Math.Min(minY, currentShip[i].Item2);
                maxY = Math.Max(maxY, currentShip[i].Item2);
            }

            newShot = GetNextComputerShot(minX, maxX, minY, maxY);
        }
        else if (currentShip.Count == 1)
        {
            Tuple<int, int>[] moves =
            {
                new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(0, -1),
                new Tuple<int, int>(0, 1)
            };
            for (int i = 0; i < 4; ++i)
            {
                int ind = rnd.Next() % (4 - i) + i;
                (moves[i], moves[ind]) = (moves[ind], moves[i]);
            }

            for (int i = 0; i < 4; ++i)
            {
                int newX = currentShip[0].Item1 + moves[i].Item1;
                int newY = currentShip[0].Item2 + moves[i].Item2;
                if (newX >= 0 && newY >= 0 && newX < 10 && newY < 10 &&
                    oppositeBoard.board[newX, newY] == TileCondition.Unknown)
                {
                    newShot = new Tuple<int, int>(newX, newY);
                }
            }
        }
        else
        {
            List<Tuple<int, int>> canShoot = GetCanShootTiles();
            newShot = canShoot[rnd.Next() % canShoot.Count];
        }
        return UpdateDataAfterShot(newShot.Item1, newShot.Item2);
    }

    public List<Tuple<int, int, bool>> MakeHumanMove(int x, int y)
    {
        if (isComputer || oppositeBoard.IsShooted(x, y))
        {
            return new List<Tuple<int, int, bool>>();
        }
        return UpdateDataAfterShot(x, y);
    }

    public bool IsWin()
    {
        return oppositeBoard.aliveTiles == 0;
    }
}