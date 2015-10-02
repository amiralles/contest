## How to write contest core test cases
Test cases are splitted into two different assemblies. One of them 
containing *standard* test cases (The same kind of test than end users will write) and the other containing the *inception* tests cases. Mostly, especial test cases that run standard test cases. (Hence inception tests ;)) 
If you gotta write a core test case, chances are you gotta write two test cases, one (the actual test) somewhere within Contest.Test.* namespace and the other (the inception test) inside _contest_core_test that will run the actual test case.

## How to run contest's tests
To run contest's core tests cases you gotta use *Inception.Test.Runner.exe*,
you cannot run these tests by using the standard contest's binaries nor
any standard procedure.
*Inception* is a special test runner designed to run contest's tests cases by using contest itself. You can use it in the same way you use contest.exe but some flags maybe disabled. (i.e. Verbosity flags).


