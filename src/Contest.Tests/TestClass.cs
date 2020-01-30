// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
#pragma warning disable 414

namespace Contest.Tests {
    using System;
    using Core;

    class TestClass {
        public Action<Runner> ThisIsATest = test =>
            test.Assert(true, "Something went wrong....");

        Action<Runner> ThisIsAnotherTest = test =>
            test.Assert(true, "Something went wrong....");

        public void ThisIsntATestMethod() { }

        void ThisIsntATestMethodEither() { }
    }
}
