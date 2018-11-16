// String Interpolation
string f = "Jupiter";
string l = "Hammon";
int brn = 1711;
int pub = 1761;
// Variable
Console.WriteLine($"{f} {l} was an African American poet born in {brn}.");
// Simple expression
Console.WriteLine($"He was first published in {pub} at the age of {pub - brn}.");
// Complex Expression
Console.WriteLine($"He'd be over {Math.Round((2018d - brn) / 100d, 0) * 100d} years old today.");                
