namespace Contest.Tests {
    using System;
    using System.IO;
    using Core;

    class TestClassThrowingTests {
        public Action<Runner> ThisThrowsAndPass = 
            runner => runner.ShouldThrow<IOException>(() => { throw new IOException(); });


        public Action<Runner> ThisDoesntThrowSoItFails = 
            runner => runner.ShouldThrow<IOException>(() => { /*.../*/ });

    }
}