namespace GameClasses;

public class GameViewModel
{
    private Game game;
    private int currentLength;
    private bool isHorizontal;

    public GameViewModel(Game game)
    {
        this.game = game;
        currentLength = -1;
        isHorizontal = false;
    }
}