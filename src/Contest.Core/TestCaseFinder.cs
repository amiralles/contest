namespace Contest.Core {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public struct TestCase {
        public string Name;
        public Action<Runner> Body;
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

        public static Func<Type, BindingFlags, List<TestCase>> FindCasesNestedTypes =
            (type, flags) => {
                var result = new List<TestCase>();
                var nestedTypes = type.GetNestedTypes(flags);
                foreach (var ntype in nestedTypes)
                    result.AddRange(FindCases(ntype));

                return result;
            };

        public static Func<Assembly, List<TestCase>> FindCasesInAssm = assm => {
            var result = new List<TestCase>();
            assm.GetTypes().Each(type => FindCases(type).Each(c => {
                if (result.Any(d => d.Body.Method.MetadataToken == c.Body.Method.MetadataToken))
                    return;
                result.Add(c);
            }));
            return result;
        };

        public static Func<Type, List<TestCase>> FindCases = type => {
            var result = new List<TestCase>();

            var inst = Activator.CreateInstance(type);

            foreach (var flag in Flags) {
                foreach (var fi in GetTestCases(type, flag)) {
                    var del = (Delegate)fi.GetValue(inst);
                    if (result.Any(tc => tc.Body.Method.MetadataToken == del.Method.MetadataToken))
                        continue;

                    result.Add(
                        new TestCase {
                            Name = fi.Name,
                            Body = (Action<Runner>)del
                        });
                }
            }

            result.AddRange(FindCasesNestedTypes(type, BindingFlags.Public));
            result.AddRange(FindCasesNestedTypes(type, BindingFlags.NonPublic));
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