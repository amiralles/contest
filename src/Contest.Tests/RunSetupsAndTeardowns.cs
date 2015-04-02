namespace Contest.Test {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class RunSetupAndTeardown {
        readonly TestCaseFinder _finder = new TestCaseFinder();

        [Test]
        public void shold_run_setup_before_case_unless_setup_is_null() {
			var tcase = new TestCase();
			var wassCalled = false;
			tcase.BeforeCase = runner => { wassCalled = true; };
			tcase.Body       = runner => { };
			tcase.Run();
			Assert.IsTrue(wassCalled, "It shouldn't called setup before running the case.");
        }

    }
}
