namespace Demo { //It doesn't match naming conventions but it reads better in the console ;)
	// ReSharper disable UnusedMember.Local
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using _  = System.Action<Contest.Core.Runner>;

    class HandlingExceptions{
        _ it_should_treat_exceptions_as_failing_tests = assert => {
            throw new Exception("asd");
        };
    }

	class ErrorMsgs{
		_ exptect_error_message_pass = expects =>
			expects.ErrMsg("foo", ()=> {throw new Exception("foo");});

		_ exptect_error_message_fails = expects =>
			expects.ErrMsg("foo", ()=> {});

		_ exptect_error_message_contains_pass = expects =>
			expects.ErrMsgContains("foo", ()=> { throw new Exception("This is a foo error.");});

		_ exptect_error_message_contains_fails = expects =>
			expects.ErrMsgContains("foo", ()=> {});

	}

    class Contest_101 {

		_ this_is_a_passing_test = assert => 
			assert.Equal(4, 2 + 2);

		_ this_is_a_failing_test = assert =>
			assert.Equal(5, 2 + 2);

        _ this_is_a__should_throw__passing_test = test =>
            test.ShouldThrow<NullReferenceException>(() => {
                object target = null;
                var dummy = target.ToString();
            });

		_ this_is_a__should_throw__failing_test = test =>
			test.ShouldThrow<NullReferenceException>(() => {
				//It doesn't throws; So it fails.
			});
    }

    class Contest_201 {

		_ before_each = test => {
			User.Create("pipe");
			User.Create("vilmis");
			User.Create("amiralles");
		};

		_ after_each = test =>
			User.Reset();

		_ find_existing_user_returns_user = assert => 
			assert.IsNotNull(User.Find("pipe"));

		_ find_non_existing_user_returns_null = assert => 
			assert.IsNull(User.Find("not exists"));

		_ create_user_adds_new_user = assert => {
			User.Create("foo");
			assert.Equal(4, User.Count());
		};
    }

	class Contest_301 {
		// setup
		_ before_echo = test => 
			test.Bag["msg"] = "Hello World!";

		//cleanup
		_ after_echo = test => 
			test.Bag["msg"] = null;

		//actual test
		_ echo = test => 
			test.Equal("Hello World!", Utils.Echo(test.Bag["msg"]));
	}

	class Utils {
		public static Func<object, object> Echo = msg => msg;
	}

	public class User {	
		static readonly List<string> _users = new List<string>();

		public static Action Reset = () => _users.Clear();
			
		public static Action<string> Create = name => _users.Add(name);

		public static Func<int> Count = () => _users.Count;

		public static Func<string, object> Find = name =>
			_users.FirstOrDefault(u => u == name);
	}
}
