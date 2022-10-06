using GameClasses;
using static NUnit.Framework.Assert;

namespace TestSeaBattle;

public class GameTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void MakeHumanMoveTest()
    {
        var playerBoard = new Board();
        var computerBoard = new Board();
        playerBoard.AddShip(0, 0, 0, 1);
        computerBoard.AddShip(0, 0, 0, 1);
        computerBoard.AddShip(9, 0, 9, 1);
        var game = new Game(playerBoard, computerBoard);
        game.human.MakeHumanMove(0, 0);
        game.human.MakeHumanMove(1, 0);
        game.computer.isComputer = false;
        game.computer.MakeHumanMove(0, 0);
        game.computer.MakeHumanMove(1, 0);
        game.computer.isComputer = true;
        game.MakeHumanMove(0, 1);
        game.MakeHumanMove(0, 3);
        int cnt = 0;
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if (i == 0 && j <= 1)
                {
                    That(game.human.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Hit));
                    That(game.computer.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Hit));
                }
                else if (i == 1 && j == 0)
                {
                    That(game.human.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Missed));
                    That(game.computer.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Missed));
                }
                else
                {
                    if (game.computer.oppositeBoard.board[i, j] == TileCondition.Missed)
                    {
                        cnt++;
                    }

                    if ((i == 0 && j <= 3) || (i == 1 && j <= 2))
                    {
                        That(game.human.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Missed));
                    }
                    else
                    {
                        That(game.human.oppositeBoard.board[i, j], Is.EqualTo(TileCondition.Unknown));
                    }
                }
            }
        }
        That(cnt, Is.EqualTo(4));
    }
}