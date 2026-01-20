using NUnit.Framework;

namespace SimpleCalculator.Test;

public class Tests
{
    [Test]
    public void SumMethod_ShouldReturnCorrectValue()
    {
        Calculator calculator = new Calculator();

        int result = calculator.Sum(4, 5);

        Assert.That(result, Is.EqualTo(9));
    }

    [Test]
    public void SubstractMethod_ShouldReturnCorrectValue()
    {
        Calculator calculator = new Calculator();

        int result = calculator.Substract(4, 5);

        Assert.That(result, Is.EqualTo(-1));
    }

    [Test]
    public void DevideMethod_ShouldReturnCorrectValue()
    {
        Calculator calculator = new Calculator();

        int result = calculator.Devide(4, 5);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void MultiplyMethod_ShouldReturnCorrectValue()
    {
        Calculator calculator = new Calculator();

        int result = calculator.Multiply(4, 5);

        Assert.That(result, Is.EqualTo(20));
    }
}

