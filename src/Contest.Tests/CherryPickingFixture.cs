namespace Contest.Test {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class CherryPickingFixture {

        //This is the most common case into the wild.
        [Test]
        public void run_only_cases_containing() {
            var cases = TestCaseFinder.FindCases(typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cpp: "*ThisIsAn*");

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
        }

        [Test]
        public void run_only_cases_ending_with() {
            var cases = TestCaseFinder.FindCases(typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cpp: "*ThisIsAnotherTest");

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
        }

        [Test]
        public void run_only_cases_starting_with() {
            var cases = TestCaseFinder.FindCases(typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cpp: "Contest.Test.TestClass.ThisIsAn*");

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
        }
    }
}