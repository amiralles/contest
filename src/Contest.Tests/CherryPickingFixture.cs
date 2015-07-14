namespace Contest.Tests {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class CherryPickingFixture {
        readonly TestCaseFinder _finder = new TestCaseFinder();

        //This is the most common case into the wild.
        [Test]
        public void run_only_cases_containing() {
            var cases = Contest.FindCases(_finder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "*ThisIsAn*");

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
        }

        [Test]
        public void run_only_cases_ending_with() {
            var cases = Contest.FindCases(_finder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "*ThisIsAnotherTest");

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
        }

        [Test]
        public void run_only_cases_starting_with() {
            var cases = Contest.FindCases(_finder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "Contest.Tests.TestClass.ThisIsAn*");

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
        }
    }
}
