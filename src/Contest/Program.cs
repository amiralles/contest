namespace Contest {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core;

    class Program {
        const string TMP = "tmp";

        static void Main(string[] args) {

            try {
                Trace.Listeners.Add(new ConsoleTraceListener());

                // Print(args);
                if (!args.Any()) {
                    PrintHelp();
                    return;
                }

                var cmd = args[0];
                switch (cmd) {
                    case "run":
                    case "r":
						if(args.Length <=1) {
							Console.WriteLine("File name expected. (The name of the assembly that contains test cases)");
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
								Console.WriteLine(ex);
							}
							return null;
						};

						if(args.Length >= 3){//run assmName cerryPicking
							//TODO: Refactor this. --no-head should be available for
							//any kind of run, nos just de ones that cherrypicks.
							var printHeaders = !(args.Length >= 4 && args[3] == "--no-head");
							RunTests(args[1], args[2], printHeaders);
						}
						else 
							RunTests(args.Length > 1 ? args[1] : null);

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
                Console.WriteLine(ex);
            }
            finally {
                //Console.ReadLine();
            }
        }

        static void CopyToLocalTmp(string root) {
			root = string.IsNullOrEmpty(root)?".":root;
#if DEBUG
			Console.WriteLine("Copying to local tmp...", TMP);
			Console.WriteLine("tmp:  '{0}'", TMP);
			Console.WriteLine("root: '{0}'", root);
#endif
            if (!Directory.Exists(TMP)){
#if DEBUG
				Console.WriteLine("Creating tmp dir '{0}'", TMP);
#endif
                Directory.CreateDirectory(TMP);
			}

            Directory.GetFiles(root, "*.dll").Each(
                f => {
				 var to = Path.Combine(TMP, Path.GetFileName(f));
#if DEBUG
					Console.WriteLine("copying '{0}' to {1}", f, to);
#endif
					File.Copy(f, to, true);
				});
#if DEBUG
			Console.WriteLine("Done!");
#endif
        }

        static void RunTests(string assmFileName, string cerryPicking=null, bool printHeaders=true) {
            Console.WriteLine("\nConfiguring Assembies....");

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

            Console.WriteLine("\nDone!\n");
            runner.Run(suite, cerryPicking, printHeaders );
        }

        static void PrintHelp() {
            Print("Commands");
            Print("exit | q => Quit.");
            Print("help | h => Show help.");
            Print("run  | r AssmFileName => Run all tests within the given file.");
        }

        static void Print(IEnumerable<string> lines) {
            lines.Each(Console.WriteLine);
        }

        static void Print(string msg, params object[] args) {
            Console.WriteLine(msg, args);
        }
    }
}
