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
        ' Disable Value text box.
        OKButton.Enabled = False

        ' Add label to status bar.
        label = New ToolStripStatusLabel()
        StatusBar.Items.AddRange(New ToolStripItem() {label})

        ' Get localized strings for user interface.
        Me.Text = rm.GetString("WindowCaption")
        Me.ValueLabel.Text = rm.GetString(NameOf(ValueLabel))
        Me.FormatLabel.Text = rm.GetString(NameOf(FormatLabel))
        Me.ResultLabel.Text = rm.GetString(NameOf(ResultLabel))
        Me.CulturesLabel.Text = rm.GetString("CultureLabel")
        Me.NumberBox.Text = rm.GetString("NumberBoxText")
        Me.DateBox.Text = rm.GetString("DateBoxText")
        Me.OKButton.Text = rm.GetString("OKButtonText")

        ' Populate CultureNames list box with culture names
        Dim cultures() As CultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures)
        ' Define a string array so that we can sort and modify the names.
        Dim names As New List(Of String)
        Dim currentIndex As Integer      ' Index of the current culture.

        For Each culture As CultureInfo In cultures
            names.Add(culture.Name)
        Next
        names.Sort()
        ' Change the name of the invariant culture so it is human readable.
        names(0) = rm.GetString("InvariantCultureName")
        ' Add the culture names to the list box.
        Me.CultureNames.Items.AddRange(names.ToArray())

        ' Make the current culture the selected culture.
        For ctr As Integer = 0 To names.Count - 1
            If names.Item(ctr) = CultureInfo.CurrentCulture.Name Then
                currentIndex = ctr
                Exit For
            End If
        Next
        Me.CultureNames.SelectedIndex = currentIndex

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
        Me.NumberBox.Checked = True
    End Sub

    Private Sub NumberBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles NumberBox.CheckedChanged
        If Me.NumberBox.Checked Then
            Me.Result.Text = String.Empty

            Me.FormatStrings.Items.Clear()
            Me.FormatStrings.Items.AddRange(numberFormats)
            Me.FormatStrings.SelectedIndex = DEFAULTSELECTION
        End If
    End Sub

    Private Sub OKButton_Click(sender As System.Object, e As System.EventArgs) Handles OKButton.Click

        label.Text = ""
        Me.Result.Text = String.Empty

        ' Get name of the current culture.
        Dim culture As CultureInfo
        Dim cultureName As String = CStr(Me.CultureNames.Items(Me.CultureNames.SelectedIndex))
        ' If the selected culture is the invariant culture, change its name.
        If cultureName = rm.GetString("InvariantCultureName") Then cultureName = String.Empty
        culture = CultureInfo.CreateSpecificCulture(cultureName)

        ' Parse string as date
        If Me.DateBox.Checked Then
            Dim dat As DateTime
            Dim dto As DateTimeOffset
            Dim ticks As Int64
            Dim hasOffset As Boolean = False

            ' Is the date a number expressed in ticks?
            If Int64.TryParse(Me.Value.Text, ticks) Then
                dat = New Date(ticks)
            Else
                ' Does the date have three components (date, time offset), or fewer than 3?
                If Regex.IsMatch(Me.Value.Text, pattern, RegexOptions.IgnoreCase) Then
                    If DateTimeOffset.TryParse(Me.Value.Text, dto) Then
                        hasOffset = True
                    Else
                        label.Text = rm.GetString("MSG_InvalidDTO")
                        valueInfo = True
                        Exit Sub
                    End If
                Else
                    ' The string is to be interpeted as a DateTime, not a DateTimeOffset.
                    If DateTime.TryParse(Me.Value.Text, dat) Then
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
                        Me.Result.Text = floatToFormat.ToString(Me.FormatStrings.Text, culture)
                    End If
                Catch ex As FormatException
                    label.Text = rm.GetString("MSG_InvalidFormat")
                    Me.formatInfo = True
                End Try
            Else
                ' Handle formatting an integer.
                '
                ' Determine whether value is out of range of an Int64

                If Not BigInteger.TryParse(Value.Text, bigintToFormat) Then
                    label.Text = rm.GetString("MSG_InvalidInteger")
                Else
                    ' Format an Int64
                    If bigintToFormat >= Int64.MinValue And bigintToFormat <= Int64.MaxValue Then
                        intToFormat = CLng(bigintToFormat)
                        Try
                            Me.Result.Text = intToFormat.ToString(Me.FormatStrings.Text, culture)
                        Catch ex As FormatException
                            label.Text = rm.GetString("MSG_InvalidFormat")
                            Me.formatInfo = True
                        End Try
                    Else
                        ' Format a BigInteger
                        Try
                            Me.Result.Text = bigintToFormat.ToString(Me.FormatStrings.Text, culture)
                        Catch ex As Exception
                            label.Text = rm.GetString("MSG_InvalidFormat")
                            Me.formatInfo = True
                        End Try
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DateBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles DateBox.CheckedChanged
        If Me.DateBox.Checked Then
            Me.Result.Text = String.Empty

            Me.FormatStrings.Items.Clear()
            Me.FormatStrings.Items.AddRange(dateFormats)
            Me.FormatStrings.SelectedIndex = DEFAULTSELECTION
        End If
    End Sub

    Private Sub Value_TextChanged(sender As Object, e As System.EventArgs) Handles Value.TextChanged
        Me.Result.Text = String.Empty

        If valueInfo Then
            label.Text = String.Empty
            valueInfo = False
        End If
        OKButton.Enabled = Not String.IsNullOrEmpty(Value.Text)
    End Sub

    Private Sub FormatStrings_SelectedValueChanged(sender As Object, e As System.EventArgs) Handles FormatStrings.SelectedValueChanged, CultureNames.SelectedValueChanged
        Me.Result.Text = String.Empty
        If formatInfo Then
            label.Text = String.Empty
            formatInfo = False
        End If
    End Sub

    Private Sub CultureNames_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CultureNames.SelectedIndexChanged
        Me.Result.Text = String.Empty
    End Sub
End Class
