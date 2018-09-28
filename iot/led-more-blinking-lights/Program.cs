using System;
using System.Devices.Gpio;
using System.Threading;

namespace led_more_blinking_lights
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // pins
            var pinOne = 17;
            var pinTwo = 16;
            var pinThree = 20;
            var pinFour = 21;

            // volume support
            var initialSleep = 100;
            var sleep = initialSleep;
            Volume volume = null;
            // this line should only be enabled if a trimpot is connected
            volume = Volume.EnableVolume();
            
            Console.WriteLine($"Let's blink some LEDs!");
            GpioController controller = new GpioController(PinNumberingScheme.Gpio);
            GpioPin ledOne = controller.OpenPin(pinOne, PinMode.Output);
            GpioPin ledTwo = controller.OpenPin(pinTwo, PinMode.Output);
            GpioPin ledThree = controller.OpenPin(pinThree, PinMode.Output);
            GpioPin buttonOne = controller.OpenPin(pinFour, PinMode.Input);

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
            {
                controller.ClosePin(pinOne);
                controller.ClosePin(pinTwo);
                controller.ClosePin(pinThree);
                controller.ClosePin(pinFour);
                Console.WriteLine("Pin cleanup complete!");
            };

            var timer1 = new TimeEnvelope(1000);
            var timer2 = new TimeEnvelope(1000);
            var timer3 = new TimeEnvelope(4000);
            var timers = new TimeEnvelope[] {timer1, timer2, timer3};

            while (true)
            {
                // behavior for ledOne
                if (timer1.Time == 0)
                {
                    Console.WriteLine($"Light LED one for 800ms");
                    ledOne.Write(PinValue.High);
                }
                else if (timer1.IsLastMultiple(200))
                {
                    Console.WriteLine($"Dim LED one for 200ms");
                    ledOne.Write(PinValue.Low);
                }
                
                // behavior for ledTwo
                if (timer2.IsMultiple(200))
                {
                    Console.WriteLine($"Light LED two for 100ms");
                    ledTwo.Write(PinValue.High);
                }
                else if (timer2.IsMultiple(100))
                {
                    Console.WriteLine($"Dim LED two for 100ms");
                    ledTwo.Write(PinValue.Low);
                }

                // behavior for ledThree
                if (timer3.Time == 0)
                {
                    Console.WriteLine("Light LED two for 2000 ms");
                    ledThree.Write(PinValue.High);
                }
                else if (timer3.IsFirstMultiple(2000))
                {
                    Console.WriteLine("Dim LED two for 2000 ms");
                    ledThree.Write(PinValue.Low);
                }

                // behavior for buttonOne
                if (volume != null)
                {
                    var update = true;
                    var value = 0;
                    while (update) 
                    {
                        (update,value) = volume.GetSleepforVolume(initialSleep);
                        if (update)
                        {
                            sleep = value;
                            Thread.Sleep(250);
                        }
                    }
                }

                while (buttonOne.Read() == PinValue.High)
                {
                    Console.WriteLine("Button one pin value high!");
                    ledOne.Write(PinValue.High);
                    ledTwo.Write(PinValue.High);
                    ledThree.Write(PinValue.High);
                    Thread.Sleep(250);
                }

                Console.WriteLine($"Sleep: {sleep}");
                Thread.Sleep(sleep); // starts at 100ms
                TimeEnvelope.AddTime(timers,100); // always stays at 100
            }
        }
    }
}
