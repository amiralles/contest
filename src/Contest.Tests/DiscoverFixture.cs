
namespace Contest.Test {
    using NUnit.Framework;
    using Core;

    [TestFixture]
    public class DiscoverFixture {
        readonly TestCaseFinder _finder = new TestCaseFinder();

        [Test]
        public void test_cases_in_assm() {
            var cases = Contest.FindCasesInAssm(_finder,typeof(TestClass).Assembly, null);
            Assert.AreEqual(7, cases.Count);
        }

        [Test]
        public void test_cases_in_class() {
            var cases = Contest.FindCases(_finder, typeof(TestClass), null);
            Assert.AreEqual(2, cases.Count);
        }

        [Test]
        public void test_cases_in_nested_classes() {
            var cases = Contest.FindCases(_finder, typeof(Wrapper), null);
            Assert.AreEqual(1, cases.Count);
        }


    }
}
