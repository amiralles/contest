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
            finally {
            }
        }

        static void CopyToLocalTmp(string root) {
            if (!Directory.Exists(TMP))
                Directory.CreateDirectory(TMP);

            Directory.GetFiles(root, "*.dll").Each(
                f => File.Copy(f, Path.Combine(TMP, Path.GetFileName(f)), true));
        }

        static void RunTests(string assmFileName) {
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
