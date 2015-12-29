
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

		class Expectation  : IExpect {
			readonly object _val;

			public Expectation(object val) {
				_val = val;
			}

			public Expectation(Action callback) {
				_val = callback;
			}

			static int Comp(object left, object right) {
				if (!(left is IComparable))
					Die($"Don't know of to compare { left?.GetType() } to { right?.GetType() }");

				if (left == null)
					return right == null ? 0 : 1;

				return ((IComparable)left).CompareTo(right);
			}

			public void ToBeLessThan(object val) { 
				var msg = $"Expected less than {val}";
				Fluent.Assert(Comp(_val, val) < 0, msg);
		  	}

			public void ToBeLessThanOrEqual(object val) {
				var msg = $"Expected less than or equal {val}";
			   	Fluent.Assert(Comp(_val, val) <= 0, msg); 
			}

			public void ToBeGreaterThan(object val) { 
				var msg = $"Expected greater than {val}";
				Fluent.Assert(Comp(_val, val) > 0, msg);
		  	}

			public void ToBeGreaterThanOrEqual(object val) { 
				var msg = $"Expected greater or equal than {val}";
				Fluent.Assert(Comp(_val, val) >= 0, msg); 
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
