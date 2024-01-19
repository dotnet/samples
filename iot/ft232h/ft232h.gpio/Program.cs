using System.Device.Gpio;
using Iot.Device.Ft232H;
using Iot.Device.FtCommon;

Console.WriteLine("Blinking LED. Press Ctrl+C to end.");

var ft232h = new Ft232HDevice(FtCommon.GetDevices()[0]);
var controller = ft232h.CreateGpioController();

int pin = Ft232HDevice.GetPinNumberFromString("D7");
controller.OpenPin(pin, PinMode.Output);
bool ledOn = true;
while (true)
{
    controller.Write(pin, ledOn ? PinValue.High : PinValue.Low);
    Thread.Sleep(1000);
    ledOn = !ledOn;
}