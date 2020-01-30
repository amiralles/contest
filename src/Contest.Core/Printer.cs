using System;
using System.Threading;

namespace Contest.Core {
	using static ConsoleColor;
	using static Console;

	static class Printer{
		static Printer() {
			// Assumin the console's forecolor is untouch.
		}

		public readonly static Action<int, long, int, int, int, int, string> PrintResults = 
			(casesCount, elapsedms, assertsCount, passCount, failCount, ignoreCount, cherry) => {
				WriteLine("".PadRight(40, '-'));
				WriteLine("ASSERTS - STATS");
				if(!string.IsNullOrEmpty(cherry)){
					WriteLine("".PadRight(40, '-'));
					WriteLine($"Cherry Picking => {cherry}");
				}
				WriteLine($"Culture {Thread.CurrentThread.CurrentCulture}");
				WriteLine("".PadRight(40, '-'));

				WriteLine($"Elapsed : {elapsedms} ms");
				Print($"Passing : {passCount}",    Green);
				Print($"Failing : {failCount}",    Red);
				Print($"Ignored : {ignoreCount}",  Yellow);
				WriteLine("".PadRight(40, '-'));
			};

		public static readonly Action<string> PrintFixName = name => {
			WriteLine("".PadRight(40, '-'));
			WriteLine(name);
			WriteLine("".PadRight(40, '-'));
		};

		public static readonly Action<string, ConsoleColor> Print =
		   	(msg, color) => {
				var bak = ForegroundColor;
				try {
					ForegroundColor = color;
					WriteLine(msg);
				}
				finally {
					ForegroundColor = bak;
				}
		};
	}
}
