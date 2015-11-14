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

    class Program {

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
						RunTests(testAssmPath, pattern, printHeaders);

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
                //ReadLine();
            }
        }

		static void TryUpdateModDat(string testAssmPath) {
			WriteLine("Updating moddat");
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

		/// This method will tells us if we can reuse the tmp copy
		/// of the test assembly and its dependencies.
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

            var finder = new TestCaseFinder();
            var suite = Contest.GetCasesInAssm(finder, assm, null);
            var runner = new Runner();
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

        static void Print(IEnumerable<string> lines) {
            lines.Each(WriteLine);
        }

        static void Print(string msg, params object[] args) {
            WriteLine(msg, args);
        }
    }
}
