# More blinking lights, with hardware controls

This [sample](Program.cs) demonstrates blinking multiple LED on different schedules and controlling the LEDs from hardware controls. The sample builds on the [Blink an LED](../led-blink/README.md) and [Trimpot](../trimpot/README.md) samples.

## Code

This sample demonstrates how to use five different elements together, three LEDs, a potentiometer and a button. There is no one specific code element to call out here. Each element is controlled in a different way. You will find that different algorithms were needed to control LED timing than in the [Blink an LED](../led-blink/README.md) example. In particular, this sample implements the following:

* Different lighting schedules for different LEDs using the [TimeEnvelope](TimeEnvelope.cs) type.
* Integrating a factor in the lighting schedule based on the value returned from the potentiometer, implemented in the [Volume  ](Volume.cs) type.

## Breadboard layout

The following [fritzing diagram](rpi-more-blinking-lights.fzz) demonstrates how you should wire your device in order to run the [program](Program.cs). It uses several pins on the Raspberry Pi.

![Rasperry Pi Breadboard diagram](rpi-more-blinking-lights_bb.png)

## Hardware elements

The following elements are used in this sample:

* [Diffused LEDs](https://www.adafruit.com/product/297)
* [Potentiometer](https://www.adafruit.com/product/356)
* [MCP3008](https://www.adafruit.com/product/856)
* [Button](https://www.adafruit.com/product/367)

## Resources

* [Using .NET Core for IoT Scenarios](../README.md)
* [All about LEDs](https://learn.adafruit.com/all-about-leds)