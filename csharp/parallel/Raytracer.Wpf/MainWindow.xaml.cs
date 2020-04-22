//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Raytracer.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double CYLINDER_HEIGHT = 3.0;
        private const double CYLINDER_WIDTH = 5.0;
        private readonly Color _darkColor = Color.FromRgb(30, 30, 30);
        private readonly Color _lightColor = Colors.White;
        private readonly ReversiGame _gameEngine;
        private readonly MinimaxSpot[,] _guiBoard;
        private Task _aiUiTask;
        private CancellationTokenSource _aiUiCts;
        private readonly TaskScheduler _uiScheduler;
        private bool _isGameOver, _isAiMoving, _isAiParallel, _isAuto;
        private Duration _progressBarDuration;
        private DoubleAnimation _progressBarAnimation;
        private bool _useAnimation;

        public MainWindow()
        {
            InitializeComponent();

            _gamePieces = new Dictionary<Point, Cylinder>();
            _ghostPieces = new Dictionary<Cylinder, Point>();

            NameScope.SetNameScope(mainViewport, new NameScope());

            ui_dopSlider.Minimum = 1.0;
            ui_dopSlider.Maximum = Environment.ProcessorCount * 2;
            ui_dopSlider.Value = Environment.ProcessorCount;
            ui_depthSlider.Minimum = 8.0;
            ui_depthSlider.Maximum = 24.0;
            ui_depthSlider.Value = 18.0;
            ui_timeoutSlider.Minimum = 1.0;
            ui_timeoutSlider.Maximum = 60.0;
            ui_timeoutSlider.Value = 8.0;
            _useAnimation = true;

            // Set the UI scheduler
            _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // Initialize the game
            _gameEngine = new ReversiGame(8, 8);
            _isGameOver = false;
            _isAiMoving = false;
            _isAiParallel = true;
            _isAuto = false;

            // Initialize the GUI's board
            _guiBoard = new MinimaxSpot[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _guiBoard[i, j] = MinimaxSpot.Empty;
                }
            }

            UpdateBoard();
        }

        private void GoAI()
        {
            if (_isGameOver)
            {
                return;
            }

            _isAiMoving = true;

            _progressBarAnimation = new DoubleAnimation(0.0, 100.0, _progressBarDuration);
            if (_isAiParallel)
            {
                ui_parProgBar.Visibility = Visibility.Visible;
                ui_parProgBar.BeginAnimation(ProgressBar.ValueProperty, _progressBarAnimation, HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                ui_seqProgBar.Visibility = Visibility.Visible;
                ui_seqProgBar.BeginAnimation(ProgressBar.ValueProperty, _progressBarAnimation, HandoffBehavior.SnapshotAndReplace);
            }

            _aiUiCts = new CancellationTokenSource();
            _aiUiTask = Task.Factory.StartNew(() =>
            {
                return _gameEngine.GetAIMove(_isAiParallel);

            }, _aiUiCts.Token, TaskCreationOptions.None, TaskScheduler.Default)
            .ContinueWith(completedTask =>
            {
                var aiMove = completedTask.Result;
                if (aiMove.Row != -1)
                {
                    _gameEngine.MakeMove(aiMove.Row, aiMove.Col);
                }
                else
                {
                    _gameEngine.PassMove();
                }

                string s;
                if (_isAiParallel)
                {
                    s = $"{_gameEngine.MovesConsidered:N}";
                    s = s.Substring(0, s.Length - 3);
                    ui_parLabel.Content = s;
                    ui_parProgBar.Visibility = Visibility.Hidden;
                }
                else
                {
                    s = $"{_gameEngine.MovesConsidered:N}";
                    s = s.Substring(0, s.Length - 3);
                    ui_seqLabel.Content = s;
                    ui_seqProgBar.Visibility = Visibility.Hidden;
                }

                UpdateBoard();
                _isAiMoving = false;

                if (_isAuto)
                {
                    _isAiParallel = !_isAiParallel;
                    GoAI();
                }
            }, _aiUiCts.Token, TaskContinuationOptions.None, _uiScheduler);
        }

        private bool UpdateBoard()
        {
            if (_isGameOver)
            {
                return false;
            }

            MinimaxSpot[,] game = _gameEngine.Board;
            MinimaxSpot[,] gui = _guiBoard;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (gui[i, j] == MinimaxSpot.Empty && game[i, j] != MinimaxSpot.Empty)
                    {
                        AddPiece(i, j, game[i, j] == MinimaxSpot.Light ? true : false);
                    }
                    else if (gui[i, j] == MinimaxSpot.Light && game[i, j] == MinimaxSpot.Dark ||
                        gui[i, j] == MinimaxSpot.Dark && game[i, j] == MinimaxSpot.Light)
                    {
                        FlipPiece(i, j);
                    }
                }
            }

            // Remove old ghost pieces.
            foreach (var c in _ghostPieces)
            {
                mainViewport.Children.Remove(c.Key);
            }
            _ghostPieces.Clear();

            // Generate new ghost pieces.
            var moves = _gameEngine.GetValidMoves();
            foreach (var m in moves)
            {
                ShowGhost(m.Row, m.Col, _gameEngine.IsLightMove);
            }

            var gameResult = _gameEngine.GetGameResult();
            var (isGameOver, message, result) = HandleGameState(gameResult);
            if (!result)
            {
                _isGameOver = isGameOver;
                MessageBox.Show(message, "GAME OVER");
            }

            return result;

            static (bool isGameOver, string message, bool result) HandleGameState(
                ReversiGameResult result) =>
            result.GameState switch
            {
                ReversiGameState.LightWon => (true, $"Light Won! {result.NumLightPieces}-{result.NumDarkPieces}", false),
                ReversiGameState.DarkWon => (true, $"Dark Won! {result.NumLightPieces}-{result.NumDarkPieces}", false),
                ReversiGameState.Draw => (true, $"Draw! {result.NumLightPieces}-{result.NumDarkPieces}", false),
                _ => (false, null, true)
            };
        }

        private void OnMainWindowKeyDown(object sender, KeyEventArgs e)
        {
            var cameraPos = mainCamera.Position;
            if (e.Key == Key.Up)
            {
                cameraPos.Z += 1;
            }
            if (e.Key == Key.Down)
            {
                cameraPos.Z -= 1;
            }
            if (e.Key == Key.T)
            {
                mainCamera.FieldOfView += 10;
            }
            if (e.Key == Key.G)
            {
                mainCamera.FieldOfView -= 10;
            }
            if (e.Key == Key.D)
            {
                _gameEngine.Dump("");
                MessageBox.Show("Dumped.");
            }

            mainCamera.Position = cameraPos;
        }

        /// <summary>
        /// Collection of pieces on the board.
        /// </summary>
        private readonly Dictionary<Point, Cylinder> _gamePieces;
        private readonly Dictionary<Cylinder, Point> _ghostPieces;

        public void AddPiece(int row, int col, bool isPlayerLight)
        {
            if (_useAnimation)
            {
                AddPieceAnimated(row, col, isPlayerLight);
            }
            else
            {
                AddPieceUnanimated(row, col, isPlayerLight);
            }
        }

        const int CYLINDER_RESOLUTION = 50;

        /// <summary>
        /// Places a piece on the gameboard by visually "dropping the piece."
        /// </summary>
        /// <param name="row">The row of the piece to place.</param>
        /// <param name="col">The column of the piece to place.</param>
        /// <param name="isBlack">If true, the piece color is black; otherwise, it's white.</param>
        public void AddPieceAnimated(int row, int col, bool isPlayerLight)
        {
            // Update the GUI board
            _guiBoard[row, col] = isPlayerLight ? MinimaxSpot.Light : MinimaxSpot.Dark;

            // Hack to match danny's orientation
            row = 7 - row;

            // create the piece and place it above the destination square
            var newPiece = new Cylinder(CYLINDER_WIDTH, CYLINDER_HEIGHT, CYLINDER_RESOLUTION, isPlayerLight
                ? new DiffuseMaterial(new SolidColorBrush(_lightColor))
                : new DiffuseMaterial(new SolidColorBrush(_darkColor)));

            GetViewCoordinates(col, row, out double centerX, out double centerY);
            newPiece.MoveTo(new Point3D(centerX, centerY, 20.0));
            mainViewport.Children.Add(newPiece);

            // animate it's descent
            var animationStoryboard = new Storyboard();

            var translateTransform = new TranslateTransform3D(
                centerX, centerY, 20.0);
            var moveDownAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(500)));
            mainViewport.RegisterName("GamePieceDrop", translateTransform);
            Storyboard.SetTargetName(moveDownAnimation, "GamePieceDrop");
            Storyboard.SetTargetProperty(moveDownAnimation, new PropertyPath(TranslateTransform3D.OffsetZProperty));
            animationStoryboard.Children.Add(moveDownAnimation);
            newPiece.Transform = translateTransform;

            animationStoryboard.Begin(mainViewport);

            mainViewport.UnregisterName("GamePieceDrop");

            _gamePieces[new Point(row, col)] = newPiece;
        }

        /// <summary>
        /// Adds a piece to the game board without animation.  This method is called when the board is
        /// initialized.
        /// </summary>
        /// <param name="col">The column of the piece to add.</param>
        /// <param name="row">The row of the piece to add.</param>
        /// <param name="isBlack">If true, the piece color is black; otherwise, it's white.</param>
        public void AddPieceUnanimated(int row, int col, bool isPlayerLight)
        {
            // Update the GUI board
            _guiBoard[row, col] = isPlayerLight ? MinimaxSpot.Light : MinimaxSpot.Dark;

            // Hack to match danny's orientation
            row = 7 - row;

            // create the piece and place it above the destination square
            var newPiece = new Cylinder(CYLINDER_WIDTH, CYLINDER_HEIGHT, CYLINDER_RESOLUTION, isPlayerLight
                ? new DiffuseMaterial(new SolidColorBrush(_lightColor))
                : new DiffuseMaterial(new SolidColorBrush(_darkColor)));
            MovePiece(newPiece, col, row);
            _gamePieces[new Point(row, col)] = newPiece;
            mainViewport.Children.Add(newPiece);
        }

        public void FlipPiece(int row, int col)
        {
            if (_useAnimation)
            {
                FlipPieceAnimated(row, col);
            }
            else
            {
                FlipPieceUnanimated(row, col);
            }
        }

        public void FlipPieceUnanimated(int row, int col)
        {
            // Update the GUI board
            _guiBoard[row, col] = _guiBoard[row, col] == MinimaxSpot.Light ? MinimaxSpot.Dark : MinimaxSpot.Light;

            // Hack to match danny's orientation
            row = 7 - row;

            // get the piece's world coordinates
            GetViewCoordinates(col, row, out double centerX, out double centerY);
            var gamePiece = _gamePieces[new Point(row, col)];

            // get the diffuse material
            var diffMaterialBrush = (gamePiece.Material as DiffuseMaterial).Brush as SolidColorBrush;
            diffMaterialBrush.Color = diffMaterialBrush.Color.Equals(_lightColor) ? _darkColor : _lightColor;
        }

        /// <summary>
        /// "Flips" the game piece by lifting it, rotating it, and fading it's color from black to white, or vice versa.
        /// </summary>
        /// <param name="col">The column of the piece to flip.</param>
        /// <param name="row">The row of the piece to flip.</param>
        public void FlipPieceAnimated(int row, int col)
        {
            // Update the GUI board
            _guiBoard[row, col] = _guiBoard[row, col] == MinimaxSpot.Light ? MinimaxSpot.Dark : MinimaxSpot.Light;

            // Hack to match danny's orientation
            row = 7 - row;

            // get the piece's world coordinates
            GetViewCoordinates(col, row, out double centerX, out double centerY);
            var gamePiece = _gamePieces[new Point(row, col)];

            // define an animation storyboard and a transform group
            var animationsStoryboard = new Storyboard();
            var transformGroup = new Transform3DGroup();

            // setup the lift animation
            var translateTransform = new TranslateTransform3D(
                centerX, centerY, CYLINDER_HEIGHT / 2.0);
            var moveUpAnimation = new DoubleAnimation(10, new Duration(TimeSpan.FromMilliseconds(500)))
            {
                AutoReverse = true
            };
            mainViewport.RegisterName("GamePieceMoveUp", translateTransform);
            Storyboard.SetTargetName(moveUpAnimation, "GamePieceMoveUp");
            Storyboard.SetTargetProperty(moveUpAnimation, new PropertyPath(TranslateTransform3D.OffsetZProperty));
            animationsStoryboard.Children.Add(moveUpAnimation);


            // setup the rotate animation
            var axisAngleRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            var rotateTransform = new RotateTransform3D(axisAngleRotation, new Point3D(-0, 0, CYLINDER_HEIGHT / 2.0))
            {
                Rotation = axisAngleRotation
            };
            var flipOverAnimation = new DoubleAnimation(0, 180, new Duration(TimeSpan.FromMilliseconds(1000)));
            mainViewport.RegisterName("GamePieceFlipOver", axisAngleRotation);
            Storyboard.SetTargetName(flipOverAnimation, "GamePieceFlipOver");
            Storyboard.SetTargetProperty(flipOverAnimation, new PropertyPath(AxisAngleRotation3D.AngleProperty));
            animationsStoryboard.Children.Add(flipOverAnimation);

            // setup the recolor animation
            // get the diffuse material
            var diffMaterialBrush = (gamePiece.Material as DiffuseMaterial).Brush as SolidColorBrush;
            var changeColorAnimation = new ColorAnimation
            {
                To = diffMaterialBrush.Color.Equals(_lightColor) ? _darkColor : _lightColor,
                Duration = TimeSpan.FromMilliseconds(1000)
            };
            mainViewport.RegisterName("GamePieceChangeColor", diffMaterialBrush);
            Storyboard.SetTargetName(changeColorAnimation, "GamePieceChangeColor");
            Storyboard.SetTargetProperty(changeColorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));
            animationsStoryboard.Children.Add(changeColorAnimation);

            // start the animation
            // note the transforms are applied in this specific order so that the piece ends up in the right location
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(translateTransform);
            gamePiece.Transform = transformGroup;
            animationsStoryboard.Begin(mainViewport);

            // unregister all animations so they can be performed again
            mainViewport.UnregisterName("GamePieceMoveUp");
            mainViewport.UnregisterName("GamePieceFlipOver");
            mainViewport.UnregisterName("GamePieceChangeColor");
        }

        public void ShowGhost(int row, int col, bool isPlayerLight)
        {
            // Hack to match danny's orientation
            row = 7 - row;

            // create the piece and place it above the destination square
            var newPiece = new Cylinder(CYLINDER_WIDTH, CYLINDER_HEIGHT, CYLINDER_RESOLUTION, isPlayerLight
                ? new DiffuseMaterial(new SolidColorBrush(_lightColor))
                : new DiffuseMaterial(new SolidColorBrush(_darkColor)));
            GetViewCoordinates(col, row, out double centerX, out double centerY);
            // make the piece transparent
            var pieceColor = (newPiece.Material as DiffuseMaterial).Color;
            pieceColor.A = 70;
            (newPiece.Material as DiffuseMaterial).Color = pieceColor;
            newPiece.MoveTo(new Point3D(centerX, centerY, 0.0));
            mainViewport.Children.Add(newPiece);

            _ghostPieces.Add(newPiece, new Point(col, row));
        }

        // moves a cylinder piece to a place on the gameboard
        private void MovePiece(Cylinder piece, int col, int row)
        {
            GetViewCoordinates(col, row, out double centerX, out double centerY);
            piece.MoveTo(new Point3D(centerX, centerY, CYLINDER_HEIGHT / 2.0));
        }

        private static void GetViewCoordinates(int col, int row, out double centerX, out double centerY)
        {
            // width of cell * number of row/col - center point of first col/row
            centerY = -(row * 10.0 - 35.0);
            centerX = col * 10.0 - 35.0;
        }

        private void OnMainViewPortMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isGameOver)
            {
                MessageBox.Show("Game is over.", "GAME OVER");
                return;
            }

            if (_isAiMoving)
            {
                MessageBox.Show("AI is moving.  Wait for your opponent.", "INVALID");
                return;
            }

            var res = GetGhostPiecePosition(e.GetPosition(mainViewport), out Point ghostPos);
            if (res)
            {
                _gameEngine.SetMinimaxKnobs((int)ui_depthSlider.Value, TimeSpan.FromSeconds((int)ui_timeoutSlider.Value), (int)ui_dopSlider.Value);
                _progressBarDuration = new Duration(_gameEngine.TimeLimit);
                int row = 7 - (int)ghostPos.Y;
                int col = (int)ghostPos.X;
                if (_gameEngine.MakeMove(row, col))
                {
                    if (UpdateBoard())
                    {
                        GoAI();
                    }
                }
                else
                {
                    MessageBox.Show("Can't move there. Shouldn't be seeing this message.", "ERROR");
                }
            }
            else
            {
                MessageBox.Show("You chose to pass.", "USER MOVE");
                _gameEngine.PassMove();
                if (UpdateBoard())
                {
                    GoAI();
                }
            }
        }

        private bool GetGhostPiecePosition(Point mousePosition, out Point ghostPosition)
        {
            ghostPosition = new Point();
            var hitTestResult = VisualTreeHelper.HitTest(mainViewport, mousePosition);
            if (hitTestResult.VisualHit is Cylinder)
            {
                var cylinder = hitTestResult.VisualHit as Cylinder;
                if (!_ghostPieces.ContainsKey(cylinder))
                {
                    return false;
                }

                ghostPosition = _ghostPieces[cylinder];
                return true;
            }
            else
            {

                return false;
            }
        }


        private void OnStartStopButtonClick(object sender, RoutedEventArgs e)
        {
            if (!_isAuto)
            {
                ui_dopSlider.IsEnabled = false;
                ui_depthSlider.IsEnabled = false;
                ui_timeoutSlider.IsEnabled = false;

                ui_startStopButton.Content = "Stop Sequential vs. Parallel";

                _gameEngine.SetMinimaxKnobs((int)ui_depthSlider.Value, TimeSpan.FromSeconds((int)ui_timeoutSlider.Value), (int)ui_dopSlider.Value);
                _progressBarDuration = new Duration(_gameEngine.TimeLimit);
                _isAuto = true;
                _isAiParallel = false;

                ui_seqPlayerLabel.Content = "Sequential Player";
                GoAI();
            }
            else
            {
                _gameEngine.Cancel();
                _aiUiCts.Cancel();
                ui_seqProgBar.Visibility = Visibility.Hidden;
                ui_parProgBar.Visibility = Visibility.Hidden;

                ui_dopSlider.IsEnabled = true;
                ui_depthSlider.IsEnabled = true;
                ui_timeoutSlider.IsEnabled = true;

                ui_startStopButton.Content = "Start Sequential vs. Parallel";

                _isAuto = false;
                _isAiMoving = false;
                _isAiParallel = true;

                ui_seqPlayerLabel.Content = "You";
            }
            ui_settings.Focus(); // stop the button from blinking due to it having focus
        }

        private void OnAnimationToggler(object sender, RoutedEventArgs e) => _useAnimation = !_useAnimation;

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            // add piece icons to the player badges
            seqPlayerViewport.Children.Add(new Cylinder(CYLINDER_WIDTH, CYLINDER_HEIGHT, CYLINDER_RESOLUTION, new DiffuseMaterial(new SolidColorBrush(_darkColor))));
            parPlayerViewport.Children.Add(new Cylinder(CYLINDER_WIDTH, CYLINDER_HEIGHT, CYLINDER_RESOLUTION, new DiffuseMaterial(new SolidColorBrush(_lightColor))));
        }
    }
}
