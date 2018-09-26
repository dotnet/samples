using System;
using System.Collections.Generic;

public class TimeEnvelope
{
    private int _count;
    private int _time = 0;
    private bool _throwOnOverflow;

    public static void AddTime(IEnumerable<TimeEnvelope> envelopes, int value)
    {
        foreach(var envelope in envelopes)
        {
            envelope.AddTime(value);
        }
    }

    public TimeEnvelope(int count, bool throwOnOverthrow = true)
    {
        _count = count;
        _throwOnOverflow = throwOnOverthrow;
    }

    public int Count
    {
        get
        {
            return _count;
        }
    }

    public int Time
    {
        get
        {
            return _time;
        }
    }

    public int AddTime(int value)
    {
        _time += value;
        if (_time == _count)
        {
            _time = 0;
        }
        else if (_throwOnOverflow && _time > _count)
        {
            throw new Exception ("TimeEnvelope count overflowed!");
        }
        return _time;
    }

    public bool IsFirstMultiple(int value)
    {
        if (_time == value)
        {
            return true;
        }
        return false;
    }

    public bool IsLastMultiple(int value)
    {
        if (_time - value == 0)
        {
            return true;
        }
        return false;
    }

    public bool IsMultiple(int value)
    {
        if (_time % value == 0)
        {
            return true;
        }
        return false;
    }
}