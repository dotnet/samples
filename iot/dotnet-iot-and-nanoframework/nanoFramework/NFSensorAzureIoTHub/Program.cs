// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.Bmxx80;
using System.Security.Cryptography.X509Certificates;
using nanoFramework.Azure.Devices.Client;
using nanoFramework.Networking;
using System.Diagnostics;

namespace NFSensorAzureIoTHub
{
    public class Program
    {
        // Wifi for M5Stack Device
        const int WifiSleepTime = 60000;
        const string MySsid = "<replace-with-valid-ssid";
        const string MyPassword = "<replace-with-valid-password>";

        // set up IoT Hub message
        const string DeviceID = "<replace-with-your-device-id>";
        const string IotBrokerAddress = "<replace-with-your-iot-hub-name>.azure-devices.net";

        // LED constraints 
        const int pin = 18;
        const int lightTime = 1000;
        const int dimTime = 2000;

        // busId number for I2C pins
        const int busId = 1;

        // X509Certificate certificates in PEM format and key
        const string cert =
@"-----BEGIN CERTIFICATE-----
Your PEM certificate
-----END CERTIFICATE-----";

        const string privateKey =
@"-----BEGIN ENCRYPTED PRIVATE KEY-----
Encrypted private key
-----END ENCRYPTED PRIVATE KEY-----";

        const string rootCA =
@"-----BEGIN CERTIFICATE-----
azurePEMCertBaltimore
-----END CERTIFICATE-----";

        public static void Main()
        {
            Debug.WriteLine(".Net Nanoframework with BMP280 Sensor!");

            // connect to wifi
            if (!ConnectToWifi()) return;

            // set up for LED and pin
            using GpioController led = new();
            led.OpenPin(pin, PinMode.Output);

            // set up for BMP280
            I2cConnectionSettings i2cSettings = new(busId, Bmp280.DefaultI2cAddress);
            I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            using var i2CBmp280 = new Bmp280(i2cDevice);

            // Create an X.509 certificate object and create device client for Azure IoT Hub
            X509Certificate2 deviceCert = new X509Certificate2(cert, privateKey, "1234");
            DeviceClient azureIoTClient = new DeviceClient(IotBrokerAddress, DeviceID, deviceCert, azureCert: new X509Certificate(rootCA));
            var isOpen = azureIoTClient.Open();
            Debug.WriteLine($"Connection is open: {isOpen}");

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
                    string temperature = readResult.Temperature.DegreesCelsius.ToString("F");
                    string pressure = readResult.Pressure.Hectopascals.ToString("F");
                    Debug.WriteLine("-----------------------------------------");
                    Debug.WriteLine($"Temperature: {temperature}\u00B0C");
                    Debug.WriteLine($"Pressure: {pressure}hPa");

                    // send to Iot Hub
                    string message = $"{{\"Temperature\":{temperature},\"Pressure\":{pressure},\"DeviceID\":\"{DeviceID}\"}}";
                    azureIoTClient.SendMessage(message);
                    Debug.WriteLine($"Data is pushed to Iot Hub: {message}");

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
                    Debug.WriteLine($"An error occured: {ex.Message}");
                }
            }
        }

        private static bool ConnectToWifi()
        {
            Debug.WriteLine("Program Started, connecting to WiFi.");

            // As we are using TLS, we need a valid date & time
            // We will wait maximum 1 minute to get connected and have a valid date
            var success = NetworkHelper.ConnectWifiDhcp(MySsid, MyPassword, setDateTime: true, token: new CancellationTokenSource(WifiSleepTime).Token);
            if (!success)
            {
                Debug.WriteLine($"Can't connect to wifi: {NetworkHelper.ConnectionError.Error}");
                if (NetworkHelper.ConnectionError.Exception != null)
                {
                    Debug.WriteLine($"NetworkHelper.ConnectionError.Exception");
                }
            }

            Debug.WriteLine($"Date and time is now {DateTime.UtcNow}");
            return success;
        }
    }
}