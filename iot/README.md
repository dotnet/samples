# Using .NET Core for IoT Scenarios

.NET Core can be used to build applications for [IoT](https://en.wikipedia.org/wiki/Internet_of_things) devices and scenarios. IoT applications typically interact with sensors, displays and input devices that require the use of [GPIO pins](https://en.wikipedia.org/wiki/General-purpose_input/output), serial ports or similar hardware. The [Raspberry Pi](https://www.raspberrypi.org/) is commonly used for IoT applications.

## Samples

The following samples demonstrate various scenarios: 

* [Blinking LED](led-blink/README.md)
* [More blinking lights](led-more-blinking-lights/README.md)
* [Trimpot (potentiometer)](trimpot/README.md)

## Libraries

These samples use the [System.Devices.Gpio](https://dotnet.myget.org/feed/dotnet-corefxlab/package/nuget/System.Devices.Gpio) library. It will be supported on Linux and Windows IoT Core. At present, the library works on Linux. The library is currently in early preview, based on [source in dotnet/corefxlab](https://github.com/dotnet/corefxlab/tree/master/src/System.Devices.Gpio). 

There are many libraries that are important beyond GPIO, I2C and related fundamental protocols. We are working on a plan where the .NET Team and the community can work together to build up a shared repository of implementations.

## Breadboard layouts

The samples expect the device pins to be connected in particular way to function, typically on a breadboard. Each example includes a [Fritzing](http://fritzing.org/home/) diagram of the required breadboard layout, such as the following one (taken from the [More blinking lights](led-more-blinking-lights/README.md) sample). 

![Rasperry Pi Breadboard diagram](led-more-blinking-lights/rpi-more-blinking-lights_bb.png)

## Requirements

You need to use at least [.NET Core 2.1](https://www.microsoft.com/net/download/archives). [.NET Core 3.0](https://github.com/dotnet/announcements/issues/82) is required for ARM64 devices.

Many of these samples use the [Raspberry Pi](https://www.raspberrypi.org/), however, .NET Core can be used for other devices. A [Raspberry Pi Starter Pack](https://www.adafruit.com/product/3058) contains enough electronics to get started on many projects.

.NET Core is supported on Raspberry Pi 2 and 3. Raspberry Pi 3 B+ is recommended, based on the faster CPU. .NET Core is not supported on Raspberry Pi Zero or any of the Raspberry Pi model A devices. .NET Core does is not supported on ARMv6 chips, only ARMv7 and ARMv8. It is not supported on Arduino.

## Resources

* [How to Use a Breadboard](https://www.youtube.com/watch?v=6WReFkfrUIk)
* [Collin's Lab: Breadboards & Perfboards](https://www.youtube.com/watch?v=w0c3t0fJhXU)
