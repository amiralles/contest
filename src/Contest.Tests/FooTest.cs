namespace Contest.Test {
    using System;
    using Core;
	using _ = System.Action<Contest.Core.Runner>;

    class FooTest{
        _ before_foo = runner => {};
//        _ after_foo  = runner => {};
        _ foo = assert => assert.Equal(1, 2);
    }
}