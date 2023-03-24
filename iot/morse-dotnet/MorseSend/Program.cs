using System.Device.Gpio;

namespace Morse.Net
{
    class Program
    {
        static Dictionary<string, char> MorseDictionary = Morse.Net.Common.MorseDictionary;

        static void Main()
        {

            Console.WriteLine("Enter a string to convert to Morse code:");
            string inputString = Console.ReadLine()?.ToUpper() ?? string.Empty;

            // Set up GPIO pin for the LED
            int pinNumber = 18;
            var gpioController = new GpioController();
            gpioController.OpenPin(pinNumber, PinMode.Output);

            // Convert input string to a list of Morse codes
            List<string> morseCodeList = new List<string>();
            foreach (char c in inputString)
            {
                string code = MorseDictionary.FirstOrDefault(x => x.Value == c).Key;
                if(code is not null)
                    morseCodeList.Add(code);
            }

            // Blink Morse code on LED
            foreach (string morseCode in morseCodeList)
            {
                foreach (char c in morseCode)
                {
                    if (c == '.') // dot
                    {
                        Console.Write('.');

                        gpioController.Write(pinNumber, PinValue.High);
                        Thread.Sleep(100);
                        gpioController.Write(pinNumber, PinValue.Low);
                        Thread.Sleep(100);
                    }
                    else if (c == '-') // dash
                    {
                        Console.Write('-');

                        gpioController.Write(pinNumber, PinValue.High);
                        Thread.Sleep(300);
                        gpioController.Write(pinNumber, PinValue.Low);
                        Thread.Sleep(100);
                    }
                    else if (c == ' ') // space
                    {
                        Console.Write(' ');

                        Thread.Sleep(700);
                    }
                }

                // Pause between letters
                Console.Write('/');
                Thread.Sleep(300);
            }
            Console.WriteLine("\nDone!");

            // Clean up GPIO resources
            gpioController.Dispose();
        }
    }
}
