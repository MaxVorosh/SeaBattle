namespace GameClasses;

public class Game
{
    public Player human;
    public Player computer;
    public Result result;

    public Game(Board humanBoard, Board computerBoard)
    {
        human = new Player(false, humanBoard, computerBoard);
        computer = new Player(true, computerBoard, humanBoard);
        human.isMoveStarted = true;
        result = Result.NotEnd;
    }

    public List<Tuple<int, int, bool>> MakeHumanMove(int x, int y)
    {
        human.isMoveStarted = true;
        var moves = new List<Tuple<int, int, bool>>();
        var humanMoves = human.MakeHumanMove(x, y);
        if (result != Result.NotEnd || humanMoves.Count == 0)
        {
            return moves;
        }
        foreach (var move in humanMoves)
        {
            moves.Add(move);
        }
        if (human.IsWin())
        {
            result = Result.PlayerWins;
            return moves;
        }
        if (human.isMoveStarted)
        {
            return moves;
        }
        computer.isMoveStarted = true;
        while (computer.isMoveStarted)
        {
            var computerMoves = computer.MakeComputerMove();
            foreach (var move in computerMoves)
            {
                moves.Add(move);
            }
        }
        if (computer.IsWin())
        {
            result = Result.ComputerWins;
        }
        return moves;
    }
}