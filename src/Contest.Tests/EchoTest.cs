namespace Contest.Tests {
	using System;
	using System.Linq;
	using Core;

	internal class EchoTest {
		static Func<object, object> Echo = msg => msg;

		Action<Runner> before_echo = test =>
			test.Bag["msg"] = "Hello World!";

		Action<Runner> after_echo = test =>
			test.Bag["msg"] = null;

		Action<Runner> echo = test =>
			test.Equal("Hello World!", Echo(test.Bag["msg"]));	
	}
}
