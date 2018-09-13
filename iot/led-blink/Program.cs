using System;
using System.Devices.Gpio;
using System.Threading;

namespace led_blink
{
    class Program
    {
        static void Main(string[] args)
        {
            var pin = 17;
            var lightTime = 1000;
            var dimTime = 200;
            
            Console.WriteLine($"Let's blink an LED!");
            GpioController controller = new GpioController(PinNumberingScheme.Gpio);
            GpioPin ledPin = controller.OpenPin(pin, PinMode.Output);
            Console.WriteLine($"GPIO pin enabled for use: {pin}");

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
            {
                controller.ClosePin(pin);
            };


            while (true)
            {
                Console.WriteLine($"Light for {lightTime}ms");
                ledPin.Write(PinValue.High);
                Thread.Sleep(lightTime);
                Console.WriteLine($"Dim for {dimTime}ms");
                ledPin.Write(PinValue.Low);
                Thread.Sleep(dimTime);
            }


        }
    }
}
