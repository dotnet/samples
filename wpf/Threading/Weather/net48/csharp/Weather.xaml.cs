using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace SDKSamples
{
    public partial class Weather : Window
    {
        public Weather() =>
            InitializeComponent();

        private async void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            // Change the status image and start the rotation animation.
            fetchButton.IsEnabled = false;
            fetchButton.Content = "Contacting Server";
            weatherText.Text = "";
            ((Storyboard)Resources["HideWeatherImageStoryboard"]).Begin(this);

            // Asynchronously fetch the weather forecast on a different thread and pause this code.
            string weather = await Task.Run(FetchWeatherFromServerAsync);

            // After async data returns, process it...
            // Set the weather image
            if (weather == "sunny")
                weatherIndicatorImage.Source = (ImageSource)Resources["SunnyImageSource"];

            else if (weather == "rainy")
                weatherIndicatorImage.Source = (ImageSource)Resources["RainingImageSource"];

            //Stop clock animation
            ((Storyboard)Resources["ShowClockFaceStoryboard"]).Stop(ClockImage);
            ((Storyboard)Resources["HideClockFaceStoryboard"]).Begin(ClockImage);
            
            //Update UI text
            fetchButton.IsEnabled = true;
            fetchButton.Content = "Fetch Forecast";
            weatherText.Text = weather;
        }

        private async Task<string> FetchWeatherFromServerAsync()
        {
            // Simulate the delay from network access
            await Task.Delay(TimeSpan.FromSeconds(4));

            // Tried and true method for weather forecasting - random numbers
            Random rand = new Random();

            if (rand.Next(2) == 0)
                return "rainy";
            
            else
                return "sunny";
        }

        private void HideClockFaceStoryboard_Completed(object sender, EventArgs args) =>
            ((Storyboard)Resources["ShowWeatherImageStoryboard"]).Begin(ClockImage);

        private void HideWeatherImageStoryboard_Completed(object sender, EventArgs args) =>
            ((Storyboard)Resources["ShowClockFaceStoryboard"]).Begin(ClockImage, true);
    }
}
