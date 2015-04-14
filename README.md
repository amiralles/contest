## contest
Contest is blend of a console test runner and a minimalist testing framework. In contrast with most popular testing frameworks, **contest** it's based on conventions and it doesn't require a whole lotta of attributes to identify tests cases, fixtures, setups and so on. This means that you won't have any syntax noise in your test cases, hence the code will be more readable and, of course, easier to maintain.

Contest it’s designed to be both lightweight and easy to use. Assuming you are comfortable writing functional C#, you will get a productivity boost from this library.**The syntax is short, sweet and right to the point** and as a bonus feature, the test runner is freaking fast! ;)

Down below you’ll find a couple of examples that will show you how to write tests using **contest**.

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
		//fixture setup.
		_ before_each = test => {
			User.Create("pipe");
			User.Create("vilmis");
			User.Create("amiralles");
		};

		//fixture teardown.
		_ after_each = test =>
			User.Reset();
		
		//actual test cases.
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
		// echo case setup.
		_ before_echo = test => 
			test.Bag["msg"] = "Hello World!";

		// echo case teardown.
		_ after_echo = test => 
			test.Bag["msg"] = null;

		//actual echo test
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

		
#### A word about conventions
As I mentioned earlier, contest it's based on conventions so you don't have to deal with noisy annotations and stuff like that. It follows a basic set of rules that _I hope_ are easy to remember.

**Every field of type System.Action<Contest.Core.Runner> within a given assembly is considered to be a test case**. As you can see in the samples, neither the class containing the field nor the filed itself have to be public. This is just for convenience; I like to save as much keystrokes as I can, but if you like to mark you classes or test cases as public, it's not a problem, that will work too.

#### How about setups and teardowns?
Well, if you been doing unit testing for a while you surely had notice that most frameworks have some kind of **setup/teardown** mechanisms (using NUnit jargon in here). Contest is not an exception, it have both, **per case** and **per fixture** setups and teardowns. The way it works is you name the field **"before_each"** for fixture wide setups and **"after_each"** for fixture wide teardowns. If you wanna per case setup/teardown, what you do is create a field and prefix its name with: **before_[case_name]** for setups and **after_[case_name]** for teardowns.
And remember, in all cases, **fields type must be System.Action\<Contest.Core.Runner\>**. Otherwise, it ain't gonna work.

#### Cherry picking
##### How to use wildcards from the console to run or exclude some tests.
**TODO:
##### How to ignore tests using .test_ignore file.
**TODO:

#### Wanna try the whole thing by yourself:
* Clone the repo.
* Build Contest.sln.
* Run =>  **contest r contest.demo.dll** from your command prompt.

#### Closing tip:
A cool thing you can do to save even more keystrokes, is to **alias** the type **System.Action\<Contest.Core.Runner\>** to **_** (or whatever you like). That's what I did in the samples above and it made test cases more readable (and nobody cares about test cases return types anyways).


**Thanks for reading! And lemme know if you have any trouble using this library**
