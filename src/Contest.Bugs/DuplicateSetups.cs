using _ = System.Action<Contest.Core.Runner>;
using static System.Console;

class fix_one {
	_ before_each = test => {
		WriteLine(">>>> setup");
	};

	_ foo = assert => assert.Pass();
	_ bar = assert => assert.Pass();
}

class fix_two {

	_ baz	   = assert => assert.Pass();
	_ bazzinga = assert => assert.Pass();
}
