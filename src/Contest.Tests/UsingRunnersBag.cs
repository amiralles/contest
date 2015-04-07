namespace Contest.Tests {
    using System;
    using System.Linq;
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class UsingRunnersBag {
        static Func<object, object> Echo = msg => msg;

        internal class EchoTest {

            Action<Runner> before_echo = test =>
                test.Bag["msg"] = "Hello World!";

            Action<Runner> after_echo = test =>
                test.Bag["msg"] = null;

            Action<Runner> echo = test =>
                test.Equal("Hello World!", Echo(test.Bag["msg"]));	
        }

        [Test]
        public void ConfigureTestVariablesDuringSetup(){
            var runner = new Runner();
            var finder = new TestCaseFinder();
            var suite = Contest.FindCasesInAssm(finder, typeof(EchoTest).Assembly, null);

			//In this particular case I only care about the echo test.
            var cases = (from c in suite.Cases
                         where c.Name == "echo"
                         select c).ToList();

            runner.Run(cases);
            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.PassCount);
        }
    }
}