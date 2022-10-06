using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameClasses;

namespace SeaBattle;

public partial class GameWindow : Window
{
    private GameViewModel game;
    private List<Tuple<int, int, bool>> nextMoves;
    private bool isMoveStarted;
    private bool gameEnded;

    public GameWindow(Board board)
    {
        game = new GameViewModel(board);
        nextMoves = new List<Tuple<int, int, bool>>();
        isMoveStarted = false;
        gameEnded = false;
        InitializeComponent();
        SetGrid(PlayerBoard);
        SetGrid(ComputerBoard);
        SetShipsImages();
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

    public void SetShipsImages()
    {
        var ships = game.GetShips();
        foreach (var ship in ships)
        {
            var im = new Image();
            string path =
                "C:\\Users\\mavor\\OneDrive\\Рабочий стол\\ВУЗ\\Основы программирования\\SeaBattle\\SeaBattle\\data\\";
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path + "ship" + ship.Item3.ToString() + "_aq_s.png");
            if (ship.Item4 && ship.Item3 > 1)
            {
                bitmapImage.Rotation = Rotation.Rotate270;
            }
            bitmapImage.EndInit();
            im.Source = bitmapImage;
            if (ship.Item4)
            {
                im.Width = 30 * ship.Item3 - 2;
                im.Height = 28;
            }
            else
            {
                im.Width = 28;
                im.Height = 30 * ship.Item3 - 2;
            }
            PlayerBoard.Children.Add(im);
            Canvas.SetLeft(im, ship.Item2 * 30 + 2);
            Canvas.SetTop(im, ship.Item1 * 30 + 2);
        }
    }

    public void OppositeBoardClick(object sender, MouseButtonEventArgs e)
    {
        if (isMoveStarted || gameEnded)
        {
            return;
        }

        isMoveStarted = true;
        var p = e.GetPosition(this);
        p = Game.TranslatePoint(p, ComputerBoard);
        int x = (int)p.X - 1;
        int y = (int)p.Y - 1;
        if (x < 0 || x >= 300 || y < 0 || y >= 300)
        {
            return;
        }
        nextMoves = game.MakeMove(x, y);
        MakeNextMoves();
        isMoveStarted = false;
    }

    private void MakeNextMoves()
    {
        bool isFirst = true;
        for (int i = 0; i < nextMoves.Count; ++i)
        {
            var move = nextMoves[i];
            var condition = game.GetCondition(move.Item1, move.Item2, isFirst);
            DrawResult(move.Item1, move.Item2, isFirst, condition == TileCondition.Hit);
            if (condition == TileCondition.Missed && move.Item3)
            {
                isFirst = false;
            }
        }

        if (game.GetGameResult() != Result.NotEnd)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        if (game.GetGameResult() == Result.ComputerWins)
        {
            EndLabel.Text = "Computer\nwins";
        }
        else
        {
            EndLabel.Text = "You win!";
        }
        gameEnded = true;
        RestartButton.Visibility = Visibility.Visible;
        var im = new Image();
        string path =
            "C:\\Users\\mavor\\OneDrive\\Рабочий стол\\ВУЗ\\Основы программирования\\SeaBattle\\SeaBattle\\data\\";
        im.Source = new BitmapImage(new Uri(path + "restart_aq.png"));
        RestartButton.Content = im;
    }

    private void DrawResult(int x, int y, bool isFirstPlayer, bool isCross)
    {
        var im = new Image();
        string path =
            "C:\\Users\\mavor\\OneDrive\\Рабочий стол\\ВУЗ\\Основы программирования\\SeaBattle\\SeaBattle\\data\\";
        string name = isCross ? "cross.png" : "miss.png";
        im.Source = new BitmapImage(new Uri(path + name));
        im.Width = 28;
        im.Height = 28;
        if (isFirstPlayer)
        {
            ComputerBoard.Children.Add(im);
        }
        else
        {
            PlayerBoard.Children.Add(im);
        }
        Canvas.SetLeft(im, 30 * x + 2);
        Canvas.SetTop(im, 30 * y + 2);
    }

    public void Restart(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
}