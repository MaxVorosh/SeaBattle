using static NUnit.Framework.Assert;
using GameClasses;

namespace TestSeaBattle;

public class BoardTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void InitializationTest()
    {
        var board = new Board();
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                That(board.board[i, j], Is.False);
            }
        }
    }

    [Test]
    public void CheckShipPositionTest()
    {
        Board board = new Board();
        for (int i = 0; i < 9; ++i)
        {
            
            board.board[i, 0] = true;
        }

        for (int i = 0; i < 10; ++i)
        {
            That(board.CheckShipPosition(i, 1, i, 4), Is.False);
            That(board.CheckShipPosition(i, 2, i, 4), Is.True);
        }
    }

    [Test]
    public void AddShipTest()
    {
        var board = new Board();
        var board2 = new Board();
        var board3 = new Board();
        var board4 = new Board();
        for (int i = 0; i < 9; ++i)
        {
            board.board[i, 0] = true;
            board2.board[i, 0] = true;
            board3.board[i, 0] = true;
            board4.board[i, 0] = true;
        }

        board.AddShip(0, 0, 3, 0);
        board2.AddShip(9, 0, 9, 3);
        board3.AddShip(2, 2, 2, 2);
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                That(board.board[i, j], Is.EqualTo(board4.board[i, j]));
                That(board2.board[i, j], Is.EqualTo(board4.board[i, j]));
                if (i != 2 || j != 2)
                {
                    That(board3.board[i, j], Is.EqualTo(board4.board[i, j]));
                }
                else
                {
                    That(board3.board[i, j], Is.True);
                    That(board4.board[i, j], Is.False);
                }
            }
        }
    }

    [Test]
    public void DeleteShipTest()
    {
        var board = new Board();
        var board2 = new Board();
        board.AddShip(0, 0, 0, 4);
        board.AddShip(1, 7, 1, 9);
        board.DeleteShip(1, 8);
        board2.AddShip(0, 0, 0, 4);
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                That(board.board[i, j], Is.EqualTo(board2.board[i, j]));
            }
        }
    }
}