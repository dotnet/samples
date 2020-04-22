Option Explicit Off
Option Infer On
Option Strict Off
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms

Namespace MatchingGame
    Public Partial Class Form1
        Inherits Form
        ' firstClicked points to the first Label control
        ' that the player clicks, but it will be null
        ' if the player hasn't clicked a label yet.
        Private firstClicked As Label = Nothing

        ' secondClicked points to the second Label control
        ' that the player clicks.
        Private secondClicked As Label = Nothing

        ' Use this Random object to choose random icons for the squares.
        Private ReadOnly random_Renamed As Random = New Random()

        ' Each of these letters is an interesting icon
        ' in the Webdings font,
        ' and each icon appears twice in this list.
        Private ReadOnly icons As List(Of String) = New List(Of String)() From {
            "!",
            "!",
            "N",
            "N",
            ",",
            ",",
            "k",
            "k",
            "b",
            "b",
            "v",
            "v",
            "w",
            "w",
            "z",
            "z"}

        ''' <summary>
        ''' Assign each icon from the list of icons to a random square
        ''' </summary>
        Private Sub AssignIconsToSquares()
            ' The TableLayoutPanel has 16 labels,
            ' and the icon list has 16 icons,
            ' so an icon is pulled at random from the list
            ' and added to each label.
            For Each control_Renamed As Control In Me.tableLayoutPanel1.Controls
                Dim iconLabel As Label = TryCast(control_Renamed, Label)
                If iconLabel IsNot Nothing Then
                    Dim randomNumber As Integer = Me.random_Renamed.[Next](Me.icons.Count)
                    iconLabel.Text = Me.icons(randomNumber)
                    iconLabel.ForeColor = iconLabel.BackColor
                    Me.icons.RemoveAt(randomNumber)
                End If
            Next
        End Sub


        Public Sub New()
            Me.InitializeComponent()
            Me.AssignIconsToSquares()
        End Sub

        ''' <summary>
        ''' Every label's Click event is handled by this event handler.
        ''' </summary>
        ''' <param name="sender">The label that was clicked.</param>
        ''' <param name="e"></param>
        Private Sub label_Click(sender As Object, e As EventArgs)
            ' The timer is only on after two non-matching
            ' icons have been shown to the player,
            ' so ignore any clicks if the timer is running
            If Me.timer1.Enabled = True Then
                Return
            End If

            Dim clickedLabel As Label = TryCast(sender, Label)

            If clickedLabel IsNot Nothing Then
                ' If the clicked label is black, the player clicked
                ' an icon that's already been revealed --
                ' ignore the click.
                If clickedLabel.ForeColor = Color.Black Then
                    ' All done - leave the if statements.
                    Return
                End If

                ' If firstClicked is null, this is the first icon
                ' in the pair that the player clicked,
                ' so set firstClicked to the label that the player
                ' clicked, change its color to black, and return.
                If Me.firstClicked Is Nothing Then
                    Me.firstClicked = clickedLabel
                    Me.firstClicked.ForeColor = Color.Black

                    ' All done - leave the if statements.
                    Return
                End If

                ' If the player gets this far, the timer isn't
                ' running and firstClicked isn't null,
                ' so this must be the second icon the player clicked
                ' Set its color to black.
                Me.secondClicked = clickedLabel
                Me.secondClicked.ForeColor = Color.Black

                ' Check to see if the player won.
                Me.CheckForWinner()

                ' If the player clicked two matching icons, keep them
                ' black and reset firstClicked and secondClicked
                ' so the player can click another icon.
                If Me.firstClicked.Text = Me.secondClicked.Text Then
                    Me.firstClicked = Nothing
                    Me.secondClicked = Nothing
                    Return
                End If

                ' If the player gets this far, the player
                ' clicked two different icons, so start the
                ' timer (which will wait three quarters of
                ' a second, and then hide the icons).
                Me.timer1.Start()
            End If
        End Sub

        ''' <summary>
        ''' This timer is started when the player clicks
        ''' two icons that don't match,
        ''' so it counts three quarters of a second
        ''' and then turns itself off and hides both icons.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub timer1_Tick(sender As Object, e As EventArgs)
            ' Stop the timer.
            Me.timer1.[Stop]()

            ' Hide both icons.
            Me.firstClicked.ForeColor = Me.firstClicked.BackColor
            Me.secondClicked.ForeColor = Me.secondClicked.BackColor

            ' Reset firstClicked and secondClicked
            ' so the next time a label is
            ' clicked, the program knows it's the first click.
            Me.firstClicked = Nothing
            Me.secondClicked = Nothing
        End Sub

        ''' <summary>
        ''' Check every icon to see if it is matched, by
        ''' comparing its foreground color to its background color.
        ''' If all of the icons are matched, the player wins.
        ''' </summary>
        Private Sub CheckForWinner()
            ' Go through all of the labels in the TableLayoutPanel,
            ' checking each one to see if its icon is matched.
            For Each control_Renamed As Control In Me.tableLayoutPanel1.Controls
                Dim iconLabel As Label = TryCast(control_Renamed, Label)

                If iconLabel IsNot Nothing Then
                    If iconLabel.ForeColor = iconLabel.BackColor Then
                        Return
                    End If
                End If
            Next

            ' If the loop didn’t return, it didn't find
            ' any unmatched icons.
            ' That means the user won. Show a message and close the form.
            MessageBox.Show("You matched all the icons!", "Congratulations!")
            Me.Close()
        End Sub

    End Class
End Namespace
