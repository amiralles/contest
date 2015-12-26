//#define DARK_TEXT

namespace Contest {
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core;
    using static System.Console;
	using static Contest.Core.ContestConstants;
	using static Contest.Core.Contest;

    class Program {


		static readonly CultureInfo Culture = new CultureInfo("en-US");

        static void Main(string[] args) {
            try {
                Trace.Listeners.Add(new ConsoleTraceListener());

#if DARK_TEXT
				Console.ForegroundColor = ConsoleColor.Black;
#endif

                if (!args.Any()) {
                    PrintHelp();
                    return;
                }

                if (args.Any(a => a == "-dbg")) {
                    WriteLine("Attach the debugger and press [Enter] to continue.");
                    ReadLine();
                }

				var printHeaders = !args.Any(a => a == "--no-head" || a == "-nh");
				var failing      = args.Any(a => a == "-f");

				//================================================
				// Listing results from previous runs.
				var listFailing  = args.Any(a => a == "-lf");
				var listSlow     = args.Any(a => a == "-yslow");
				var listFast     = args.Any(a => a == "-yfast");
				//================================================

				//clean args list (no flags).
				args = (from a in args where !a.StartsWith("-") select a).ToArray();

                var cmd = args[0];
                switch (cmd) {
                    case "run":
                    case "r":
						if(args.Length <= 1) {
							WriteLine("File name expected. (The name of the assembly that contains test cases)");
							return;
						}

						var testAssmPath = args[1];
						if (!ReuseTestAssm(testAssmPath)) {
							CopyToLocalTmp(Path.GetDirectoryName(testAssmPath));
							TryUpdateModDat(testAssmPath);
						}
#if DEBUG
						else {
							WriteLine("Reusing local copy.");
						}
#endif

						AppDomain.CurrentDomain.AssemblyResolve += (s, e) => {
							try {
								var name = string.Format("{0}.dll", e.Name.Split(',')[0]);
								var localpath = Path.GetFullPath(Path.Combine(TMP, name));
								if (File.Exists(localpath)) {
									var assm = Assembly.LoadFile(localpath);
									Debug.Assert(assm != null);
									return assm;
								}
							}
							catch (Exception ex) {
								WriteLine(ex);
							}
							return null;
						};

						var pattern  = args.Length >= 3 ? args[2] : null;

						// ================================================
						// Listing content from previous run;
						// None of these options actually run the tests.
						if (listFailing) {
							ShowPreviousFails(testAssmPath);
							return;
						}
						if (listSlow) {
							ShowSlowTests(testAssmPath);
							return;
						}
						if (listFast) {
							ShowFastTests(testAssmPath);
							return;
						}
						// ================================================
						
						// Either of these options execute tests;
						if (failing)
							RunFailingTests(testAssmPath);
						else 
							RunTests(testAssmPath, pattern, printHeaders);
						//

                        break;
                    case "help":
                    case "h":
                    default: {
						 PrintHelp();
						 break;
					}
                }
            }
            catch (Exception ex) {
                WriteLine(ex);
            }
			finally {
				Contest.Shutdown();
			}
        }

		static Dictionary<string, long> GetTestsTimes(string assmFileName) {
			var timeFile = Path.Combine(TMP, GetTimeFileName(assmFileName));
			if (!File.Exists(timeFile))
				return null;

			var content = File.ReadAllLines(timeFile);

			var testsTimes = new Dictionary<string, long>();
			for (int i = 0; i < content.Length; i++) {
				var tmp = content[i].Split('|');
				if (tmp.Length != 2)
					continue;

				var test = tmp[0].Trim();
				var time = long.Parse(tmp[1].Trim());
				testsTimes[test] = time;
			}

			return testsTimes;
		}

		// Shows slow tests first.
		static void ShowSlowTests(string assmFileName) {
			var testsTimes = GetTestsTimes(assmFileName);
			if (testsTimes == null)
				return;

			var sortedTestsTimes = (from testTime in testsTimes 
							       orderby testTime.Value descending 
							       select testTime).ToArray();

			PrintTests(sortedTestsTimes);
		}

		// Shows fast tests first.
		static void ShowFastTests(string assmFileName) {
			var testsTimes = GetTestsTimes(assmFileName);
			if (testsTimes == null)
				return;

			var sortedTestsTimes = (from testTime in testsTimes 
							       orderby testTime.Value ascending 
							       select testTime).ToArray();

			PrintTests(sortedTestsTimes);
		}

