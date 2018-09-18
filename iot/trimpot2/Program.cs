using System;
using System.Devices.Gpio;
using System.Devices.Spi;
using System.Threading;

namespace trimpot2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!!!");          
            var connection = new SpiConnectionSettings(0,0);
            connection.ClockFrequency = 1000000;
            connection.Mode = SpiMode.Mode0;
            var spi = new UnixSpiDevice(connection);
            
            while (true)
            {
                /*
                command = 0b11 << 6                  # Start bit, single channel read
                command |= (adc_number & 0x07) << 3  # Channel number (in 3 bits)
                # Note the bottom 3 bits of command are 0, this is to account for the
                # extra clock to do the conversion, and the low null bit returned at
                # the start of the response.
                resp = self._spi.transfer([command, 0x0, 0x0])
                */
                var command = 0b11 << 6;
                command |= (adc_number & 0x07) << 3
                var value = spi.TransferFullDuplex()
                Console.WriteLine(value);
                Thread.Sleep(5);
            }
        }
    }
}
