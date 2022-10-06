using GameClasses;
using static NUnit.Framework.Assert;

namespace TestSeaBattle;

public class PlayerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetCanShootTilesTest()
    {
        var player = new Player(false, new Board(), new Board());
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 9; j += 2)
            {
                player.oppositeBoard.board[i, j] = TileCondition.Hit;
                player.oppositeBoard.board[i, j + 1] = TileCondition.Missed;
            }
        }
        
        var tiles = player.GetCanShootTiles();
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                var tile = new Tuple<int, int>(i, j);
                if (i <= 7)
                {
                    That(tiles.Contains(tile), Is.False);
                }
                else
                {
                    That(tiles.Contains(tile), Is.True);
                }
            }
        }
    }

    [Test]
    public void MakeHumanMoveMissingTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 4);
        var player = new Player(false, new Board(), board);
        var moves = player.MakeHumanMove(1, 3);
        That(moves.Count, Is.EqualTo(1));
        That(moves[0].Item1, Is.EqualTo(1));
        That(moves[0].Item2, Is.EqualTo(3));
        That(moves[0].Item3, Is.EqualTo(true));
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (i == 1 && j == 3)
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Missed));
                }
                else
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Unknown));
                }
            }
        }
    }

    [Test]
    public void MakeHumanMoveInjuredTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 2);
        var player = new Player(false, new Board(), board);
        var moves = player.MakeHumanMove(0, 1);
        That(moves.Count, Is.EqualTo(1));
        That(moves[0].Item1, Is.EqualTo(0));
        That(moves[0].Item2, Is.EqualTo(1));
        That(moves[0].Item3, Is.EqualTo(true));
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (i == 0 && j == 1)
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Hit));
                }
                else
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Unknown));
                }
            }
        }
    }
    
    [Test]
    public void MakeHumanMoveKilledTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 0);
        var player = new Player(false, new Board(), board);
        var moves = player.MakeHumanMove(0, 0);
        That(moves.Count, Is.EqualTo(4));
        for (int i = 0; i < 4; ++i)
        {
            That(moves.Contains(new Tuple<int, int, bool>(i / 2, i % 2, i == 0)), Is.True);
        }
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (i == 0 && j == 0)
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Hit));
                }
                else if (i <= 1 && j <= 1)
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Missed));
                }
                else
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Unknown));
                }
            }
        }
    }

    [Test]
    public void MakeComputerMoveFullRandomTest()
    {
        var player = new Player(true, new Board(), new Board());
        for (int i = 0; i < 10; ++i)
        {
            player.oppositeBoard.board[i, 0] = TileCondition.Missed;
        }
        player.MakeComputerMove();
        int cnt = 0;
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (j == 0)
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Missed));
                }
                else
                {
                    if (player.oppositeBoard.board[i, j] == TileCondition.Missed)
                    {
                        cnt++;
                    }
                }
            }
        }
        That(cnt, Is.EqualTo(1));
    }

    [Test]
    public void MakeComputerMoveNotRandomTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 3);
        var player = new Player(false, new Board(), board);
        player.MakeHumanMove(0, 0);
        player.MakeHumanMove(0, 1);
        player.isComputer = true;
        player.MakeComputerMove();
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (i == 0 && j <= 2)
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Hit));
                }
                else
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Unknown));
                }
            }
        }
    }

    [Test]
    public void MakeComputerMoveSemiRandomTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 2);
        var player = new Player(false, new Board(), board);
        player.MakeHumanMove(0, 0);
        player.isComputer = true;
        player.MakeComputerMove();
        bool isRight = false;
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (i == 0 && j == 0)
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Hit));
                }
                else if (i == 0 && j == 1)
                {
                    isRight = player.oppositeBoard.board[i, j] == TileCondition.Hit;
                }
                else if (i == 1 && j == 0)
                {
                    if (isRight)
                    {
                        That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Unknown));
                    }
                    else
                    {
                        That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Missed));
                        That(player.oppositeBoard.board[j, i], Is.EqualTo(TileCondition.Unknown));
                    }
                }
                else
                {
                    That(player.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Unknown));
                }
            }
        }
    }

    [Test]
    public void IsWinTest()
    {
        var board = new Board();
        for (int i = 1; i < 10; i += 2)
        {
            for (int j = 1; j < 10; j += 2)
            {
                board.AddShip(i, j, i, j);
            }
        }

        var player = new Player(false, new Board(), board);
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                That(player.IsWin(), Is.False);
                player.MakeHumanMove(i, j);
            }
        }
        That(player.IsWin, Is.True);
    }
}