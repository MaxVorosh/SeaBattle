using System.Net.Http.Headers;

namespace GameClasses;

public class GameViewModel
{
    private Game game;

    public GameViewModel(Board personBoard)
    {
        Board randomBoard = generateRandomBoard();
        this.game = new Game(personBoard, randomBoard);
    }

    private List<Tuple<int, int>> Shuffle(List<Tuple<int, int>> data)
    {
        var rnd = new Random();
        for (int i = 0; i < data.Count; ++i)
        {
            int x = rnd.Next() % (data.Count - i) + i;
            (data[i], data[x]) = (data[x], data[i]);
        }

        return data;
    }

    public Board generateRandomBoard()
    {
        var rnd = new Random();
        List<Tuple<int, int>> tiles = new List<Tuple<int, int>>();
        bool[] isHorizontal = new bool[10];
        List<int> length = new List<int>();
        for (int i = 0; i < 10; ++i)
        {
            isHorizontal[i] = rnd.Next() % 2 == 0;
            for (int j = 0; j < 10; ++j)
            {
                tiles.Add(new Tuple<int, int>(i, j));
            }
        }
        tiles = Shuffle(tiles);
        for (int i = 4; i >= 1; --i)
        {
            for (int j = 0; j < 5 - i; ++j)
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
            for (int i = 0; i < tiles.Count; ++i)
            {
                int x = tiles[i].Item1;
                int y = tiles[i].Item2;
                int nextX = x;
                int nextY = y;
                if (isHorizontal[le])
                {
                    nextX += length[le];
                }
                else
                {
                    nextY += length[le];
                }

                if (randomBoard.CheckShipPosition(x, y, nextX, nextY))
                {
                    randomBoard.AddShip(x, y, nextX, nextY);
                    break;
                }
            }
        }
        return randomBoard;
    }
}