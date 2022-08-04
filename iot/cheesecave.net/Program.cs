using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Text;

namespace CheeseCaveDotnet;

class Device
{
    private static readonly int s_pin = 21;
    private static GpioController s_gpio;
    private static I2cDevice s_i2cDevice;
    private static Bme280 s_bme280;

    // Global constants.
    const double DesiredTempLimit = 5;          // Acceptable range above or below the desired temp, in degrees F.
    const double DesiredHumidityLimit = 10;     // Acceptable range above or below the desired humidity, in percentages.
    const int IntervalInMilliseconds = 5000;    // Interval at which telemetry is sent to the cloud.

    // Global variables.
    private static DeviceClient s_deviceClient;
    private static stateEnum s_fanState = stateEnum.off;                      

    // The device connection string to authenticate the device with your IoT hub.
    private static readonly string s_deviceConnectionString = "YOUR DEVICE CONNECTION STRING HERE";

    // Enum for the state of the fan for cooling/heating, and humidifying/de-humidifying.
    enum stateEnum
    {
        off,
        on,
        failed
    }

    private static void Main(string[] args)
    {
        // Initialize the GPIO controller
        s_gpio = new GpioController();
        s_gpio.OpenPin(s_pin, PinMode.Output);

        // Get a reference to a device on the I2C bus
        var i2cSettings = new I2cConnectionSettings(1, Bme280.DefaultI2cAddress);
        s_i2cDevice = I2cDevice.Create(i2cSettings);

        // Create a reference to the BME280
        s_bme280 = new Bme280(s_i2cDevice);

        ColorMessage("Cheese Cave device app.\n", ConsoleColor.Yellow);

        // Create the device client and connect to the IoT hub using the MQTT protocol.
        s_deviceClient = DeviceClient.CreateFromConnectionString(s_deviceConnectionString, TransportType.Mqtt);

        // Create a handler for the direct method call
        s_deviceClient.SetMethodHandlerAsync("SetFanState", SetFanState, null).Wait();

        MonitorConditionsAndUpdateTwinAsync();

        Console.ReadLine();
        s_gpio.ClosePin(s_pin);
    }


    // Async method to send device twin updates.
    private static async void MonitorConditionsAndUpdateTwinAsync()
    {
        while (true)
        {
            Bme280ReadResult sensorOutput = s_bme280.Read();         

            // Update the Twin
            await UpdateTwin(
                    sensorOutput.Temperature.Value.DegreesFahrenheit, 
                    sensorOutput.Humidity.Value.Percent);

            await Task.Delay(IntervalInMilliseconds);
        }
    }

    // Handle the direct method call
    private static Task<MethodResponse> SetFanState(MethodRequest methodRequest, object userContext)
    {
        if (s_fanState is stateEnum.failed)
        {
            // Acknowledge the direct method call with a 400 error message.
            string result = "{\"result\":\"Fan failed\"}";
            RedMessage("Direct method failed: " + result);
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
        }
        else
        {
            try
            {
                var data = Encoding.UTF8.GetString(methodRequest.Data);

                // Remove quotes from data.
                data = data.Replace("\"", "");

                // Parse the payload, and trigger an exception if it's not valid.
                s_fanState = (stateEnum)Enum.Parse(typeof(stateEnum), data);
                GreenMessage("Fan set to: " + data);

                // Control the "fan" (LED)
                s_gpio.Write(s_pin, s_fanState == stateEnum.on ? PinValue.High : PinValue.Low);

                // Acknowledge the direct method call with a 200 success message.
                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            catch
            {
                // Acknowledge the direct method call with a 400 error message.
                string result = "{\"result\":\"Invalid parameter\"}";
                RedMessage("Direct method failed: " + result);
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }
        }
    }

    private static async Task UpdateTwin(double currentTemperature, double currentHumidity)
    {
        // Report the properties back to the IoT Hub.
        var reportedProperties = new TwinCollection();
        reportedProperties["fanstate"] = s_fanState.ToString();
        reportedProperties["humidity"] = Math.Round(currentHumidity, 2);
        reportedProperties["temperature"] = Math.Round(currentTemperature, 2);
        await s_deviceClient.UpdateReportedPropertiesAsync(reportedProperties);

        GreenMessage("Twin state reported: " + reportedProperties.ToJson());
    }

    private static void ColorMessage(string text, ConsoleColor clr)
    {
        Console.ForegroundColor = clr;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    private static void GreenMessage(string text) => 
        ColorMessage(text, ConsoleColor.Green);

    private static void RedMessage(string text) => 
        ColorMessage(text, ConsoleColor.Red);
}
