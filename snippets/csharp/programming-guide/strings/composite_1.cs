// Composite Formatting
string firstName = "Phillis";
string lastName = "Wheatley";
int yearBorn = 1753;
int firstPublished = 1773;
// Variable
Console.WriteLine("{0} {1} was an African American poet born in {2}.", firstName, lastName, yearBorn);
// Simple expression
Console.WriteLine("She was first published in {0} at the age of {1}.", firstPublished, firstPublished - yearBorn);
// Complex Expression
Console.WriteLine("She'd be over {0} years old today.", Math.Round((2018d - yearBorn) / 100d) * 100d);