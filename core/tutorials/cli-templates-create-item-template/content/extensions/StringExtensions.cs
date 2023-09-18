namespace System;

public static class StringExtensions
{
    public static string Reverse(this string value)
    {
        char[] tempArray = value.ToCharArray();
        Array.Reverse(tempArray);
        return new string(tempArray);
    }
}
