namespace GameClasses;

public class Game
{
    /// <summary>
    /// Game class. Includes 2 players and game result
    /// List<Tuple<int, int, bool>> MakeHumanMove(int x, int y):
    /// Makes human move by shooting (x, y) tile. Than get computer's response. Returns changed tiles with bool marks 
    /// </summary>
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
        human.isMoveStarted = true; // Part of human move
        var moves = new List<Tuple<int, int, bool>>();
        var humanMoves = human.MakeHumanMove(x, y); // makes human move
        if (result != Result.NotEnd || humanMoves.Count == 0)
        {
            return moves; // If game ends or we can't shoot (x, y)
        }
        foreach (var move in humanMoves)
        {
            moves.Add(move);
        }
        if (human.IsWin())
        {
            result = Result.PlayerWins;
            return moves; // Game ends by human win
        }
        if (human.isMoveStarted)
        {
            return moves; // If human can shoot once again before computer
        }
        computer.isMoveStarted = true; // Computer move part
        while (computer.isMoveStarted)
        {
            var computerMoves = computer.MakeComputerMove(); // Makes computer move
            foreach (var move in computerMoves)
            {
                moves.Add(move);
            }
        }
        if (computer.IsWin())
        {
            result = Result.ComputerWins; // Game ends by computer win
        }
        return moves;
    }
}