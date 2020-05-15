using NUnit.Framework;
using System.Collections.Generic;

namespace NUnit.Project
{
    public class BySequentialParamOrder
    {
        private readonly Queue<int> _testCalls = new Queue<int>();

        public BySequentialParamOrder()
        {
            for (var i = 1; i < 6; ++i)
            {
                _testCalls.Enqueue(i * 3);
            }
        }

        [Test, Sequential]
        public void Test([Values(3, 6, 9, 12, 15)] int number) =>
            Assert.AreEqual(_testCalls.Dequeue(), number);
    }
}