namespace Contest.Core {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class Runner {
        public int PassCount, FailCount, AssertsCount;
        public long Elapsed;

        public void Assert(bool cond, string errMsg) {
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

        public void Run(List<TestCase> cases) {
            Print("".PadRight(80, '='), ConsoleColor.White);

            var watch = Stopwatch.StartNew();
            cases.Each(c => {
                try {
                    Console.WriteLine("\n" + c.Name);
                    c.Body(this);
                }
                catch (Exception ex) {
                    Fail(ex.Message);
                }
            });

            watch.Stop();
            Elapsed = watch.ElapsedMilliseconds;
            PrintResults(cases.Count, Elapsed);
            Environment.ExitCode = FailCount;
        }

        void PrintResults(int casesCount, long elapsedMilliseconds) {
            Print("".PadRight(80, '='), ConsoleColor.White);
            Print(string.Format("Test Count: {0}", casesCount), ConsoleColor.White);
            Print(string.Format("Total Assertions: {0}", AssertsCount), ConsoleColor.White);
            Print(string.Format("Elapsed Time: {0} ms", elapsedMilliseconds), ConsoleColor.White);
            Print(string.Format("Passing: {0}", PassCount), ConsoleColor.Green);
            Print(string.Format("Failing: {0}", FailCount), ConsoleColor.Red);
            Print("".PadRight(80, '='), ConsoleColor.White);
        }

        readonly Action<string, ConsoleColor> Print = (msg, color) => {
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