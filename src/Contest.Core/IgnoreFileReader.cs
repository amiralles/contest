
namespace Contest.Core {
    using System;
    using System.IO;

    public static class IgnoreFileReader {
        const string CONTEST_IGNORE_PATH = "./.test_ignore";

        public static Func<string[]> ReadAllLines = () => 
            !File.Exists(CONTEST_IGNORE_PATH) 
                ? new string[0] 
                : File.ReadAllLines(CONTEST_IGNORE_PATH);
    }
}
