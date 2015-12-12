namespace Contest {
	using System;
	using Contest.Core;
	using static Contest.Core.Contest;

	public class SyntaxSugar {
		/// Points to the runner that is running the current session.
		static Runner _theRunner;

		public static void SetRunner(Runner theRunner) {
			if (_theRunner != null)
				Die($"Can't set TheRunner more than once.");

			if (theRunner == null)
				Die($"Can't set the TheRunner to null.");

			_theRunner = theRunner;
		}

		public static Runner GetRunner() {
			if (_theRunner == null)
				Die("Syntax sugar is disabled becuse 'TheRunner' is not configured\n." + 
						"You should call SetRunner fisrt.");

			return _theRunner;
		}

		public static void IsNull(object val, string errMsg = null) {
			GetRunner().IsNull(val, errMsg);
		}

		public static void IsNotNull(object val, string errMsg = null) {
			GetRunner().IsNotNull(val, errMsg);
		}

		public static void IsTrue(bool cnd, string errMsg = null) {
			GetRunner().IsTrue(cnd, errMsg);
		}

		public static void IsFalse(bool cnd, string errMsg = null) {
			GetRunner().IsFalse(cnd, errMsg);
		}

		public static void Equal(object left, object right, string errMsg = null) {
			GetRunner().Equal(left, right, errMsg);
		}

		public static void NotEqual(object left, object right, string errMsg = null) {
			GetRunner().NotEqual(left, right, errMsg);
		}

		public static void Assert(bool cnd, string errMsg = null) {
			GetRunner().Assert(cnd, errMsg);
		}

		public static void ErrMsgContains(string text, Action callback) {
			GetRunner().ErrMsgContains(text, callback);
		}

		public static void ErrMsg(string text, Action callback) {
			GetRunner().ErrMsg(text, callback);
		}

		public static void Throws<T>(Action callback) where T : Exception {
			GetRunner().Throws<T>(callback);
		}

		public static void ShouldThrow<T>(Action callback) where T : Exception {
			GetRunner().ShouldThrow<T>(callback);
		}

		public static void Fail(string errMsg) {
			GetRunner().Fail(errMsg);
		}

		public static void Pass() {
			GetRunner().Pass();
		}
	}
}
