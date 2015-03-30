
namespace Contest.Test {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class RunFixture {
        //TODO: Add wildcards to run some tests, exlude others, etc...
        //TODO: Add the option to re-run failing tests.
        readonly TestCaseFinder _finder = new TestCaseFinder();

        [Test]
        public void run_test_suite() {
            var cases = Contest.FindCases(_finder, typeof(TestClassOnePassOnFail), null);
            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(1, runner.PassCount, "Fail PassCount");
            Assert.AreEqual(1, runner.FailCount, "Fail FailCount");
            Assert.AreEqual(2, runner.AssertsCount, "Fail AssertsCount");
        }

        [Test]
        public void run_throwing_test_suite() {
            var cases = Contest.FindCases(_finder, typeof(TestClassThrowingTests), null);
            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(1, runner.PassCount, "Fail PassCount");
            Assert.AreEqual(1, runner.FailCount, "Fail FailCount");
            Assert.AreEqual(2, runner.AssertsCount, "Fail AssertsCount");
        }
    }
}
