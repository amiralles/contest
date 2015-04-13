# contest
Contest is blend of a console test runner and a minimalist testing framework. In contrast with most popular testing frameworks, Contest it's based on conventions, so it doesn't require a whole lotta of attributes to identify tests cases, fixtures, setups and so on and so forth. In the end, this means there are no syntax’s noise in your test code; hence it's more readable and easy to mantein.

Contest it’s designed to be both lightweight and easy to use. Assuming you are comfortable writing functional C#, you will get a huge productivity boost from this library. The syntax is neat and right to the point. And a bonus feature, the test runner is freaking fast ;)

Down below you’ll find couple of samples on how to write tests using contest.

To run the demo test suite included in this project, just clone the repo, build Contest.sln and run:  contest r contest.demo.dll from your command prompt.

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

		
### A word about conventions
As I mentioned earlier, contest it's based on conventions so you don't have to deal with noisy annotations and stuff like that to make your tests work. Contest follows a basic set of rules that I hope are easy to remember.  
**Every field of type System.Action<Contest.Core.Runner> within a given assembly is considered to be a test case**. As you can see in the samples, neither the class containing the field nor the filed itself have to be public. This is just for convenience; I like to save as much keystrokes as I can, but if you like to mark you classes or test cases as public, it's not a problem, that whill work too.

#### How about setups and teardowns?
Well, if you been doing unit testing for a while you surely had notice that most frameworks have some kind of **setup/teardown** mechanisms (using NUnit jargon here). Contest is not an exception, it have both, per case and per fixture setups and teardowns. The way it works is you name the field **"before_each"** for fixture wide setups and **"after_each"** for fixture wide teardowns. If you wanna per case setup/teardown, what you do is create a field and prefix its name with: **before_[case_name]** for setups and **after_[case_name]** for teardowns.
And remember, in all cases, **fields type must be System.Action<Contest.Core.Runner>**. Otherwise, it ain't gonna work.

### Closing tip:
A cool thing you can do to save even more keystrokes, is to **alias** the type **System.Action<Contest.Core.Runner>** to **_** (or whatever you like). That's what I did in the samples above, it makes test cases more readable and nobody cares about test cases return types anyways.

**Thanks for reading! And lemme know if you have any trouble using this library**
