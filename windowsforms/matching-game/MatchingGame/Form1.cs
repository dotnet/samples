using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MatchingGame
{
    public partial class Form1 : Form
    {
        // firstClicked points to the first Label control
        // that the player clicks, but it will be null
        // if the player hasn't clicked a label yet.
        Label firstClicked = null;

        // secondClicked points to the second Label control
        // that the player clicks.
        Label secondClicked = null;

        // Use this Random object to choose random icons for the squares.
        Random random = new Random();

        // Each of these letters is an interesting icon
        // in the Webdings font,
        // and each icon appears twice in this list.
        List<string> icons = new List<string>()
        {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "w", "w", "z", "z"
        };

        /// <summary>
        /// Assign each icon from the list of icons to a random square
        /// </summary>
        private void AssignIconsToSquares()
        {
            // The TableLayoutPanel has 16 labels,
            // and the icon list has 16 icons,
            // so an icon is pulled at random from the list
            // and added to each label.
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                if (iconLabel != null)
                {
                    int randomNumber = random.Next(icons.Count);
                    iconLabel.Text = icons[randomNumber];
                    iconLabel.ForeColor = iconLabel.BackColor;
                    icons.RemoveAt(randomNumber);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            AssignIconsToSquares();
        }

        /// <summary>
        /// Every label's Click event is handled by this event handler.
        /// </summary>
        /// <param name="sender">The label that was clicked.</param>
        /// <param name="e"></param>
        private void label_Click(object sender, EventArgs e)
        {
            // The timer is only on after two non-matching
            // icons have been shown to the player,
            // so ignore any clicks if the timer is running
            if (timer1.Enabled == true)
                return;

            Label clickedLabel = sender as Label;

            if (clickedLabel != null)
            {
                // If the clicked label is black, the player clicked
                // an icon that's already been revealed --
                // ignore the click.
                if (clickedLabel.ForeColor == Color.Black)
                    // All done - leave the if statements.
                    return;

                // If firstClicked is null, this is the first icon
                // in the pair that the player clicked,
                // so set firstClicked to the label that the player
                // clicked, change its color to black, and return.
                if (firstClicked == null)
                {
                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = Color.Black;

                    // All done - leave the if statements.
                    return;
                }

                // If the player gets this far, the timer isn't
                // running and firstClicked isn't null,
                // so this must be the second icon the player clicked
                // Set its color to black.
                secondClicked = clickedLabel;
                secondClicked.ForeColor = Color.Black;

                // Check to see if the player won.
                CheckForWinner();

                // If the player clicked two matching icons, keep them
                // black and reset firstClicked and secondClicked
                // so the player can click another icon.
                if (firstClicked.Text == secondClicked.Text)
                {
                    firstClicked = null;
                    secondClicked = null;
                    return;
                }

                // If the player gets this far, the player
                // clicked two different icons, so start the
                // timer (which will wait three quarters of
                // a second, and then hide the icons).
                timer1.Start();
            }
        }

        /// <summary>
        /// This timer is started when the player clicks
        /// two icons that don't match,
        /// so it counts three quarters of a second
        /// and then turns itself off and hides both icons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Stop the timer.
            timer1.Stop();

            // Hide both icons.
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            // Reset firstClicked and secondClicked
            // so the next time a label is
            // clicked, the program knows it's the first click.
            firstClicked = null;
            secondClicked = null;
        }

        /// <summary>
        /// Check every icon to see if it is matched, by
        /// comparing its foreground color to its background color.
        /// If all of the icons are matched, the player wins.
        /// </summary>
        private void CheckForWinner()
        {
            // Go through all of the labels in the TableLayoutPanel,
            // checking each one to see if its icon is matched.
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;

                if (iconLabel != null)
                {
                    if (iconLabel.ForeColor == iconLabel.BackColor)
                        return;
                }
            }

            // If the loop didn’t return, it didn't find
            // any unmatched icons.
            // That means the user won. Show a message and close the form.
            MessageBox.Show("You matched all the icons!", "Congratulations!");
            Close();
        }
    }
}
