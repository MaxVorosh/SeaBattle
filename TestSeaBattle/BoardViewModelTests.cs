using GameClasses;
using static NUnit.Framework.Assert;

namespace TestSeaBattle;

public class BoardViewModelTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void DeleteShipTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 3);
        var boardModel = new BoardViewModel(board);
        That(boardModel.DeleteShip(1, 1), Is.EqualTo(ClickResult.Sleep));
        That(boardModel.DeleteShip(0, 2), Is.EqualTo(ClickResult.Delete));
    }

    [Test]
    public void CanPutShipTest()
    {
        var board = new Board();
        board.AddShip(0, 0, 0, 3);
        var boardModel = new BoardViewModel(board);
        boardModel.SetShip(2);
        var firstList = new List<Tuple<int, int>>();
        firstList.Add(new Tuple<int, int>(0, 3));
        firstList.Add(new Tuple<int, int>(0, 4));
        That(boardModel.CanPutShip(0, 3), Is.EqualTo(new Tuple<bool, List<Tuple<int, int>>>(false, firstList)));
        var secondList = new List<Tuple<int, int>>();
        secondList.Add(new Tuple<int, int>(4, 4));
        secondList.Add(new Tuple<int, int>(4, 5));
        That(boardModel.CanPutShip(4, 4), Is.EqualTo(new Tuple<bool, List<Tuple<int, int>>>(true, secondList)));
    }

    [Test]
    public void PutShipTest()
    {
        var boardModel = new BoardViewModel(new Board());
        boardModel.SetShip(2);
        var result = boardModel.PutShip(0, 0);
        That(result, Is.EqualTo(ClickResult.Put));
        boardModel.SetShip(3);
        result = boardModel.PutShip(0, 1);
        That(result, Is.EqualTo(ClickResult.Sleep));
        boardModel.SetShip(3);
        boardModel.ChangeRotation();
        result = boardModel.PutShip(5, 5);
        That(result, Is.EqualTo(ClickResult.Put));
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                if ((i == 0 && j <= 1) || (i >= 5 && i <= 7 && j == 5))
                {
                    That(boardModel.GetBoard().board[i, j], Is.True);
                }
                else
                {
                    That(boardModel.GetBoard().board[i, j], Is.False);
                }
            }
        }
    }
}