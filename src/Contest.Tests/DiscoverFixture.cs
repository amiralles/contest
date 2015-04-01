
namespace Contest.Test {
    using System.Linq;
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

		//before test cases
        [Test]
        public void before_test_cases_in_assm() {
            var cases = Contest.FindCasesInAssm(_finder, typeof(FooTest).Assembly, null);
            var casesWithSetup = (from c in cases
                                  where c.BeforeCase.Method.MetadataToken 
										!= TestCase.DefaultBeforeCase.Method.MetadataToken
                                  select c).ToList();

            Assert.AreEqual(2, casesWithSetup.Count);
        }

        [Test]
        public void before_test_cases_in_class() {
            var cases = Contest.FindCases(_finder, typeof(FooTest), null);
            Assert.AreEqual(1, Contest.FindBeforeCases(cases).Count);
        }

        [Test]
        public void before_test_cases_in_nested_class() {
            var cases = Contest.FindCases(_finder, typeof(BarTest), null);
            Assert.AreEqual(1, Contest.FindBeforeCases(cases).Count);
        }
    }
}
