using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Contest.Core;
using Contest.Tests;

namespace Inception.Test.Runner {
	using static Console;
	using static ContestConstants;

    class Program {

        static void Main(string[] args) {

            try {
                Trace.Listeners.Add(new ConsoleTraceListener());

                if (!args.Any()) {
                    PrintHelp();
                    return;
                }

                if (args.Any(a => a == "-dbg")) {
                    WriteLine("Attach the debugger and press [Enter] to continue.");
                    ReadLine();
                }

				var printHeaders = !args.Any(a => a == "--no-head" || a == "-nh");

				//clean args list (no flags).
				args = (from a in args where !a.StartsWith("-") select a).ToArray();

                var cmd = args[0];
                switch (cmd) {
                    case "run":
                    case "r":
						if(args.Length <=1) {
							WriteLine("File name expected. (The name of the assembly that contains test cases)");
							return;
						}

						var root = Path.GetDirectoryName(args[1]);
						CopyToLocalTmp(root);
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

						//args[0] == cmd
						var testAssm = args[1];
						var pattern  = args.Length >= 3 ? args[2] : null;

						RunTests(testAssm, pattern, printHeaders);

                        break;
                    default: {
						 PrintHelp();
						 break;
					}
                }
            }
            catch (Exception ex) {
                WriteLine(ex);
            }
        }

        static void CopyToLocalTmp(string root) {
			root = string.IsNullOrEmpty(root)?".":root;
#if DEBUG
			WriteLine("Copying to local tmp...");
			WriteLine("tmp:  '{0}'", TMP);
			WriteLine("root: '{0}'", root);
#endif
            if (!Directory.Exists(TMP)){
#if DEBUG
				WriteLine("Creating tmp dir '{0}'", TMP);
#endif
                Directory.CreateDirectory(TMP);
			}

            Directory.GetFiles(root, "*.dll").Each(
                f => {
				 var to = Path.Combine(TMP, Path.GetFileName(f) ?? "");
#if DEBUG
					WriteLine("copying '{0}' to {1}", f, to);
#endif
					File.Copy(
						f ?? throw new ArgumentNullException(nameof(f)), 
						to, true);
				});
#if DEBUG
			WriteLine("Done!");
#endif
        }

        static void RunTests(string assmFileName, string cerryPicking=null, bool printHeaders=true) {
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


			// IMPORTANT!
			// This is the _only_ difference with main program. should refator and
			// get just one program asap!
			Func<Type, bool> ifNonCoreTest = t => t != typeof(contest_core_tests);
			//
            var finder = new TestCaseFinder(getIgnoredFromFile: null, ignoreType: ifNonCoreTest);

            var suite = Contest.Core.Contest.GetCasesInAssm(finder, assm, null);
            var runner = new Contest.Core.Runner();
			SyntaxSugar.SetRunner(runner);//To enable syntax sugar.

			runner.Verbose = true;

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
			Print("");
			Print("-- More --");
			ReadLine();

            Print("=================================================================================");
            Print("| Flags                                                                         |");
            Print("=================================================================================");
            Print("| name  | summary                                                               |");
            Print("=================================================================================");
            Print("| -nh   | Don't print fixture names.                                            |");
            Print("| -dbg  | Stop the runner until the user presses [Enter].                       |");
            Print("=================================================================================");
			Print("");
			Print("-- More --");
			ReadLine();

            Print("=================================================================================");
            Print("| Alias                                                                         |");
            Print("=================================================================================");
            Print("| name      | summary                                                           |");
            Print("=================================================================================");
            Print("| --no-head | -nh                                                               |");
            Print("| r         | run                                                               |");
            Print("=================================================================================");
			Print("");

            Print("\nMore about contest at:");
            Print("https://github.com/amiralles/contest\n");
		   
        }

        static void Print(string msg, params object[] args) {
            WriteLine(msg, args);
        }
    }
}
