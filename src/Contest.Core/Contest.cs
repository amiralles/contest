namespace Contest.Core {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class Contest {
        const BindingFlags 
            INST_PUB = BindingFlags.Public | BindingFlags.Instance, 
            INST_PRI = BindingFlags.NonPublic | BindingFlags.Instance, 
            STA_PUB  = BindingFlags.Public | BindingFlags.Static, 
            STA_PRI  = BindingFlags.NonPublic | BindingFlags.Static;

        const string 
			BEFORE = "BEFORE_",
			AFTER  = "AFTER_";

        static readonly BindingFlags[] Flags;

        static Contest() {
            Flags = new[] { INST_PUB, INST_PRI, STA_PUB, STA_PRI };
        }

        static readonly Func<Delegate,Delegate, bool> SameMetaToken = (left, right) =>
             left.Method.MetadataToken == right.Method.MetadataToken;

        public static Func<TestCaseFinder, Assembly, string, TestSuite> FindCasesInAssm = 
            (finder, assm, ignorePatterns) => {
                var suite = new TestSuite();
                var cases =
                    from type in assm.GetTypes()
                    from c in FindCases(finder, type, ignorePatterns).Cases
                    select c;

                cases.Each(c => {
                    if (suite.Cases.Any(d => c.Body == null || SameMetaToken(d.Body, c.Body))) {
                        return;
                    }
                    suite.Cases.Add(c);
                });

                var setups = FindSetups(suite);
                WireSetups(suite, setups);

                var teardowns = FindTeardowns(suite);
                WireTeardowns(suite, teardowns);

                return ActualCases(suite, setups, teardowns);
            };

        static readonly Func<TestSuite, List<TestCase>, List<TestCase>, TestSuite> ActualCases = 
			(suite, setups, teardowns) => {
                var actualcases = suite.Cases.Except(setups).Except(teardowns);
                var result = new TestSuite();
                result.Cases.AddRange(actualcases);
                return result;
            };


        static readonly Func<TestSuite,List<TestCase>> FindSetups = suite =>
                (from c in suite.Cases
                 where c.Name.ToUpper().StartsWith(BEFORE)
                 select c)
                .ToList();

        static readonly Func<TestSuite,List<TestCase>> FindTeardowns = suite =>
                (from c in suite.Cases
                 where c.Name.ToUpper().StartsWith(AFTER)
                 select c)
                .ToList();


        static readonly Action<TestSuite, List<TestCase>> WireSetups = (suite, setups) =>
            setups.Each(bc => {
                var tcase = suite.Cases.FirstOrDefault(
                    c => c.Name.ToUpper() == bc.Name.ToUpper().Replace(BEFORE, ""));

                if (tcase != null)
                    tcase.BeforeCase = bc.Body;
            });

        static readonly Action<TestSuite, List<TestCase>> WireTeardowns = (suite, teardowns) =>
            teardowns.Each(ac => {
                var tcase = suite.Cases.FirstOrDefault(
                    c => c.Name.ToUpper() == ac.Name.ToUpper().Replace(AFTER, ""));

                if (tcase != null)
                    tcase.AfterCase = ac.Body;
            });

        public static Func<TestCaseFinder, Type, BindingFlags, string, TestSuite> FindCasesNestedTypes =
            (finder, type, flags, ignorePatterns) => {
                var result = new TestSuite();
                var nestedTypes = type.GetNestedTypes(flags);
                foreach (var ntype in nestedTypes)
                    result.Cases.AddRange(FindCases(finder, ntype, ignorePatterns).Cases);

                return result;
            };

        public static Func<TestCaseFinder, Type, string, TestSuite> FindCases = 
            (finder, type, ignorePatterns) => {

                var result = new TestSuite();
                var inst = Activator.CreateInstance(type);

                foreach (var flag in Flags) {
                    foreach (var fi in GetTestCases(type, flag)) {
                        var del = (Delegate)fi.GetValue(inst);
                        if (result.Cases.Any(tc => SameMetaToken(tc.Body, del)))
                            continue;

                        var tcfullname = string.Format("{0}.{1}", type.FullName, fi.Name);
                        result.Cases.Add(
                            new TestCase {
                                FixName = type.FullName,
                                Name = fi.Name,
                                Body = (Action<Runner>)del,
                                Ignored = MatchIgnorePattern(finder, tcfullname, ignorePatterns),
                            });
                    }
                }

                var findNestedPublic    = FindCasesNestedTypes(finder, type, BindingFlags.Public, ignorePatterns);
                var findNestedNonPublic = FindCasesNestedTypes(finder, type, BindingFlags.NonPublic, ignorePatterns);
                result.Cases.AddRange(findNestedPublic.Cases);
                result.Cases.AddRange(findNestedNonPublic.Cases);

                return result;
            };


        static readonly Func<TestCaseFinder, string, string, bool> MatchIgnorePattern = 
            (finder, casefullname, ignorePatterns) => {

                var cmdIgnorePatterns = //<= from cmd line.
                    ignorePatterns == null ? new string[0] : ignorePatterns.Split(' ');

                var patterns =
                    cmdIgnorePatterns
                        .Union(finder.GetIgnoredPatternsFromFile() ?? new string[0]) //<= from file.
                        .ToArray();

                PrintIgnoredPatterns(patterns);

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

        static void PrintIgnoredPatterns(string[] patterns) {
            Console.WriteLine("".PadRight(30, '='));
            Console.WriteLine("Ignored Patterns:");
            Console.WriteLine("".PadRight(30, '='));

            Console.WriteLine(string.Join(",", patterns));

            Console.WriteLine("".PadRight(30, '='));
        }

        static readonly Func<Type, BindingFlags, List<FieldInfo>> GetTestCases =
            (type, flags) => (
                from fi in type.GetFields(flags)
                where fi.FieldType == typeof(Action<Runner>)
                select fi
                ).ToList();

    }
}
