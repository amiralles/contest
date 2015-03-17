
namespace Contest.Core {
    using System;

    public struct TestCase {
        public string Name, FixName;
        public Action<Runner> Body;
        public bool Ignored;
    }
}
