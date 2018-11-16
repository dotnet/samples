// String Interpolation
string firstName = "Jupiter";
string lastName = "Hammon";
int yearBorn = 1711;
int firstPublished = 1761;
// Variable
Console.WriteLine($"{firstName} {lastName} was an African American poet born in {yearBorn}.");
// Simple expression
Console.WriteLine($"He was first published in {firstPublished} at the age of {firstPublished - yearBorn}.");
// Complex Expression
Console.WriteLine($"He'd be over {Math.Round((2018d - yearBorn) / 100d) * 100d} years old today.");                
