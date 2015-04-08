﻿namespace Contest.Core {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

	
    public class Runner {
        
        static readonly Func<bool, bool> Not = cnd => !cnd;

        public Runner() {
        }

        public int PassCount, FailCount, AssertsCount, TestCount, IgnoreCount;
        public long Elapsed;
        public readonly Dictionary<string, object> Bag = new Dictionary<string, object>(); 

        public void Run(TestSuite suite, string cpp = null /*cherry picking pattern.*/) {
            Run(suite.Cases, cpp);
        }

        public void Run(List<TestCase> cases, string cpp = null /*cherry picking pattern.*/) {

            Printer.Print("".PadRight(40, '='), ConsoleColor.White);

            var watch = Stopwatch.StartNew();
            var cherryPick = !string.IsNullOrEmpty(cpp);
            string currfix = null;
            cases.Each(c => {
                if(c.FixName != currfix) 
					Printer.PrintFixName((currfix = c.FixName));

                if (c.Ignored || (cherryPick && !cpp.Match(c.GetFullName()))) {
                    IgnoreCount++;
                    return;
                }

                try {
                    Console.WriteLine(c.Name);
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
            Printer.PrintResults(
					cases.Count, Elapsed, AssertsCount, PassCount,FailCount, IgnoreCount);

            Environment.ExitCode = FailCount;
        }


        //All of these helpers end up calling the one and only "Assert" method.
        public void IsNull(object value, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? "Expected null. (Got {0}).".Interpol(value)
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
                ? "Expected equal to {0} (Got {1}).".Interpol(expected, actual)
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
