namespace Contest.Test {
    using System;
    using Core;
	using _ = System.Action<Contest.Core.Runner>;

    class BarTest{
        class NestedBarTest{
            _ before_bar = runner => {};
            _ after_bar  = runner => {};
            _ bar = assert => assert.Equal(1, 2);
        }
    }
}
