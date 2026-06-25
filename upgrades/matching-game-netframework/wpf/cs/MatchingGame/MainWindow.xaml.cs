using MatchingGame.Logic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MatchingGame
{
    public partial class MainWindow : Window
    {
        private Game _game;
        private readonly DispatcherTimer _closeCardTimer;
        private static readonly Brush HiddenBrush = new SolidColorBrush(Colors.CornflowerBlue);
        private static readonly Brush VisibleBrush = new SolidColorBrush(Colors.Black);

        public MainWindow()
        {
            InitializeComponent();
            _closeCardTimer = new DispatcherTimer();
            _closeCardTimer.Interval = TimeSpan.FromMilliseconds(750);
            _closeCardTimer.Tick += CloseCardTimer_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                var button = new Button();
                button.Style = (Style)Resources["CardButtonStyle"];
                button.Click += Card_Click;
                cardGrid.Children.Add(button);
            }

            StartNewGame();
        }

        private void StartNewGame()
        {
            _game = Game.Create();

            for (int i = 0; i < cardGrid.Children.Count; i++)
            {
                var button = (Button)cardGrid.Children[i];
                int col = i % _game.Width;
                int row = i / _game.Width;
                button.Content = _game.GetCard(col, row).ToString();
                button.Foreground = HiddenBrush;
            }

            UpdateCards();
        }

        private void UpdateCards()
        {
            for (int i = 0; i < cardGrid.Children.Count; i++)
            {
                var button = (Button)cardGrid.Children[i];
                int col = i % _game.Width;
                int row = i / _game.Width;
                button.Foreground = _game.IsOpen(col, row) ? VisibleBrush : HiddenBrush;
            }
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (_closeCardTimer.IsEnabled)
                return;

            if (sender is Button button)
            {
                int index = cardGrid.Children.IndexOf(button);
                int col = index % _game.Width;
                int row = index / _game.Width;

                if (_game.IsOpen(col, row))
                    return;

                _game.OpenCard(col, row);
                UpdateCards();

                if (_game.RemainingCardsInTurn > 0)
                    return;

                CheckForWinner();

                if (_game.CompleteTurn())
                    return;

                _closeCardTimer.Start();
            }
        }

        private void CheckForWinner()
        {
            if (!_game.IsComplete())
                return;

            var bestScore = GameSettings.Instance.BestScore;
            var currentScore = _game.Turns;
            GameSettings.Instance.UpdateScore(_game.Turns);

            string text;
            if (bestScore == 0)
                text = $"It took you {currentScore} turns to complete. Keep it up!";
            else if (bestScore < currentScore)
                text = $"It took you {currentScore - bestScore} more turns than your previous best. Try harder!";
            else
                text = $"You set a new best with only {currentScore} turns!";

            MessageBox.Show(text, "Congratulations!");
            StartNewGame();
        }

        private void CloseCardTimer_Tick(object sender, EventArgs e)
        {
            _closeCardTimer.Stop();
            _game.CloseCards();
            UpdateCards();
        }
    }
}

