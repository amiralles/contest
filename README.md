# contest
Contest is blend of a console test runner and a minimalistic testing framework. In contrast with most popular testing frameworks it’s based on conventions and it doesn’t require a whole lotta of attributes to identify tests, fixtures, setups, etc., etc…  So the syntax’s noise is pretty low.

Contest it’s designed to be both lightweight and easy to use. Assuming you are comfortable writing functional C#, you will get a huge productivity boost from this library. The syntax is neat and right to the point. And a bonus feature, the test runner is freaking fast ;)

Down below you’ll find couple of samples on how to write tests using contest.

To run the demo test suite included in this project, just clone the repo, build Contest.sln and run:  contest r contest.demo.dll

_Please keep in mind this is a protoype and it's not production ready (yet). It'll be relased in the near future tough. Stay tuned!_
```cs
	using _  = System.Action<Contest.Core.Runner>;

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
```
