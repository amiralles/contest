namespace Contest.Test {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class DiscoverFixture {

        [Test]
        public void TestCasesInAssm() {
            var cases = TestCaseFinder.FindCasesInAssm(typeof(TestClass).Assembly);
            Assert.AreEqual(7, cases.Count);
        }

        [Test]
        public void TestCasesInClass() {
            var cases = TestCaseFinder.FindCases(typeof(TestClass));
            Assert.AreEqual(2, cases.Count);
        }

        [Test]
        public void TestCasesInNestedClasses() {
            var cases = TestCaseFinder.FindCases(typeof(Wrapper));
            Assert.AreEqual(1, cases.Count);
        }

    }
}