		static void PrintTests(KeyValuePair<string, long>[] testsTimes) {
			foreach(var tt in testsTimes)
				WriteLine($"{tt.Key} => {tt.Value} ms");
		}

		static void TryUpdateModDat(string testAssmPath) {
#if DEBUG
			WriteLine("Updating moddat");
#endif
			try {
				var moddat = new FileInfo(testAssmPath).LastWriteTime;
				File.WriteAllText(Path.Combine(TMP, MODDAT), moddat.ToString(DATE_FORMAT));
			}
			catch (Exception ex) { //Not a big deal, we can update this on the next run.
#if DEBUG
				WriteLine("WARN: Couldn't update moddat.");
				WriteLine(ex.Message);
#endif
			}
		}


		/// This method will tell us if we can reuse the tmp copy
		/// and its dependencies.
		static bool ReuseTestAssm(string testAssmPath) {
			var moddatPath = Path.Combine(TMP, MODDAT);
			if (!File.Exists(moddatPath))
				return false;

			var tmp = File.ReadAllLines(moddatPath);
			if (tmp.Length == 0)
				return false;

			var strdat = tmp[0].Trim();

			DateTime moddat;
			if(!DateTime.TryParseExact(strdat, DATE_FORMAT, Culture, DateTimeStyles.None, out moddat)) {
				WriteLine("WARN: Couldn't parse moddat.");
				return false;
			}

			return moddat == new FileInfo(testAssmPath).LastWriteTime;
		}


		/// Creates a local copy of the directory that contains the 
		/// test assembly.
        static void CopyToLocalTmp(string testAssmSrcDir) {
			var srcDir = string.IsNullOrEmpty(testAssmSrcDir) ? "." : testAssmSrcDir;
#if DEBUG
			WriteLine("Copying to local tmp...", TMP);
			WriteLine("tmp: '{0}'", TMP);
			WriteLine("src: '{0}'", srcDir);
#endif
            if (!Directory.Exists(TMP)){
#if DEBUG
				WriteLine("Creating tmp dir '{0}'", TMP);
#endif
                Directory.CreateDirectory(TMP);
			}

            Directory.GetFiles(srcDir, "*.dll").Each(
                f => {
				 var to = Path.Combine(TMP, Path.GetFileName(f));
#if DEBUG
					WriteLine("copying '{0}' to {1}", f, to);
#endif
					File.Copy(f, to, true);
				});
#if DEBUG

			WriteLine("Done!");
#endif
        }


		/// Shows failing tests from the last run.
		static void ShowPreviousFails(string assmFileName) {
			var failingFile = Path.Combine(TMP, GetFailFileName(assmFileName));

			// If this file does not exists or exists but it's empty,
			// there are no failing tests.
			if (!File.Exists(failingFile)) { 
				WriteLine("There are no previous failing tests. Nothing to do.");
				return;
			}
			
			var content = File.ReadAllLines(failingFile);
			if (content.Length == 0) { 
				WriteLine("There are no previous failing tests. Nothing to do.");
				return;
			}

			WriteLine("");
			WriteLine("".PadRight(70, '='));
			WriteLine("Previous Failing Tests");
			WriteLine("".PadRight(70, '='));

			foreach(var line in content)
				WriteLine(line);

			WriteLine("".PadRight(70, '='));
			WriteLine("");
		}


		/// Rerun failing tests from the last run.
        static void RunFailingTests(string assmFileName) {
			var failingFile = Path.Combine(TMP, GetFailFileName(assmFileName));

			// If this file does not exists or exists but it's empty,
			// there are no failing tests.
			if (!File.Exists(failingFile)) { 
				WriteLine("There are no previous failing tests. Nothing to do.");
				return;
			}
			
			var content = File.ReadAllLines(failingFile);
			if (content.Length == 0) { 
				WriteLine("There are no previous failing tests. Nothing to do.");
				return;
			}
			var failingTests = new List<string> (content);

			// Assumes file name WITH extension.
            var fullpath = Path.GetFullPath(assmFileName);
            DieIf(!File.Exists(fullpath), $"File not found '{assmFileName}'.");

			// Load all cases.
            var assm = Assembly.LoadFile(fullpath);
			DieIf(assm == null, "Can't load assembly '{assmFileName}'.");

            var finder = new TestCaseFinder();
            var suite  = Contest.GetCasesInAssm(finder, assm, null);

			// Find failing cases.
			var failingSuite = new TestSuite(
									from   c in suite.Cases 
									where  failingTests.Contains(c.GetFullName())
									select c);

            var runner = CreateRunner(assmFileName, assm);

			SyntaxSugar.SetRunner(runner);//To enable syntax sugar.

			// Run only failing tests.
            runner.Run(failingSuite);
		}

