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

        static readonly BindingFlags[] Flags;

        static Contest() {
            Flags = new[] { INST_PUB, INST_PRI, STA_PUB, STA_PRI };
        }

        public static Func<TestCaseFinder, Type, BindingFlags, string, List<TestCase>> FindCasesNestedTypes =
            (finder, type, flags, ignorePatterns) => {
                var result = new List<TestCase>();
                var nestedTypes = type.GetNestedTypes(flags);
                foreach (var ntype in nestedTypes)
                    result.AddRange(FindCases(finder, ntype, ignorePatterns));

                return result;
            };

        public static Func<TestCaseFinder, Assembly, string, List<TestCase>> FindCasesInAssm = 
            (finder, assm, ignorePatterns) => {
                var result = new List<TestCase>();
                assm.GetTypes().Each(type => FindCases(finder, type, ignorePatterns).Each(c => {
                    if (result.Any(d => d.Body.Method.MetadataToken == c.Body.Method.MetadataToken))
                        return;
                    result.Add(c);
                }));
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

        public static Func<TestCaseFinder, Type, string, List<TestCase>> FindCases = 
            (finder, type, ignorePatterns) => {

                var result = new List<TestCase>();

                var inst = Activator.CreateInstance(type);

                foreach (var flag in Flags) {
                    foreach (var fi in GetTestCases(type, flag)) {
                        var del = (Delegate)fi.GetValue(inst);
                        if (result.Any(tc => tc.Body.Method.MetadataToken == del.Method.MetadataToken))
                            continue;

                        var tcfullname = string.Format("{0}.{1}", type.FullName, fi.Name);
                        result.Add(
                            new TestCase {
                                FixName = type.FullName,
                                Name = fi.Name,
                                Body = (Action<Runner>)del,
                                Ignored = MatchIgnorePattern(finder, tcfullname, ignorePatterns)
                            });
                    }
                }

                var findNestedPublic    = FindCasesNestedTypes(finder, type, BindingFlags.Public, ignorePatterns);
                var findNestedNonPublic = FindCasesNestedTypes(finder, type, BindingFlags.NonPublic, ignorePatterns);
                result.AddRange(findNestedPublic);
                result.AddRange(findNestedNonPublic);
                return result;
            };


        static readonly Func<Type, BindingFlags, List<FieldInfo>> GetTestCases =
            (type, flags) => (
                from fi in type.GetFields(flags)
                where fi.FieldType == typeof(Action<Runner>)
                select fi
                ).ToList();

    }
}