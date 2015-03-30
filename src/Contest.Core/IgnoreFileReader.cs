
namespace Contest.Core {
    using System;
    using System.IO;

    public class IgnoreFileReader {
        const string CTIGNORE_PATH = "./.test_ignore";

        public static Func<string[]> ReadAllLines = () => {
            if (!File.Exists(CTIGNORE_PATH))
                return new string[0];

            return File.ReadAllLines(CTIGNORE_PATH);
        };
    }
}
