namespace Contest.Core {
    using System;
    using System.Collections.Generic;

    public class TestCaseFinder {
        public TestCaseFinder(Func<string[]> getIgnoredFromFile = null, Func<Type, bool> ignoreType = null) {
            GetIgnoredPatternsFromFile = 
                getIgnoredFromFile ?? GetIgnoredPatternsFromFile;

			IgnoreType = ignoreType ?? (t => false);
        }

		public readonly Func<Type, bool> IgnoreType;

        public Func<string[]> GetIgnoredPatternsFromFile = () => {
            var lines = IgnoreFileReader.ReadAllLines();
            var patterns  = new List<string>();
            (lines ?? new string[0]).Each(ln => {
                if (ln.StartsWith("#"))//<= comment.
                    return;

                patterns.AddRange(ln.Split(new[] { ",", " ", ";" },
                    StringSplitOptions.RemoveEmptyEntries));
            });
            return patterns.ToArray();
        };

    }
}
