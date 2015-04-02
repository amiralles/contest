namespace Contest.Test {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class RunSetupAndTeardown {

        [Test]
        public void should_run_setup_before_case_unless_setup_is_null() {
			var tcase = new TestCase();
			var wasCalled = false;
			tcase.BeforeCase = runner => { wasCalled = true; };
			tcase.Body       = runner => { };
			tcase.Run(new Runner());
			Assert.IsTrue(wasCalled, "It shouldn't called setup before running the case.");
        }

        [Test]
        public void should_run_teardown_after_case_unless_teardown_is_null() {
			var tcase = new TestCase();
			var wasCalled = false;
			tcase.AfterCase = runner => { wasCalled = true; };
			tcase.Body       = runner => { };
			tcase.Run(new Runner());
			Assert.IsTrue(wasCalled, "It shouldn't called setup before running the case.");
        }

    }
}
