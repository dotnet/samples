using System.Device.I2c;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Ft232H;
using Iot.Device.FtCommon;

var devices = FtCommon.GetDevices();
var ft232h = new Ft232HDevice(devices[0]);
var i2cSettings = new I2cConnectionSettings(0, Bme280.SecondaryI2cAddress);

using var i2cDevice = ft232h.CreateI2cDevice(i2cSettings);
using var bme280 = new Bme280(i2cDevice);

int measurementTime = bme280.GetMeasurementDuration();

while (true)
{
    Console.Clear();

    bme280.SetPowerMode(Bmx280PowerMode.Forced);
    Thread.Sleep(measurementTime);

    bme280.TryReadTemperature(out var tempValue);
    bme280.TryReadPressure(out var preValue);
    bme280.TryReadHumidity(out var humValue);
    bme280.TryReadAltitude(out var altValue);

    Console.WriteLine($"Temperature: {tempValue.DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Pressure: {preValue.Hectopascals:#.##} hPa");
    Console.WriteLine($"Relative humidity: {humValue.Percent:#.##}%");
    Console.WriteLine($"Estimated altitude: {altValue.Meters:#} m");

    Thread.Sleep(1000);
}