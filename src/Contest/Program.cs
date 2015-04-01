namespace Contest {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core;

    class Program {
        static void Main(string[] args) {

            try {
                Print(args);
                if (!args.Any()) {
                    PrintHelp();
                    return;
                }

                var cmd = args[0];
                switch (cmd) {
                    case "run":
                    case "r":
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
        }

        static void RunTests(string assmFileName) {
            if (string.IsNullOrEmpty(assmFileName))
                throw new ArgumentException("Assembly name is required.");

            if (!assmFileName.EndsWith(".dll"))
                assmFileName = string.Format("{0}.dll", assmFileName);

            var fullpath = Path.GetFullPath(assmFileName);
            if (!File.Exists(fullpath)) {

				var pwd = Directory.GetCurrentDirectory();

				throw new IOException(
						string.Format("Pwd: {0}\n", pwd) + 
						string.Format("File not found. ('{0}')", assmFileName));
			}

            var assm = Assembly.LoadFile(fullpath);
            if (assm == null)
                throw new Exception(string.Format("Can't load assembly '{0}'.", assmFileName));

            var finder = new TestCaseFinder();
            var suite = Contest.FindCasesInAssm(finder, assm, null);
            var runner = new Runner();
            runner.Run(suite);
        }

        static void PrintHelp() {
            Print("Commands");
            Print("exit | q => Quit.");
            Print("help | h => Show help.");
            Print("(run | r) AssmFileName => Run Tests.");
        }

        static void Print(IEnumerable<string> lines) {
            lines.Each(Console.WriteLine);

        }

        static void Print(string msg, params object[] args) {
            Console.WriteLine(msg, args);
        }
    }
}
