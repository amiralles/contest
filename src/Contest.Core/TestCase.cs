
namespace Contest.Core {
    using System;

    public class TestCase {
        public string Name, FixName;
        public Action<Runner> Body;
        public bool Ignored;
        public Action<Runner> BeforeCase;
        public Action<Runner> AfterCase;

        public string GetFullName() {
            return string.Format("{0}.{1}", FixName, Name);
        }


        public override string ToString() {
            return Name;
        }

        public void Run() {
            throw new NotImplementedException();
        }
    }
}
