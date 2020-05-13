Imports System.Globalization
Imports System.Numerics
Imports System.Resources
Imports System.Text.RegularExpressions

Public Class Form1

    Private label As ToolStripStatusLabel

    Private ReadOnly rm As New ResourceManager("Formatter.Resources", Me.GetType().Assembly)
    Private decimalSeparator As String
    Private amDesignator, pmDesignator, aDesignator, pDesignator As String
    Private pattern As String

    ' Flags to indicate presence of error information in status bar
    Dim valueInfo As Boolean
    Dim formatInfo As Boolean

    Private ReadOnly numberFormats() As String = {"C", "D", "E", "e", "F", "G", "N", "P", "R", "X", "x"}
    Private Const DEFAULTSELECTION As Integer = 5
    Private ReadOnly dateFormats() As String = {"g", "d", "D", "f", "F", "g", "G", "M", "O", "R", "s", "t", "T", "u", "U", "Y"}

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ' Disable OK button.
        OKButton.Enabled = False

        ' Add label to status bar.
        label = New ToolStripStatusLabel()
        StatusBar.Items.AddRange(New ToolStripItem() {label})

        ' Get localized strings for user interface.
        Text = rm.GetString("WindowCaption")
        ValueLabel.Text = rm.GetString(NameOf(ValueLabel))
        FormatLabel.Text = rm.GetString(NameOf(FormatLabel))
        ResultLabel.Text = rm.GetString(NameOf(ResultLabel))
        CulturesLabel.Text = rm.GetString("CultureLabel")
        NumberBox.Text = rm.GetString("NumberBoxText")
        DateBox.Text = rm.GetString("DateBoxText")
        OKButton.Text = rm.GetString("OKButtonText")

        ' Populate CultureNames list box with culture names
        Dim cultures() As CultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures)
        ' Define a string list so that we can sort and modify the names.
        Dim names As New List(Of String)
        Dim currentIndex As Integer      ' Index of the current culture.

        For Each culture As CultureInfo In cultures
            names.Add(culture.Name)
        Next
        names.Sort()
        ' Change the name of the invariant culture so it is human readable.
        names(0) = rm.GetString("InvariantCultureName")
        ' Add the culture names to the list box.
        CultureNames.Items.AddRange(names.ToArray())

        ' Make the current culture the selected culture.
        For ctr As Integer = 0 To names.Count - 1
            If names.Item(ctr) = CultureInfo.CurrentCulture.Name Then
                currentIndex = ctr
                Exit For
            End If
        Next
        CultureNames.SelectedIndex = currentIndex

        ' Get decimal separator.
        decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator

        ' Get am, pm designators.
        amDesignator = DateTimeFormatInfo.CurrentInfo.AMDesignator
        If amDesignator.Length >= 1 Then
            aDesignator = amDesignator.Substring(0, 1)
        Else
            aDesignator = String.Empty
        End If
        pmDesignator = DateTimeFormatInfo.CurrentInfo.PMDesignator
        If pmDesignator.Length >= 1 Then
            pDesignator = pmDesignator.Substring(0, 1)
        Else
            pDesignator = String.Empty
        End If
        ' For regex pattern for date and time components.
        pattern = $"^\s*\S+\s+\S+\s+\S+(\s+\S+)?(?<!{amDesignator}|{aDesignator}|{pmDesignator}|{pDesignator})\s*$"

        ' Select NumberBox for numeric string and populate combo box.
        NumberBox.Checked = True
    End Sub

    Private Sub NumberBox_CheckedChanged(sender As Object, e As EventArgs) Handles NumberBox.CheckedChanged
        If NumberBox.Checked Then
            Result.Text = String.Empty

            FormatStrings.Items.Clear()
            FormatStrings.Items.AddRange(numberFormats)
            FormatStrings.SelectedIndex = DEFAULTSELECTION
        End If
    End Sub

    Private Sub OKButton_Click(sender As Object, e As EventArgs) Handles OKButton.Click

        label.Text = ""
        Result.Text = String.Empty

        ' Get name of the current culture.
        Dim culture As CultureInfo
        Dim cultureName As String = CStr(CultureNames.Items(CultureNames.SelectedIndex))
        ' If the selected culture is the invariant culture, change its name.
        If cultureName = rm.GetString("InvariantCultureName") Then cultureName = String.Empty
        culture = CultureInfo.CreateSpecificCulture(cultureName)

        ' Parse string as date
        If DateBox.Checked Then
            Dim dat As Date
            Dim dto As DateTimeOffset
            Dim ticks As Long
            Dim hasOffset As Boolean = False

            ' Is the date a number expressed in ticks?
            If Long.TryParse(Value.Text, ticks) Then
                dat = New Date(ticks)
            Else
                ' Does the date have three components (date, time offset), or fewer than 3?
                If Regex.IsMatch(Value.Text, pattern, RegexOptions.IgnoreCase) Then
                    If DateTimeOffset.TryParse(Value.Text, dto) Then
                        hasOffset = True
                    Else
                        label.Text = rm.GetString("MSG_InvalidDTO")
                        valueInfo = True
                        Exit Sub
                    End If
                Else
                    ' The string is to be interpeted as a DateTime, not a DateTimeOffset.
                    If Date.TryParse(Value.Text, dat) Then
                        hasOffset = False
                    Else
                        label.Text = rm.GetString("MSG_InvalidDate")
                        valueInfo = True
                        Exit Sub
                    End If
                End If
            End If

            ' Format date value.

            Me.Result.Text = If(hasOffset, dto, dat).ToString(Me.FormatStrings.Text, culture)
        Else
            ' Handle formatting of a number.
            Dim intToFormat As Long
            Dim bigintToFormat As BigInteger = BigInteger.Zero
            Dim floatToFormat As Double


            ' Format a floating point value.
            If Value.Text.Contains(decimalSeparator) OrElse Value.Text.ToUpper(CultureInfo.InvariantCulture).Contains("E") Then
                Try
                    If Not Double.TryParse(Value.Text, floatToFormat) Then
                        label.Text = rm.GetString("MSG_InvalidFloat")
                    Else
                        Result.Text = floatToFormat.ToString(FormatStrings.Text, culture)
                    End If
                Catch ex As FormatException
                    label.Text = rm.GetString("MSG_InvalidFormat")
                    formatInfo = True
                End Try
            Else
                ' Handle formatting an integer.
                '
                ' Determine whether value is out of range of an Int64

                If Not BigInteger.TryParse(Value.Text, bigintToFormat) Then
                    label.Text = rm.GetString("MSG_InvalidInteger")
                Else
                    ' Format an Int64
                    If bigintToFormat >= Long.MinValue And bigintToFormat <= Long.MaxValue Then
                        intToFormat = CLng(bigintToFormat)
                        Try
                            Result.Text = intToFormat.ToString(FormatStrings.Text, culture)
                        Catch ex As FormatException
                            label.Text = rm.GetString("MSG_InvalidFormat")
                            formatInfo = True
                        End Try
                    Else
                        ' Format a BigInteger
                        Try
                            Result.Text = bigintToFormat.ToString(FormatStrings.Text, culture)
                        Catch ex As Exception
                            label.Text = rm.GetString("MSG_InvalidFormat")
                            formatInfo = True
                        End Try
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DateBox_CheckedChanged(sender As Object, e As EventArgs) Handles DateBox.CheckedChanged
        If DateBox.Checked Then
            Result.Text = String.Empty

            FormatStrings.Items.Clear()
            FormatStrings.Items.AddRange(dateFormats)
            FormatStrings.SelectedIndex = DEFAULTSELECTION
        End If
    End Sub

    Private Sub Value_TextChanged(sender As Object, e As EventArgs) Handles Value.TextChanged
        Result.Text = String.Empty

        If valueInfo Then
            label.Text = String.Empty
            valueInfo = False
        End If
        OKButton.Enabled = Not String.IsNullOrEmpty(Value.Text)
    End Sub

    Private Sub FormatStrings_SelectedValueChanged(sender As Object, e As EventArgs) Handles FormatStrings.SelectedValueChanged, CultureNames.SelectedValueChanged
        Result.Text = String.Empty
        If formatInfo Then
            label.Text = String.Empty
            formatInfo = False
        End If
    End Sub

    Private Sub CultureNames_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CultureNames.SelectedIndexChanged
        Result.Text = String.Empty
    End Sub
End Class
