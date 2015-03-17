
namespace Contest.Test {
    using NUnit.Framework;
    using Core;

    [TestFixture]
    public class DiscoverFixture {

        [Test]
        public void test_cases_in_assm() {
            var cases = TestCaseFinder.FindCasesInAssm(typeof(TestClass).Assembly, null);
            Assert.AreEqual(7, cases.Count);
        }

        [Test]
        public void test_cases_in_class() {
            var cases = TestCaseFinder.FindCases(typeof(TestClass), null);
            Assert.AreEqual(2, cases.Count);
        }

        [Test]
        public void test_cases_in_nested_classes() {
            var cases = TestCaseFinder.FindCases(typeof(Wrapper), null);
            Assert.AreEqual(1, cases.Count);
        }


    }
}
