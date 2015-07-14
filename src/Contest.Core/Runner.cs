namespace Contest.Core {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

	
    public class Runner {
        string _currCase;
        
        static readonly Func<bool, bool> Not = cnd => !cnd;
        readonly Dictionary<string, string> _errors = new Dictionary<string, string>();

        public Runner() {
        }

        public int PassCount, FailCount, AssertsCount, TestCount, IgnoreCount;
        public long Elapsed;
        public readonly Dictionary<string, object> Bag = new Dictionary<string, object>(); 

        public void Run(TestSuite suite, string cherryPicking = null /*cherry picking pattern.*/) {
            Run(suite.Cases, cherryPicking);
        }

        public void Run(List<TestCase> cases, string cherryPicking = null /*cherry picking pattern.*/) {

            Printer.Print("".PadRight(40, '='), ConsoleColor.White);

            var cherryPick = !string.IsNullOrEmpty(cherryPicking);
            string currfix = null;
            var watch = Stopwatch.StartNew();
            cases.Each(c => {
                if(c.FixName != currfix) 
					Printer.PrintFixName((currfix = c.FixName));

                if (c.Ignored || (cherryPick && !cherryPicking.Match(c.GetFullName()))) {
                    IgnoreCount++;
                    return;
                }

                try {
                    Console.WriteLine(c.Name);
                    _currCase = c.Name;
                    c.Run(this);//<= ensure setups/teardowns.
                }
                catch (Exception ex) {
                    Fail(ex.Message);
                }
				finally{
					Console.WriteLine();
				}
            });

            watch.Stop();
            Elapsed = watch.ElapsedMilliseconds;
            TestCount = cases.Count;
            Printer.PrintResults(cases.Count, Elapsed, AssertsCount, PassCount,FailCount, IgnoreCount, cherryPicking);

            if(FailCount>0)
                DumpErrors();

            Environment.ExitCode = FailCount;
        }

        void DumpErrors(){

            Console.WriteLine("Errors List");
            foreach(var name in _errors.Keys)
                // Printer.Print("Fail\n{0}".Interpol(errMsg), ConsoleColor.Red);
                Printer.Print("{0} - {1}".Interpol(name, _errors[name]), ConsoleColor.Red);
        }

        //All of these helpers end up calling the one and only "Assert" method.
        public void IsNull(object value, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? "Expected null. (Got {0}).".Interpol(value ?? "null")
                : errMsg;

            Assert(value == null, msg);
        }

        public void IsNotNull(object value, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? "Expected NOT null."
                : errMsg;

            Assert(value != null, msg);
        }

        public void Equal(object expected, object actual, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? "Expected equal to {0} (Got {1}).".Interpol(expected, actual ?? "null")
                : errMsg;

            Func<bool> cond = () =>
                expected == null ?
                actual == null :
                expected.Equals(actual);

            Assert(cond(), msg);
        }

        public void NotEqual(object left, object right, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
               ? "Expected NOT equal to {0}.".Interpol(right)
                : errMsg;

            Func<bool> cond = () =>
                left == null ?
                right != null :
                Not(left.Equals(right));

            Assert(cond(), msg);
        }


        public void Assert(bool cond, string errMsg = null) {
            AssertsCount++;

            if (!cond) {
                Fail(errMsg);
                return;
            }

            Pass();
        }

        public void ShouldThrow<T>(Action body) where T : Exception {
            try {
                AssertsCount++;
                body();
                Fail(ExpectedException(typeof(T)));
            }
            catch (Exception ex) {
                Type expected = typeof(T), actual = ex.GetType();
                if (expected != actual) {
                    Fail(WrongKindaException(expected, actual));
                    return;
                }

                Pass();
            }
        }


        void Fail(string errMsg) {
            FailCount++;
            Printer.Print("Fail\n{0}".Interpol(errMsg), ConsoleColor.Red);
            _errors[_currCase] = errMsg;
        }

        void Pass() {
            PassCount++;
            Printer.Print("Pass!", ConsoleColor.Green);
        }

        static readonly Func<Type, Type, string> WrongKindaException = 
			(exptected, got) =>
                "Wrong Kind of Exception. Exptected '{0}' got '{1}'.".Interpol(exptected, got);

        static readonly Func<Type, string> ExpectedException = extype =>
                "Exception of type '{0}' was expected.".Interpol(extype);

    }
}
