namespace Contest.Core {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public struct TestCase {
        public string Name;
        public Action<Runner> Body;
        public bool Ignored;
    }

    public class TestCaseFinder {
        const BindingFlags ipub = BindingFlags.Public | BindingFlags.Instance;
        const BindingFlags ipri = BindingFlags.NonPublic | BindingFlags.Instance;
        const BindingFlags spub = BindingFlags.Public | BindingFlags.Static;
        const BindingFlags spri = BindingFlags.NonPublic | BindingFlags.Static;

        static readonly BindingFlags[] Flags;

        static TestCaseFinder() {
            Flags = new[] { ipub, ipri, spub, spri };
        }

        public static Func<Type, BindingFlags, string[], List<TestCase>> FindCasesNestedTypes =
            (type, flags, ignorePatterns) => {
                var result = new List<TestCase>();
                var nestedTypes = type.GetNestedTypes(flags);
                foreach (var ntype in nestedTypes)
                    result.AddRange(FindCases(ntype, ignorePatterns));

                return result;
            };

        public static Func<Assembly, string[], List<TestCase>> FindCasesInAssm = 
            (assm, ignorePatterns) => {
                var result = new List<TestCase>();
                assm.GetTypes().Each(type => FindCases(type, ignorePatterns).Each(c => {
                    if (result.Any(d => d.Body.Method.MetadataToken == c.Body.Method.MetadataToken))
                        return;
                    result.Add(c);
            }));
            return result;
        };

        static Func<string, string[], bool> MatchIgnorePattern = 
            (casefullname, ignorePatterns) => {

            foreach(var pattern in ignorePatterns){
                if(pattern == "*")
                    return true;

                //TODO: add globbing.
                if(pattern.EndsWith("*") && pattern.StartsWith("*"))
                    return casefullname.Contains(pattern.Replace("*",""));

                if(pattern.EndsWith("*"))
                    return casefullname.StartsWith(pattern.Replace("*",""));

                if(pattern.StartsWith("*"))
                    return casefullname.EndsWith(pattern.Replace("*",""));

                if(pattern == casefullname)
                    return true;
            }

            return false;
        };

        public static Func<Type, string[], List<TestCase>> FindCases = 
            (type, ignorePatterns) => {

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
                            Name = fi.Name,
                            Body = (Action<Runner>)del,
                            Ignored = MatchIgnorePattern(tcfullname, ignorePatterns)
                        });
                }
            }

            result.AddRange(FindCasesNestedTypes(type, BindingFlags.Public, ignorePatterns));
            result.AddRange(FindCasesNestedTypes(type, BindingFlags.NonPublic, ignorePatterns));
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
