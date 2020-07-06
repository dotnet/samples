Option Strict On

Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class Form1
    Inherits Form

    Public Sub New()
        Me.Text = "Task Dialog Demos"

        Dim currentButtonCount As Integer = 0
        Dim AddButtonForAction = New Action(Of String, Action)(
            Sub(name, action)
                currentButtonCount += 1
                Dim nextButton = currentButtonCount

                Dim button = New Button() With {
                    .Text = name,
                    .Size = New Size(180, 23),
                    .Location = New Point(nextButton \ 20 * 200 + 20, (nextButton Mod 20) * 30)
                }

                AddHandler button.Click, Sub(s, e) action()

                Controls.Add(button)
            End Sub)

        AddButtonForAction("Confirmation Dialog (3x)", AddressOf ShowSimpleTaskDialog)
        AddButtonForAction("Close Document Confirmation", AddressOf ShowCloseDocumentTaskDialog)
        AddButtonForAction("Minesweeper Difficulty", AddressOf ShowMinesweeperDifficultySelectionTaskDialog)
        AddButtonForAction("Auto-Closing Dialog", AddressOf ShowAutoClosingTaskDialog)
        AddButtonForAction("Multi-Page Dialog (modeless)", AddressOf ShowMultiPageTaskDialog)
        AddButtonForAction("Elevation Required", AddressOf ShowElevatedProcessTaskDialog)
        AddButtonForAction("Events Demo", AddressOf ShowEventsDemoTaskDialog)
    End Sub

    Private Sub ShowSimpleTaskDialog()
        ' Show a message box.
        Dim messageBoxResult As DialogResult = MessageBox.Show(
            Me,
            text:="Stopping the operation might leave your database in a corrupted state. Are you sure you want to stop?",
            caption:="Confirmation [Message Box]",
            buttons:=MessageBoxButtons.YesNo,
            icon:=MessageBoxIcon.Warning,
            defaultButton:=MessageBoxDefaultButton.Button2)

        If messageBoxResult = DialogResult.Yes Then
            Console.WriteLine("User confirmed to stop the operation.")
        End If

        ' Show a task dialog (simple).
        Dim result As TaskDialogButton = TaskDialog.ShowDialog(Me, New TaskDialogPage() With {
            .Text = "Stopping the operation might leave your database in a corrupted state.",
            .Heading = "Are you sure you want to stop?",
            .Caption = "Confirmation (Task Dialog)",
            .Buttons = New TaskDialogButtonCollection() From
            {
                TaskDialogButton.Yes,
                TaskDialogButton.No
            },
            .Icon = TaskDialogIcon.Warning,
            .DefaultButton = TaskDialogButton.No
        })

        If result = TaskDialogButton.Yes Then
            Console.WriteLine("User confirmed to stop the operation.")
        End If

        ' Show a task dialog (enhanced).
        Dim page = New TaskDialogPage() With {
            .Heading = "Are you sure you want to stop?",
            .Text = "Stopping the operation might leave your database in a corrupted state.",
            .Caption = "Confirmation (Task Dialog)",
            .Icon = TaskDialogIcon.Warning,
            .AllowCancel = True,
            .Verification = New TaskDialogVerificationCheckBox() With
            {
                .Text = "Do not show again"
            },
            .Buttons = New TaskDialogButtonCollection() From
            {
                TaskDialogButton.Yes,
                TaskDialogButton.No
            },
            .DefaultButton = TaskDialogButton.No
        }

        Dim resultButton = TaskDialog.ShowDialog(Me, page)

        If resultButton = TaskDialogButton.Yes Then
            If page.Verification.Checked Then _
                Console.WriteLine("Do not show this confirmation again.")

            Console.WriteLine("User confirmed to stop the operation.")
        End If
    End Sub

    Private Sub ShowCloseDocumentTaskDialog()
        ' Create the page which we want to show in the dialog.
        Dim btnCancel As TaskDialogButton = TaskDialogButton.Cancel
        Dim btnSave As TaskDialogButton = New TaskDialogButton("&Save")
        Dim btnDontSave As TaskDialogButton = New TaskDialogButton("Do&n't save")

        Dim page = New TaskDialogPage() With {
            .Caption = "My Application",
            .Heading = "Do you want to save changes to Untitled?",
            .Buttons = New TaskDialogButtonCollection() From
            {
                btnCancel,
                btnSave,
                btnDontSave
            }
        }

        ' Show a modal dialog, then check the result.
        Dim result As TaskDialogButton = TaskDialog.ShowDialog(Me, page)

        If result = btnSave Then
            Console.WriteLine("Saving")
        ElseIf result = btnDontSave Then
            Console.WriteLine("Not saving")
        Else
            Console.WriteLine("Canceling")
        End If
    End Sub

    Private Sub ShowMinesweeperDifficultySelectionTaskDialog()
        Dim page = New TaskDialogPage() With {
            .Caption = "Minesweeper",
            .Heading = "What level of difficulty do you want to play?",
            .AllowCancel = True,
            .Footnote = New TaskDialogFootnote() With {
                .Text = "Note: You can change the difficulty level later " &
                    "by clicking Options on the Game menu."
            },
            .Buttons = New TaskDialogButtonCollection() From
            {
                New TaskDialogCommandLinkButton("&Beginner", "10 mines, 9 x 9 tile grid") With
                {
                    .Tag = 10
                },
                New TaskDialogCommandLinkButton("&Intermediate", "40 mines, 16 x 16 tile grid") With
                {
                    .Tag = 40
                },
                New TaskDialogCommandLinkButton("&Advanced", "99 mines, 16 x 30 tile grid") With
                {
                    .Tag = 99
                }
            }
        }

        Dim result As TaskDialogButton = TaskDialog.ShowDialog(Me, page)

        If TypeOf result.Tag Is Integer Then
            Dim resultingMines = DirectCast(result.Tag, Integer)
            Console.WriteLine($"Playing with {resultingMines} mines...")
        Else
            Console.WriteLine("User canceled.")
        End If
    End Sub

    Private Sub ShowAutoClosingTaskDialog()
        Const textFormat As String = "Reconnecting in {0} seconds..."
        Dim remainingTenthSeconds As Integer = 50

        Dim reconnectButton = New TaskDialogButton("&Reconnect now")
        Dim cancelButton = TaskDialogButton.Cancel

        ' Display the form's icon in the task dialog.
        ' Note however that the task dialog will not scale the icon.
        Dim page = New TaskDialogPage() With
        {
            .Heading = "Connection lost; reconnecting...",
            .Text = String.Format(textFormat, (remainingTenthSeconds + 9) \ 10),
            .Icon = New TaskDialogIcon(Me.Icon),
            .ProgressBar = New TaskDialogProgressBar() With
            {
                .State = TaskDialogProgressBarState.Paused
            },
            .Buttons = New TaskDialogButtonCollection() From
            {
                reconnectButton,
                cancelButton
            }
        }

        ' Create a WinForms timer that raises the Tick event every tenth second.
        Using timer = New Timer() With {
            .Enabled = True,
            .Interval = 100
        }
            AddHandler timer.Tick,
                Sub(s, e)
                    remainingTenthSeconds -= 1
                    If remainingTenthSeconds > 0 Then
                        ' Update the remaining time and progress bar.
                        page.Text = String.Format(textFormat, (remainingTenthSeconds + 9) \ 10)
                        page.ProgressBar.Value = 100 - remainingTenthSeconds * 2
                    Else
                        ' Stop the timer and click the "Reconnect" button - this will
                        ' close the dialog.
                        timer.Enabled = False
                        reconnectButton.PerformClick()
                    End If
                End Sub

            Dim result As TaskDialogButton = TaskDialog.ShowDialog(Me, page)
            If result = reconnectButton Then
                Console.WriteLine("Reconnecting.")
            Else
                Console.WriteLine("Not reconnecting.")
            End If
        End Using
    End Sub

    Private Sub ShowMultiPageTaskDialog()
        ' Disable the "Yes" button and only enable it when the check box is checked.
        ' Also, don't close the dialog when this button is clicked.
        Dim initialButtonYes = TaskDialogButton.Yes
        initialButtonYes.Enabled = False
        initialButtonYes.AllowCloseDialog = False

        ' A modeless dialog can be minimizable.
        Dim initialPage = New TaskDialogPage() With {
            .Caption = "My Application",
            .Heading = "Clean up database?",
            .Text = $"Do you really want to do a clean-up?{vbLf}This action is irreversible!",
            .Icon = TaskDialogIcon.ShieldWarningYellowBar,
            .AllowCancel = True,
            .AllowMinimize = True,
            .Verification = New TaskDialogVerificationCheckBox() With {
                .Text = "I know what I'm doing"
            },
            .Buttons = New TaskDialogButtonCollection() From {
                TaskDialogButton.No,
                initialButtonYes
            },
            .DefaultButton = TaskDialogButton.No
        }

        ' For the "In Progress" page, don't allow the dialog to close, by adding
        ' a disabled button (if no button was specified, the task dialog would
        ' get an (enabled) 'OK' button).
        Dim inProgressCloseButton = TaskDialogButton.Close
        inProgressCloseButton.Enabled = False

        Dim inProgressPage = New TaskDialogPage() With {
            .Caption = "My Application",
            .Heading = "Operation in progress...",
            .Text = "Please wait while the operation is in progress.",
            .Icon = TaskDialogIcon.Information,
            .AllowMinimize = True,
            .ProgressBar = New TaskDialogProgressBar() With {
                .State = TaskDialogProgressBarState.Marquee
            },
            .Expander = New TaskDialogExpander() With {
                .Text = "Initializing...",
                .Position = TaskDialogExpanderPosition.AfterFootnote
            },
            .Buttons = New TaskDialogButtonCollection() From {
                inProgressCloseButton
            }
        }

        ' Add an invisible Cancel button where we will intercept the Click event
        ' to prevent the dialog from closing (when the User clicks the "X" button
        ' in the title bar or presses ESC or Alt+F4).
        Dim invisibleCancelButton = TaskDialogButton.Cancel
        invisibleCancelButton.Visible = False
        invisibleCancelButton.AllowCloseDialog = False
        inProgressPage.Buttons.Add(invisibleCancelButton)

        Dim finishedPage = New TaskDialogPage() With {
            .Caption = "My Application",
            .Heading = "Success!",
            .Text = "The operation finished.",
            .Icon = TaskDialogIcon.ShieldSuccessGreenBar,
            .AllowMinimize = True,
            .Buttons = New TaskDialogButtonCollection() From {
                TaskDialogButton.Close
            }
        }

        Dim showResultsButton As TaskDialogButton = New TaskDialogCommandLinkButton("Show &Results")
        finishedPage.Buttons.Add(showResultsButton)

        ' Enable the "Yes" button only when the checkbox is checked.
        Dim checkBox As TaskDialogVerificationCheckBox = initialPage.Verification
        AddHandler checkBox.CheckedChanged,
            Sub(sender, e)
                initialButtonYes.Enabled = checkBox.Checked
            End Sub

        ' When the user clicks "Yes", navigate to the second page.
        AddHandler initialButtonYes.Click,
            Sub(sender, e)
                ' Navigate to the "In Progress" page that displays the
                ' current progress of the background work.
                initialPage.Navigate(inProgressPage)

                ' NOTE: When you implement a "In Progress" page that represents
                ' background work that is done e.g. by a separate thread/task,
                ' which eventually calls Control.Invoke()/BeginInvoke() when
                ' its work is finished in order to navigate or update the dialog,
                ' then DO NOT start that work here already (directly after
                ' setting the Page property). Instead, start the work in the
                ' TaskDialogPage.Created event of the new page.
                '
                ' See comments in the code sample in https://github.com/dotnet/winforms/issues/146
                ' for more information.
            End Sub

        ' Simulate work by starting an async operation from which we are updating the
        ' progress bar and the expander with the current status.
        ' Note: VB.NET doesn't support 'await foreach' and async methods returning
        ' IAsyncEnumerable<T> as in C#, so we use a callback instead.
        Dim StreamBackgroundOperationProgressAsync = New Func(Of Action(Of Integer), Task)(
            Async Function(callback) As Task
                ' Note: The code here will run in the GUI thread - use
                ' "Await Task.Run(...)" to schedule CPU-intensive operations in a
                ' worker thread.

                ' Wait a bit before reporting the first progress.
                Await Task.Delay(2800)

                For i As Integer = 0 To 100 Step 4
                    ' Report the progress.
                    callback(i)

                    ' Wait a bit to simulate work.
                    Await Task.Delay(200)
                Next
            End Function)

        AddHandler inProgressPage.Created,
            Async Sub(s, e)
                ' Run the background operation and iterate over the streamed values to update
                ' the progress. Because we call the async method from the GUI thread,
                ' it will use this thread's synchronization context to run the continuations,
                ' so we don't need to use Control.[Begin]Invoke() to schedule the callbacks.
                Dim progressBar = inProgressPage.ProgressBar

                Await StreamBackgroundOperationProgressAsync(
                    Sub(progressValue)
                        ' When we display the first progress, switch the marquee progress bar
                        ' to a regular one.
                        If progressBar.State = TaskDialogProgressBarState.Marquee Then _
                            progressBar.State = TaskDialogProgressBarState.Normal

                        progressBar.Value = progressValue
                        inProgressPage.Expander.Text = $"Progress: {progressValue} %"
                    End Sub)

                ' Work Is finished, so navigate to the third page.
                inProgressPage.Navigate(finishedPage)
            End Sub

        ' Show the dialog (modeless).
        Dim result As TaskDialogButton = TaskDialog.ShowDialog(initialPage)
        If result = showResultsButton Then
            Console.WriteLine("Showing Results!")
        End If
    End Sub

    Private Sub ShowElevatedProcessTaskDialog()
        Dim page = New TaskDialogPage() With {
            .Heading = "Settings saved - Service Restart required",
            .Text = "The service needs to be restarted to apply the changes.",
            .Icon = TaskDialogIcon.ShieldSuccessGreenBar,
            .Buttons = New TaskDialogButtonCollection() From
            {
                TaskDialogButton.Close
            }
        }

        Dim restartNowButton = New TaskDialogCommandLinkButton("&Restart now")
        page.Buttons.Add(restartNowButton)

        restartNowButton.ShowShieldIcon = True
        AddHandler restartNowButton.Click,
            Sub(s, e)
                restartNowButton.AllowCloseDialog = True
                restartNowButton.Enabled = False

                ' Try to start an elevated cmd.exe.
                Dim psi = New ProcessStartInfo("cmd.exe", "/k echo Hi, this is an elevated command prompt.") With {
                    .UseShellExecute = True,
                    .Verb = "runas"
                }

                Try
                    Process.Start(psi)?.Dispose()
                Catch ex As Win32Exception When ex.NativeErrorCode = 1223
                    ' The user canceled the UAC prompt, so don't close the dialog and
                    ' re-enable the restart button.
                    restartNowButton.AllowCloseDialog = False
                    restartNowButton.Enabled = True
                    Return
                End Try
            End Sub

        TaskDialog.ShowDialog(Me, page)
    End Sub

    Private Sub ShowEventsDemoTaskDialog()
        Dim page1 = New TaskDialogPage() With {
            .Caption = Text,
            .Heading = "Event Demo",
            .Text = "Event Demo..."
        }

        AddHandler page1.Created, Sub(s, e) Console.WriteLine("Page1 Created")
        AddHandler page1.Destroyed, Sub(s, e) Console.WriteLine("Page1 Destroyed")
        AddHandler page1.HelpRequest, Sub(s, e) Console.WriteLine("Page1 HelpRequest")

        page1.Expander = New TaskDialogExpander("Expander") With {
            .Position = TaskDialogExpanderPosition.AfterFootnote
        }

        AddHandler page1.Expander.ExpandedChanged, Sub(s, e) Console.WriteLine("Expander ExpandedChanged: " & page1.Expander.Expanded)

        Dim buttonOK = TaskDialogButton.OK
        Dim buttonHelp = TaskDialogButton.Help
        Dim buttonCancelClose = New TaskDialogCommandLinkButton("C&ancel Close", allowCloseDialog:=False)
        Dim buttonShowInnerDialog = New TaskDialogCommandLinkButton("&Show (modeless) Inner Dialog", "(and don't cancel the Close)")
        Dim buttonNavigate = New TaskDialogCommandLinkButton("&Navigate", allowCloseDialog:=False)

        page1.Buttons.Add(buttonOK)
        page1.Buttons.Add(buttonHelp)
        page1.Buttons.Add(buttonCancelClose)
        page1.Buttons.Add(buttonShowInnerDialog)
        page1.Buttons.Add(buttonNavigate)

        AddHandler buttonOK.Click, Sub(s, e) Console.WriteLine($"Button '{s}' Click")
        AddHandler buttonHelp.Click, Sub(s, e) Console.WriteLine($"Button '{s}' Click")

        AddHandler buttonCancelClose.Click,
            Sub(s, e)
                Console.WriteLine($"Button '{s}' Click")
            End Sub

        AddHandler buttonShowInnerDialog.Click,
            Sub(s, e)
                Console.WriteLine($"Button '{s}' Click")
                TaskDialog.ShowDialog(New TaskDialogPage() With {
                    .Text = "Inner Dialog"
                })
                Console.WriteLine($"(returns) Button '{s}' Click")
            End Sub

        AddHandler buttonNavigate.Click,
            Sub(s, e)
                Console.WriteLine($"Button '{s}' Click")

                ' Navigate to a New page.
                Dim page2 = New TaskDialogPage() With {
                    .Heading = "AfterNavigation.",
                    .Buttons = New TaskDialogButtonCollection() From {
                        TaskDialogButton.Close
                    }
                }

                AddHandler page2.Created, Sub(s2, e2) Console.WriteLine("Page2 Created")
                AddHandler page2.Destroyed, Sub(s2, e2) Console.WriteLine("Page2 Destroyed")

                page1.Navigate(page2)
            End Sub

        page1.Verification = New TaskDialogVerificationCheckBox("&CheckBox")
        AddHandler page1.Verification.CheckedChanged, Sub(s, e) Console.WriteLine("CheckBox CheckedChanged: " & page1.Verification.Checked)

        Dim radioButton1 = page1.RadioButtons.Add("Radi&oButton 1")
        Dim radioButton2 = page1.RadioButtons.Add("RadioB&utton 2")

        AddHandler radioButton1.CheckedChanged, Sub(s, e) Console.WriteLine("RadioButton1 CheckedChanged: " & radioButton1.Checked)
        AddHandler radioButton2.CheckedChanged, Sub(s, e) Console.WriteLine("RadioButton2 CheckedChanged: " & radioButton2.Checked)

        Dim dialogResult = TaskDialog.ShowDialog(page1)
        Console.WriteLine("---> Dialog Result: " & dialogResult?.ToString())
    End Sub
End Class
