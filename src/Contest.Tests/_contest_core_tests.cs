// ReSharper disable UnusedMember.Local
#pragma warning disable 414 
namespace Contest.Tests {
    using System;
    using System.Linq;
    using Core;
    using static Core.SyntaxSugar;
    using static Core.Chatty;
    using static Core.BDD;
    using _ = System.Action<Core.Runner>;


	//TODO: Remove duplicate code from Contest.Run.Tests (Is almost identical to main Program.)
    public class contest_core_tests {

		// For cherry picking tests.
        static readonly TestCaseFinder _cherryPickingFinder = new TestCaseFinder(null, 
				t => typeof(contest_core_tests) == t);

		// For discovery tests.
        static readonly TestCaseFinder _discoveryFinder = new TestCaseFinder(null,
				t => typeof(contest_core_tests) == t);


		// _ depends_on_init = assert => {
		// 	// If Global.Foo is not initialized, this will throw.
		// 	var foo = Global.Foo.ToString();
		// 	assert.Pass();
		// };
        //

        // Oneliners syntax.
        _ assert_that_is          = assert => That(123).Is(123);
        _ assert_that_is_not      = assert => That(123).IsNot(456);
        _ assert_that_is_null     = assert => That(null).IsNull();
        _ assert_that_is_not_null = assert => That(123).IsNotNull();
        _ assert_that_is_true     = assert => That(true).IsTrue();
        _ assert_that_is_false    = assert => That(false).IsFalse();


        // BDD syntax.
        _ expect_123_to_be_123           = assert => Expect(123).ToBe(123);
        _ expect_123_not_to_be_456       = assert => Expect(123).NotToBe(456);
        _ expect_null_to_be_null         = assert => Expect(null).ToBe(null);
        _ expect_not_null_to_be_not_null = assert => Expect(123).NotToBe(null);
        _ expect_true_to_be_true         = assert => Expect(true).ToBe(true);
        _ expect_false_to_be_false       = assert => Expect(false).ToBe(false);
        _ expect_to_throw_null_reference = assert => { 
			object obj = null;
			// ReSharper disable once PossibleNullReferenceException
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			Expect(() => obj.ToString()).ToThrow<NullReferenceException>();
		};

		//TODO: Does it worth to add the Not operation?
		//      It will save us some code. Instead of implementing a
		//      negation method per comparison, we can just do:
		//      Not(GreaterThan), Not(LessThan) and so on....
		//
		_ less_than           = assert => Expect(111).ToBeLessThan(123);
		_ less_than_or_eq     = assert => Expect(111).ToBeLessThanOrEqual(123);
		_ less_than_or_eq1    = assert => Expect(111).ToBeLessThanOrEqual(111);

		_ greater_than        = assert => Expect(123).ToBeGreaterThan(111);
		_ greater_than_or_eq  = assert => Expect(123).ToBeGreaterThanOrEqual(111);
		_ greater_than_or_eq1 = assert => Expect(123).ToBeGreaterThanOrEqual(123);


        // BDD (alt) syntax.
		_ expect_2_plus_2_to_be_4     = expect => (2+2).ToBe(4);
		_ expect_2_plus_2_not_to_be_5 = expect => (2+2).NotToBe(5);

		_ less_than_alt           =  expect => 111.ToBeLessThan(123);
		_ less_than_or_eq_alt     =  expect => 111.ToBeLessThanOrEqual(123);
		_ less_than_or_eq1_alt    =  expect => 111.ToBeLessThanOrEqual(111);

		_ greater_than_alt        =  expect => 123.ToBeGreaterThan(111);
		_ greater_than_or_eq_alt  =  expect => 123.ToBeGreaterThanOrEqual(111);
		_ greater_than_or_eq1_alt =  expect => 123.ToBeGreaterThanOrEqual(123);

        _ expect_err_msg = assert => {
			object obj = null;
			// ReSharper disable once PossibleNullReferenceException
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			Expect(() => obj.ToString()).ErrMsg("Object reference not set to an instance of an object");
		};

        _ expect_err_msg_contains = assert => {
			object obj = null;
			// ReSharper disable once PossibleNullReferenceException
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			Expect(() => obj.ToString()).ErrMsgContains("reference not set");
		};

		// Assertions syntax sugar
		_ assert_is_null_sugar     = assert => IsNull(null);

		_ assert_is_not_null_sugar = assert => IsNotNull(new {});

		_ assert_is_true_sugar     = assert => IsTrue(true);

		_ assert_is_false_sugar    = assert => IsFalse(false);

		_ assert_equal_sugar       = assert => Equal(2, 2);

		_ assert_not_equal_sugar   = assert => NotEqual(2, 3);

		_ assert_sugar             = assert => Assert(true);

		_ should_throw_sugar       = assert => 
			ShouldThrow<NullReferenceException>(() => {
				string str = null;
				// ReSharper disable once PossibleNullReferenceException
				// ReSharper disable once UnusedVariable
				int    len = str.Length;	
			});
		
		class DisposableClass : IDisposable {
			public bool Disposed;

