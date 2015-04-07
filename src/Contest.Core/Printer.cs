namespace Contest.Core {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	class Printer{
		// void PrintResults(int casesCount, long elapsedMilliseconds) 
		public readonly static Action<int, long, int, int, int, int> PrintResults = 
			(casesCount, elapsedms, assertsCount, passCount, failCount, ignoreCount) => {
				Print("".PadRight(40, '='), ConsoleColor.White);
				Print("Test    : {0}".Interpol(casesCount), ConsoleColor.White);
				Print("Asserts : {0}".Interpol(assertsCount), ConsoleColor.White);
				Print("Elapsed : {0} ms".Interpol(elapsedms), ConsoleColor.White);
				Print("Passing : {0}".Interpol(passCount), ConsoleColor.Green);
				Print("Failing : {0}".Interpol(failCount), ConsoleColor.Red);
				Print("Ignored : {0}".Interpol(ignoreCount), ConsoleColor.Yellow);
				Print("".PadRight(40, '='), ConsoleColor.White);
			};

		public readonly static Action<string> PrintFixName = name => {
			Print("".PadRight(40, '='), ConsoleColor.Cyan);
			Print(name, ConsoleColor.Cyan);
			Print("".PadRight(40, '='), ConsoleColor.Cyan);
		};

		public readonly static Action<string, ConsoleColor> Print = (msg, color) => {
			var fcolor = Console.ForegroundColor;
			try {
				Console.ForegroundColor = color;
				Console.WriteLine(msg);
			}
			finally {
				Console.ForegroundColor = fcolor;
			}
		};
	}
}
