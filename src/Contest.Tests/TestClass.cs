namespace Contest.Test {
    using System;
    using Core;

    // ReSharper disable UnusedMember.Local
    class TestClass {
        public Action<Runner> ThisIsATest = runner =>
            runner.Assert(1 == 2, "Something went wrong....");

        Action<Runner> ThisIsAnotherTest = runner =>
            runner.Assert(1 == 2, "Something went wrong....");

        public void ThisIsntATestMethod() { }

        void ThisIsntATestMethodEither() { }
    }
}