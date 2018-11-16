// String Interpolation
string jupiterFirst = "Jupiter";
string jupiterLast = "Hammon";
int jupiterBorn = 1711;
int jupiterPub = 1761;
// Variables
Console.WriteLine($"{jupiterFirst} {jupiterLast} was an African American poet born in {jupiterBorn}.");
// Simple expression
Console.WriteLine($"He was first published in {jupiterPub} at the age of {jupiterPub - jupiterBorn}.");
// Complex Expressions
Console.WriteLine($"He'd be over {Math.Round((2018d - jupiterBorn) / 100d, 0) * 100d} years old today."

// Output:
// Jupiter Hammon was an African American poet born in 1711.
// He was first published in 1761 at the age of 50.
// He'd be over 300 years old today. 