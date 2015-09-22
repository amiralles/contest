using _ = System.Action<Contest.Core.Runner>;
using static System.Console;

/*
class feature_proposal {
    // runs only once before any test within the containing class.
	_ before_any = test => { ; };

	// runs only once per class upon fixture completion. 
	_ after_all  = test => {; };
}
*/

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
