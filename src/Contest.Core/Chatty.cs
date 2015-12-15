
namespace Contest {
	using System;
	using Contest.Core;
	using Fluent = Contest.SyntaxSugar;
	using static Contest.Core.Contest;

	public interface IAssertion {
			void Is(object val);
			void IsNot(object val);
			void IsNull();
			void IsNotNull();
			void IsTrue();
			void IsFalse();
			// TODO: Exceptions
			// void Throws();
	
	}

	public interface IExpect {
			void ToBe(object val);
			void NotToBe(object val);
			// TODO: Exceptions
			// void ToThrow(object val);
	
	}

	public class Chatty {

		class Expectation  : IExpect {
			readonly object _val;

			public Expectation(object val) {
				_val = val;
			}

			public void ToBe(object val) {
				var emsg = $"Expected to be {_val}({_val?.GetType()}) but was {val} ({val?.GetType()}).";
				Fluent.Equal(_val, val, emsg);	
			}

			public void NotToBe(object val) {
				var emsg = $"Expected Not to be {val}.";
				Fluent.NotEqual(_val, val, emsg);	
			}
		
		}

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

		public static IExpect Expect(object val) {
			return new Expectation(val);
		}
	}
}
