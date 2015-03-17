
namespace Contest.Core {
    using System;

    public struct TestCase {
        public string Name, FixName;
        public Action<Runner> Body;
        public bool Ignored;

        public string GetFullName() {
            return string.Format("{0}.{1}", FixName, Name);
        }
    }
}
