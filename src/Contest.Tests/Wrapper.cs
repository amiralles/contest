namespace Contest.Tests {
    using System;
    using Core;
    // ReSharper disable UnusedMember.Local
    //This is a trick to reduce syntax noise.
    

    class Wrapper {
        class Nested {
            Action<Runner> ThisIsATest = runner =>
                runner.Assert(1 == 2, "Something went wrong....");

            void ThisIsntATestMethod() { }
        }
    }
}