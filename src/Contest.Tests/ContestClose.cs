
using Contest.Core;
using static Contest.Core.Contest;
// ReSharper disable CheckNamespace

public class ContestClose {

	public void Shutdown(Runner runner) {
#if DEBUG
		if (runner == null)
			Die("Internal error. Runner can't be null.");
#endif
		// ReSharper disable once PossibleNullReferenceException
		runner.Bag["ClosingMsg"]="We salute you.";
	}
}

