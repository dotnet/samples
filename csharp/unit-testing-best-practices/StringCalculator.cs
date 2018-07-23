using System;

namespace UnitTestingBestPractices
{
    public class StringCalculator
    {
        private const int MAXIMUM_RESULT = 1000;

        public int Add(string numbers)
        {
            if (numbers == null)
            {
                throw new ArgumentNullException(nameof(numbers));
            }

            if (numbers.Contains(","))
            {
                return HandleMultipleNumbers(numbers);
            }

            return HandleSingleNumber(numbers);
        }

        private static int HandleMultipleNumbers(string numbers)
        {
            var numbersArray = numbers.Split(',');
            var sum = 0;

            foreach (var number in numbersArray)
            {
                sum += HandleSingleNumber(number);
            }

            ValidateResult(sum);

            return sum;
        }

        private static int HandleSingleNumber(string number)
        {
            return int.Parse(number);
        }

        private static void ValidateResult(int sum)
        {
            if (sum > MAXIMUM_RESULT)
            {
                throw new OverflowException();
            }
        }
    }
}
