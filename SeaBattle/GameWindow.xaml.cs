using System.Windows;
using GameClasses;

namespace SeaBattle;

public partial class GameWindow : Window
{
    private GameViewModel game;
    
    public GameWindow(Board board)
    {
        game = new GameViewModel(board);
        InitializeComponent();
    }
}