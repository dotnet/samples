using System.Device.Gpio;
using System.Diagnostics;
using System.Text;

namespace Morse.Net
{
    class Program
    {

        static Dictionary<string, char> MorseDictionary = Morse.Net.Common.MorseDictionary;

        static void Main()
        {
            // Set up GPIO pin for input
            int pinNumber = 21;
            var gpioController = new GpioController();
            gpioController.OpenPin(pinNumber, PinMode.InputPullDown);

            // Set up variables for receiving Morse code
            List<string> morseCodeList = new List<string>();
            StringBuilder currentCharacterCode = new StringBuilder();
            DateTime lastEventTime = DateTime.MinValue;

            Console.WriteLine("Listening for Morse code...");

            Debug.WriteLine($"Current pin status is {gpioController.Read(pinNumber)}");

            // Register callback for events on GPIO pin
            gpioController.RegisterCallbackForPinValueChangedEvent(
                pinNumber, PinEventTypes.Rising | PinEventTypes.Falling,
                (sender, eventArgs) =>
                {
                    var eventTime = DateTime.Now;
                    TimeSpan timeSinceLastEvent = eventTime - lastEventTime;
                    if (timeSinceLastEvent < TimeSpan.FromMilliseconds(90))
                    {
                        // Ignore events that are too close together (debounce)
                        return;
                    }

                    if (eventArgs.ChangeType is PinEventTypes.Falling)
                    {
                        // Falling voltage means the receiver **is** receiving a signal.
                        // We need to look at how long it's been since the last signal and
                        // determine whether we're still parsing the last character or if
                        // we're processing an new character (or new word, in the case of a space).

                        Debug.WriteLine($"Falling after {timeSinceLastEvent.TotalMilliseconds} ms");
                        
                        if (timeSinceLastEvent >= TimeSpan.FromMilliseconds(700))
                        {
                            // It's been more than 700 ms, that's a space after the end of character
                            if (currentCharacterCode.Length > 0)
                            {
                                ParseMorseCharacter(currentCharacterCode);
                            }
                            Console.Write(" ");
                        }
                        else if (timeSinceLastEvent >= TimeSpan.FromMilliseconds(290))
                        {
                            // It's been ~300 ms, that's end of character with no space
                            ParseMorseCharacter(currentCharacterCode);
                        }
                    }
                    else
                    {
                        // Rising voltage means the receiver has **stopped** receiving a signal.
                        // We need to figure out whether the signal was a . (dot) or a - (dash).

                        Debug.WriteLine($"Rising after {timeSinceLastEvent.TotalMilliseconds} ms");

                        if (timeSinceLastEvent < TimeSpan.FromMilliseconds(150))
                        {
                            currentCharacterCode.Append(".");
                        }
                        else
                        {
                            currentCharacterCode.Append("-");
                        }
                    }

                    lastEventTime = eventTime;
                });

            // Wait indefinitely
            while (true)
            {
                if (DateTime.Now > lastEventTime.AddMilliseconds(1000) && currentCharacterCode.Length > 0)
                {
                    // If we stop receiving signals for more than a second,
                    // the transmission is done (at least for now). Display the
                    // current character in the buffer and await further signals.

                    ParseMorseCharacter(currentCharacterCode);
                }
                Thread.Sleep(1000);
            }
        }

        static void ParseMorseCharacter(StringBuilder currentCharacterCode)
        {
            var morseCode = currentCharacterCode.ToString();
            if (MorseDictionary.ContainsKey(morseCode))
            {
                Console.Write(MorseDictionary[morseCode]);
            }
            else
            {
                Console.Write("?");
            }
            currentCharacterCode.Clear();
        }
    }
}
