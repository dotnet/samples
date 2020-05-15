using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTest.Project
{
    [TestClass]
    public class ByPriorityOrder
    {
        public static bool Test1Called;
        public static bool Test2ACalled;
        public static bool Test2BCalled;
        public static bool Test3Called;

        [TestMethod, Priority(5)]
        public void Test3()
        {
            Test3Called = true;

            Assert.IsTrue(Test1Called);
            Assert.IsTrue(Test2ACalled);
            Assert.IsTrue(Test2BCalled);
        }

        [TestMethod, Priority(0)]
        public void Test2B()
        {
            Test2BCalled = true;

            Assert.IsTrue(Test1Called);
            Assert.IsTrue(Test2ACalled);
            Assert.IsFalse(Test3Called);
        }

        [TestMethod]
        public void Test2A()
        {
            Test2ACalled = true;

            Assert.IsTrue(Test1Called);
            Assert.IsFalse(Test2BCalled);
            Assert.IsFalse(Test3Called);
        }

        [TestMethod, Priority(-5)]
        public void Test1()
        {
            Test1Called = true;

            Assert.IsFalse(Test2ACalled);
            Assert.IsFalse(Test2BCalled);
            Assert.IsFalse(Test3Called);
        }
    }
}