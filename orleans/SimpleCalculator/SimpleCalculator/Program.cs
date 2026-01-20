namespace SimpleCalculator;
class Program
{
    static void Main(string[] args)
    {
        Calculator calculator = new Calculator();

        Console.Write("Enter number: ");
        int a = int.Parse(Console.ReadLine());
        Console.Write("Enter number: ");
        int b = int.Parse(Console.ReadLine());

        Console.Write("Enter operator: ");
        char character = char.Parse(Console.ReadLine());

        int result = default;

        switch (character)
        {
            case '+': result = calculator.Sum(a, b); break;
            case '-': result = calculator.Substract(a, b); break;
            case '*': result = calculator.Multiply(a, b); break;
            case '/': result = calculator.Devide(a, b); break;
        }

        Console.Write("Result is: {0}", result);
        Console.ReadKey();
    }
}

