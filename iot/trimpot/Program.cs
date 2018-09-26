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

            // This sample implements two different ways of accessing the MCP3008.
            // The SPI option is enabled in the sample by default, but you can switch
            // to the GPIO bit-banging option by switching which one is commented out.
            // The sample uses local functions to make it easier to switch between
            // the two implementations

            // SPI implementation
            Mcp3008 GetMCP3008WithSPI()
            {
                var connection = new SpiConnectionSettings(0,0);
                connection.ClockFrequency = 1000000;
                connection.Mode = SpiMode.Mode0;
                var spi = new UnixSpiDevice(connection);
                var mcp3008 = new Mcp3008(spi);
                return mcp3008;
            }


            // the GPIO (via bit banging) implementation
            Mcp3008 GetMCP3008WithGPIO()
            {
                GpioController controller = new GpioController(PinNumberingScheme.Gpio);
                var mcp3008 = new Mcp3008(controller, 18, 23, 24, 25);
                return mcp3008;
            }

            // Using SPI implementation
            var mcp = GetMCP3008WithSPI();

            // Using GPIO implementation
            // var mcp = GetMCP3008WithGPIO();

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
