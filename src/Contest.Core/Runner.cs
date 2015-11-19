namespace Contest.Core {
    using System;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Diagnostics;
    using static System.Console;
	using static Contest;
	using static ContestConstants;

	
    public class Runner {
        string _currCase, _currCaseFullName;
        
        static readonly Func<bool, bool> Not = cnd => !cnd;
        readonly Dictionary<string, string> _errors = new Dictionary<string, string>();
		readonly List<string> _failingCasesFullNames = new List<string>();
		readonly string _assmName;

        public Runner(string assmName = null) {
			_assmName = assmName;
        }

		public bool Verbose = false;
        public int PassCount, FailCount, AssertsCount, TestCount, IgnoreCount;
        public long Elapsed;
        public readonly Dictionary<string, object> Bag = new Dictionary<string, object>(); 


        public void Run(TestSuite suite, string cherryPicking = null, bool printHeaders=true) {
            Run(suite.Cases, cherryPicking, printHeaders);
        }

        public void Run(List<TestCase> cases, string cherryPicking = null, bool printHeaders=true) {

            Printer.Print("".PadRight(40, '='), BackgroundColor);

            var cherryPick = !string.IsNullOrEmpty(cherryPicking);
            var currfix    = (string) null;
            var watch      = Stopwatch.StartNew();
            cases.Each(c => {
				if(Verbose && printHeaders) {
					if(c.FixName != currfix) 
						Printer.PrintFixName((currfix = c.FixName));
				}

                if (c.Ignored || (cherryPick && !cherryPicking.Match(c.GetFullName()))) {
                    IgnoreCount++;
                    return;
                }

				var innerwatch      = Stopwatch.StartNew();
                try { // <= ensure setups/teardowns.
                    WriteLine(c.Name);
                    _currCase = c.Name;
                    _currCaseFullName = c.GetFullName();
                    c.Run(this);

					innerwatch.Stop();
                }
                catch (Exception ex) {

					innerwatch.Stop();
                    Fail(ex.Message);
                }
				finally {
					WriteLine($"{c.Name} took {innerwatch.ElapsedMilliseconds} ms.");
					WriteLine();
				}
            });

            watch.Stop();
            Elapsed = watch.ElapsedMilliseconds;
            TestCount = cases.Count;
			
			if(Verbose)
				Printer.PrintResults(cases.Count, Elapsed, AssertsCount, PassCount, FailCount, IgnoreCount, cherryPicking);

			if(FailCount > 0) {
				DumpErrors();
				if (_assmName != null)
					SaveFailingCases(_assmName);
			}

            Environment.ExitCode = FailCount;
        }

        void DumpErrors(){
			if(!Verbose)
				return;

            WriteLine("Errors List");
            foreach(var name in _errors.Keys)
                Printer.Print("{0} - {1}".Interpol(name, _errors[name]), ConsoleColor.Red);
        }


		void SaveFailingCases(string assmName) {
			var failingFile = Path.Combine(TMP, GetFailFileName(assmName));
			var content  = string.Join("\n", _failingCasesFullNames);
			File.WriteAllText(path: failingFile, contents: content);
		}

        //All of these helpers end up calling the one and only "Assert" method.
        public void IsNull(object value, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? "Expected null. (Got {0}).".Interpol(value ?? "null")
                : errMsg;

            Assert(value == null, msg);
        }

        public void IsNotNull(object value, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg) ? "Expected NOT null." : errMsg;

            Assert(value != null, msg);
        }

        public void IsTrue(object cond, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg) ? "Expected true." : errMsg;
            Assert(Convert.ToBoolean(cond), msg);
		}

        public void IsFalse(object cond, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg) ? "Expected false." : errMsg;
            Assert(!Convert.ToBoolean(cond), msg);
		}

		const string NULL = "null";
        public void Equal(object expected, object actual, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
                ? $"Expected => { expected ?? NULL } ({ expected?.GetType() })\n" +
			      $"Actual   => { actual   ?? NULL } ({ actual?.GetType() }).\n" 
				: errMsg;

            Func<bool> cond = () =>
                expected == null ?
                actual == null :
                expected.Equals(actual);

            Assert(cond(), $"\n{msg}");
        }

        public void NotEqual(object left, object right, string errMsg = null) {
            var msg = string.IsNullOrEmpty(errMsg)
               ? "Expected NOT equal to {0}.".Interpol(right)
                : errMsg;

            Func<bool> cond = () => 
				left == null ? right != null : Not(left.Equals(right));

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


		static Func<string, string, bool> MsgEq = (lhs, rhs) => lhs == rhs;
		static Func<string, string, bool> MsgContains = (msg, chunck) => 
			msg != null && msg.Contains(chunck);

        void ErrMsg(Func<string, string, bool> compStrat, string msg, Action body) {
			Action failWithMsg = () => Fail("Expected Error Message => {0}".Interpol(msg));
            try {
                AssertsCount++;
                body();
				failWithMsg();
            }
            catch (TargetInvocationException ex) {
				if(ex.InnerException != null && compStrat(ex.InnerException.Message, msg))
					Pass();
				else
					failWithMsg();
			}
            catch (Exception ex) {
				if(compStrat(ex.Message, msg))
					Pass();
				else
					failWithMsg();
            }
		}

        public void ErrMsgContains(string text, Action body) {
			ErrMsg(MsgContains, text, body);
		}

        public void ErrMsg(string msg, Action body) {
			ErrMsg(MsgEq, msg, body);
		}

		// Just for NUnit muscle memory.
        public void Throws<T>(Action body) where T : Exception {
			ShouldThrow<T>(body);
		}

        public void ShouldThrow<T>(Action body) where T : Exception {
            try {
                AssertsCount++;
                body();
                Fail(ExpectedException(typeof(T)));
            }
            catch (TargetInvocationException ex) {
                Type expected = typeof(T), actual = ex.InnerException.GetType();
                if (expected != actual) {
                    Fail(WrongKindaException(expected, actual));
                    return;
                }

                Pass();
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


        public void Fail(string errMsg) {
            FailCount++;
			if(Verbose)
				Printer.Print("Fail\n{0}".Interpol(errMsg), ConsoleColor.Red);

			if(_currCase != null)
				_errors[_currCase] = errMsg;
			else
				_errors["unknown_case"] = errMsg;

			// This allows us to rerun failing cases.
			if(!_failingCasesFullNames.Contains(_currCaseFullName))
				_failingCasesFullNames.Add(_currCaseFullName);
			//
        }

        public void Pass() {
            PassCount++;

			if(Verbose)
				Printer.Print("Pass!", ConsoleColor.Green);
        }

        static readonly Func<Type, Type, string> WrongKindaException = 
			(exptected, got) =>
                "Wrong Kind of Exception. Exptected '{0}' got '{1}'.".Interpol(exptected, got);

        static readonly Func<Type, string> ExpectedException = extype =>
                "Exception of type '{0}' was expected.".Interpol(extype);

    }
}
