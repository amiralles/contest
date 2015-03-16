namespace Contest.Test {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class RunFixture {
		//TODO: Add wildcards to run some tests, exlude others, etc...
		//TODO: Add the option to re-run failing tests.
        [Test]
        public void RunTestSuite() {
            var cases = TestCaseFinder.FindCases(typeof(TestClassOnePassOnFail));
            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(1, runner.PassCount);
            Assert.AreEqual(1, runner.FailCount);
            Assert.AreEqual(2, runner.AssertsCount);
        }
        
        [Test]
        public void RunThrowingTestSuite() {
            var cases = TestCaseFinder.FindCases(typeof(TestClassThrowingTests));
            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(1, runner.PassCount);
            Assert.AreEqual(1, runner.FailCount);
            Assert.AreEqual(2, runner.AssertsCount);
        }
    }
}