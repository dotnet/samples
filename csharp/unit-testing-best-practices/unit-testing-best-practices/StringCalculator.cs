using System;

namespace UnitTestingBestPractices
{
    public class StringCalculator
    {
        private const int MAXIMUM_RESULT = 1000;

        public int Add(string numbers)
        {
            if (string.IsNullOrEmpty(numbers))
            {
                throw new ArgumentException(nameof(numbers));
            }

            int result;
            if (numbers.Contains(","))
            {
                result = HandleMultipleNumbers(numbers);
            }
            else
            {
                result = HandleSingleNumber(numbers);
            }

            return ValidateResult(result);
        }

        private static int HandleMultipleNumbers(string numbers)
        {
            var numbersArray = numbers.Split(',');
            var sum = 0;

            foreach (var number in numbersArray)
            {
                sum += HandleSingleNumber(number);
            }

            return ValidateResult(sum);
        }

        private static int HandleSingleNumber(string number)
        {
            return int.Parse(number);
        }

        private static int ValidateResult(int sum)
        {
            if (sum > MAXIMUM_RESULT)
            {
                throw new OverflowException();
            }

            return sum;
        }
    }
}
