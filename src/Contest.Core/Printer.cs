namespace Contest.Core {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	class Printer{
		static ConsoleColor White  = ConsoleColor.White;
		static ConsoleColor Red    = ConsoleColor.Red;
		static ConsoleColor Green  = ConsoleColor.Green;
		static ConsoleColor Yellow = ConsoleColor.Yellow;

		// void PrintResults(int casesCount, long elapsedMilliseconds) 
		public readonly static Action<int, long, int, int, int, int, string> PrintResults = 
			(casesCount, elapsedms, assertsCount, passCount, failCount, ignoreCount, cherry) => {
				Print("".PadRight(40, '='), White);
				Print("STATS", White);
				if(!string.IsNullOrEmpty(cherry)){
					Print("".PadRight(40, '='), White);
					Print("Cherry Picking => {0}".Interpol(cherry), White);
				}
				Print("".PadRight(40, '='), White);
				Print("Test    : {0}".Interpol(casesCount), White);
				Print("Asserts : {0}".Interpol(assertsCount), White);
				Print("Elapsed : {0} ms".Interpol(elapsedms), White);
				Print("Passing : {0}".Interpol(passCount), Green);
				Print("Failing : {0}".Interpol(failCount), Red);
				Print("Ignored : {0}".Interpol(ignoreCount), Yellow);
				Print("".PadRight(40, '='), White);
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
