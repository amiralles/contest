## contest
Contest is a minimalist, cross-platform, unit testing framework for .NET. 

Contest comes bundled with a lightweight console test runner, 
and in contrast with most popular testing frameworks in the .NET space, it's based on conventions, 
so it doesn't require a whole lot of attributes to identify test cases, setups, and so on. Point in case the testing code 
reads almost like plain English. (And, of course, it runs on Linux and Mac as well!)

Down below, you’ll find a couple of examples of how to write tests using a contest. Just add 
a reference to **Contest.Core.dll**, and you will be pretty much ready to go.

As you will see in the examples below, **contest** supports a wide range of assertions and testing styles, 
which you can mix and match to meet your preferences. From fluent assertions to BDD (and everything in between), 
**contest** will help you find a style you enjoy while writing tests.

_*Note: While this is working code, it is not production-ready. It might get released in the near future, tough._


#### Fluent Assertions
```cs
// This using statement enables fluent assertions.
using static Contest.Core.Chatty;
using _  = System.Action<Contest.Core.Runner>;


// Some basic math
_ add_two_numbers = assert => That(2 + 2).Is(4);

// Login system
_ when_an_admin_usr_logs_in = assert => That(usr.IsAdmin).IsTrue();

_ regular_users_shouldnt_have_root_access = assert => That(regUsr.HasRootAccess).IsFalse();


```
        
#### BDD API
For those who like the BDD approach better, you may want to try the contest's BDD-inspired API.

```cs
using static Contest.Core.BDD;
using _  = System.Action<Contest.Core.Runner>;


// Some basic math
_ add_two_numbers   = expect => (2 + 2).ToBe(4);
_ mul_two_numbers   = expect => (2 * 3).NotToBe(5);

// Login system
_ when_an_admin_usr_logs_in = expect => usr.IsAdmin.ToBe(true);

_ regular_users_shouldnt_have_root_access = expect => regularUsr.HasRootAccess.ToBe(false);

// Alternative syntax. (It's handy to test exceptions and stuff like that).
_ cant_access_members_on_null_pointers = assert => {
    object obj = null;

    // You can go this way
    Expect(() => obj.ToString()).ToThrow<NullReferenceException>();

    // or this way
    Expect(() => obj.ToString()).ErrMsg("Object reference not set to an instance of an object");

    // or this other way
    Expect(() => obj.ToString()).ErrMsgContains("reference not set to an instance");
}

// You can also use:
// * ToBeGreatThan
// * ToBeGreatThanOrEqual
// * ToBeLessThanOrEqual
// * ToBeLessThan

// Comming soon:
// * NotToBeGreatThan
// * NotToBeGreatThanOrEqual
// * NotToBeLessThanOrEqual
// * NotToBeLessThan
```


#### Plain old "lambda syntax"
This is the original contest's syntax, and it works the same way it always did.

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

```



#### Wanna hack right away
Just go to https://github.com/amiralles/intro_contest and take Contest for a spin.



#### Syntax Sugar
If you like the lambda approach but also like to write as little code as possible, 
you can go with the contest's syntax sugar.

```cs
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

#### How to add assembly-level initialization code

Sometimes, you need to execute some initialization code before running any test case. With Contest, 
you can do that by adding a special type to your project. Add a new class called `ContestInit`, 
create a `Setup` method, and put the initialization code in it.

```cs
using Contest.Core;

public class ContestInit {
    void Setup(Runner runner) {
        runner.Bag["album"] = "Rock or Bust!";
    }
}
```
(*) To avoid false positives or a corrupt state, the Contest will abort the execution if this method fails.


#### How to add assembly-level cleanup code

Contest also allows you to run code when it finishes running tests. To do this, 
you must add another *special type* called **ContestClose**. In this case, you'll have to create 
a **Shutdown** method and put the cleanup code there.


```cs
using Contest.Core;

public class ContestClose {
    void Shutdown(Runner runner) {
        // Some cleanup code...
    }
}
```

