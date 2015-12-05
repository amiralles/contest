
namespace Contest.Core {
	using System;
	using System.Reflection;
	using static Contest;

	public class SpecialType {
		public Type Type;
		public MethodInfo Method;

		public SpecialType(Type t, MethodInfo mi) {
			DieIf(t == null || mi == null, "Must specify type and method info.");

			Type   = t;
			Method = mi;
		}
	}
}

