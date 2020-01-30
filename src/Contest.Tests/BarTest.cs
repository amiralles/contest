// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Local
#pragma warning disable 414

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
