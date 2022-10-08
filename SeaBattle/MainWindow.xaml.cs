using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameClasses;

namespace SeaBattle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoardViewModel board;
        private List<Border> currentShip;
        private Border?[,] ships;
        private Button currentButtonShip;
        private Stack<Button>[] hiddenButtons;
        private int selectedShips;
        private int boardSize;
        private int maxShipLength = 4;
        private int tileSize = 30;
        private TextBlock[] horizontalTextBlocks;
        private TextBlock[] verticalTextBlocks;
        public MainWindow()
        {
            board = new BoardViewModel(new Board());
            boardSize = board.GetBoard().GetBoardSize();
            currentShip = new List<Border>();
            ships = new Border[boardSize, boardSize];
            currentButtonShip = new Button();
            hiddenButtons = new Stack<Button>[maxShipLength];
            for (int i = 0; i < maxShipLength; ++i)
            {
                hiddenButtons[i] = new Stack<Button>();
            }
            selectedShips = 0;
            horizontalTextBlocks = new TextBlock[boardSize];
            verticalTextBlocks = new TextBlock[boardSize];
            InitializeComponent();
            SetBoardGrid();
            SetButtons();
            DrawNotation();
        }
        
        private void AddNotationTextBlock(int row, int column, string text, bool isLeft)
        {
            // Add one notation block
            var textBlock = new TextBlock();
            textBlock.Text = text;
            Board.Children.Add(textBlock);
            Grid.SetColumn(textBlock, column);
            Grid.SetRow(textBlock, row);
            if (isLeft)
            {
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                verticalTextBlocks[row] = textBlock;
            }
            else
            {
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                horizontalTextBlocks[column] = textBlock;
            }
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.FontSize = 10;
        }

        private void UpdateNotation(int row, int column)
        {
            // Update notation block after change of border
            if (row == boardSize - 1)
            {
                Board.Children.Remove(horizontalTextBlocks[column]);
                Char letter = Convert.ToChar('a' + column);
                AddNotationTextBlock(row, column, letter.ToString(), false);
            }
            if (column == 0)
            {
                Board.Children.Remove(verticalTextBlocks[row]);
                AddNotationTextBlock(row, column, (boardSize - row).ToString(), true);
            }
        }

        public void DrawNotation()
        {
            // Draw all notation
            for (int i = 0; i < boardSize; ++i)
            {
                Char letter = Convert.ToChar('a' + i);
                AddNotationTextBlock(boardSize - 1, i, letter.ToString(), false);
                AddNotationTextBlock(i, 0, (boardSize - i).ToString(), true);
            }
        }

        public void SetButtons()
        {
            // Set buttons of ships
            var names = new List<string>();
            var sizes = new List<int>();
            for (int i = 1; i <= maxShipLength; ++i)
            {
                for (int j = 0; j < maxShipLength + 1 - i; ++j)
                {
                    names.Add("ship" + i.ToString() + "_aq_s.png");
                    sizes.Add(i);
                }
            }

            for (int i = 0; i < maxShipLength * (maxShipLength + 1) / 2; ++i)
            {
                var button = new Button();
                button.Height = sizes[i] * tileSize - 4 * (sizes[i] - 1);
                button.Width = tileSize;
                button.Click += new RoutedEventHandler(ShipClick);
                button.Background = new SolidColorBrush(Colors.Aquamarine);
                var img = new Image();
                img.Source = new BitmapImage(new Uri(@"C:\\Users\mavor\\OneDrive\\Рабочий стол\\ВУЗ\\Основы программирования\\SeaBattle\\SeaBattle\\data\\" + names[i]));
                button.Content = img;
                Buttons.Children.Add(button);
                Grid.SetColumn(button, i % 5);
                Grid.SetRow(button, i / 5 + 1);
            }
        }

        public void ShipClick(object sender, RoutedEventArgs e)
        {
            // Event of click on the ship-button
            var button = (Button)e.Source;
            int length = ((int)button.Height - 1) / tileSize + 1;
            board.SetShip(length);
            currentButtonShip = button;
        }

        public void SetBoardGrid()
        {
            // Set row- and column-definitions on the board, draw tiles
            for (int i = 0; i < boardSize; ++i)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(tileSize);
                Board.RowDefinitions.Add(rowDefinition);
                var columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(tileSize);
                Board.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < boardSize; ++i)
            {
                for (int j = 0; j < boardSize; ++j)
                {
                    var borderTile = new Border();
                    borderTile.BorderBrush = new SolidColorBrush(Colors.Navy);
                    borderTile.BorderThickness = new Thickness(1);
                    Board.Children.Add(borderTile);
                    Grid.SetColumn(borderTile, i);
                    Grid.SetRow(borderTile, j);
                }
            }
        }

        public void RotateCurrentShip(object sender, RoutedEventArgs e)
        {
            // Event of change rotation of the next ships
            board.ChangeRotation();
        }
        
        public void DeleteCurrentShip(object sender, RoutedEventArgs e)
        {
            // Event, that prepares to delete next ship
            board.PrepareToDelete();
        }
        
        public void StartGame(object sender, RoutedEventArgs e)
        {
            // Open new window if all ships are set
            if (selectedShips == (maxShipLength + 1) *maxShipLength / 2)
            {
                var gameWindow = new GameWindow(board.GetBoard());
                gameWindow.Show();
                this.Close();
            }
        }

        public void UpdateBoard()
        {
            // Set borders of added ships
            for (int i = 0; i < boardSize; ++i)
            {
                for (int j = 0; j < boardSize; ++j)
                {
                    if (board.IsShip(i, j) && ships[i, j] == null)
                    {
                        ships[i, j] = new Border();
                        ships[i, j].Background = new SolidColorBrush(Colors.Teal);
                        ships[i, j].BorderBrush = new SolidColorBrush(Colors.Navy);
                        ships[i, j].BorderThickness = new Thickness(1);
                        Board.Children.Add(ships[i, j]);
                        Grid.SetColumn(ships[i, j], j);
                        Grid.SetRow(ships[i, j], i);
                        UpdateNotation(i, j);
                    }
                    else if (!board.IsShip(i, j) && ships[i, j] != null)
                    {
                        Board.Children.Remove(ships[i, j]);
                        ships[i, j] = null;
                    }
                }
            }
        }

        private void SetShip(object sender, MouseButtonEventArgs e)
        {
            // Put ship on the board
            var p = e.GetPosition(this);
            p = PrepareWindow.TranslatePoint(p, Board);
            var x = Convert.ToInt32(p.X - 1);
            var y = Convert.ToInt32(p.Y - 1);
            if (x < 0 || y < 0 || x >= boardSize * tileSize || y >= boardSize * tileSize)
                return;
            ClickResult moveResult = board.Click(x, y);
            if (moveResult == ClickResult.Sleep)
            {
                return;
            }
            UpdateBoard();
            if (moveResult == ClickResult.Put)
            {
                currentButtonShip.Visibility = Visibility.Hidden;
                hiddenButtons[board.GetLastLength() - 1].Push(currentButtonShip);
                currentButtonShip = new Button();
                selectedShips++;
                return;
            }

            int shipLength = board.GetLastDeleteLength();
            var button = hiddenButtons[shipLength - 1].Pop();
            button.Visibility = Visibility.Visible;
            selectedShips--;
        }

        public void SetLighting(object sender, MouseEventArgs e)
        {
            DeleteBordersLighting();
            var p = e.GetPosition(this);
            p = PrepareWindow.TranslatePoint(p, Board);
            var x = Convert.ToInt32(p.X - 1);
            var y = Convert.ToInt32(p.Y - 1);
            if (x < 0 || y < 0 || x >= boardSize * tileSize || y >= boardSize * tileSize)
                return;
            var result = board.CanPutShip(y / tileSize, x / tileSize);
            SolidColorBrush color;
            if (result.Item1)
            {
                color = new SolidColorBrush(Colors.GreenYellow);
            }
            else
            {
                color = new SolidColorBrush(Colors.Red);
            }

            foreach (var tile in result.Item2)
            {
                var border = new Border();
                border.Background = color;
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = new SolidColorBrush(Colors.Navy);
                border.MouseDown += new MouseButtonEventHandler(SetShip);
                Board.Children.Add(border);
                Grid.SetColumn(border, tile.Item2);
                Grid.SetRow(border, tile.Item1);
                currentShip.Add(border);
                UpdateNotation(tile.Item1, tile.Item2);
            }
        }

        public void DeleteLighting(object sender, MouseEventArgs e)
        {
            // Event of delete previous lighting
            DeleteBordersLighting();
        }

        public void DeleteBordersLighting()
        {
            // Delete previous lighting
            foreach (var border in currentShip)
            {
                Board.Children.Remove(border);
            }
            currentShip.Clear();
        }
    }
}