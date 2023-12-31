﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
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
using System.Xml.Schema;
using System.Windows.Threading;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
            {
                { GridValue.Empty, Images.Empty }, {GridValue.Snake, Images.CBody }, { GridValue.Food, Images.Food }
            };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 }, { Direction.Right, 90 }, { Direction.Down, 180 }, { Direction.Left, 270 }
        };
        private readonly int rows = 20;
        private readonly int cols = 20;
        private int HighscoreValue = 0;
        private int Total = 0;
        private int Green = 2;
        private int Pink = 0;
        private int Blue = 0;
        private int Yellow = 0;
        private int PinkPrice = 50;
        private int BluePrice = 75;
        private int YellowPrice = 100;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private int boostSpeed = 0;
        public MainWindow()
        {
            InitializeComponent();
            LoadValues();
            ScoreText.Text = $"Highscore: {HighscoreValue}";
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100 - boostSpeed);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Shop1.Visibility = Visibility.Hidden;
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(.5, .5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }
        private async Task RunGame()
        {
            Draw();
            Overlay.Visibility = Visibility.Hidden;
            gameRunning = true;
            Audio.Game.Play();
            Audio.GameOver.Stop();
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.A: gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.D: gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.W: gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.S: gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.Left: gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right: gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up: gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down: gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.Space: boostSpeed = (boostSpeed == 0) ? 50 : 0;
                    break;
                case Key.Escape: System.Windows.Application.Current.Shutdown();
                    break;
            }

        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"{gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }
        private async Task ShowGameOver()
        {
            gameRunning = false;
            await DrawDeadSnake();
            Audio.Game.Stop();
            Audio.GameOver.Play();
            Values();
            ScoreText.Text = $"Highscore: {HighscoreValue}";
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Visibility = Visibility.Visible;
            ShopText.Visibility = Visibility.Visible;
            Shop1.Visibility = Visibility.Hidden;
            OverlayText.Text = "Press Any Key To Start";
            ScoreText.Text = $"Highscore: {HighscoreValue}";
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(0);
            }
        }
        public void Values()
        {
            if (gameState.Score > HighscoreValue)
            {
                HighscoreValue = gameState.Score;
            }
                Total = Total + gameState.Score;
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\SnakeValues.txt");
                sw.WriteLine($"{HighscoreValue},{Total},{Green},{Pink},{Blue},{Yellow}");
                sw.Close();
            
        }
        public void LoadValues()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\SnakeValues.txt"))
            {
                StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\SnakeValues.txt");
               
                string TotalValue = sr.ReadLine();
                HighscoreValue = int.Parse(TotalValue.Split(',')[0]);
                Total = int.Parse(TotalValue.Split(',')[1]);
                Green = int.Parse(TotalValue.Split(',')[2]);
                Pink = int.Parse(TotalValue.Split(',')[3]);
                Blue = int.Parse(TotalValue.Split(',')[4]);
                Yellow = int.Parse(TotalValue.Split(',')[5]);
                sr.Close();
            }
        }

        private void ShopText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OverlayText.Visibility = Visibility.Hidden;
            ShopText.Visibility = Visibility.Hidden;
            Shop1.Visibility = Visibility.Visible;
            EquipPinkSnake.Visibility = Visibility.Hidden;
            EquipBlueSnake.Visibility = Visibility.Hidden;
            EquipYellowSnake.Visibility = Visibility.Hidden;
            if (Pink == 1 || Pink == 2)
            {
                PinkSnakePrice.Visibility = Visibility.Hidden;
                PinkSnake.Visibility = Visibility.Hidden;
                EquipPinkSnake.Visibility = Visibility.Visible;
            }
            if (Blue == 1 || Blue == 2)
            {
                BlueSnakePrice.Visibility = Visibility.Hidden;
                BlueSnake.Visibility = Visibility.Hidden;
                EquipBlueSnake.Visibility = Visibility.Visible;
            }
            if (Yellow == 1 || Yellow == 2)
            {
                YellowSnakePrice.Visibility = Visibility.Hidden;
                YellowSnake.Visibility = Visibility.Hidden;
                EquipYellowSnake.Visibility = Visibility.Visible;
            }
            Audio.GameOver.Stop();
            Audio.Shopkeep1.Play();
            ScoreText.Text = $"Fruit: {Total}";
        }

        private void PinkSnake_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Total >= PinkPrice && Pink == 0)
            {
                Pink = 1;
                Total = Total - PinkPrice;
                ScoreText.Text = $"Fruit: {Total}";
                Values();
                PinkSnakePrice.Visibility = Visibility.Hidden;
                PinkSnake.Visibility = Visibility.Hidden;
                EquipPinkSnake.Visibility = Visibility.Visible;
            }
        }

        private void BlueSnake_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Total >= BluePrice && Blue == 0)
            {
                Blue = 1;
                Total = Total - BluePrice;
                ScoreText.Text = $"Fruit: {Total}";
                Values();
                BlueSnakePrice.Visibility = Visibility.Hidden;
                BlueSnake.Visibility = Visibility.Hidden;
                EquipBlueSnake.Visibility = Visibility.Visible;
            }
        }

        private void YellowSnake_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Total >= YellowPrice && Yellow == 0)
            {
                Yellow = 1;
                Total = Total - YellowPrice;
                ScoreText.Text = $"Fruit: {Total}";
                Values();
                YellowSnakePrice.Visibility = Visibility.Hidden;
                YellowSnake.Visibility = Visibility.Hidden;
                EquipYellowSnake.Visibility = Visibility.Visible;
            }
        }

        private void EquipPinkSnake_MouseDown(object sender, MouseButtonEventArgs e)
        {
                Blue = (Blue == 2) ? 1 : Blue;
                Yellow = (Yellow == 2) ? 1 : Yellow;
                Green = (Green == 2) ? 1 : Green;
                Pink = 2;
                EquipPinkSnake.Text = "Equipped";
                EquipBlueSnake.Text = "Blue Snake";
                EquipYellowSnake.Text = "Yellow Snake";
                Images.CBody = Images.PinkBody;


        }

        private void EquipBlueSnake_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Pink = (Pink == 2) ? 1 : Pink;
            Yellow = (Yellow == 2) ? 1 : Yellow;
            Green = (Green == 2) ? 1 : Green;
            Blue = 2;
            EquipBlueSnake.Text = "Equipped";
            EquipPinkSnake.Text = "Pink Snake";
            EquipYellowSnake.Text = "Yellow Snake";
            Images.CBody = Images.BlueBody;
        }

        private void EquipYellowSnake_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Blue = (Blue == 2) ? 1 : Blue;
            Pink = (Pink == 2) ? 1 : Pink;
            Green = (Green == 2) ? 1 : Green;
            Yellow = 2;
            EquipYellowSnake.Text = "Equipped";
            EquipPinkSnake.Text = "Pink Snake";
            EquipBlueSnake.Text = "Blue Snake";
            Images.CBody = Images.YellowBody;
        }
    }
}
