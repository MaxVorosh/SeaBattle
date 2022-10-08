using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameClasses;

namespace SeaBattle;

public partial class GameWindow : Window
{
    /// <summary>
    /// Interaction for GameWindow.xaml
    /// </summary>
    private GameViewModel game;
    private List<Tuple<int, int, bool>> nextMoves;
    private bool isMoveStarted;
    private bool gameEnded;
    private int boardSize;
    private int tileSize = 30;
    private int thickness = 1;
    private int deltaCanvas = 14;

    public GameWindow(Board board)
    {
        game = new GameViewModel(board);
        boardSize = board.GetBoardSize();
        nextMoves = new List<Tuple<int, int, bool>>();
        isMoveStarted = false;
        gameEnded = false;
        InitializeComponent();
        SetGrid(PlayerBoard);
        SetGrid(ComputerBoard);
        DrawNotation(PlayerNotation);
        DrawNotation(ComputerNotation);
        SetShipsImages();
    }

    private void AddNotationTextBlock(int row, int column, string text, Canvas canvas, bool isLeft)
    {
        // Add one notation block
        var textBlock = new TextBlock();
        textBlock.Text = text;
        canvas.Children.Add(textBlock);
        if (isLeft)
        {
            Canvas.SetLeft(textBlock, 0);
            Canvas.SetTop(textBlock, row * tileSize + deltaCanvas);
        }
        else
        {
            Canvas.SetLeft(textBlock, column * tileSize + deltaCanvas);
            Canvas.SetTop(textBlock, boardSize * tileSize + deltaCanvas);
        }
        textBlock.FontWeight = FontWeights.Bold;
        textBlock.FontSize = 10;
    }
    public void DrawNotation(Canvas canvas)
    {
        // Draw all notation
        for (int i = 0; i < boardSize; ++i)
        {
            Char letter = Convert.ToChar('a' + i);
            AddNotationTextBlock(boardSize - 1, i, letter.ToString(), canvas, false);
            AddNotationTextBlock(i, 0, (boardSize - i).ToString(), canvas, true);
        }
    }

    public void SetGrid(Canvas canvas)
    {
        // Sets grid for particular canvas
        for (int i = 0; i < boardSize; ++i)
        {
            for (int j = 0; j < boardSize; ++j)
            {
                var border = new Border();
                border.BorderThickness = new Thickness(thickness);
                border.BorderBrush = new SolidColorBrush(Colors.Navy);
                border.Width = tileSize;
                border.Height = tileSize;
                canvas.Children.Add(border);
                Canvas.SetLeft(border, i * tileSize + thickness);
                Canvas.SetTop(border, j * tileSize + thickness);
            }
        }
    }

    public void SetShipsImages()
    {   // Set ship images on player's canvas
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
                im.Width = tileSize * ship.Item3 - 2 * thickness;
                im.Height = tileSize - 2 * thickness;
            }
            else
            {
                im.Width = tileSize - 2 * thickness;
                im.Height = tileSize * ship.Item3 - 2 * thickness;
            }
            PlayerBoard.Children.Add(im);
            Canvas.SetLeft(im, ship.Item2 * tileSize + 2 * thickness);
            Canvas.SetTop(im, ship.Item1 * tileSize + 2 * thickness);
        }
    }

    public void OppositeBoardClick(object sender, MouseButtonEventArgs e)
    {   // Event of click on computer's board
        if (isMoveStarted || gameEnded)
        {
            return;
        }

        isMoveStarted = true;
        var p = e.GetPosition(this);
        p = Game.TranslatePoint(p, ComputerBoard);
        int x = (int)p.X - thickness;
        int y = (int)p.Y - thickness;
        if (x < 0 || x >= boardSize * tileSize || y < 0 || y >= boardSize * tileSize)
        {
            return;
        }
        nextMoves = game.MakeMove(x, y);
        MakeNextMoves();
        isMoveStarted = false;
    }

    private async void MakeNextMoves()
    {   // Draw changed tiles
        bool isFirst = true;
        for (int i = 0; i < nextMoves.Count; ++i)
        {
            var move = nextMoves[i];
            if (!isFirst && move.Item3)
            {
                await Task.Delay(500);
            }
            var condition = game.GetCondition(move.Item1, move.Item2, isFirst);
            DrawResult(move.Item1, move.Item2, isFirst, condition == TileCondition.Hit);
            if (condition == TileCondition.Missed && move.Item3)
            {
                isFirst = false;
                EndLabel.Text = "Wait...";
            }
        }

        EndLabel.Text = "Your turn";

        if (game.GetGameResult() != Result.NotEnd)
        {
            EndGame();
        }
    }

    private void EndGame()
    {   // Show end game object
        if (game.GetGameResult() == Result.ComputerWins)
        {
            EndLabel.Text = "Computer\nwins";
        }
        else
        {
            EndLabel.Text = "You\nwin!";
        }
        EndLabel.FontSize = 14;
        gameEnded = true;
        RestartButton.Visibility = Visibility.Visible;
        var im = new Image();
        string path =
            "C:\\Users\\mavor\\OneDrive\\Рабочий стол\\ВУЗ\\Основы программирования\\SeaBattle\\SeaBattle\\data\\";
        im.Source = new BitmapImage(new Uri(path + "restart_aq.png"));
        RestartButton.Content = im;
    }

    private void DrawResult(int x, int y, bool isFirstPlayer, bool isCross)
    {   // Draw changed tile
        var im = new Image();
        string path =
            "C:\\Users\\mavor\\OneDrive\\Рабочий стол\\ВУЗ\\Основы программирования\\SeaBattle\\SeaBattle\\data\\";
        string name = isCross ? "cross.png" : "miss.png";
        im.Source = new BitmapImage(new Uri(path + name));
        im.Width = tileSize - 2 * thickness;
        im.Height = tileSize - 2 * thickness;
        if (isFirstPlayer)
        {
            ComputerBoard.Children.Add(im);
        }
        else
        {
            PlayerBoard.Children.Add(im);
        }
        Canvas.SetLeft(im, tileSize * x + 2 * thickness);
        Canvas.SetTop(im, tileSize * y + 2 * thickness);
    }

    public void Restart(object sender, RoutedEventArgs e)
    {   // Open new window
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
}