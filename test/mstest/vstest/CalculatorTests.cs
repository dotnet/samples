namespace MSTestVSTest;

[TestClass]
public sealed class CalculatorTests
{
    [TestMethod]
    public void Add_TwoNumbers_ReturnsExpectedSum()
    {
        int result = 2 + 3;

        Assert.AreEqual(5, result);
    }
}
