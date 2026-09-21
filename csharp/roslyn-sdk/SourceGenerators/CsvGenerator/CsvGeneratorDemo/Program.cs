// Demonstrate using the strongly-typed class generated from Cities.csv.

using CsvGenerated;

Console.WriteLine("=== Cities loaded from CSV ===");
Console.WriteLine();

foreach (Cities city in Cities.All)
{
    Console.WriteLine(city);
}

Console.WriteLine();
Console.WriteLine($"Total cities: {Cities.All.Count}");
