Imports System.Windows.Media.Animation

Public Class Weather

    Private Async Sub FetchButton_Click(sender As Object, e As RoutedEventArgs)

        ' Change the status image and start the rotation animation.
        fetchButton.IsEnabled = False
        fetchButton.Content = "Contacting Server"
        weatherText.Text = ""
        DirectCast(Resources("HideWeatherImageStoryboard"), Storyboard).Begin(Me)

        ' Asynchronously fetch the weather forecast on a different thread and pause this code.
        Dim weatherType As String = Await Task.Run(AddressOf FetchWeatherFromServerAsync)

        ' After async data returns, process it...
        ' Set the weather image
        If weatherType = "sunny" Then
            weatherIndicatorImage.Source = DirectCast(Resources("SunnyImageSource"), ImageSource)

        ElseIf weatherType = "rainy" Then
            weatherIndicatorImage.Source = DirectCast(Resources("RainingImageSource"), ImageSource)

        End If

        ' Stop clock animation
        DirectCast(Resources("ShowClockFaceStoryboard"), Storyboard).Stop(ClockImage)
        DirectCast(Resources("HideClockFaceStoryboard"), Storyboard).Begin(ClockImage)

        ' Update UI text
        fetchButton.IsEnabled = True
        fetchButton.Content = "Fetch Forecast"
        weatherText.Text = weatherType
    End Sub

    Private Async Function FetchWeatherFromServerAsync() As Task(Of String)

        ' Simulate the delay from network access
        Await Task.Delay(TimeSpan.FromSeconds(4))

        ' Tried and true method for weather forecasting - random numbers
        Dim rand As New Random()

        If rand.Next(2) = 0 Then
            Return "rainy"
        Else
            Return "sunny"
        End If

    End Function

    Private Sub HideClockFaceStoryboard_Completed(sender As Object, e As EventArgs)
        DirectCast(Resources("ShowWeatherImageStoryboard"), Storyboard).Begin(ClockImage)
    End Sub

    Private Sub HideWeatherImageStoryboard_Completed(sender As Object, e As EventArgs)
        DirectCast(Resources("ShowClockFaceStoryboard"), Storyboard).Begin(ClockImage, True)
    End Sub
End Class
