
namespace Contest.Tests {
    using System;
    using System.Linq;
    using Contest.Core;
    using _ = System.Action<Contest.Core.Runner>;

    class contest_core_tests {

		// For cherry picking tests.
        static readonly TestCaseFinder _cerryPeekingFinder = new TestCaseFinder(null, 
				t => typeof(contest_core_tests) == t);

		// For discovery tests.
        static readonly TestCaseFinder _discoveryFinder = new TestCaseFinder();

        _ discover_test_cases_in_assm = assert => {
            var cases = Contest.GetCasesInAssm(_discoveryFinder, typeof(TestClass).Assembly, null).Cases;
            assert.Equal(10, cases.Count);
        };

        _ discover_test_cases_in_class = assert => {
            var cases = Contest.FindCases(_discoveryFinder, typeof(TestClass), null).Cases;
            assert.Equal(2, cases.Count);
        };

        _ discover_test_cases_in_nested_classes = assert => {
            var cases = Contest.FindCases(_discoveryFinder, typeof(Wrapper), null).Cases;
            assert.Equal(1, cases.Count);
        };

        _ dicsover_before_test_cases_in_assm = assert => {
            var suite = Contest.GetCasesInAssm(_discoveryFinder, typeof(FooTest).Assembly, null);
			var casesWithSetup = (from c in suite.Cases
								 where c.BeforeCase != null
								 select c).Count();

            assert.Equal(3, casesWithSetup);
        };

        _ discover_after_test_cases_in_class = assert => {
            var suite = Contest.GetCasesInAssm(_discoveryFinder, typeof(FooTest).Assembly, null);
			var casesWithTeardown = (from c in suite.Cases
								 where c.AfterCase != null
								 select c).Count();

            assert.Equal(3, casesWithTeardown);
        };


		// This is the common case.
        _ cherry_pick_cases_containing = assert => {
            var cases = Contest.FindCases(_cerryPeekingFinder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "*ThisIsAn*");

            assert.Equal(2, runner.TestCount, "Fail TestCount");
            assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
        };


        _ cherry_pick_cases_ending_with = assert => {
            var cases = Contest.FindCases(_cerryPeekingFinder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "*ThisIsAnotherTest");

            assert.Equal(2, runner.TestCount, "Fail TestCount");
            assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
        };


        _ cherry_pick_cases_starting_with = assert => {
            var cases = Contest.FindCases(_cerryPeekingFinder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "Contest.Tests.TestClass.ThisIsAn*");

            assert.Equal(2, runner.TestCount, "Fail TestCount");
            assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
        };


		_ read_empty_ignore_file = assert => {
			IgnoreFileReader.ReadAllLines = () => null;
			assert.Equal(new string[0], new TestCaseFinder().GetIgnoredPatternsFromFile());
		};