			public void Dispose() {
				Disposed = true;
			}
		}

		class DisposableClassThrows : IDisposable {
			public bool Disposed = false;

			public void Dispose() {
				throw new Exception("Intentional exception.");
			}
		}

		_ should_call_dispose_on_disposable_tests_when_shutting_down_contest =
			assert => {
				var disp = new DisposableClass();
				Contest.Disposables.Add(disp);
				Contest.Shutdown();

				assert.IsTrue(disp.Disposed);
			};

		_ should_dispose_each_disposable_test =
			assert => {
				Contest.Disposables.Clear();
				Contest.ResetCounters();

				var disp1 = new DisposableClass();
				var disp2 = new DisposableClassThrows();
				var disp3 = new DisposableClass();
				Contest.Disposables.Add(disp1);
				Contest.Disposables.Add(disp2);
				Contest.Disposables.Add(disp3);

				Contest.Shutdown();
				assert.Equal(3, Contest.DisposedCount);
				assert.Equal(1, Contest.DisposeErrCount);
			};

		// ==============================================================================================
		// Assm level setups.
		// ==============================================================================================
		_ get_all_types = assert =>
			assert.IsTrue(Contest.GetAllTypes(typeof(ContestClose).Assembly).Length >= 1);

		_ get_init_callback = assert => {
			var setup = Contest.GetInitCallbackOrNull(typeof(ContestInit).Assembly);
			assert.IsNotNull(setup);
		};

		_ invoke_init_callback = assert => {
			var setup = Contest.GetInitCallbackOrNull(typeof(ContestInit).Assembly);
			setup(assert);// <= See ContestInit.Setup
			assert.Equal("Rock or Bust!", assert.Bag["legend"]);

		};

		_ find_assm_level_setup = assert => {
			assert.IsNotNull(Contest.GetSingleOrNullAssmLevelSpecialType(
						new [] { typeof(ContestInit) },
						lookInit: true));
		};

		_ find_assm_level_setup_returns_null_when_there_is_no_ContestInit = assert => {
			assert.IsNull(Contest.GetSingleOrNullAssmLevelSpecialType(
						new [] { typeof(string) },
						lookInit: true));
		};

		_ find_assm_level_setup_throws_if_gets_more_than_one = assert =>
			assert.ErrMsgContains("more than one", () => {
					Contest.GetSingleOrNullAssmLevelSpecialType(
							new [] { typeof(ContestInit), typeof(ContestInit)},
							lookInit: true);
					});
			
		// ==============================================================================================
		// Assm level teardown
		// ==============================================================================================
		_ get_all_types_close = assert =>
			assert.IsTrue(Contest.GetAllTypes(typeof(ContestClose).Assembly).Length >= 1);

		_ get_shutdown_callback = assert => {
			var shutdown = Contest.GetShutdownCallbackOrNull(typeof(ContestClose).Assembly);
			assert.IsNotNull(shutdown);
		};

		_ invoke_shutdown_callback = assert => {
			var shutdown = Contest.GetShutdownCallbackOrNull(typeof(ContestClose).Assembly);
			shutdown(assert);// <= See ContestClose.Shutdown
			assert.Equal("We salute you.", assert.Bag["ClosingMsg"]);
		};

		_ find_assm_level_shutdown = assert => {
			assert.IsNotNull(Contest.GetSingleOrNullAssmLevelSpecialType(
						new [] { typeof(ContestClose) },
						lookInit: false));
		};

		_ find_assm_level_shutdown_returns_null_when_there_is_no_ContestClose = assert => {
			assert.IsNull(Contest.GetSingleOrNullAssmLevelSpecialType(
						new [] { typeof(string) },
						lookInit: false));
		};

		_ find_assm_level_shutdown_throws_if_gets_more_than_one = assert =>
			assert.ErrMsgContains("more than one", () => {
					Contest.GetSingleOrNullAssmLevelSpecialType(
							new [] { typeof(ContestClose), typeof(ContestClose)},
							lookInit: false);
					});
			
		// ==============================================================================================

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
            var cases = Contest.FindCases(_cherryPickingFinder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "*ThisIsAn*");

            assert.Equal(2, runner.TestCount, "Fail TestCount");
            assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
        };


        _ cherry_pick_cases_ending_with = assert => {
            var cases = Contest.FindCases(_cherryPickingFinder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "*ThisIsAnotherTest");

            assert.Equal(2, runner.TestCount, "Fail TestCount");
            assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
        };


        _ cherry_pick_cases_starting_with = assert => {
            var cases = Contest.FindCases(_cherryPickingFinder, typeof(TestClass), null);
            var runner = new Runner();
            runner.Run(cases, cherryPicking: "Contest.Tests.TestClass.ThisIsAn*");

            assert.Equal(2, runner.TestCount, "Fail TestCount");
            assert.Equal(1, runner.IgnoreCount, "Fail IgnoreCount");
        };


		_ read_empty_ignore_file = assert => {
			IgnoreFileReader.ReadAllLines = () => null;
			assert.Equal(0, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
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
            // ReSharper disable once EqualExpressionComparison
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
