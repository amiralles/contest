using Contest.Core;
using static Contest.Core.Contest;

public class Global {
	public static string Foo = null;
}

public class ContestInit {

	// For Shutdow use public instead. (So we check detection of
	// both modifiers).
	void Setup(Runner runner) {
#if DEBUG
		if (runner == null)
			Die("Internal error. Runner can't be null.");
#endif
		Global.Foo = "Hello World!";
		runner.Bag["legend"]="Rock or Bust!";
	}
}

