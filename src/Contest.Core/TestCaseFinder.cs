namespace Contest.Core {
    using System;
    using System.Collections.Generic;

    // ReSharper disable InconsistentNaming
    public class TestCaseFinder {
        public TestCaseFinder(Func<string[]> getIgnoredFromFile = null) {
            GetIgnoredPatternsFromFile = 
                getIgnoredFromFile ?? GetIgnoredPatternsFromFile;
        }

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
