
namespace Contest.Core {
	using System;
	using Fluent = SyntaxSugar;
	using static Contest;

	public class Chatty {

		class Assertion : IAssertion {
			readonly object _val;

			public Assertion(object val) { 
				_val = val; 
			}

			public void Is(object val) {
				Fluent.Equal(_val, val);
			}

			public void IsNot(object val) {
				Fluent.NotEqual(_val, val);
			}

			public void IsNull() {
				Fluent.IsNull(_val);
			}

			public void IsNotNull() {
				Fluent.IsNotNull(_val);
			}

			public void IsTrue() {
				Fluent.IsTrue(GetBoolOrDie(_val));
			}

			public void IsFalse() {
				Fluent.IsFalse(GetBoolOrDie(_val));
			}

			static bool GetBoolOrDie(object val) {
				if (val is bool)
					return (bool) val;
				else if(val is IConvertible)
					return Convert.ToBoolean(val);

				Die($"{val} is not a bool value.");
				return false;
			}

		}

		public static IAssertion That(object val) {
			return new Assertion(val);
		}

	}
}
