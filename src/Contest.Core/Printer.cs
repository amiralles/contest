namespace Contest.Core {
	using System;
	using System.Threading;
	using System.Collections.Generic;
	using System.Diagnostics;
	using static System.ConsoleColor;
	using static System.Console;

	class Printer{
		static readonly ConsoleColor Default;

		static Printer() {
			// Assumin the console's forecolor is untouch.
			Default = Console.ForegroundColor;
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

		public readonly static Action<string> PrintFixName = name => {
			WriteLine("".PadRight(40, '-'));
			WriteLine(name);
			WriteLine("".PadRight(40, '-'));
		};

		public readonly static Action<string, ConsoleColor> Print =
		   	(msg, color) => {
				var bak = Console.ForegroundColor;
				try {
					Console.ForegroundColor = color;
					Console.WriteLine(msg);
				}
				finally {
					Console.ForegroundColor = bak;
				}
		};
	}
}
