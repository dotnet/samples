using System.Device.Gpio;
using System.Device.Spi;
using Iot.Device.Adc;
using Iot.Device.Ft232H;
using Iot.Device.FtCommon;

var devices = FtCommon.GetDevices();
var ft232h = new Ft232HDevice(devices[0]);
var hardwareSpiSettings = new SpiConnectionSettings(0, 3) { ClockFrequency = 1_000_000, DataBitLength = 8, ChipSelectLineActiveState = PinValue.Low };
using SpiDevice spi = ft232h.CreateSpiDevice(hardwareSpiSettings);
using var mcp = new Mcp3008(spi);
while (true)
{
    Console.Clear();
    double value = mcp.Read(0);
    Console.WriteLine($"{value}");
    Console.WriteLine($"{Math.Round(value/10.23, 1)}%");
    Thread.Sleep(500);
}