namespace Contest.Test {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class RunnerFixture {

        [Test]
        public void AssertMethod_ResultIsTrue() {
            var runner = new Runner();
            runner.Assert(1==1);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.PassCount);
        }

        [Test]
        public void AssertMethod_ResultIsFalse() {
            var runner = new Runner();
            runner.Assert(1==2);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.FailCount);
        }


        [Test]
        public void AssertEqualsMethod_NullValue_ResultIsTrue() {
            var runner = new Runner();
            runner.Equal(null,null);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.PassCount);
		}

        [Test]
        public void AssertEqualsMethod_NullValue_ResultIsFalse() {
            var runner = new Runner();
            runner.Equal(null,123);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.FailCount);
		}

        [Test]
        public void AssertEqualsMethod_ResultIsTrue() {
            var runner = new Runner();
            runner.Equal(1,1);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.PassCount);
		}

        [Test]
        public void AssertEqualsMethod_ResultIsFalse() {
            var runner = new Runner();
            runner.Equal(1,2);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.FailCount);
        }

        [Test]
        public void AssertNotEqualsMethod_ResultIsTrue() {
            var runner = new Runner();
            runner.NotEqual(1,2);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.PassCount);
		}

        [Test]
        public void AssertNotEqualsMethod_ResultIsFalse() {
            var runner = new Runner();
            runner.NotEqual(1,1);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.FailCount);
        }

        [Test]
        public void AssertIsNull_ResultIsTrue() {
            var runner = new Runner();
            runner.IsNull(null);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.PassCount);
        }

        [Test]
        public void AssertIsNull_ResultIsFalse() {
            var runner = new Runner();
            runner.IsNull(123);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.FailCount);
        }

        [Test]
        public void AssertIsNotNull_ResultIsTrue() {
            var runner = new Runner();
            runner.IsNotNull(123);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.PassCount);
        }

        [Test]
        public void AssertIsNotNull_ResultIsFalse() {
            var runner = new Runner();
            runner.IsNotNull(null);

            Assert.AreEqual(1, runner.AssertsCount);
            Assert.AreEqual(1, runner.FailCount);
        }

    }
}
