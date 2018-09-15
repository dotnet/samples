using System;
using System.Devices.Gpio;
using System.Devices.Spi;
using System.Threading;

namespace led_pwm
{
    class Program
    {
        static void Main(string[] args)
        {
            var SPICLK = 18;
            var SPIMISO = 23;
            var SPIMOSI = 24;
            var SPICS = 25;

            Console.WriteLine($"Let's read potentiometer values!!!");
            GpioController controller = new GpioController(PinNumberingScheme.Gpio);
            GpioPin SPICLKPin = controller.OpenPin(SPICLK, PinMode.Output);
            GpioPin SPIMISOPin = controller.OpenPin(SPIMISO, PinMode.Input);
            GpioPin SPIMOSIPin = controller.OpenPin(SPIMOSI, PinMode.Output);
            GpioPin SPICSPin = controller.OpenPin(SPICS, PinMode.Output);
            Console.WriteLine($"GPIO pins enabled for use: {SPICLK}, {SPIMISO}, {SPIMOSI}, {SPICS}");

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
            {
                controller.ClosePin(SPICLK);
                controller.ClosePin(SPIMISO);
                controller.ClosePin(SPIMOSI);
                controller.ClosePin(SPICS);
            };

            var last_read = 0;      // this keeps track of the last potentiometer value
            var tolerance = 5;      // to keep from being jittery we'll only change
                                    // volume when the pot has moved more than 5 'counts'

            while (true)
            {
                var commandout = 0;
                var trim_pot = 0;


                SPICSPin.Write(PinValue.High);
                SPICLKPin.Write(PinValue.Low);
                SPICSPin.Write(PinValue.Low);

                commandout |= 0x18;
                commandout <<= 3;

                for (var i = 0; i < 5; i++)
                {
                    if ((commandout & 0x80) > 0)
                    {
                        SPIMOSIPin.Write(PinValue.High);
                    }
                    else
                    {
                        SPIMOSIPin.Write(PinValue.Low);
                    }

                    commandout <<= 1;
                    SPICLKPin.Write(PinValue.High);
                    SPICLKPin.Write(PinValue.Low);
                }

                for (var i = 0; i < 12; i++)
                {
                    SPICLKPin.Write(PinValue.High);
                    SPICLKPin.Write(PinValue.Low);
                    trim_pot <<= 1;

                    if (SPIMISOPin.Read() == PinValue.High)
                    {
                        trim_pot |= 0x1;
                    }
                }

                SPICSPin.Write(PinValue.High);
                
                trim_pot >>= 1;
                var pot_adjust = Math.Abs(trim_pot - last_read);

                Console.WriteLine($"trim_pot: {trim_pot}");
                Console.WriteLine($"pot_adjust: {pot_adjust}");
                Console.WriteLine($"last_read: {last_read}");

                if (pot_adjust > tolerance)
                {
                    last_read = trim_pot;
                    Console.WriteLine("***Changed***");
                    var volume = trim_pot / 10.24;
                    volume = Math.Round(volume);
                    Console.WriteLine($"Volume: {volume}");
                }
                
                Thread.Sleep(500);
            }

        }

    }
}
