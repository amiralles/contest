
namespace Contest.Core {
	using System;

	public interface IAssertion {
			void Is(object val);
			void IsNot(object val);
			void IsNull();
			void IsNotNull();
			void IsTrue();
			void IsFalse();
			// TODO: Exceptions
			// TODO: greaterThan, lessThan, lessThanOrEq, greaterThanOrEq.
	
	}
}
