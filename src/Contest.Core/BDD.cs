
namespace Contest.Core {
	using System;
	using Fluent = SyntaxSugar;
	using static Contest;
	using static System.Console;

	public static class BDD {
		public static IExpect Expect(Action val) {
			return new Expectation(val);
		}

		public static IExpect Expect(object val) {
			return new Expectation(val);
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
				var emsg = $"Expected to be {val}({val?.GetType()}) but was {_val} ({_val?.GetType()}).";
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
}
