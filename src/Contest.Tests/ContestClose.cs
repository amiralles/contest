
using Contest.Core;
using static Contest.Core.Contest;

public class ContestClose {

	public void Shutdown(Runner runner) {
#if DEBUG
		if (runner == null)
			Die("Internal error. Runner can't be null.");
#endif
		runner.Bag["ClosingMsg"]="We salute you.";
	}
}

