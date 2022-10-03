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

    public void MakeHumanMove(int x, int y)
    {
        if (result != Result.NotEnd || !human.MakeHumanMove(x, y))
        {
            return;
        }
        if (computer.IsLose())
        {
            result = Result.PlayerWins;
            return;
        }
        if (human.isMoveStarted)
        {
            return;
        }

        computer.isMoveStarted = true;
        while (computer.isMoveStarted)
        {
            computer.MakeComputerMove();
        }

        if (human.IsLose())
        {
            result = Result.ComputerWins;
        }
    }
}