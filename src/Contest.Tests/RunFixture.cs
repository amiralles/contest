namespace Contest.Test {
    using Core;
	using _ = System.Action<Contest.Core.Runner>;

    public class RunFixture {
		//TODO: Add wildcards to run some tests, exlude others, etc...
		//TODO: Add the option to re-run failing tests.
        _ run_test_suite = test =>{
            var cases = TestCaseFinder.FindCases(typeof(TestClassOnePassOnFail),null);
            var runner = new Runner();
            runner.Run(cases);

            test.Assert(1 ==  runner.PassCount, "Fail PassCount");
            test.Assert(1 ==  runner.FailCount, "Fail FailCount");
            test.Assert(2 ==  runner.AssertsCount, "Fail AssertsCount");
        };

        _ run_throwing_test_suite  = test => {
            var cases = TestCaseFinder.FindCases(typeof(TestClassThrowingTests),null);
            var runner = new Runner();
            runner.Run(cases);

            test.Assert(1 == runner.PassCount, "Fail PassCount");
            test.Assert(1 == runner.FailCount, "Fail FailCount");
            test.Assert(2 == runner.AssertsCount, "Fail AssertsCount");
        };
    }
}
