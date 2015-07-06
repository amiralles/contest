
namespace Contest.Core {
    using System;

    public class TestCase {
        public string Name, FixName;
        public Action<Runner> Body;
        public bool Ignored;
        public Action<Runner> BeforeCase;
        public Action<Runner> AfterCase;

        public string GetFullName() {
            return "{0}.{1}".Interpol(FixName, Name);
        }


        public override string ToString() {
            return Name;
        }

        public void Run(Runner runner) {
            if (Body == null)
                throw TestBodyCantBeNull(Name);

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
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;//<= to increase fails count.
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

        Func<string, Exception> TestBodyCantBeNull = name =>
            new Exception("Test case's body can't be null. (case name: {0})".Interpol(name));
    }
}
