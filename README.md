## contest
Contest is minimalistic testing framework bundled with a freaking fast console runner. In contrast with most popular testing frameworks, it's based on conventions and it doesn't require a whole lotta of attributes to identify test cases, setups, and so on and so forth... So in the end, the code reads almost like plain english.

Down below you’ll find a couple of examples on how to write tests using Contest. Just add a reference to **Contest.Core.dll** and you are pretty much ready to go.

_Please keep in mind this is a protoype and is not production ready (yet). It'll be relased in the near future tough. Stay tuned!_
```cs
	using _  = System.Action<Contest.Core.Runner>;

    /// Basic features.
    class Contest_101 {

		_ passing_test = assert => 
			assert.Equal(4, 2 + 2);

		_ failing_test = assert =>
			assert.Equal(5, 2 + 2);
	
		_ expected_exception_passing_test = test =>
			test.ShouldThrow<NullReferenceException>(() => {
				object target = null;
				var dummy = target.ToString();
				//================^ null reference ex.
			});
	
		_ expected_exception_failing_test = test =>
			test.ShouldThrow<NullReferenceException>(() => {
				//It doesn't throws; So it fails.
			});
    }
    
    // Per fixture setup/teardown.
    class Contest_102 {
		// Fixture setup.
		_ before_each = test => {
			User.Create("pipe");
			User.Create("vilmis");
			User.Create("amiralles");
		};

		// Fixture teardown.
		_ after_each = test => {
			User.Reset();
		};
		
		// Actual test cases.
		_ should_find_existing_users = assert => 
			assert.IsNotNull(User.Find("pipe"));

		_ should_return_null_when_cant_find_users = assert => 
			assert.IsNull(User.Find("not_exists"));

		_ should_add_new_users = assert => {
			User.Create("foo");
			assert.Equal(4, User.Count());
		};
    }

	// Per test case setup/teardown.
	class Contest_103 {
		_ before_echo = test => 
			test.Bag["msg"] = "Hello World!";

		_ after_echo = test => 
			test.Bag["msg"] = null;

        //actual test.
		_ echo = test => 
			test.Equal("Hello World!", Utils.Echo(test.Bag["msg"]));
	}

    //=================================================================
    // Dummies
    //=================================================================
	class Utils {
		public static Func<object, object> Echo = msg => msg;
	}
	
	public class User {	
		static readonly List<string> _users = new List<string>();
		public static Action Reset = () => _users.Clear();
		public static Action<string> Create = name => _users.Add(name);
		public static Func<int> Count = () => _users.Count;
		public static Func<string, object> Find = name => _users.FirstOrDefault(u => u == name);
	}
    //=================================================================
```
#### Contest API
I guess this section is selfexplanatory ;)

```
	//Assertions
	IsNull(value [, errMsg])
	IsNotNull(value [, errMsg])
	IsTrue(cond [, errMsg = null])
	IsFalse(cond [, errMsg = null])
	Equal(expected, actual [, errMsg])
	NotEqual(expected, actual [, errMsg])
	Assert(bool cond [, string errMsg]

	// Errors checking.
	ErrMsgContains(text, action);
	ErrMsg(msg, action);
	Throws<TypeOfException>(action);
	ShouldThrow<TypeOfException>(action);

	// Utily methods.
	// Increases the failing tests count and prints the err msg.
	Fail(errMsg);

	// Increases the passing tests count.
	Pass();
```
		
#### A word about conventions
As I mentioned earlier, contest it's based on conventions, so you don't have to deal with noisy annotations and stuff like that. It follows a basic set of rules that are easy to remember.

##### For test cases
**Every field of type System.Action\<Contest.Core.Runner\> within a given class is considered to be a test case**. As you saw in the previous samples, neither the class containing the field nor the filed itself have to be public. This is just for convenience. I like to save as much keystrokes as I can, but it will work with the public modifier as well.

##### For setups and teardowns
If you 've been doing unit testing for a while you surely had notice that most frameworks have some kind of **setup/teardown** mechanisms (using NUnit jargon in here). Contest is not an exception, it have both, **per case** and **per fixture** setups and teardowns. The way it works is you name the field **"before_each"** for fixture wide setups and **"after_each"** for fixture wide teardowns. If you wanna per case setup/teardown, what you do is create a field and prefix its name with: **before_[case_name]** for setups and **after_[case_name]** for teardowns.
And remember, in all cases, **fields type must be System.Action\<Contest.Core.Runner\>**. Otherwise, it ain't gonna work.

#### Cherry picking
##### You can use wildcards to ask Contest to filter your fixtures and only run matching tests.
```bash
contest run test.dll *test_name_contains*
contest run test.dll test_name_starts_with*
contest run test.dll *test_name_ends_with
```

##### How to ignore tests using .test\_ignore file.
**TODO:

#### How to install
Obviously, you can clone the repo, build from sources and get the binaries. But you can also get [contest from nuget](https://www.nuget.org/packages/Contest/)

#### How to run *Contest*
The easiest way to run Contest, it's by adding _contest.exe_ to your path. Once you 've done that, you can go to whatever directory you want and just execute: **contest run test\_cases.dll**

#### What the hell is that underscore thing?
A cool thing you can do to save even more keystrokes, is to **alias** the type **System.Action\<Contest.Core.Runner\>** to **_** (or whatever you like). That's what I did in the samples above to get more readable test code (and nobody cares about test cases's return types anyways ;)).


