using GameClasses;
using static NUnit.Framework.Assert;

namespace TestSeaBattle;

public class GameViewModelTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GenerateRandomBoardTest()
    {
        var gameModel = new GameViewModel(new Board());
        var board = gameModel.GenerateRandomBoard();
        bool[,] direction = new bool[10, 10];
        for (int i = 0; i < 10; ++i)
        {
            int cnt = 0;
            for (int j = 0; j < 10; ++j)
            {
                if (board.board[i, j])
                {
                    cnt++;
                }
                else
                {
                    cnt = 0;
                }
                if (cnt >= 2)
                {
                    direction[i, j] = true;
                    direction[i, j - 1] = true;
                }
                That(cnt <= 4, Is.True);
            }
        }

        for (int i = 0; i < 10; ++i)
        {
            int cnt = 0;
            for (int j = 0; j < 10; ++j)
            {
                if (board.board[j, i])
                {
                    cnt++;
                }
                else
                {
                    cnt = 0;
                }
                That(cnt <= 4, Is.True);
                if (cnt >= 2)
                {
                    That(direction[j, i], Is.False);
                    That(direction[j - 1, i], Is.False);
                }
            }
        }
    }

    [Test]
    public void GetShipsTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 3);
        board.AddShip(5, 5, 5, 5);
        board.AddShip(8, 8, 9, 8);
        var gameViewModel = new GameViewModel(board);
        var ships = gameViewModel.GetShips();
        That(ships.Count, Is.EqualTo(3));
        That(ships.Contains(new Tuple<int, int, int, bool>(0, 0, 4, true)), Is.True);
        That(ships.Contains(new Tuple<int, int, int, bool>(5, 5, 1, true)), Is.True);
        That(ships.Contains(new Tuple<int, int, int, bool>(8, 8, 2, false)), Is.True);
    }
}