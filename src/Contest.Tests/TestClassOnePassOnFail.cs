namespace Contest.Test {
    using System;
    using Core;
    // ReSharper disable UnusedMember.Local
    // ReSharper disable EqualExpressionComparison

    class TestClassOnePassOnFail {
        public Action<Runner> ThisPass = runner =>
            runner.Assert(1 == 1, "Something went wrong....");

        Action<Runner> ThisFail = runner =>
            runner.Assert(1 == 2, "Something went wrong....");

    }
}