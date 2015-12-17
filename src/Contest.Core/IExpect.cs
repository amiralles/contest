
namespace Contest.Core {
	using System;

	public interface IExpect {
		void ToBe(object val);
		void NotToBe(object val);
		void ToThrow<T>() where T : Exception;
		void ErrMsg(string errMsg);
		void ErrMsgContains(string errMsg);

		// TODO: greatThen, lessThan, lessThanOrEq, greatThanOrEq.
	}
}

