using System;
using UnitTestingBestPractices;
using Xunit;

namespace UnitTestingBestPracticesBetterTests
{
    public class StringCalculatorTests
    {
        // <SnippetGoodTestCase>
        [Fact]
        public void Add_SingleNumber_ReturnsSameNumber()
        {
            var stringCalculator = new StringCalculator();

            var actual = stringCalculator.Add("0");

            Assert.Equal(0, actual);
        }
        // </SnippetGoodTestCase>

        [Fact]
        public void Add_TwoNumbers_ReturnsSumOfNumbers()
        {
            var stringCalculator = new StringCalculator();

            var actual = stringCalculator.Add("0,1");

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Add_MultipleNumbers_ReturnsSumOfNumbers()
        {
            var stringCalculator = new StringCalculator();

            var actual = stringCalculator.Add("0,1,2");

            Assert.Equal(3, actual);
        }

        // <SnippetNoMagicStrings>
        [Fact]
        void Add_MaximumSumResult_ThrowsOverflowException()
        {
            var stringCalculator = new StringCalculator();
            const string MAXIMUM_RESULT = "1001";

            Action actual = () => stringCalculator.Add(MAXIMUM_RESULT);

            Assert.Throws<OverflowException>(actual);
        }
        // </SnippetNoMagicStrings>

        // <SnippetNoMultipleAsserts>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Add_NullOrEmpty_ThrowsArgumentException(string input)
        {
            var stringCalculator = new StringCalculator();

            Action actual = () => stringCalculator.Add(input);

            Assert.Throws<ArgumentException>(actual);
        }
        // </SnippetNoMultipleAsserts>
    }
}
