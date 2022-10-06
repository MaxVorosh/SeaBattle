using GameClasses;
using static NUnit.Framework.Assert;

namespace TestSeaBattle;

public class HideBoardTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void IsShipKilled()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 0);
        board.AddShip(5, 5, 5, 7);
        var hideBoard = new HideBoard(board);
        That(hideBoard.IsShipKilled(0, 0), Is.True);
        That(hideBoard.IsShipKilled(5, 5), Is.False);
        hideBoard.board[5, 6] = TileCondition.Hit;
        That(hideBoard.IsShipKilled(5, 7), Is.False);
        hideBoard.board[5, 5] = TileCondition.Hit;
        That(hideBoard.IsShipKilled(5, 7), Is.True);
    }

    [Test]
    public void OpenTileTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 0);
        board.AddShip(5, 5, 5, 7);
        var hideBoard = new HideBoard(board);
        That(hideBoard.OpenTile(0, 0), Is.EqualTo(ShootResult.Killed));
        That(hideBoard.OpenTile(5, 6), Is.EqualTo(ShootResult.Injured));
        That(hideBoard.OpenTile(1, 1), Is.EqualTo(ShootResult.Missed));
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if ((i == 0 && j == 0) || (i == 5 && j == 6))
                {
                    That(hideBoard.board[i, j], Is.EqualTo(TileCondition.Hit));
                }
                else if (i == 1 && j == 1)
                {
                    That(hideBoard.board[i, j], Is.EqualTo(TileCondition.Missed));
                }
                else
                {
                    That(hideBoard.board[i, j], Is.EqualTo(TileCondition.Unknown));
                }
            }
        }
    }
}