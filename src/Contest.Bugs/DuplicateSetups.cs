using _ = System.Action<Contest.Core.Runner>;
using static System.Console;

class fixure_wide_setup_teardown {
	_ before_each = test => { WriteLine(">>>> fixture setup"); };
	_ after_each  = test => { WriteLine(">>>> fixture teardown"); };
	 

	_ foo = assert => assert.Pass();
	_ bar = assert => assert.Pass();
}

class case_specific_setup_teardown {

	_ before_foo = test => { WriteLine(">>>> foo setup"); };
	_ after_foo  = test => { WriteLine(">>>> foo teardown"); };

	_ foo      = assert => assert.Pass();
	_ baz	   = assert => assert.Pass();
	_ bazzinga = assert => assert.Pass();
}
