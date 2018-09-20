using System;
using System.Devices.Gpio;
using System.Devices.Spi;
using System.Threading;

namespace trimpot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Trimpot!");

            // This code should be uncommented to enable accessing
            // the MCP3008 via SPI
            /* 
            var connection = new SpiConnectionSettings(0,0);
            connection.ClockFrequency = 1000000;
            connection.Mode = SpiMode.Mode0;
            var spi = new UnixSpiDevice(connection);

            var mcp = new Mcp3008(spi);
            */

            // This code should be uncommented to enable accessing
            // the MCP3008 via GPIO (via bit banging)

            /* 
            GpioController controller = new GpioController(PinNumberingScheme.Gpio);
            var mcp = new Mcp3008(controller, 18, 23, 24, 25);
            */

            while (true)
            {
                double value = mcp.Read(0);
                value = value / 10.24;
                value = Math.Round(value);
                Console.WriteLine(value);
                Thread.Sleep(500);
            }
        }
    }
}
