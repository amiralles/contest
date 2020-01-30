// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Local
#pragma warning disable 414

namespace Contest.Tests {
    using System;
    using Core;

    class Wrapper {
        class Nested {
            Action<Runner> ThisIsATest = runner =>
                runner.Assert(1 == 2, "Something went wrong....");

            void ThisIsNotATestMethod() { }
        }
    }
}