		_ read_ignore_file_comma_sep_values = assert => {
			IgnoreFileReader.ReadAllLines = () =>
				new[] { "*foo, *bar*, test*" };

			assert.Equal(3, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
		};


		_ read_ignore_file_colon_sep_values = assert => {
			IgnoreFileReader.ReadAllLines = () =>
				new[] { "*foo; *bar*; test*" };

			assert.Equal(3, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
		};


		_ read_ignore_file_space_sep_values = assert => {
			IgnoreFileReader.ReadAllLines = () =>
				new[] { "*foo *bar* test*" };

			assert.Equal(3, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
		};


		_ read_ignore_file_newline_sep_values = assert => {
			IgnoreFileReader.ReadAllLines = () =>
				"*foo\n *bar*\n test*\n".Split('\n');

			assert.Equal(3, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
		};


		_ read_ignore_file_should_trim_white_space = assert => {
			IgnoreFileReader.ReadAllLines = () =>
				new[] { "  *foo,    *bar*,    test*  " };
			var patterns = new TestCaseFinder().GetIgnoredPatternsFromFile();
			assert.Equal(0, patterns.Count(p => p.Contains(' ')));
		};


		_ ignore_file_ignore_all_cases = assert => {
			var cases = Contest.FindCases(
					new TestCaseFinder(getIgnoredFromFile: () => new[] { "*" }),
					typeof(TestClass),
					null);

			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(2, runner.IgnoreCount, "Fail IgnoreCount");
		};


		_ ignore_file_ignore_cases_ending_with = assert => {

			var cases = Contest.FindCases(
					new TestCaseFinder(() => new[] { "*AnotherTest" }),
					typeof(TestClass),
					null);

			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
		};


		_ ignore_file_ignore_cases_starting_with = assert => {

			var cases = Contest.FindCases(
					new TestCaseFinder(getIgnoredFromFile: () => new[] {
						"Contest.Tests.TestClass.ThisIsAT*"
						}),
					typeof(TestClass),
					null);

			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");

		};


		_ ignore_file_ignore_cases_when_contains = assert => {
			var cases = Contest.FindCases(
					new TestCaseFinder(getIgnoredFromFile: () => new[] { "*Tes*" }),
					typeof(TestClass),
					null);

			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(2, runner.IgnoreCount, "Fail IgnoreCount");

		};


		_ ignore_file_ignore_no_cases = assert => {
			var cases = Contest.FindCases(
					new TestCaseFinder(getIgnoredFromFile: () => null),
					typeof(TestClass),
					null);

			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(0, runner.IgnoreCount, "Fail IgnoreCount");
		};


		_ from_cmd_line_ignore_all_cases = assert => {
			var cases = Contest.FindCases(
					new TestCaseFinder(),
					typeof(TestClass),
					"*");

			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(2, runner.IgnoreCount, "Fail IgnoreCount");
		};


		_ from_cmd_line_ignore_cases_ending_with = assert => {
			var cases = Contest.FindCases(
					new TestCaseFinder(),
					typeof(TestClass),
					"*AnotherTest");

			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
		};


		_ from_cmd_line_ignore_cases_starting_with = assert => {
			var cases = Contest.FindCases(
					new TestCaseFinder(),
					typeof(TestClass),
					"Contest.Tests.TestClass.ThisIsAT*");

			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
		};

		
		_ from_cmd_line_ignore_cases_when_contains = assert => {
			var cases = Contest.FindCases(new TestCaseFinder(), typeof(TestClass), "*Tes*");
			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(2, runner.IgnoreCount, "Fail IgnoreCount");

		};


		_ from_cmd_line_ignore_no_cases = assert => {
			var cases = Contest.FindCases(new TestCaseFinder(), typeof(TestClass), null);
			var runner = new Runner();
			runner.Run(cases);

			assert.Equal(2, runner.TestCount, "Fail TestCount");
			assert.Equal(0, runner.IgnoreCount, "Fail IgnoreCount");
		};

		// Utils
		_ utils_match_cherry_picking_pattern = assert =>
            assert.IsTrue("*ThisIsAn*".Match("ThisIsAnotherTest"));

		// Runner
        _ run_test_suite = assert => {
            var cases = Contest.FindCases(new TestCaseFinder(), typeof(TestClassOnePassOnFail), null);
            var runner = new Runner();
            runner.Run(cases);

            assert.Equal(1, runner.PassCount, "Fail PassCount");
            assert.Equal(1, runner.FailCount, "Fail FailCount");
            assert.Equal(2, runner.AssertsCount, "Fail AssertsCount");
        };

        _ run_throwing_test_suite = assert => {
            var cases = Contest.FindCases(new TestCaseFinder(), typeof(TestClassThrowingTests), null);
            var runner = new Runner();
            runner.Run(cases);

            assert.Equal(1, runner.PassCount, "Fail PassCount");
            assert.Equal(1, runner.FailCount, "Fail FailCount");
            assert.Equal(2, runner.AssertsCount, "Fail AssertsCount");
        };

        _ runner_assertMethod_ResultIsTrue = assert => {
            var runner = new Runner();
			runner.Assert(1 == 1);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.PassCount);
        };

        _ runner_assertMethod_ResultIsFalse = assert => {
            var runner = new Runner();
            runner.Assert(1 == 2);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.FailCount);
        };


        _ runner_assertEqualsMethod_NullValue_ResultIsTrue = assert => {
            var runner = new Runner();
            runner.Equal(null,null);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.PassCount);
		};

        _ runner_assertEqualsMethod_NullValue_ResultIsFalse = assert => {
            var runner = new Runner();
            runner.Equal(null,123);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.FailCount);
		};

        _ runner_assertEqualsMethod_ResultIsTrue = assert => {
            var runner = new Runner();
            runner.Equal(1,1);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.PassCount);
		};

        _ runner_assertEqualsMethod_ResultIsFalse = assert => {
            var runner = new Runner();
            runner.Equal(1,2);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.FailCount);
        };

        _ runner_assertNotEqualsMethod_ResultIsTrue = assert => {
            var runner = new Runner();
            runner.NotEqual(1,2);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.PassCount);
		};

        _ runner_assertNotEqualsMethod_ResultIsFalse = assert => {
            var runner = new Runner();
            runner.NotEqual(1,1);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.FailCount);
        };

        _ runner_assertIsNull_ResultIsTrue = assert => {
            var runner = new Runner();
            runner.IsNull(null);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.PassCount);
        };

        _ runner_assertIsNull_ResultIsFalse = assert => {
            var runner = new Runner();
            runner.IsNull(123);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.FailCount);
        };

        _ runner_assertIsNotNull_ResultIsTrue = assert => {
            var runner = new Runner();
            runner.IsNotNull(123);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.PassCount);
        };

        _ runner_assertIsNotNull_ResultIsFalse = assert => {
            var runner = new Runner();
            runner.IsNotNull(null);

            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.FailCount);
        };

        _ should_run_setup_before_case_unless_setup_is_null = assert => {
			var tcase = new TestCase();
			var wasCalled = false;
			tcase.BeforeCase = runner => { wasCalled = true; };
			tcase.Body       = runner => { };
			tcase.Run(new Runner());
			assert.IsTrue(wasCalled, "It shouldn't called setup before running the case.");
        };

        _ should_run_teardown_after_case_unless_teardown_is_null = assert => {
			var tcase = new TestCase();
			var wasCalled = false;
			tcase.AfterCase = runner => { wasCalled = true; };
			tcase.Body       = runner => { };
			tcase.Run(new Runner());
			assert.IsTrue(wasCalled, "It shouldn't called setup before running the case.");
        };


        _ bag_ConfigureTestVariablesDuringSetup = assert => {
            var runner = new Runner();
            var finder = new TestCaseFinder();
            var suite = Contest.GetCasesInAssm(finder, typeof(EchoTest).Assembly, null);

			//In this particular case I only care about the echo test.
            var cases = (from c in suite.Cases
                         where c.Name == "echo"
                         select c).ToList();

            runner.Run(cases);
            assert.Equal(1, runner.AssertsCount);
            assert.Equal(1, runner.PassCount);
        };

	}
}
