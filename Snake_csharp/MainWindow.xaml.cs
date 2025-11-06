using System.Net.Sockets;
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
using Snake_csharp;

namespace Snake_csharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.SecondSnake, Images.SecondBody },
            { GridValue.Food, Images.Food },
            { GridValue.Portal, Images.Portal },
            { GridValue.Poison, Images.Poison },
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Down, 180 },
            { Direction.Left, 270 },
            { Direction.Right, 90 },
        };
        private readonly int rows = 15, cols = 16;
        private Dictionary<GameModes, int> highScores;
        private readonly Image[,] gridImages;
        private GameStateClassic gameState;
        private bool gameRunning = false;
        private GameModes currentMode = GameModes.Classic;
        public MainWindow()
        {
            InitializeComponent();
            InitHighScores();
            gridImages = SetupGrid();
        }

        private async Task Rungame()
        {
            gameState = ModeHandler(currentMode);
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            
        }

        private GameStateClassic ModeHandler(GameModes mode)
        {
            return mode switch
            {
                GameModes.Classic => new GameStateClassic(rows, cols),
                GameModes.Poison => new GameStatePoison(rows, cols),
                GameModes.TwoSnakes => new GameStateTwoSnakes(rows, cols),
                GameModes.Immortal => new GameStateImmortal(rows, cols),
                GameModes.ThroughWalls => new GameStateThroughWalls(rows, cols),
                GameModes.HeadIsTail => new GameStateHeadIsTail(rows, cols),
                GameModes.Portal => new GameStatePortal(rows, cols),
                _ => throw new Exception("Invalid Game Mode"),
            };
        }

        private void ModeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModeList.SelectedItem is ListBoxItem item)
            {
                int mode = int.Parse(item.Tag.ToString());
                currentMode = (GameModes)mode;
                ShowPressAnyKeyOverlay();
            }
        }

        private void ModeList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ModeList.SelectedItem is ListBoxItem item)
            {
                int mode = int.Parse(item.Tag.ToString());
                currentMode = (GameModes)mode;
                ShowPressAnyKeyOverlay();
            }
        }

        private void ShowPressAnyKeyOverlay()
        {
            MenuPanel.Visibility = Visibility.Collapsed;
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press Any Key to Start";
            OverlayText.Visibility = Visibility.Visible;
        }

        private void ShowGameOverScreen()
        {
            Overlay.Visibility = Visibility.Visible;

            MenuPanel.Visibility = Visibility.Collapsed;
            OverlayText.Visibility = Visibility.Collapsed;

            GameOverPanel.Visibility = Visibility.Visible;
        }

        private void RestartSameMode(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
            GameOverPanel.Visibility = Visibility.Collapsed;
            ShowPressAnyKeyOverlay();
        }

        private void ReturnToMenu(object sender, RoutedEventArgs e)
        {
            GameOverPanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;

        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if(!gameRunning && MenuPanel.Visibility != Visibility.Visible)
            {
                gameRunning = true;
                await Rungame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
                return;
            switch (e.Key)
            {
                case Key.Left:
                case Key.A:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                case Key.D:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                case Key.W:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                case Key.S:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols/ (double)rows);
            for (int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    images[r,c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            DrawSecondSnakeHead();
            ScoreText.Text = $"Score: {gameState.Score}|High Score: {highScores[currentMode]}";
        }
        private void DrawGrid()
        {
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private void DrawSecondSnakeHead()
        {
            Position secondHeadPos = gameState.SecondHeadPosition();
            if (secondHeadPos != null)
            {
                Image secondImage = gridImages[secondHeadPos.Row, secondHeadPos.Col];
                secondImage.Source = Images.SecondHead;
                int secondRotation = dirToRotation[gameState.Dir.Opposite()];
                secondImage.RenderTransform = new RotateTransform(secondRotation);
            }
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            List<Position> secondPositions = gameState.SecondSnakePositions() != null ?
                new List<Position>(gameState.SecondSnakePositions()) : null;

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                if (secondPositions != null)
                {
                    pos = secondPositions[i];
                    source = (i == 0) ? Images.SecondDeadHead : Images.SecondDeadBody;
                    gridImages[pos.Row, pos.Col].Source = source;
                }
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for(int i=3;i>0;i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            SetHighScore();
            ShowGameOverScreen();
        }
        private void SetHighScore()
        {
            if (gameState.Score > highScores[currentMode])
            {
                highScores[currentMode] = gameState.Score;
                GameOverText.Text = "New High Score!";
            }
        }
        private void InitHighScores()
        {
            highScores = new Dictionary<GameModes, int>();
            foreach (GameModes mode in Enum.GetValues(typeof(GameModes)))
            {
                highScores[mode] = 0;
            }
        }       
    }
}