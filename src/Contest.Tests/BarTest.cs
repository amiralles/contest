namespace Contest.Tests {
    using System;
    using Core;

    class BarTest{
        class NestedBarTest{
            Action<Runner> before_bar = runner => {};
            Action<Runner> after_bar  = runner => {};
            Action<Runner> bar = assert => assert.Equal(1, 2);
        }
    }
}