\* Keep in mind that these special types are meant for **global (assembly-level) configuration**. If you need _test level_ or _class level_ configuration,
  use the before/after callbacks instead (as shown in the samples above).

#### How to add class-level initialization code
Use this technique when you want a piece of code to run once (and only once) before any test within the class.
*(We've been trying different approaches for this particular feature, but plain old constructors ended up being the best choice.)

```cs
class FooTest {
    public FooTest () {
        // Your init code goes in here.
    }
}

```

#### How to add class-level cleanup code
Just modify your test class to implement the IDisposable interface and put your cleanup code inside the dispose method.
Contest will execute this method before exiting the program.

```cs
class BarTest : IDisposable {
    public void Dispose() {
        // Cleanup code in here.
    }
}

```



#### How to install
Obviously, you can clone the repo, build from sources, and get the binaries. But you can also get [contest from nuget](https://www.nuget.org/packages/Contest/)

#### How to run
The easiest way to run Contest is by adding _contest.exe_ to your path. Once you've done that, you can go to whatever directory and just execute:

```sh
contest run test_cases.dll
```

#### How to debug test cases
If you were using VS + ReSharper or TestDriven.NET or similar tools, 
you would likely want to set a breakpoint and do some step-by-step debugging when you find a failing test, right?

Well, while contest doesn't have an integrated "VS experience", 
you still have the chance to debug your tests using a **switch** that will freeze the execution and let you attach the VS debugger.

Set a breakpoint as usual, run `contest` using the **-dbg** flag, attach the VS debugger, go back to the console, and hit [Enter].

```bash
 contest run test_cases.dll -dbg
```

I know, this is far from ideal, but if you are used to step-by-step debugger, this is "a" way to do it :)


#### Cherry picking
##### You can use wildcards to ask Contest to filter your fixtures and only run matching tests.
```bash
contest run test.dll *test_name_contains*
contest run test.dll test_name_starts_with*
contest run test.dll *test_name_ends_with
```


#### How to test your code under different culture settings
Often times you have to make sure that your code works under different regional settings. Some testing 
frameworks allow you to do that by adding data annotations to your test cases, something like this:

```cs
[Test]
[SetCulture("es-AR")]
public void FooTest() {
    // Your culture sensitve test case.
}
```

Although it works, it's really tedious. With Contest you can achieve the same thing without touching your code. 

```
# To test under es-AR.
contest run test.dll * -ci es_AR

# To test under en-US
contest run test.dll * -ci en_US
```

The **-ci** flag allows you to override the culture that contests will use to run your test cases. So, 
if you set that flag, the whole thing will run under that specific culture.

_Note: You can use wildcards to run just some test cases under a specific culture and leave the rest as is._

```
# Let's run just the tests defined in the Foo module under es-AR culture settings
contest run test.dll *Foo* -ci es_AR
```


#### How to rerun failing tests
Most test runners come with a handy feature that allows you to filter 
and run only test cases that failed in the previous run. (I used this feature a lot with ReSharper's test runner.) 
You can do this with contests, too; just add the **-f** flag, and you are all set.

```bash
 contest run test\_cases.dll -f
```
* You can also see failing tests without running them using the **-lf** flag.

```bash
 contest run test\_cases.dll -lf
```


#### How to look for slow tests
Whether you are fixing performance issues or just wanna speed up your tests, Contest can tell you where to start. Just run:

```bash
 contest run test/_cases.dll -yslow
```

The command above will print a list of test cases sorted by execution time. (Slowest tests first).
You can combine this command with less or any similar tool to get the top 10/20 cases that are worth investigating.

You can also list the fastest tests first. (OK, I don't see a point either, but still...)
```bash
 contest run test/_cases.dll -yfast
```


#### How to ignore tests using .test\_ignore file.
**TODO:


#### How to report issues
The best way to report an issue is by providing a failing test case. Which is dead simple if you use https://github.com/amiralles/intro_contest


**Whether if you have problems using this library, found a bug o wanna a new feature, feel free to contact me. I'll get back to you as soon as I can.**
