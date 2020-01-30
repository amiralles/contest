
namespace Contest.Core {
	public static class BDDExtensions {
		public static void ToBeLessThan(this object expected, object val) {
			BDD.Expect(expected).ToBeLessThan(val);
		}

		public static void ToBeLessThanOrEqual(this object expected, object val) {
			BDD.Expect(expected).ToBeLessThanOrEqual(val);
		}

		public static void ToBeGreaterThan(this object expected, object val) {
			BDD.Expect(expected).ToBeGreaterThan(val);
		}

		public static void ToBeGreaterThanOrEqual(this object expected, object val) {
			BDD.Expect(expected).ToBeGreaterThanOrEqual(val);
		}

		public static void ToBe(this object expected, object val) {
			BDD.Expect(expected).ToBe(val);
		}

		public static void NotToBe(this object expected, object val) {
			BDD.Expect(expected).NotToBe(val);
		}
	
	}
}
