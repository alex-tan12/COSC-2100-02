// Author: Alex Tan
// Date:   Oct 8th 2025
// Desc:   Two-player hotseat Tic Tac Toe without using arrays in logic

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private string _current = "X";
        private int _scoreX, _scoreO, _scoreCats;

        public MainWindow()
        {
            InitializeComponent();
            ResetBoard();
            UpdateCurrentPlayerLabel();
        }

        // --- UI helpers -------------------------------------------------------

        private void UpdateCurrentPlayerLabel()
        {
            // Show "Name (Mark)" if name given, else just mark
            string name = _current == "X" ? TxtXName.Text : TxtOName.Text;
            TxtCurrent.Text = string.IsNullOrWhiteSpace(name) ? _current : $"{name} ({_current})";
        }

        private void SetTile(Button b, string mark)
        {
            b.Content = mark;
            b.IsEnabled = false;
        }

        private void EnableTiles(bool enable)
        {
            foreach (var b in AllTiles())
            {
                b.IsEnabled = enable && b.Content == null;
            }
        }

        private Button[] AllTiles() => new[]
        {
            B1,B2,B3,B4,B5,B6,B7,B8,B9
        };

        private void ClearTileHighlights()
        {
            foreach (var b in AllTiles())
                b.ClearValue(BackgroundProperty); // revert to style background
        }

        // --- Game actions -----------------------------------------------------

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button tile || tile.Content != null) return;

            SetTile(tile, _current);
            if (TryEndRoundOrContinue())
                return;

            // Next player's turn
            _current = _current == "X" ? "O" : "X";
            UpdateCurrentPlayerLabel();
        }

        private bool TryEndRoundOrContinue()
        {
            // Check all possible lines by calling the function with 3 buttons each.
            if (CheckLine(B1, B2, B3) || CheckLine(B4, B5, B6) || CheckLine(B7, B8, B9) ||
                CheckLine(B1, B4, B7) || CheckLine(B2, B5, B8) || CheckLine(B3, B6, B9) ||
                CheckLine(B1, B5, B9) || CheckLine(B3, B5, B7))
            {
                OnWin();
                return true;
            }

            // Cats game: all disabled/filled and no winner
            bool full = true;
            foreach (var b in AllTiles())
                if (b.Content == null) { full = false; break; }

            if (full)
            {
                _scoreCats++;
                LblScoreCats.Text = _scoreCats.ToString();
                MessageBox.Show("Cat’s game! Nobody wins this round.", "Round Over");
                ResetBoard(keepStarter: true); // keep the same starter by default
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if all three buttons contain equal non-null content. Highlights winning line.
        /// </summary>
        private bool CheckLine(Button b1, Button b2, Button b3)
        {
            var c1 = b1.Content?.ToString();
            var c2 = b2.Content?.ToString();
            var c3 = b3.Content?.ToString();

            if (!string.IsNullOrEmpty(c1) && c1 == c2 && c2 == c3)
            {
                HighlightWin(b1, b2, b3);
                return true;
            }
            return false;
        }

        private void HighlightWin(params Button[] buttons)
        {
            var highlight = (Brush)FindResource("WinHighlight");
            foreach (var b in buttons) b.Background = highlight;
        }

        private void OnWin()
        {
            // Announce winner using player names if available
            string winnerName = _current == "X" ? TxtXName.Text : TxtOName.Text;
            string label = string.IsNullOrWhiteSpace(winnerName) ? _current : $"{winnerName} ({_current})";

            // Update score
            if (_current == "X")
            {
                _scoreX++;
                LblScoreX.Text = _scoreX.ToString();
            }
            else
            {
                _scoreO++;
                LblScoreO.Text = _scoreO.ToString();
            }

            EnableTiles(false);
            MessageBox.Show($"{label} is the Winner!", "Winner Announcement", MessageBoxButton.OK, MessageBoxImage.Information);

            // Start next round with the **other** player for variety, or keep the winner—choose your rule.
            // Here we keep the next round starting with the other player:
            _current = _current == "X" ? "O" : "X";
            ResetBoard(keepStarter: true);
        }

        private void ResetBoard(bool keepStarter = false)
        {
            ClearTileHighlights();
            foreach (var b in AllTiles())
            {
                b.Content = null;
                b.IsEnabled = true;
            }

            if (!keepStarter) _current = "X";
            UpdateCurrentPlayerLabel();
        }

        private void ResetScores()
        {
            _scoreX = _scoreO = _scoreCats = 0;
            LblScoreX.Text = "0";
            LblScoreO.Text = "0";
            LblScoreCats.Text = "0";
        }

        // --- Buttons on the right --------------------------------------------

        private void BtnChooseStarter_Click(object sender, RoutedEventArgs e)
        {
            // Simple random choice between X and O
            _current = (System.DateTime.Now.Ticks % 2 == 0) ? "X" : "O";
            ResetBoard(keepStarter: true);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetBoard(keepStarter: true);
        }

        private void BtnResetScores_Click(object sender, RoutedEventArgs e)
        {
            ResetBoard(keepStarter: false);
            ResetScores();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
