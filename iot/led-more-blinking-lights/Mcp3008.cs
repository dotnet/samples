using System;
using System.Devices.Gpio;
using System.Devices.Spi;
public class Mcp3008
{

    private SpiDevice _spiDevice;
    private GpioPin _CLK;
    private GpioPin _MISO;
    private GpioPin _MOSI;
    private GpioPin _CS;


    public Mcp3008(SpiDevice spiDevice)
    {
        _spiDevice = spiDevice;
    }

    public Mcp3008(GpioController controller, int CLK, int MISO, int MOSI, int CS)
    {
        _CLK = controller.OpenPin(CLK, PinMode.Output);
        _MISO = controller.OpenPin(MISO, PinMode.Input);
        _MOSI = controller.OpenPin(MOSI, PinMode.Output);
        _CS = controller.OpenPin(CS, PinMode.Output);
    }

    public int Read(int adc_channel)
    {
        if (adc_channel < 0 && adc_channel > 7)
        {
            throw new ArgumentException("ADC channel must be within 0-7 range.");
        }

        if (_spiDevice != null)
        {
            return ReadSpi(adc_channel);
        }
        else
        {
            return ReadGpio(adc_channel);
        }
    }

    private int ReadGpio(int adc_channel)
    {
        //port of https://gist.github.com/ladyada/3151375

        while (true)
        {
            var commandout = 0;
            var trim_pot = 0;


            _CS.Write(PinValue.High);
            _CLK.Write(PinValue.Low);
            _CS.Write(PinValue.Low);

            commandout |= 0x18;
            commandout <<= 3;

            for (var i = 0; i < 5; i++)
            {
                if ((commandout & 0x80) > 0)
                {
                    _MOSI.Write(PinValue.High);
                }
                else
                {
                    _MOSI.Write(PinValue.Low);
                }

                commandout <<= 1;
                _CLK.Write(PinValue.High);
                _CLK.Write(PinValue.Low);
            }

            for (var i = 0; i < 12; i++)
            {
                _CLK.Write(PinValue.High);
                _CLK.Write(PinValue.Low);
                trim_pot <<= 1;

                if (_MISO.Read() == PinValue.High)
                {
                    trim_pot |= 0x1;
                }
            }

            _CS.Write(PinValue.High);

            trim_pot >>= 1;
            //var pot_adjust = Math.Abs(trim_pot - last_read);
            return trim_pot;
        }
    }

    private int ReadSpi(int adc_channel)
    {
        // ported code from:
        // https://github.com/adafruit/Adafruit_Python_MCP3008/blob/master/Adafruit_MCP3008/MCP3008.py

        int command = 0b11 << 6;                  //Start bit, single channel read
        command |= (adc_channel & 0x07) << 3;  // Channel number (in 3 bits)
        var input = new Byte[] { (Byte)command, 0, 0 };
        var output = new Byte[3];
        _spiDevice.TransferFullDuplex(input, output);

        var result = (output[0] & 0x01) << 9;
        result |= (output[1] & 0xFF) << 1;
        result |= (output[2] & 0x80) >> 7;
        result = result & 0x3FF;
        return result;
    }
}