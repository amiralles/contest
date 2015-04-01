
namespace Contest.Test {
    using System.Linq;
    using NUnit.Framework;
    using Core;

    [TestFixture]
    public class DiscoverFixture {
        readonly TestCaseFinder _finder = new TestCaseFinder();

        [Test]
        public void test_cases_in_assm() {
            var cases = Contest.FindCasesInAssm(_finder,typeof(TestClass).Assembly, null).Cases;
            Assert.AreEqual(7, cases.Count);
        }

        [Test]
        public void test_cases_in_class() {
            var cases = Contest.FindCases(_finder, typeof(TestClass), null).Cases;
            Assert.AreEqual(2, cases.Count);
        }

        [Test]
        public void test_cases_in_nested_classes() {
            var cases = Contest.FindCases(_finder, typeof(Wrapper), null).Cases;
            Assert.AreEqual(1, cases.Count);
        }

		//before test cases
        [Test]
        public void before_test_cases_in_assm() {
            var suite = Contest.FindCasesInAssm(_finder, typeof(FooTest).Assembly, null);
            Assert.AreEqual(2, suite.Stats.BeforeCases.Count);
        }

        [Test]
        public void before_test_cases_in_class() {
            var suite = Contest.FindCases(_finder, typeof(FooTest), null);
            Assert.AreEqual(2, suite.Stats.BeforeCases.Count);
        }

        [Test]
        public void before_test_cases_in_nested_class() {
            var suite = Contest.FindCases(_finder, typeof(BarTest), null);
            Assert.AreEqual(2, suite.Stats.BeforeCases.Count);
        }
    }
}
