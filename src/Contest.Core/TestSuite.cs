namespace Contest.Core {
    using System.Linq;
    using System.Collections.Generic;

    public class TestSuite {
        public readonly List<TestCase> Cases = new List<TestCase>();
	
		public TestSuite() {
		}

		public TestSuite(IEnumerable<TestCase> cases) {
			Cases = cases.ToList();
		}
    }
}
