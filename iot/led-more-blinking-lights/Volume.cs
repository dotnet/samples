using System;
using System.Devices.Spi;

public class Volume
{

    public static Volume EnableVolume()
    {
        var connection = new SpiConnectionSettings(0,0);
        connection.ClockFrequency = 1000000;
        connection.Mode = SpiMode.Mode0;
        var spi = new UnixSpiDevice(connection);
        var mcp3008 = new Mcp3008(spi);
        var volume = new Volume(mcp3008);
        volume.Init();
        return volume;
    }
    
    private Mcp3008 _mcp3008;
    private int _lastValue = 0;

    public Volume(Mcp3008 mcp3008)
    {
        _mcp3008 = mcp3008;
    }

    public int GetVolumeValue()
    {
        double value = _mcp3008.Read(0);
        value = value / 10.24;
        value = Math.Round(value);
        return (int)value;
    }

    private void Init()
    {
        _lastValue = GetVolumeValue();
    }

    public (bool update, int value) GetSleepforVolume(int sleep)
    {
        var value = GetVolumeValue();
        if (value > _lastValue - 2 && value < _lastValue +2)
        {
            return (false,0);
        }

        _lastValue = value;
        Console.WriteLine($"Volume: {value}");

        var tenth = value / 10;

        if (tenth == 5)
        {
            return (true, sleep);
        }

        var factor = 5 - tenth;
        factor = factor * 2;
        factor = Math.Abs(factor);

        var newValue = 0;

        if (tenth < 5)
        {
            newValue = sleep * factor;
        }
        else 
        {
            newValue = sleep / factor;
        }

        if (newValue >=10 && newValue <=1000)
        {
            return (true,newValue);
        }
        return (true, sleep);
    }
}