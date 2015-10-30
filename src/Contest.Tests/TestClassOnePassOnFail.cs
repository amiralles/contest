#pragma warning disable 414

namespace Contest.Tests {
    using System;
    using Core;

    class TestClassOnePassOnFail {
        public Action<Runner> ThisPass = runner =>
            runner.Assert(1 == 1, "Something went wrong....");

        Action<Runner> ThisFail = runner =>
            runner.Assert(1 == 2, "Something went wrong....");

    }
}
