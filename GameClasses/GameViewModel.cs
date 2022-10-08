using System.Net.Http.Headers;

namespace GameClasses;

public class GameViewModel
{
    /// <summary>
    /// MVVM-type model that linked GameWindow and Game classes
    /// Board GenerateRandomBoard() - returns board with random ships location
    /// List<Tuple<int, int, int, bool>> GetShips():
    /// Returns ships, that are on the board in format (xCoord, yCoord, Length of ship, is horizontal rotation)
    /// List<Tuple<int, int, bool>> MakeMove(int xCoord, int yCoord):
    /// Makes move on tile, that have (xCoord, yCoord) coords regarding of window
    /// Returns changed tiles with bool mark
    /// TileCondition GetCondition(int y, int x, bool isFirstPlayer):
    /// returns information about tile (x, y) on the opposite board
    /// </summary>
    private Game game;

    private int boardSize;
    private const int maxShip = 4;
    private const int tileSize = 30;

    public GameViewModel(Board personBoard)
    {
        boardSize = personBoard.GetBoardSize();
        Board randomBoard = GenerateRandomBoard();
        game = new Game(personBoard, randomBoard);
    }

    private List<Tuple<int, int>> Shuffle(List<Tuple<int, int>> data)
    {
        // Shuffles list
        var rnd = new Random();
        for (int i = 0; i < data.Count; ++i)
        {
            int x = rnd.Next() % (data.Count - i) + i;
            (data[i], data[x]) = (data[x], data[i]);
        }

        return data;
    }

    public Board GenerateRandomBoard()
    {
        var rnd = new Random();
        List<Tuple<int, int>> tiles = new List<Tuple<int, int>>();
        bool[] isHorizontal = new bool[boardSize];
        List<int> length = new List<int>(); // List of ship's length
        for (int i = 0; i < boardSize; ++i)
        {
            isHorizontal[i] = rnd.Next() % 2 == 0;
            for (int j = 0; j < boardSize; ++j)
            {
                tiles.Add(new Tuple<int, int>(i, j));
            }
        }
        tiles = Shuffle(tiles); // shuffled list with coords
        for (int i = maxShip; i >= 1; --i)
        {
            for (int j = 0; j < maxShip + 1 - i; ++j)
            {
                length.Add(i); 
            }
        }

        return PutShipsOnRandomBoard(isHorizontal, tiles, length);

        
    }

    private Board PutShipsOnRandomBoard(bool[] isHorizontal, List<Tuple<int, int>> tiles, List<int> length)
    {
        var randomBoard = new Board();
        for (int le = 0; le < length.Count; ++le)
        {
            // For length from big to small
            for (int i = 0; i < tiles.Count; ++i)
            {
                int x = tiles[i].Item1;
                int y = tiles[i].Item2;
                int nextX = x;
                int nextY = y;
                if (isHorizontal[le])
                {
                    nextX += length[le] - 1;
                }
                else
                {
                    nextY += length[le] - 1;
                }
                // nextX, nextY - end of ship
                if (randomBoard.CheckShipPosition(x, y, nextX, nextY))
                {
                    randomBoard.AddShip(x, y, nextX, nextY); // If we can put ship on random tile - we do it
                    break;
                }
            }
        }
        return randomBoard;
    }

    public List<Tuple<int, int, int, bool>> GetShips()
    {
        bool[,] playerBoard = game.human.playerBoard.board;
        List<Tuple<int, int, int, bool>> result = new List<Tuple<int, int, int, bool>>();
        for (int i = 0; i <boardSize; ++i)
        {
            for (int j = 0; j < boardSize; ++j)
            {   // Run for all coords
                // If it is first ship tile
                if ((i == 0 || !playerBoard[i - 1, j]) && (j == 0 || !playerBoard[i, j - 1]) && playerBoard[i, j])
                {
                    int moveX = 0, moveY = 0, x = i, y = j; // x, y - end coords of ship
                    bool isHorizontal = i == boardSize - 1 || !playerBoard[i + 1, j]; // if i + 1 not valid or i + 1 is empty
                    int cnt = 0;
                    if (i == boardSize - 1 || !playerBoard[i + 1, j])
                        moveY = 1;
                    else
                        moveX = 1;
                    while (x <= boardSize - 1 && y <= boardSize - 1 && playerBoard[x, y])
                    {
                        cnt++;
                        x += moveX;
                        y += moveY;
                    }
                    result.Add(new Tuple<int, int, int, bool>(i, j, cnt, isHorizontal));
                }
            }
        }
        return result;
    }

    public List<Tuple<int, int, bool>> MakeMove(int xCoord, int yCoord)
    {
        int x = yCoord / tileSize;
        int y = xCoord / tileSize; // Converts coords to tile
        var result = game.MakeHumanMove(x, y);
        for (int i = 0; i < result.Count; ++i)
        {
            // Change x, y to present it to window
            result[i] = new Tuple<int, int, bool>(result[i].Item2, result[i].Item1, result[i].Item3);
        }
        return result;
    }

    public TileCondition GetCondition(int y, int x, bool isFirstPlayer)
    {
        if (isFirstPlayer)
        {
            return game.human.oppositeBoard.board[x, y];
        }
        return game.computer.oppositeBoard.board[x, y];
    }

    public Result GetGameResult()
    {
        return game.result;
    }
}