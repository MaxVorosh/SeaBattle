namespace GameClasses;

public class Player
{
    /// <summary>
    /// Class of a game player, includes self-board and other-board. Also contains information, is it computer
    /// Tuple<int, int> GetNextComputerShot(int minX, int maxX, int minY, int maxY):
    /// returns coords of next computer shot if computer was hit a part of ship from (minX, minY) to (maxX, maxY)
    /// List<Tuple<int, int, bool>> SetMissedShots(int x, int y):
    /// returns coords of neighbours of a killed ship, that contains tile (x, y), that will be marked as missed
    /// List<Tuple<int, int, bool>> UpdateDataAfterShot(int x, int y):
    /// Gets coords of a shot. Open tile with this coords and set missed shots, if it's necessary
    /// Returns coords of changed tiles with bool mark. True if that coord changed by player, false, if automatically
    /// List<Tuple<int, int>> GetCanShootTiles():
    /// returns coords where we can shoot
    /// List<Tuple<int, int, bool>> MakeComputerMove():
    /// makes computer move. Returns tiles, that was changed with bool marks
    /// List<Tuple<int, int, bool>> MakeHumanMove(int x, int y):
    /// makes human move, by shooting in (x, y) tile. Returns coords with bool marks
    /// bool IsWin() - returns true if player win, else false
    /// </summary>
    public bool isComputer;
    public Board playerBoard;
    public HideBoard oppositeBoard;
    public List<Tuple<int, int>> currentShip; // Ship, that we hit
    private Random rnd;
    public bool isMoveStarted; // Is our turn

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
        {   // Ship is Horizontal
            minY -= 1;
            maxY += 1;
            x = minX;
            if (minY < 0 || (oppositeBoard.board[x, minY] != TileCondition.Unknown || rnd.Next() % 2 == 0) && maxY < 10)
                y = maxY; // If minY is not valid or random chooses maxY
            else
                y = minY;
        }
        else
        {   //Ship is Vertical
            y = minY;
            minX -= 1;
            maxX += 1;
            if (minX < 0 || (oppositeBoard.board[minX, y] != TileCondition.Unknown || rnd.Next() % 2 == 0) && maxX < 10)
                x = maxX; // If minX is not valid or random chooses MaxX
            else
                x = minX;
        }

        return new Tuple<int, int>(x, y);
    }

    public List<Tuple<int, int, bool>> SetMissedShots(int x, int y)
    {
        var result = new List<Tuple<int, int, bool>>();
        int[] movesX = { 1, -1, 0, 0, 1, 1, -1, -1};
        int[] movesY = { 0, 0, 1, -1, 1, -1, 1, -1}; // Arrays of neighbours of a particular tile
        TileCondition[,] board = oppositeBoard.board;
        for (int i = 0; i < 4; ++i)
        {
            int currentX = x;
            int currentY = y; // Coords, that will be run in particular direction
            while (currentX >= 0 && currentX < 10 && currentY >= 0 && currentY < 10 &&
                   board[currentX, currentY] == TileCondition.Hit) //If coords are valid and there's a hit ship there
            {
                for (int k = 0; k < 8; ++k)
                {
                    int newX = currentX + movesX[k];
                    int newY = currentY + movesY[k]; // NeighbourTile coords
                    if (newX >= 0 && newX < 10 && newY >= 0 && newY < 10 && board[newX, newY] == TileCondition.Unknown)
                    {   // Coords are valid and tile should be missed
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
        var result = oppositeBoard.OpenTile(x, y); // Open tile (x, y)
        var tiles = new List<Tuple<int, int, bool>>();
        tiles.Add(new Tuple<int, int, bool>(x, y, true));
        if (result == ShootResult.Killed)
        {   // Ship is killed
            currentShip = new List<Tuple<int, int>>(); // old ship is not exist
            var missed = SetMissedShots(x, y);
            foreach (var miss in missed)
            {
                tiles.Add(miss);
            }
        }
        else if (result == ShootResult.Missed)
        {   // We missed
            isMoveStarted = false;
        }
        else
        {   // We hit but not kill
            currentShip.Add(new Tuple<int, int>(x, y));
        }

        return tiles;
    }

    public List<Tuple<int, int>> GetCanShootTiles()
    {
        List<Tuple<int, int>> canShoot = new List<Tuple<int, int>>();
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                // Runs for every board coord
                if (!oppositeBoard.IsShooted(i, j))
                {
                    canShoot.Add(new Tuple<int, int>(i, j)); // If we not shoot, we can do it
                }
            }
        }

        return canShoot;
    }

    public List<Tuple<int, int, bool>> MakeComputerMove()
    {
        if (!isComputer)
            return new List<Tuple<int, int, bool>>(); // Humans cannot make computer moves

        Tuple<int, int> newShot = new Tuple<int, int>(-1, -1);
        if (currentShip.Count >= 2)
        {   // We should choose one of 2 opportunities
            int minX = currentShip[0].Item1, maxX = currentShip[0].Item1;
            int minY = currentShip[0].Item2, maxY = currentShip[0].Item2; // Ship coords
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
        {   // We should choose one of 4 opportunities
            Tuple<int, int>[] moves =
            {
                new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(0, -1),
                new Tuple<int, int>(0, 1)
            };
            for (int i = 0; i < 4; ++i)
            {
                int ind = rnd.Next() % (4 - i) + i;
                (moves[i], moves[ind]) = (moves[ind], moves[i]); // Shuffle moves
            }

            for (int i = 0; i < 4; ++i)
            {
                int newX = currentShip[0].Item1 + moves[i].Item1;
                int newY = currentShip[0].Item2 + moves[i].Item2;
                if (newX >= 0 && newY >= 0 && newX < 10 && newY < 10 &&
                    oppositeBoard.board[newX, newY] == TileCondition.Unknown)
                {
                    newShot = new Tuple<int, int>(newX, newY); // If we can shoot there, we shoot
                }
            }
        }
        else
        {
            List<Tuple<int, int>> canShoot = GetCanShootTiles();
            newShot = canShoot[rnd.Next() % canShoot.Count]; // Get random tile, because we have no information
        }
        return UpdateDataAfterShot(newShot.Item1, newShot.Item2);
    }

    public List<Tuple<int, int, bool>> MakeHumanMove(int x, int y)
    {
        if (isComputer || oppositeBoard.IsShooted(x, y))
        {   // Computers cannot make human moves; we cannot shoot one tile twice
            return new List<Tuple<int, int, bool>>(); 
        }
        return UpdateDataAfterShot(x, y);
    }

    public bool IsWin()
    {
        return oppositeBoard.aliveTiles == 0; // Opponent have no alive ships
    }
}