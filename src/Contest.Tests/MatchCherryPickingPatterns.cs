namespace Contest.Test {
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class MatchCherryPickingPatterns{
        [Test]
        public void Contains(){
            Assert.IsTrue("*ThisIsAn*".Match("ThisIsAnotherTest"));
        }
    }
}