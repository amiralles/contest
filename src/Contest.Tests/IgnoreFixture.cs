
namespace Contest.Test {
    using NUnit.Framework;
    using Core;

    [TestFixture]
    public class IgnoreFixture {

        [Test]
        public void ignore_all_cases() {
            var cases = TestCaseFinder.FindCases(typeof(TestClass), "*");
            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(2, runner.IgnoreCount, "Fail IgnoreCount");
        }

        [Test]
        public void ignore_cases_ending_with() {
            var cases = TestCaseFinder.FindCases(typeof(TestClass), "*AnotherTest");
            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");

        }


        [Test]
        public void ignore_cases_starting_with() {

            var cases = TestCaseFinder.FindCases(typeof(TestClass),
                "Contest.Test.TestClass.ThisIsAT*");

            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");

        }
        [Test]
        public void ignore_cases_when_contains() {

            var cases = TestCaseFinder.FindCases(typeof(TestClass), "*Tes*");
            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(2, runner.IgnoreCount, "Fail IgnoreCount");

        }

        [Test]
        public void ignore_no_cases() {

            var cases = TestCaseFinder.FindCases(typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases);

            Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
            Assert.AreEqual(0, runner.IgnoreCount, "Fail IgnoreCount");
        }

    }
}