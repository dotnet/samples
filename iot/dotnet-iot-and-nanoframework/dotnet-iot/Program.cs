// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.Bmxx80;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.Devices.Client;
using System.Text;

namespace DNSensorAzureIoTHub
{
    public class Program
    {
        // set up IoT Hub message
        const string DeviceID = "<replace-with-your-device-id>";
        const string IotBrokerAddress = "<replace-with-your-iot-hub-name>.azure-devices.net";

         // LED constraints 
        const int pin = 18;
        const int lightTime = 1000;
        const int dimTime = 2000;

        // busId number for I2C pins
        const int busId = 1;

        public static void Main()
        {
            Console.WriteLine($".Net IoT with BMP280 Sensor!");

            // set up for LED and pin
            using GpioController led = new();
            led.OpenPin(pin, PinMode.Output);
    
            // setup for BMP280
            I2cConnectionSettings i2cSettings = new(busId, Bmp280.DefaultI2cAddress);
            I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            using var i2CBmp280 = new Bmp280(i2cDevice);

            // Create an X.509 certificate object.
            var cert = new X509Certificate2($"{DeviceID}.pfx", "1234");
            var auth = new DeviceAuthenticationWithX509Certificate(DeviceID, cert);
            DeviceClient azureIoTClient = DeviceClient.Create(IotBrokerAddress, auth, TransportType.Mqtt);

            if (azureIoTClient == null)
            {
                Console.WriteLine("Failed to create DeviceClient!");
            }
            else
            {
                Console.WriteLine("Successfully created DeviceClient!");
                Console.WriteLine("Press CTRL+D to stop application");
            }

            while (true)
            {
                try
                {
                    // set higher sampling and perform a synchronous measurement
                    i2CBmp280.TemperatureSampling = Sampling.LowPower;
                    i2CBmp280.PressureSampling = Sampling.UltraHighResolution;
                    var readResult = i2CBmp280.Read();

                    // led on
                    led.Write(pin, PinValue.High);
                    Thread.Sleep(lightTime);

                    // print out the measured data
                    string temperature = readResult.Temperature?.DegreesCelsius.ToString("F");
                    string pressure = readResult.Pressure?.Hectopascals.ToString("F");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine($"Temperature: {temperature}\u00B0C");
                    Console.WriteLine($"Pressure: {pressure}hPa");

                    // send to Iot Hub
                    string message = $"{{\"Temperature\":{temperature},\"Pressure\":{pressure},\"DeviceID\":\"{DeviceID}\"}}";
                    Message eventMessage = new Message(Encoding.UTF8.GetBytes(message));
                    azureIoTClient.SendEventAsync(eventMessage).Wait();
                    Console.WriteLine($"Data is pushed to Iot Hub: {message}");

                    // blink and led off
                    led.Write(pin, PinValue.Low);
                    Thread.Sleep(75);
                    led.Write(pin, PinValue.High);
                    Thread.Sleep(75);
                    led.Write(pin, PinValue.Low);
                    Thread.Sleep(75);
                    led.Write(pin, PinValue.High);
                    Thread.Sleep(75);
                    led.Write(pin, PinValue.Low);
                    Thread.Sleep(dimTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }
            }
        }
    }
}