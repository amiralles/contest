﻿// ReSharper disable UnusedMember.Local
#pragma warning disable 414

namespace Contest.Tests {
    using System;
    using Core;

    class FooTest{
        Action<Runner> before_foo = runner => {};
        Action<Runner> after_foo  = runner => {};
        Action<Runner> foo = assert => assert.Equal(1, 2);
    }
}
