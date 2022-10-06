using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameClasses;

namespace SeaBattle;

public partial class GameWindow : Window
{
    private GameViewModel game;

    public GameWindow(Board board)
    {
        game = new GameViewModel(board);
        InitializeComponent();
        SetGrid(PlayerBoard);
        SetGrid(ComputerBoard);
    }

    public void SetGrid(Canvas canvas)
    {
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                var border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = new SolidColorBrush(Colors.Navy);
                border.Width = 30;
                border.Height = 30;
                canvas.Children.Add(border);
                Canvas.SetLeft(border, i * 30 + 1);
                Canvas.SetTop(border, j * 30 + 1);
            }
        }
    }
}