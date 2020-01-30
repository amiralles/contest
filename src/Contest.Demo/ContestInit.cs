using Contest.Core;
using static Contest.Core.Contest;
// ReSharper disable UnusedMember.Local

// ReSharper disable once CheckNamespace
// ReSharper disable once ClassNeverInstantiated.Global
public class Global {
	public static object Foo = null;
}

public class ContestInit {
	// For Shutdown use public instead. (So we check detection of
	// both modifiers).
	void Setup(Runner runner) {
#if DEBUG
		if (runner == null)
			Die("Internal error. Runner can't be null.");
#endif
		Global.Foo = "asd";
	}
}
