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
        public MainWindow()
        {
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
        }
        
        public void DeleteCurrentShip(object sender, RoutedEventArgs e)
        {
        }
        
    }
}