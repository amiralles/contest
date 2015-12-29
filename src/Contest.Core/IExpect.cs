
namespace Contest.Core {
	using System;

	public interface IExpect {
		void ToBeLessThan(object val);
		void ToBeLessThanOrEqual(object val);
		void ToBeGreaterThan(object val);
		void ToBeGreaterThanOrEqual(object val);

		void ToBe(object val);
		void NotToBe(object val);
		void ToThrow<T>() where T : Exception;
		void ErrMsg(string errMsg);
		void ErrMsgContains(string errMsg);

	}
}

