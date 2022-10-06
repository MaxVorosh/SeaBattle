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
        public MainWindow()
        {
            board = new BoardViewModel(new Board());
            currentShip = new List<Border>();
            ships = new Border[10, 10];
            currentButtonShip = new Button();
            hiddenButtons = new Stack<Button>[4];
            for (int i = 0; i < 4; ++i)
            {
                hiddenButtons[i] = new Stack<Button>();
            }
            selectedShips = 0;
            InitializeComponent();
            SetBoardGrid();
            SetButtons();
        }

        public void SetButtons()
        {
            var names = new List<string>();
            var sizes = new List<int>();
            for (int i = 1; i <= 4; ++i)
            {
                for (int j = 0; j < 5 - i; ++j)
                {
                    names.Add("ship" + i.ToString() + "_aq_s.png");
                    sizes.Add(i);
                }
            }

            for (int i = 0; i < 10; ++i)
            {
                var button = new Button();
                button.Height = sizes[i] * 30 - 4 * (sizes[i] - 1);
                button.Width = 30;
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
            var button = (Button)e.Source;
            int length = ((int)button.Height - 1) / 30 + 1;
            board.SetShip(length);
            currentButtonShip = button;
        }

        public void SetBoardGrid()
        {
            for (int i = 0; i < 10; ++i)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(30);
                Board.RowDefinitions.Add(rowDefinition);
                var columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(30);
                Board.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
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
            board.ChangeRotation();
        }
        
        public void DeleteCurrentShip(object sender, RoutedEventArgs e)
        {
            board.PrepareToDelete();
        }
        
        public void StartGame(object sender, RoutedEventArgs e)
        {
            if (selectedShips == 10)
            {
                this.Close();
            }
        }

        public void UpdateBoard()
        {
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
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
            var p = e.GetPosition(this);
            p = PrepareWindow.TranslatePoint(p, Board);
            var x = Convert.ToInt32(p.X - 1);
            var y = Convert.ToInt32(p.Y - 1);
            if (x < 0 || y < 0 || x >= 300 || y >= 300)
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
            if (x < 0 || y < 0 || x >= 300 || y >= 300)
                return;
            var result = board.CanPutShip(y / 30, x / 30);
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
            }
        }

        public void DeleteLighting(object sender, MouseEventArgs e)
        {
            DeleteBordersLighting();
        }

        public void DeleteBordersLighting()
        {
            foreach (var border in currentShip)
            {
                Board.Children.Remove(border);
            }
            currentShip.Clear();
        }
    }
}