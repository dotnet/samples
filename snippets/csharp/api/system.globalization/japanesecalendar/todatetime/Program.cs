using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        var cal = new JapaneseCalendar();
        var jaJp = new CultureInfo("ja-JP");
        jaJp.DateTimeFormat.Calendar = cal;
        var date1 = cal.ToDateTime(2,1,1,0,0,0,0,JapaneseCalendar.CurrentEra);
        Console.WriteLine($"Japanese calendar date: {date1.ToString("D", jaJp)}, " +
                          $"Gregorian calendar date: {date1.ToString("D", CultureInfo.InvariantCulture)}");

        var date2 = cal.ToDateTime(6,11,7,0,0,0,0,GetEraIndex("大正"));
        Console.WriteLine($"Japanese calendar date: {date2.ToString("D", jaJp)}, " +
                          $"Gregorian calendar date: {date2.ToString("D", CultureInfo.InvariantCulture)}");

        int GetEraIndex(string eraName)
        {
           foreach (var ctr in cal.Eras)
              if (jaJp.DateTimeFormat.GetEraName(ctr) == eraName)
                 return ctr;

           return 0; 
        }
    }
}
// The example displays the following output:
//   Japanese calendar date: 平成2年1月1日, Gregorian calendar date: Monday, 01 January 1990
//   Japanese calendar date: 大正6年11月7日, Gregorian calendar date: Wednesday, 07 November 1917
