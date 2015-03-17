
namespace Contest.Test {
    using Core;
    using _ = System.Action<Contest.Core.Runner>;

    public class IgnoreFixture{

        _ ignore_all_cases = test => {

            var cases = TestCaseFinder.FindCases(typeof(TestClass), "*");
            var runner = new Runner();
            runner.Run(cases);

            test.Assert(2 ==  runner.TestCount,   "Fail TestCount");
            test.Assert(2 ==  runner.IgnoreCount, "Fail IgnoreCount");

        };

        _ ignore_cases_starting_with = test => {

            var cases = TestCaseFinder.FindCases(typeof(TestClass), "This*");
            var runner = new Runner();
            runner.Run(cases);

            test.Assert(2 ==  runner.TestCount,   "Fail TestCount");
            test.Assert(1 ==  runner.IgnoreCount, "Fail IgnoreCount");

        };

        _ ignore_cases_ending_with = test => {

            var cases = TestCaseFinder.FindCases(typeof(TestClass), "*AnotherTest");
            var runner = new Runner();
            runner.Run(cases);

            test.Assert(2 ==  runner.TestCount,   "Fail TestCount");
            test.Assert(1 ==  runner.IgnoreCount, "Fail IgnoreCount");

        };

        _ ignore_cases_when_contains = test => {

            var cases = TestCaseFinder.FindCases(typeof(TestClass), "*Tes*");
            var runner = new Runner();
            runner.Run(cases);

            test.Assert(2 ==  runner.TestCount,   "Fail TestCount");
            test.Assert(2 ==  runner.IgnoreCount, "Fail IgnoreCount");

        };

        _ ignore_no_cases = test => {

            var cases = TestCaseFinder.FindCases(typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases);

            test.Assert(2 ==  runner.TestCount,   "Fail TestCount");
            test.Assert(0 ==  runner.IgnoreCount, "Fail IgnoreCount");

        };

    }
}
