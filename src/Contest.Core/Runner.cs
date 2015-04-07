namespace Contest.Core {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class Runner {
        static readonly Func<bool, bool> Not = cnd => !cnd;
        public readonly Dictionary<string, object> Bag = new Dictionary<string, object>(); 

        public int PassCount, FailCount, AssertsCount, TestCount, IgnoreCount;
        public long Elapsed;

        public void Run(TestSuite suite, string cpp = null /*cherry picking pattern.*/) {
            Run(suite.Cases, cpp);
        }

        public void Run(List<TestCase> cases, string cpp = null /*cherry picking pattern.*/) {

            Print("".PadRight(40, '='), ConsoleColor.White);

            var watch = Stopwatch.StartNew();
            var cherryPick = !string.IsNullOrEmpty(cpp);
            // string currfix = null;\
            cases.Each(c => {
                // TODO: fix this.
                // if(c.FixName != currfix) 
                // PrintFixName((currfix = c.FixName));

                if (c.Ignored || (cherryPick && !cpp.Match(c.GetFullName()))) {
                    IgnoreCount++;
                    return;
                }

                try {
                    Console.WriteLine("\n" + c.Name);
                    c.Run(this);//<= ensure setups/teardowns.
                }
                catch (Exception ex) {
                    Fail(ex.Message);
                }
            });

            watch.Stop();
            Elapsed = watch.ElapsedMilliseconds;
            TestCount = cases.Count;
            PrintResults(cases.Count, Elapsed);
            Environment.ExitCode = FailCount;
        }


        //All of these helpers end up calling the one and only "Assert" method.
        public void IsNull(object value, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? string.Format("Expected null. (Got {0}).", value)
                : errMsg;

            Assert(value == null, msg);
        }

        public void IsNotNull(object value, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? string.Format("Expected NOT null.", value)
                : errMsg;

            Assert(value != null, msg);
        }

        public void Equal(object expected, object actual, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? string.Format("Expected equal to {0} (Got {1}).", expected, actual)
                : errMsg;

            Func<bool> cond = () =>
                expected == null ?
                actual == null :
                expected.Equals(actual);

            Assert(cond(), msg);
        }

        public void NotEqual(object left, object right, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? string.Format("Expected NOT equal to {0}.", right)
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
                Fail(ExpectedExcetion(typeof(T)));
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

        void PrintResults(int casesCount, long elapsedMilliseconds) {
            Print("".PadRight(40, '='), ConsoleColor.White);
            Print(string.Format("Test    : {0}", casesCount), ConsoleColor.White);
            Print(string.Format("Asserts : {0}", AssertsCount), ConsoleColor.White);
            Print(string.Format("Elapsed : {0} ms", elapsedMilliseconds), ConsoleColor.White);
            Print(string.Format("Passing : {0}", PassCount), ConsoleColor.Green);
            Print(string.Format("Failing : {0}", FailCount), ConsoleColor.Red);
            Print(string.Format("Ignored : {0}", IgnoreCount), ConsoleColor.Yellow);
            Print("".PadRight(40, '='), ConsoleColor.White);
        }

        readonly static Action<string> PrintFixName = name => {
            Print("".PadRight(40, '>'), ConsoleColor.Cyan);
            Print(name, ConsoleColor.Cyan);
            Print("".PadRight(40, '>'), ConsoleColor.Cyan);
        };

        readonly static Action<string, ConsoleColor> Print = (msg, color) => {
            var fcolor = Console.ForegroundColor;
            try {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
            }
            finally {
                Console.ForegroundColor = fcolor;
            }
        };

        void Fail(string errMsg) {
            Print(string.Format("Fail\n{0}", errMsg), ConsoleColor.Red);
            FailCount++;
        }

        void Pass() {
            PassCount++;
            Print("Pass!", ConsoleColor.Green);
        }

        static readonly Func<Type, Type, string> WrongKindaException = (exptected, got) =>
                string.Format("Wrong Kind of Exception. Exptected '{0}' got '{1}'.", exptected, got);

        static readonly Func<Type, string> ExpectedExcetion = extype =>
                string.Format("Exception of type '{0}' was expected.", extype);

    }
}
