
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
                if (BeforeCase != null) {
#if DEBUG
					Console.WriteLine("Running setup for '{0}'", Name);
#endif
                    BeforeCase(runner);
                }

#if DEBUG
					Console.WriteLine("Running '{0}'", Name);
#endif
                Body(runner);
            }
            finally {
                if (AfterCase != null) {
#if DEBUG
					Console.WriteLine("Running teardown for '{0}'", Name);
#endif
                    AfterCase(runner);
                }
            }
        }

        Exception TestBodyCantBeNull() {
            return new Exception(string.Format(
                "Test case's body can't be null. (case name: {0})", Name));
        }
    }
}
