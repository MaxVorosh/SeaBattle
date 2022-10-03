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
            if (minY < 0 || rnd.Next() % 2 == 0)
                y = maxY;
            else
                y = minY;
        }
        else
        {
            y = minY;
            minX -= 1;
            maxX += 1;
            if (minX < 0 || rnd.Next() % 2 == 0)
                x = maxX;
            else
                x = minX;
        }

        return new Tuple<int, int>(x, y);
    }

    public void UpdateDataAfterShot(int x, int y)
    {
        var result = oppositeBoard.OpenTile(x, y);
        if (result == ShootResult.Killed)
        {
            currentShip = new List<Tuple<int, int>>();
        }

        if (result == ShootResult.Missed)
        {
            isMoveStarted = false;
        }
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

    public bool MakeComputerMove()
    {
        if (!isComputer)
            return false;

        Tuple<int, int> newShot;
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
                new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(0, -1), new Tuple<int, int>(0, 1)
            };
            int ind = rnd.Next() % 4;
            newShot = new Tuple<int, int>(currentShip[0].Item1 + moves[ind].Item1,
                currentShip[0].Item2 + moves[ind].Item2);
        }
        else
        {
            List<Tuple<int, int>> canShoot = GetCanShootTiles();
            newShot = canShoot[rnd.Next() % canShoot.Count];
        }
        UpdateDataAfterShot(newShot.Item1, newShot.Item2);
        return true;
    }

    public bool MakeHumanMove(int x, int y)
    {
        if (isComputer || oppositeBoard.IsShooted(x, y))
        {
            return false;
        }
        UpdateDataAfterShot(x, y);
        return true;
    }

    public bool IsLose()
    {
        return oppositeBoard.aliveTiles == 0;
    }
}