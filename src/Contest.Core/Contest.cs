namespace Contest.Core {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class TestSuite{
        public readonly List<TestCase> Cases = new List<TestCase>();
        public readonly TestStats Stats = new TestStats();

        public class TestStats  {
            public readonly Dictionary<string, Action<Runner>> BeforeCases = 
                new Dictionary<string, Action<Runner>>();
        }

    }

    public class Contest {
        const BindingFlags 
            INST_PUB = BindingFlags.Public | BindingFlags.Instance, 
            INST_PRI = BindingFlags.NonPublic | BindingFlags.Instance, 
            STA_PUB  = BindingFlags.Public | BindingFlags.Static, 
            STA_PRI  = BindingFlags.NonPublic | BindingFlags.Static;

        static readonly BindingFlags[] Flags;

        static Contest() {
            Flags = new[] { INST_PUB, INST_PRI, STA_PUB, STA_PRI };
        }

        public static Func<TestCaseFinder, Type, BindingFlags, string, TestSuite> FindCasesNestedTypes =
            (finder, type, flags, ignorePatterns) => {
                var result = new TestSuite();
                var nestedTypes = type.GetNestedTypes(flags);
                foreach (var ntype in nestedTypes){
                    var tmpSuite = FindCases(finder, ntype, ignorePatterns);
                    result.Cases.AddRange(tmpSuite.Cases);
                    tmpSuite.Stats.BeforeCases.Each(bc => 
                        result.Stats.BeforeCases[bc.Key]=bc.Value);
                }

                return result;
            };

        public static Func<TestCaseFinder, Assembly, string, TestSuite> FindCasesInAssm = 
            (finder, assm, ignorePatterns) => {
                var result = new TestSuite();
                assm.GetTypes().Each(type => FindCases(finder, type, ignorePatterns).Cases.Each(c => {
                    if (result.Cases.Any(d => d.Body.Method.MetadataToken == c.Body.Method.MetadataToken))
                        return;
                    result.Cases.Add(c);
                }));
                return result;
            };

        public static Func<TestCaseFinder, Type, string, TestSuite> FindCases = 
            (finder, type, ignorePatterns) => {

                var result = new TestSuite();
                var inst = Activator.CreateInstance(type);

                foreach (var flag in Flags) {
                    foreach (var fi in GetTestCases(type, flag)) {
                        var del = (Delegate)fi.GetValue(inst);
                        if (result.Cases.Any(tc => tc.Body.Method.MetadataToken == del.Method.MetadataToken))
                            continue;

                        var tcfullname = string.Format("{0}.{1}", type.FullName, fi.Name);
                        result.Cases.Add(
                            new TestCase {
                                FixName = type.FullName,
                                Name = fi.Name,
                                Body = (Action<Runner>)del,
                                Ignored = MatchIgnorePattern(finder, tcfullname, ignorePatterns),
                                BeforeCase = TestCase.DefaultBeforeCase
                            });
                    }
                }

                var findNestedPublic    = FindCasesNestedTypes(finder, type, BindingFlags.Public, ignorePatterns);
                var findNestedNonPublic = FindCasesNestedTypes(finder, type, BindingFlags.NonPublic, ignorePatterns);
                result.Cases.AddRange(findNestedPublic.Cases);
                result.Cases.AddRange(findNestedNonPublic.Cases);

                //find before cases, after cases ang purge the list.
                var beforeCases = FindBeforeCases(result.Cases);
                beforeCases.Each(bc => result.Stats.BeforeCases[bc.Name] = bc.Body);

                var filtered = (from c in result.Cases
                          where !beforeCases.Contains(c)
                          select c).ToList();

                result.Cases.Clear();
                result.Cases.AddRange(filtered);
                return result;
            };


        public static Func<List<TestCase>,List<TestCase>> FindBeforeCases = 
            cases => (
                    from c in cases
                    where c.Name.ToUpper().StartsWith("BEFORE_")
                    select c
                ).ToList();


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
