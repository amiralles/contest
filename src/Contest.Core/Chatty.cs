
namespace Contest {
	using System;
	using Contest.Core;
	using Fluent = Contest.SyntaxSugar;
	using static Contest.Core.Contest;
	using static System.Console;

	public interface IAssertion {
			void Is(object val);
			void IsNot(object val);
			void IsNull();
			void IsNotNull();
			void IsTrue();
			void IsFalse();
			// TODO: Exceptions
			// void Throws();
			// TODO: greateThen, lessThan, lessThanOrEq, greaterThanOrEq.
	
	}

	public interface IExpect {
			void ToBe(object val);
			void NotToBe(object val);
			void ToThrow<T>() where T : Exception;
			void ErrMsg(string errMsg);
			void ErrMsgContains(string errMsg);

			// TODO: greateThen, lessThan, lessThanOrEq, greaterThanOrEq.
	
	}

	public static class BDD {
		public static IExpect Expect(Action val) {
			return new Expectation(val);
		}

		public static IExpect Expect(object val) {
			return new Expectation(val);
		}

		public static void ToBe(this object expect, object val) {
			Die("Not impl");
		}

		public static void NotToBe(this object expect, object val) {
			Die("Not impl");
		}
		
		//TODO: Add the rest of the API.

		class Expectation  : IExpect {
			readonly object _val;

			public Expectation(object val) {
				_val = val;
			}

			public Expectation(Action callback) {
				_val = callback;
			}

			public void ToBe(object val) {
				var emsg = $"Expected to be {_val}({_val?.GetType()}) but was {val} ({val?.GetType()}).";
				Fluent.Equal(_val, val, emsg);	
			}

			public void NotToBe(object val) {
				var emsg = $"Expected Not to be {val} (val?.GetType()).";
				Fluent.NotEqual(_val, val, emsg);	
			}

			public void ToThrow<T>() where T : Exception {
				var cb = _val as Action;
				DieIf(cb == null, "ToThrow expects a callback. ie. Expect(()=>{/* your code */}).ToThrow..");
				Fluent.Throws<T>(cb);
			}

			public void ErrMsg (string errMsg) {
				var cb = _val as Action;
				DieIf(cb == null, "ErrMsg expects a callback. ie. Expect(()=>{/* your code */}).ErrMsg..");
				Fluent.ErrMsg(errMsg, cb);
			}

			public void ErrMsgContains (string errMsg){
				var cb = _val as Action;
				DieIf(cb == null, "ErrMsgContains expects a callback. ie. Expect(()=>{/* your code */}).ErrMsgContains...");

				Fluent.ErrMsgContains(errMsg, cb);
			}
	
		}
	}

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
