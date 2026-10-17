namespace NUnitVSTest;

public sealed class CalculatorTests
{
    [Test]
    public void Add_TwoNumbers_ReturnsExpectedSum()
    {
        int result = 2 + 3;

        Assert.That(result, Is.EqualTo(5));
    }
}
