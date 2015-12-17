
namespace Contest.Core {
	using System;

	public static class BDDExtensions {
		public static void ToBe(this object expected, object val) {
			BDD.Expect(expected).ToBe(val);
		}

		public static void NotToBe(this object expected, object val) {
			BDD.Expect(expected).NotToBe(val);
		}
	
	}
}
