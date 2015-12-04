namespace Contest.Core {
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BF = System.Reflection.BindingFlags;
    using static System.Linq.Expressions.Expression;

    public class Contest {
        const BF
            INS_PUB = BF.Instance | BF.Public,
            INS_PRI = BF.Instance | BF.NonPublic,
            STA_PUB = BF.Static   | BF.Public,
            STA_PRI = BF.Static   | BF.NonPublic;

        const string
            BEFORE = "BEFORE_",
            AFTER = "AFTER_",
            BEFORE_EACH = "BEFORE_EACH",
            AFTER_EACH = "AFTER_EACH";

        static readonly BF[] Flags;

        static Contest() {
            Flags = new[] { INS_PUB, INS_PRI, STA_PUB, STA_PRI };
        }

		/// Return true if the test case points to an anonymous method.
        static readonly Func<TestCase, bool> IsInlineCase = tcase => tcase.Body == null;

		/// Returns true if both delegates points to the same method.
        static readonly Func<Delegate, Delegate, bool> SameMetaToken =
            (left, right) =>
                left != null
                && right != null
                && left.Method.MetadataToken == right.Method.MetadataToken;

        static readonly Func<TestCaseFinder, Assembly, string, TestSuite> BuildTestSuiteFromAssm =
            (finder, assm, ignorePatterns) => {
                var suite = new TestSuite();
                var cases =
                    from type in assm.GetTypes()
                    from c in FindCases(finder, type, ignorePatterns).Cases
                    select c;

                suite.Cases.AddRange(
                    from c in cases
                    where !suite.Cases.Any(
                        c1 => IsInlineCase(c) || SameMetaToken(c1.Body, c.Body))
                    select c);

                return suite;
            };

		public static void DieIf(bool cond, string errmsg) {
			if (cond)
				throw new Exception(errmsg);
		}


		public static void Die(string errmsg) {
			throw new Exception(errmsg);
		}

		public static string  GetTimeFileName(string assmFileName) {
			DieIf(assmFileName == null, "assmFileName can't be null.");
			return $"{Path.GetFileNameWithoutExtension(assmFileName)}.time";
		}

		public static string  GetFailFileName(string assmFileName) {
			DieIf(assmFileName == null, "assmFileName can't be null.");
			return $"{Path.GetFileNameWithoutExtension(assmFileName)}.fail";
		}

		// //Convention based.
		// class ContestInit { //<= Assembly level initialization.
		//     void Setup(runner) {
		//			//Init code here	
		//     }
		// }
		//
		// class ContestClose { //<= Assembly level cleanup.
		//     void Shutdown(runner) {
		//          //Cleanup code here.
		//     }
		// }
		//
		
		
		/// Returns all types from the given assembly.
		/// (Including nested and private types).
		public static Type[] GetAllTypes (Assembly assm) {
			return GetTypesR(assm.GetTypes());
		}

		static Type[] GetTypesR (Type[] types) {
			var res = new List<Type>();
			foreach(var t in types) {
				res.Add(t);

				var nested = GetTypesR(t.GetNestedTypes());
				if (nested.Length == 0)
					continue;
				res.AddRange(nested);
			}
			
			return res.ToArray();
		}

		// TODO: A really poorly named function....
		/// Checks if the array of types contains one and only one 'special type'
		/// that match the specified lookInit flag.
		/// (By Special types we mean types that Contest uses for assembly level setup/shutdown
		/// operations).
		/// If this method succed, the result is a type that can be instanciated and 
		/// used as is to create a global assm level init/close callback.
		public static Type GetSingleOrNullAssmLevelSpecialType(Type[] types, bool lookInit) {
			DieIf(types == null, $"{nameof(types)} can't be null");

			//TODO: Magic strings to consts.
			var name   = lookInit ? "ContestInit" : "ContestClose";
			var method = lookInit ? "Setup"       : "Shutdown";
			var res    = (from t in types
						 where t.Name == name
						 select t).ToArray();
			
			if (res.Length == 0)
				return null;

			if (res.Length == 1) {
				// Has the Setup method?
				// If not, Die("Should have Setup method");
				var argst = new [] { typeof(Runner) };
				var mi = res[0].GetMethod(method, argst);
				if (mi != null)
					return res[0];

				// Non public? try private.
				var flags = BF.NonPublic | BF.InvokeMethod | BF.Instance | BF.DeclaredOnly;
				mi = res[0].GetMethod(method, flags, null, argst, null);
				if (mi != null)
					return res[0];

				Die($"The class {name} exists, but it doesn't have the '{method}' method.");
			}


			var role = (lookInit ? "initializer" : "cleaner");
			Die($"Can't have more than one assembly level {role} and we have {res.Length}." + 
			    $"Please look for clases named {name} and keep just one of them.");

			return null;
		}

		public static Action<Runner> GetInitCallbackOrNull (Assembly assm) {
			Action<Runner> res = null;
			var t  = GetSingleOrNullAssmLevelSpecialType(GetAllTypes(assm), lookInit: true);
			if (t != null) {

				// At this point neither of these operations shuld fail.
				// (That's why we don't check anything).
				var instance = Activator.CreateInstance(t, true);
				var argst = new [] { typeof(Runner) };
				var mi       = t.GetMethod("Setup", argst);

				//No public, try private.
				if (mi == null) {
					var flags = BF.NonPublic | BF.InvokeMethod | BF.Instance | BF.DeclaredOnly;
					mi = t.GetMethod("Setup", flags, null, argst, null);
					DieIf(mi == null, "Internal error. Can't find 'Setup' method.");
				}

				// Expressions
				// Paramters
				var runnerP   = Parameter(typeof(Runner), "runner");
				// Body
				// contestInit.Setup(runner); <= Esta es la llamada que tenemos que generar.
				var contestInit = Constant(instance, t);
				var callSetup   = Call(contestInit, mi, new Expression[] { runnerP });
				var steps       = new Expression[] {
										contestInit,
										callSetup };
				var block       = Block(new [] { runnerP }, steps);

				res = Lambda<Action<Runner>>(block, runnerP).Compile();

			}

			return res;
		}

		public static Action<Runner> GetShutdownCallbackOrNull (Assembly assm) {
			return null;
		}

		/// Returns a suite of "actual" test cases from the given assembly.
		/// (Setups and Teardowns are NOT part of the result set).
        public static Func<TestCaseFinder, Assembly, string, TestSuite> GetCasesInAssm =
            (finder, assm, ignorePatterns) => {
                var rawsuite = BuildTestSuiteFromAssm(finder, assm, ignorePatterns);
                var setups = FindSetups(rawsuite);
                WireSetups(rawsuite, setups);

                var teardowns = FindTeardowns(rawsuite);
                WireTeardowns(rawsuite, teardowns);

                var actualcases = ActualCases(rawsuite, setups, teardowns);

                WireGlobalSetups(rawsuite, setups);
                WireGlobalTeardowns(rawsuite, teardowns);
                return actualcases;
            };

        internal static Func<TestCaseFinder, Type, BF, string, TestSuite>
            FindCasesInNestedTypes = (finder, type, flags, ignorePatterns) => {
                var result = new TestSuite();
                var nestedTypes = type.GetNestedTypes(flags);
                foreach (var ntype in nestedTypes)
                    result.Cases.AddRange(FindCases(finder, ntype, ignorePatterns).Cases);

                return result;
            };

        internal static Func<TestCaseFinder, Type, string, TestSuite> FindCases =
            (finder, type, ignorePatterns) => {

                var result = new TestSuite();
				if (finder.IgnoreType(type))
					return result;

                if (type.IsAbstract || type.ContainsGenericParameters || !type.HasDefaultCtor())
                    return result;

                object inst = null;
                try {
                    inst = Activator.CreateInstance(type, true);
                }
                catch {
                    Console.WriteLine("WARN: Couldn't create instance of '{0}'.", type.Name);
                    return result;
                }


                foreach (var flag in Flags) {
                    foreach (var fi in GetTestCases(type, flag)) {
                        var del = (Delegate)fi.GetValue(inst);
                        if (result.Cases.Any(tc => SameMetaToken(tc.Body, del)))
                            continue;

                        var tcfullname = "{0}.{1}".Interpol(type.FullName, fi.Name);
                        result.Cases.Add(
                            new TestCase {
                                FixName = type.FullName,
                                Name    = fi.Name,
                                Body    = (Action<Runner>)del,
                                Ignored = MatchIgnorePattern(finder, tcfullname, ignorePatterns),
                            });
                    }
                }

                var findNestedPublic = FindCasesInNestedTypes(
                        finder, type, BF.Public, ignorePatterns);

                var findNestedNonPublic = FindCasesInNestedTypes(
                        finder, type, BF.NonPublic, ignorePatterns);

                result.Cases.AddRange(findNestedPublic.Cases);
                result.Cases.AddRange(findNestedNonPublic.Cases);

                return result;
            };

        static readonly Func<Type, BF, List<FieldInfo>> GetTestCases =
            (type, flags) => (
                from   fi in type.GetFields(flags)
                where  fi.FieldType == typeof(Action<Runner>)
                select fi).ToList();

        static readonly Func<List<TestCase>,  TestCase> GetGlobalSetup = 
			(setups) => 
				setups.FirstOrDefault(t => t.Name.ToUpper() == BEFORE_EACH);

        static readonly Func<List<TestCase>,  TestCase> GetGlobalTearDown =
		   	(teardowns) => 
				teardowns.FirstOrDefault(t => t.Name.ToUpper() == AFTER_EACH);

        static readonly Action<TestSuite, List<TestCase>> WireGlobalSetups = (actual, setups) => {
            var gsup = GetGlobalSetup(setups);
            if (gsup == null)
                return;

            (from c in actual.Cases
             where c.BeforeCase != null
             select c).Each(c => {
				 if(c.FixName != gsup.FixName)
					return;

                 var tsup = c.BeforeCase;
                 c.BeforeCase = runner => {
                     tsup(runner);
                     gsup.Body(runner);
                 };
             });

            (from c in actual.Cases where c.BeforeCase == null select c)
				.Each(c => { 
						if(c.FixName != gsup.FixName)
							return;

						c.BeforeCase = gsup.Body;
					});
        };

        static readonly Action<TestSuite, List<TestCase>> WireGlobalTeardowns =
            (actual, teardowns) => {
                var gtd = GetGlobalTearDown(teardowns);
                if (gtd == null)
                    return;

                (from c in actual.Cases
                 where c.AfterCase != null
                 select c).Each(c => {
					 if(c.FixName != gtd.FixName)
						return;

                     var td = c.AfterCase;
                     c.AfterCase = runner => {
                         td(runner);
                         gtd.Body(runner);
                     };
                 });

                (from c in actual.Cases
                 where c.AfterCase == null
                 select c).Each(c => {
					if(c.FixName != gtd.FixName)
						return;
					 
					 c.AfterCase = gtd.Body;
				});
            };



        static readonly Func<TestSuite, List<TestCase>, List<TestCase>, TestSuite>
            ActualCases = (suite, setups, teardowns) => {
                var actualcases = suite.Cases.Except(setups).Except(teardowns);
                var result = new TestSuite();
                result.Cases.AddRange(actualcases);
                return result;
            };


        static readonly Func<TestSuite, List<TestCase>> FindSetups = suite =>
                (from c in suite.Cases
                 where c.Name.ToUpper().StartsWith(BEFORE)
                 select c)
                .ToList();

        static readonly Func<TestSuite, List<TestCase>> FindTeardowns = suite =>
                (from c in suite.Cases
                 where c.Name.ToUpper().StartsWith(AFTER)
                 select c)
                .ToList();


        static readonly Action<TestSuite, List<TestCase>> WireSetups =
            (suite, setups) => setups.Each(bc => {
                var tcase = suite.Cases.FirstOrDefault(
                    c => { 
							if(c.FixName != bc.FixName)
								return false;

							var fname   = c.GetFullName().ToUpper();
							if(fname == BEFORE_EACH)
							 	return false;

							var bcfname = bc.GetFullName().ToUpper().Replace(BEFORE, "");
							return fname == bcfname;
						});

                //Specific teardows takes precedence over generic ones.
                if (tcase != null)
                    tcase.BeforeCase = bc.Body;
            });

        static readonly Action<TestSuite, List<TestCase>> WireTeardowns =
            (suite, teardowns) => teardowns.Each(ac => {
                var tcase = suite.Cases.FirstOrDefault(
                    c => c.GetFullName().ToUpper() == ac.GetFullName().ToUpper().Replace(AFTER, ""));

                //Specific teardows takes precedence over generic ones.
                if (tcase != null)
                    tcase.AfterCase = ac.Body;
            });

        static readonly Func<TestCaseFinder, string, string, bool> MatchIgnorePattern =
            (finder, casefullname, ignorePatterns) => {

                var cmdIgnorePatterns = //<= from cmd line.
                    ignorePatterns == null ? new string[0] : ignorePatterns.Split(' ');

                var patterns =
                    cmdIgnorePatterns
                        .Union(finder.GetIgnoredPatternsFromFile() ?? new string[0]) //<= from file.
                        .ToArray();

#if DEBUG
                PrintIgnoredPatterns(patterns);
#endif

                foreach (var pattern in patterns) {
                    if (pattern == "*")
                        return true;

                    //TODO: add globbing.
                    if (pattern.EndsWith("*") && pattern.StartsWith("*"))
                        return casefullname.Contains(pattern.Replace("*", ""));

                    if (pattern.EndsWith("*"))
                        return casefullname.StartsWith(pattern.Replace("*", ""));

                    if (pattern.StartsWith("*"))
                        return casefullname.EndsWith(pattern.Replace("*", ""));

                    if (pattern == casefullname)
                        return true;
                }

                return false;
            };

#if DEBUG
        static void PrintIgnoredPatterns(string[] patterns) {
			if (patterns == null || patterns.Length == 0)
				return;
            Console.WriteLine("".PadRight(30, '='));
            Console.WriteLine("Ignored Patterns:");
            Console.WriteLine("".PadRight(30, '='));

            Console.WriteLine(string.Join(",", patterns));

            Console.WriteLine("".PadRight(30, '='));
        }
#endif
    }
}
