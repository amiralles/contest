using Contest.Core;
using static Contest.Core.Contest;

public class ContestInit {

	// For Shutdow use public instead. (So we check detection of
	// both modifiers).
	void Setup(Runner runner) {
#if DEBUG
		if (runner == null)
			Die("Internal error. Runner can't be null.");
#endif
		runner.Bag["legend"]="Rock or Bust!";
	}
}