		static Runner CreateRunner(string assmFileName, Assembly assm) {
			var runner = new Runner(assmFileName);
			runner.BeforeAny = Contest.GetInitCallbackOrNull(assm);
			runner.AfterAll  = Contest.GetShutdownCallbackOrNull(assm);
			runner.Verbose   = true;
			return runner;
		}

        static void RunTests(string assmFileName, string cerryPicking = null, bool printHeaders = true) {
            WriteLine("\nConfiguring Assembies....");

            if (string.IsNullOrEmpty(assmFileName))
                throw new ArgumentException("Assembly name is required.");

            if (!assmFileName.EndsWith(".dll"))
                assmFileName = "{0}.dll".Interpol(assmFileName);

            var fullpath = Path.GetFullPath(assmFileName);

            if (!File.Exists(fullpath)) {
                var pwd = Directory.GetCurrentDirectory();
                    throw new IOException(
                        "Pwd: {0}\n".Interpol(pwd) +
                        "File not found. ('{0}')".Interpol(assmFileName));
            }

            var assm = Assembly.LoadFile(fullpath);
            if (assm == null)
                throw new Exception("Can't load assembly '{0}'.".Interpol(assmFileName));

            var finder = new TestCaseFinder();
            var suite  = Contest.GetCasesInAssm(finder, assm, null);
            var runner = CreateRunner(assmFileName, assm);
			SyntaxSugar.SetRunner(runner);//To enable syntax sugar.
            WriteLine("\nDone!\n");
            runner.Run(suite, cerryPicking, printHeaders );
        }

        static void PrintHelp() {
            Print("=================================================================================");
            Print("| Contest - Console Test Runner                                                 |");
            Print("=================================================================================");

            Print("=================================================================================");
            Print("| Commands                                                                      |");
            Print("=================================================================================");
            Print("| name  | args              | summary                                           |");
            Print("=================================================================================");
            Print("| help  |                   | Shows help.                                       |");
            Print("| run   | test.dll          | Runs all tests within the given file.             |");
            Print("| run   | test.dll wildcard | Runs all tests within the given file where the    |");
		    Print("|       |                   | test name matches the wildcard pattern.           |");
            Print("=================================================================================");

			Write(":");
			if (ReadLine() == "q") Environment.Exit(0);

            Print("=================================================================================");
            Print("| Flags                                                                         |");
            Print("=================================================================================");
            Print("| name   | summary                                                              |");
            Print("=================================================================================");
            Print("| -nh    | Don't print fixture names.                                           |");
            Print("| -dbg   | Stop the runner until the user presses [Enter].                      |");
            Print("| -f     | Rerun previous failing tests.                                        |");
            Print("| -lf    | List previous  failing tests.                                        |");
            Print("| -lslow | List slow tests from previous run.                                   |");
            Print("| -lfast | List fast tests from previous run.                                   |");
            Print("=================================================================================");
			Print("");

			Write(":");
			if (ReadLine() == "q") Environment.Exit(0);

            Print("=================================================================================");
            Print("| Alias                                                                         |");
            Print("=================================================================================");
            Print("| name      | summary                                                           |");
            Print("=================================================================================");
            Print("| --no-head | -nh                                                               |");
            Print("| r         | run                                                               |");
            Print("=================================================================================");
			Print("");
			Write(":");
			if (ReadLine() == "q") Environment.Exit(0);

            Print("\nMore about contest at:");
            Print("https://github.com/amiralles/contest\n");
		   
        }

        static void Print(IEnumerable<string> lines) {
            lines.Each(WriteLine);
        }

        static void Print(string msg, params object[] args) {
            WriteLine(msg, args);
        }
    }
}
