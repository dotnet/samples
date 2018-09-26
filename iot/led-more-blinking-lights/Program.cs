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
            
            Console.WriteLine($"Let's blink some LEDs!");
            GpioController controller = new GpioController(PinNumberingScheme.Gpio);
            GpioPin ledOne = controller.OpenPin(pinOne, PinMode.Output);
            GpioPin ledTwo = controller.OpenPin(pinTwo, PinMode.Output);
            GpioPin ledThree = controller.OpenPin(pinThree, PinMode.Output);
            Console.WriteLine($"GPIO pins enabled for use: {pinOne}, {pinTwo}, {pinThree}");

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
            {
                controller.ClosePin(pinOne);
                controller.ClosePin(pinTwo);
                controller.ClosePin(pinThree);
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
                    Console.WriteLine("Light LED two for 5000 ms");
                    ledThree.Write(PinValue.High);
                }
                else if (timer3.IsFirstMultiple(2000))
                {
                    Console.WriteLine("Dim LED two for 5000 ms");
                    ledThree.Write(PinValue.Low);
                }

                Thread.Sleep(100);
                TimeEnvelope.AddTime(timers,100);
            }
        }
    }
}
