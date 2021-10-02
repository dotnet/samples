# Using Azure SDK with BMP280 on M5Stack with .NET nanoFramework

M5Stack is a modular, stackable, and portable device which is powered with an ESP-32 core. And .NET nanoFramework image can be loaded in the board to enable the writing of managed code applications.

In this sample [M5Stack Gray](https://docs.m5stack.com/en/core/gray) will be used but same sample code can run on any ESP32 and  any other nanoFramework supported devices.

This sample application includes a scenario:
Every 3 seconds application reads temperature value from BMP280 sensor and turns on LED after reading the value. Then this temperature value is pushed to IoT Hub with device name and after every successful push LED will blink and turn off. Data will be handled by IoT Hub message routing and will be pushed to blob storage to store.

![M5Stack with nanoFramework to Azure Iot Hub](../images/M5Stack-Azure-flow.png)

## Getting Started with nanoFramework

* [Getting Started Guide for managed code (C#)](https://docs.nanoframework.net/content/getting-started-guides/getting-started-managed.html)
* [List of suported devices/sensors by NanoFramework](https://github.com/nanoframework/nanoFramework.Azure.Devices)
* [nanoFramework Samples](https://github.com/nanoframework/Samples)

## Prerequisites

* M5Stack with nanoFramework installed
* BMP280 Pressure/Temperature sensor breakout
* Jumper wires
* 5 mm LED
* 330 Ω resistor
* Breadboard (optional)

## Prepare the hardware

Use the hardware components to build the circuit as depicted in the following diagram:

![M5Stack with nanoFramework diagram](../images/M5Stack-BMP280_bb.png)

## Pins

The following are the connections from the M5Stack to the BME280 breakout:

* 3.3V to VIN OR 3V3 (shown in red)
* Ground to GND (grey)
* SDA (GPIO 21 for ESP32) to SDI (blue)
* SCL (GPIO 22 for ESP32) to SCK (yellow)
* GPIO 18 to LED anode (longer, positive lead)
* LED cathode (shorter, negative lead) to 330 Ω resistor (either end)
* 330 Ω resistor (other end) to ground

## Run the Solution

Use your Azure Subscription for following steps:

1. Make sure your [Pins](#-Pins) are correctly connected.

2. Create an IoT Hub using your Azure Subscription, you can use free tier of IoT Hub. You'll need IoT Hub name in the next steps in your code.

3. Create certificate using OpenSLL following [Create test certificate using OpenSSL](../create-certificate.md) documentation. Don't forget to change `raspberry-pi` to a unique name to your m5stack device in the tutorial commands.

4. If you followed above tutorial, you'll already have your device, if not create a device in your IoT Hub

    * Go to IoT Devices in Azure IoT Hub
    ![Create a Device in your IoT Hub](../images/iot-hub-create-iot-device-1.png)

    * Give a unique name to your device and select `X.509 CA Signed` as Authentication type
    ![Create a Device in your IoT Hub](../images/iot-hub-create-iot-device-2.png)

5. Provide your IoT Hub details, `Device ID` and `IoT Hub Name` into `Program.cs`. Open your solution with Microsoft Visual Studio 2019.

    ```csharp
    const string DeviceID = "<replace-with-your-device-id>";
    const string IotBrokerAddress = "<replace-with-your-iot-hub-name>.azure-devices.net";
    ```

6. Provide your `PEM` certificate, `Private Key` and `Azure Root CA` in string formats into  `Program.cs`. You can access these values from "Step 3 - [Create test certificate using OpenSSL](create-certificate.md)"

    ```csharp
    / X509Certificate certificates in PEM format and key
    const string cert =
    @"-----BEGIN CERTIFICATE-----
    <replace-with-your-pem-certificate-value>
    -----END CERTIFICATE-----";
    const string privateKey =
    @"-----BEGIN ENCRYPTED PRIVATE KEY-----
    <replace-with-your-encrypted-private-key-value>
    -----END ENCRYPTED PRIVATE KEY-----";
    const string rootCA =
    @"-----BEGIN CERTIFICATE-----
    <replace-with-your-azurePEMCertBaltimore-value>
    -----END CERTIFICATE-----";
    ```

7. Enter your WiFi Settings in the `Program.cs` file

    ```csharp
    const string  MySsid = "<replace-with-valid-ssid";
    const string  MyPassword = "<replace-with-valid-password>";
    ```

### Building the sample

1. Go to sample and Double-click the Visual Studio Solution (.sln) file.
2. Press Ctrl+Shift+B, or select **Build** \> **Build Solution**.
3. Make sure to click on device explorer and select your device.

The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.

### Deploying the sample

* Select Build > Deploy Solution.

### Deploying and running the sample

* To debug the sample and then run it, press F5 or select Debug >  Start Debugging.

## Result

![Result M5Stack circuit](../images/M5Stack-BMP280_integration.png)

If you enable Message routing with Custom endpoint, you can save all inputs into an Azure Blob Storage

* First Create a route

    ![Iot Hub Message Routing](../images/iot-hub-message-routing-1.png)

* Create a custom endpoint to connect to your blob storage data will be save in below format:
`{iothub}/{partition}/{YYYY}/{MM}/{DD}/{HH}/{mm}` Once connection is succesfull you can see the status as healty

    ![Iot Hub Message Routing](../images/iot-hub-message-routing-2.png)

* Blob storage contains all data from IoT Hub in a container

    ![Iot Hub Message Routing](../images/iot-hub-message-routing-3.png)

and single JSON file structure looks like below:

```json
{
    "EnqueuedTimeUtc": "2021-09-09T13:12:18.3950000Z",
    "Properties": {},
    "SystemProperties": {
        "connectionDeviceId": "m5stack",
        "connectionAuthMethod": "{\"scope\":\"device\",\"type\":\"sas\",\"issuer\":\"iothub\",\"acceptingIpFilterRule\":null}",
        "connectionDeviceGenerationId": "637667231818779957",
        "contentType": "application/json",
        "contentEncoding": "",
        "enqueuedTime": "2021-09-09T13:12:18.3950000Z"
    },
    "Body": "eyJUZW1wZXJhdHVyZSI6MjcuNTYsIlByZXNzdXJlIjoxMDA2LjY5LCJEZXZpY2VJRCI6Im01c3RhY2sifQ=="
}
```

Body message is encoded with Base64 format, if you decode the value, you'll retrieve the message we sent to Azure IoT Hub:

```json
{
    "Temperature": 27.56,
    "Pressure": 1006.69,
    "DeviceID": "m5stack"
}
```

## References

* [nanoFramework AMQP sample](https://github.com/nanoframework/Samples/tree/main/samples/AMQP)
* [nanoFramework Azure SDK](https://github.com/nanoframework/nanoFramework.Azure.Devices)
* [Complete Azure MQTT sample using BMP280 sensor](https://github.com/nanoframework/Samples/tree/main/samples/AzureMQTTTwinsBMP280Sleep)
