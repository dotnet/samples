Imports System.Globalization
Imports System.Numerics
Imports System.Resources
Imports System.Text.RegularExpressions

Public Class Form1

    Private _label As ToolStripStatusLabel

    Private ReadOnly _rm As New ResourceManager("Formatter.Resources", [GetType]().Assembly)
    Private _decimalSeparator As String
    Private _amDesignator, _pmDesignator, _aDesignator, _pDesignator As String
    Private _pattern As String

    ' Flags to indicate presence of error information in status bar
    Private _valueInfo As Boolean

    Private _formatInfo As Boolean

    Private ReadOnly _numberFormats() As String = {"C", "D", "E", "e", "F", "G", "N", "P", "R", "X", "x"}
    Private Const DefaultSelection As Integer = 5
    Private ReadOnly _dateFormats() As String = {"g", "d", "D", "f", "F", "g", "G", "M", "O", "R", "s", "t", "T", "u", "U", "Y"}

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Disable OK button.
        OKButton.Enabled = False

        ' Add label to status bar.
        _label = New ToolStripStatusLabel()
        StatusBar.Items.AddRange(New ToolStripItem() {_label})

        ' Get localized strings for user interface.
        Text = _rm.GetString("WindowCaption")
        ValueLabel.Text = _rm.GetString(NameOf(ValueLabel))
        FormatLabel.Text = _rm.GetString(NameOf(FormatLabel))
        ResultLabel.Text = _rm.GetString(NameOf(ResultLabel))
        CulturesLabel.Text = _rm.GetString("CultureLabel")
        NumberBox.Text = _rm.GetString("NumberBoxText")
        DateBox.Text = _rm.GetString("DateBoxText")
        OKButton.Text = _rm.GetString("OKButtonText")

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
        names(0) = _rm.GetString("InvariantCultureName")
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
        _decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator

        ' Get am, pm designators.
        _amDesignator = DateTimeFormatInfo.CurrentInfo.AMDesignator
        If _amDesignator.Length >= 1 Then
            _aDesignator = _amDesignator.Substring(0, 1)
        Else
            _aDesignator = String.Empty
        End If
        _pmDesignator = DateTimeFormatInfo.CurrentInfo.PMDesignator
        If _pmDesignator.Length >= 1 Then
            _pDesignator = _pmDesignator.Substring(0, 1)
        Else
            _pDesignator = String.Empty
        End If
        ' For regex pattern for date and time components.
        _pattern = $"^\s*\S+\s+\S+\s+\S+(\s+\S+)?(?<!{_amDesignator}|{_aDesignator}|{_pmDesignator}|{_pDesignator})\s*$"

        ' Select NumberBox for numeric string and populate combo box.
        NumberBox.Checked = True
    End Sub

    Private Sub NumberBox_CheckedChanged(sender As Object, e As EventArgs) Handles NumberBox.CheckedChanged
        If NumberBox.Checked Then
            Result.Text = String.Empty

            FormatStrings.Items.Clear()
            FormatStrings.Items.AddRange(_numberFormats)
            FormatStrings.SelectedIndex = DefaultSelection
        End If
    End Sub

    Private Sub OKButton_Click(sender As Object, e As EventArgs) Handles OKButton.Click

        _label.Text = ""
        Result.Text = String.Empty

        ' Get name of the current culture.
        Dim culture As CultureInfo
        Dim cultureName As String = CStr(CultureNames.Items(CultureNames.SelectedIndex))
        ' If the selected culture is the invariant culture, change its name.
        If cultureName = _rm.GetString("InvariantCultureName") Then cultureName = String.Empty
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
                If Regex.IsMatch(Value.Text, _pattern, RegexOptions.IgnoreCase) Then
                    If DateTimeOffset.TryParse(Value.Text, dto) Then
                        hasOffset = True
                    Else
                        _label.Text = _rm.GetString("MSG_InvalidDTO")
                        _valueInfo = True
                        Exit Sub
                    End If
                Else
                    ' The string is to be interpeted as a DateTime, not a DateTimeOffset.
                    If Date.TryParse(Value.Text, dat) Then
                        hasOffset = False
                    Else
                        _label.Text = _rm.GetString("MSG_InvalidDate")
                        _valueInfo = True
                        Exit Sub
                    End If
                End If
            End If

            ' Format date value.

            Result.Text = If(hasOffset, dto, dat).ToString(FormatStrings.Text, culture)
        Else
            ' Handle formatting of a number.
            Dim intToFormat As Long
            Dim bigintToFormat As BigInteger = BigInteger.Zero
            Dim floatToFormat As Double

            ' Format a floating point value.
            If Value.Text.Contains(_decimalSeparator) OrElse Value.Text.ToUpper(CultureInfo.InvariantCulture).Contains("E"c) Then
                Try
                    If Not Double.TryParse(Value.Text, floatToFormat) Then
                        _label.Text = _rm.GetString("MSG_InvalidFloat")
                    Else
                        Result.Text = floatToFormat.ToString(FormatStrings.Text, culture)
                    End If
                Catch ex As FormatException
                    _label.Text = _rm.GetString("MSG_InvalidFormat")
                    _formatInfo = True
                End Try
            Else
                ' Handle formatting an integer.
                '
                ' Determine whether value is out of range of an Int64

                If Not BigInteger.TryParse(Value.Text, bigintToFormat) Then
                    _label.Text = _rm.GetString("MSG_InvalidInteger")
                Else
                    ' Format an Int64
                    If bigintToFormat >= Long.MinValue And bigintToFormat <= Long.MaxValue Then
                        intToFormat = CLng(bigintToFormat)
                        Try
                            Result.Text = intToFormat.ToString(FormatStrings.Text, culture)
                        Catch ex As FormatException
                            _label.Text = _rm.GetString("MSG_InvalidFormat")
                            _formatInfo = True
                        End Try
                    Else
                        ' Format a BigInteger
                        Try
                            Result.Text = bigintToFormat.ToString(FormatStrings.Text, culture)
                        Catch ex As Exception
                            _label.Text = _rm.GetString("MSG_InvalidFormat")
                            _formatInfo = True
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
            FormatStrings.Items.AddRange(_dateFormats)
            FormatStrings.SelectedIndex = DefaultSelection
        End If
    End Sub

    Private Sub Value_TextChanged(sender As Object, e As EventArgs) Handles Value.TextChanged
        Result.Text = String.Empty

        If _valueInfo Then
            _label.Text = String.Empty
            _valueInfo = False
        End If
        OKButton.Enabled = Not String.IsNullOrEmpty(Value.Text)
    End Sub

    Private Sub FormatStrings_SelectedValueChanged(sender As Object, e As EventArgs) Handles FormatStrings.SelectedValueChanged, CultureNames.SelectedValueChanged
        Result.Text = String.Empty
        If _formatInfo Then
            _label.Text = String.Empty
            _formatInfo = False
        End If
    End Sub

    Private Sub CultureNames_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CultureNames.SelectedIndexChanged
        Result.Text = String.Empty
    End Sub

End Class
