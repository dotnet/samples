using NUnit.Framework;

namespace NUnit.Project
{
    public class ByOrder
    {
        public static bool Test1Called;
        public static bool Test2ACalled;
        public static bool Test2BCalled;
        public static bool Test3Called;

        [Test, Order(5)]
        public void Should_Run_Last_After_Other_Tests_And_Validate_Previous_Execution_State()
        {
            Test3Called = true;

            Assert.That(Test1Called);
            Assert.That(Test2ACalled is false);
            Assert.That(Test2BCalled);
        }

        [Test, Order(0)]
        public void Should_Run_Second_After_First_Test_And_Validate_Only_First_Has_Run()
        {
            Test2BCalled = true;

            Assert.That(Test1Called);
            Assert.That(Test2ACalled is false);
            Assert.That(Test3Called is false);
        }

        [Test]
        public void Should_Run_Third_When_No_Order_Is_Specified_And_All_Previous_Tests_Are_Called()
        {
            Test2ACalled = true;

            Assert.That(Test1Called);
            Assert.That(Test2BCalled);
            Assert.That(Test3Called);
        }

        [Test, Order(-5)]
        public void Should_Run_First_Before_All_Other_Tests_And_Ensure_No_Prior_Execution()
        {
            Test1Called = true;

            Assert.That(Test2ACalled is false);
            Assert.That(Test2BCalled is false);
            Assert.That(Test3Called is false);
        }
    }
}