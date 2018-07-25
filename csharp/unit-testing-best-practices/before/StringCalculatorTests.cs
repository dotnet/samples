using System;
using UnitTestingBestPractices;
using Xunit;

namespace UnitTestingBestPracticesBefore
{
    public class StringCalculatorTests
    {
        // <SnippetSetup>
        private readonly StringCalculator stringCalculator;
        public StringCalculatorTests()
        {
            stringCalculator = new StringCalculator();
        }
        // </SnippetSetup>

        // <SnippetPoorTestCase>
        [Fact]
        public void Test_Single()
        {
            // Arrange
            var stringCalculator = new StringCalculator();

            // Assert
            Assert.Equal(0, stringCalculator.Add("0"));
        }
        // </SnippetPoorTestCase>

        // <SnippetNotMinimallyPassing>
        [Fact]
        public void Add_SingleNumber_ReturnsSameNumber()
        {
            var stringCalculator = new StringCalculator();

            var actual = stringCalculator.Add("42");

            Assert.Equal(42, actual);
        }
        // </SnippetNotMinimallyPassing>

        // <SnippetUsingSetupVariable>
        [Fact]
        public void Add_TwoNumbers_ReturnsSumOfNumbers()
        {
            var result = stringCalculator.Add("0,1");

            Assert.Equal(1, result);
        }

        [Fact]
        public void Add_ThreeNumbers_ReturnsSumOfNumbers()
        {
            var result = stringCalculator.Add("0,1,2");

            Assert.Equal(3, result);
        }
        // </SnippetUsingSetupVariable>

        // <SnippetLogicInTests>
        [Fact]
        public void Add_MultipleNumbers_ReturnsCorrectResults()
        {
            var stringCalculator = new StringCalculator();
            var expected = 0;
            var testCases = new[]
            {
                "0,0",
                "0,1",
                "1,1",
                "1,2"
            };

            foreach (var test in testCases)
            {
                Assert.Equal(expected++, stringCalculator.Add(test));
            }

        }
        // </SnippetLogicInTests>

        // <SnippetMagicString>
        [Fact]
        public void Add_BigNumber_ThrowsException()
        {
            var stringCalculator = new StringCalculator();

            Action actual = () => stringCalculator.Add("1001");

            Assert.Throws<OverflowException>(actual);
        }
        // </SnippetMagicString>

        // <SnippetMultipleAsserts>
        [Fact]
        public void Add_EdgeCases_ThrowsCorrectExceptions()
        {
            Assert.Throws<ArgumentException>(() => stringCalculator.Add(null));
            Assert.Throws<OverflowException>(() => stringCalculator.Add("1001"));
        }
        // </SnippetMultipleAsserts>
    }
}
