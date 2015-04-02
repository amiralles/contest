
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

        public void Run(Runner runner) {
            if (Body == null)
                throw TestBodyCantBeNull();

            try {
                if (BeforeCase != null)
                    BeforeCase(runner);

                Body(runner);
            }
            finally {
                if (AfterCase != null)
                AfterCase(runner);
            }
        }

        Exception TestBodyCantBeNull() {
            return new Exception(string.Format(
                "Test case's body can't be null. (case name: {0})", Name));
        }
    }
}
