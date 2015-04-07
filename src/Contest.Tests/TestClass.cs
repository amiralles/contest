namespace Contest.Tests {
    using System;
    using Core;

    // ReSharper disable UnusedMember.Local
    class TestClass {
        public Action<Runner> ThisIsATest = test =>
            test.Assert(1 == 1, "Something went wrong....");

        Action<Runner> ThisIsAnotherTest = test =>
            test.Assert(1 == 1, "Something went wrong....");

        public void ThisIsntATestMethod() { }

        void ThisIsntATestMethodEither() { }
    }
}
