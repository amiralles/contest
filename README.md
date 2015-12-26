## contest
Contest is a minimalist, cross platform, unit testing framework for .NET. It comes bundled with a lightweight console test runner and  in contrast with most popular testing frameworks, it's based on conventions, so it doesn't require a whole lotta of attributes to identify test cases, setups, and so on... Point in case, the code reads almost like plain english.
(And of course, it runs on linux and mac as well!)

Down below you’ll find a couple of examples on how to write tests using contest. Just add a reference to **Contest.Core.dll** and you are pretty much ready to go.

As you will see in the examples below **contest** supports a wide range of assertions and testing styles which you can mix and match to meet your preferences. From fluent assertions to BDD (and everything in between) **contest** will help you to find a style that you enjoy while writing tests.

_*Note: While this is working code, is not production ready (yet). It'll be relased in the near future, tough. Stay tuned!_


#### Fluent Assertions
```
	// This using statement enables fluent assertions.
	using static Contest.Core.Chatty;
	
	// Some basic math
	_ add_two_numbers = assert => That(2 + 2).Is(4);

	// Login system
	_ when_an_admin_usr_logs_in = assert => That(usr.IsAdmin).IsTrue();

	_ regular_users_shouldnt_have_root_access = assert => That(regUsr.HasRootAccess).IsFalse();


```
		
#### BDD API
For those who like the BDD approach better, you may wanna try contest's BDD API.

```
	using static Contest.Core.BDD;
	
	// Some basic math
	_ add_two_numbers   = expect => (2 + 2).ToBe(4);
	_ mul_two_numbers   = expect => (2 * 3).NotToBe(5);

	// Login system
	_ when_an_admin_usr_logs_in = exect => usr.IsAdmin.ToBe(true);

	_ regular_users_shouldnt_have_root_access = expect => regularUsr.HasRootAccess.ToBe(false);

	// Alternative syntax. (It's handy to test exceptions and stuff like that).
	_ cant_access_members_on_null_pointers = assert => {
		object obj = null;

		// this way
		Expect(() => obj.ToString()).ToThrow<NullReferenceException>();

		// or this way
		Expect(() => obj.ToString()).ErrMsg("Object reference not set to an instance of an object");

		// or another way
		Expect(() => obj.ToString()).ErrMsgContains("reference not set to an instance");
	}

	// Comming soon:
	// * ToBeGreatThan
	// * ToBeGreatThanOrEqual
	// * ToBeLessThanOrEqual
	// * ToBeLessThan

	// * NotToBeGreatThan
	// * NotToBeGreatThanOrEqual
	// * NotToBeLessThanOrEqual
	// * NotToBeLessThan
```


#### Plain old "lambda syntax"
This is the original contest's syntax and it works the same as always did.

```
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

#### Syntax Sugar
If you like the lambda approach but also like to write as less code as possible, you can go with contest's syntax sugar.

```
	using _  = System.Action<Contest.Core.Runner>;

	// By adding this using statement you have access
    // to the whole constest API thru helper methods.
	using static Contest.Core.SyntaxSugar;

    class TestSomeSugar {
		
		// Instead of writing this:
		_ passing_test = assert => 
			assert.Equal(4, 2 + 2);

		// You can write this:
		_ passing_test = assert => Equal(4, 2 + 2);

		//*(It is less code and also reads better without the second assert word).

	}
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

	// Utility methods.
	// Increases the failing tests count and prints the err msg.
	Fail(errMsg);

	// Increases the passing tests count.
	Pass();
```



#### How to add assembly level initialization code

Sometimes you need to execute a piece of code before running any test case. With contest you can do that by adding a *special type* to your project. Just add a new class called **ContestInit**, create a **Setup** method and put the initialization code in it.

```
using Contest.Core;

public class ContestInit {
	void Setup(Runner runner) {
		runner.Bag["album"] = "Rock or Bust!";
	}
}
```
(*) To avoid false positives or corrupt state, contest will abort the execution if this method fails.


#### How to add assembly level cleanup code

Contest also allows you to run code when it finishes running tests. To do this you will need to add another *special type* called **ContestClose**. In this case you'll have to create a **Shutdown** method and put the cleanup code in there.


```
using Contest.Core;

public class ContestClose {
	void Shutdown(Runner runner) {
		// Some cleanup code...
	}
}
```

* Keep in mind that these *special types* are meant to used for **global, assmebly level configuration**. If you need _test level_ or _class level_ configuration, use the before/after callbacks instead. (As shown in the samples above).

#### How to add class level initialization code
Use this technique when you want a piece of code to run once (and only once) before any test within the class.
*(We 've been trying different approaches for this particular feature but in the end, plain old constructors ended up being the best choice).

```
	class FooTest {
		public FooTest () {
			// Your init code goes in here.
		}
	}

```

#### How to add class level cleanup code
Just modify your test class to implement the IDisposable interface and put your cleanup code inside the dispose method.
Contest will execute this method before exiting the program.

```
	class BarTest : IDisposable {

		public void Dispose() {
			// Cleanup code in here.
		}
	}

```



#### How to install
Obviously, you can clone the repo, build from sources and get the binaries. But you can also get [contest from nuget](https://www.nuget.org/packages/Contest/)

#### How to run
The easiest way to run contest, it's by adding _contest.exe_ to your path. Once you 've done that, you can go to whatever directory and just execute: **contest run test\_cases.dll**

#### How to debug test cases
If you were using VS + ReSharper or TestDriven.NET or similar tools, chances are that when you find a failing test you wanna place a breakpoint and do some step by step debugging, right?

Well, while contest doesn't have an integrated test runner, you still have the chance to debug your tests using a **switch** that will freeze the execution and let you attach the VS debugguer.
Just place a breakpoint as usual, run contest using the **-dbg** switch, attach the VS's debugger, and go back to the console and hit [Enter].

```bash
 contest run test\_cases.dll -dbg
```

I know this is far from ideal, but keep in mind that contest it's meant to be used from the console. (And I wanna keep it as lightweight as possible).
If you are using this library is probably because you are using a plain text editor, in which case you don't have a debugger anyways, so...


#### Cherry picking
##### You can use wildcards to ask Contest to filter your fixtures and only run matching tests.
```bash
contest run test.dll *test_name_contains*
contest run test.dll test_name_starts_with*
contest run test.dll *test_name_ends_with
```

#### How to rerun failing tests
Most tests runners comes with a handy feature that allows you to filter and run only test cases that had failed in the previous run. (I used this feature a lot with ReSharper's test runner). You can do this with contest too, just add the **-f** flag and you are all set.

```bash
 contest run test\_cases.dll -f
```
* You can also see failing tests without running them using the **-lf** flag.

```bash
 contest run test\_cases.dll -lf
```

#### How to ignore tests using .test\_ignore file.
**TODO:


#### How to look for slow tests
Weather if your are fixing performance issues or just wanna speed up your tests, contest can tell you where to start. Just run:

```bash
 contest run test/_cases.dll -yslow
```

The command above will print a list of test cases sorted by execution time. (Slowest tests first).
You can combine this command with *less* or any tool like that and get the top 10/20 cases that worth to look at.

You can also list fastest tests firts. (OK, I don't see a point either, but it was just two lines away ;))
```bash
 contest run test/_cases.dll -yfast
```


**Whether if you have problems using this library, found a bug o wanna a new feature, feel free to contact me. I'll be back at you as soon as a can.**
