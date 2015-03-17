namespace Contest.Test {
    using Core;
	using _ = System.Action<Contest.Core.Runner>;


        public class DiscoverFixture {

		_ test_cases_in_assm = test =>{
            var cases = TestCaseFinder.FindCasesInAssm(typeof(TestClass).Assembly,null);
            test.Assert(7 == cases.Count);
		};

		_ test_cases_in_class = test=>{
            var cases = TestCaseFinder.FindCases(typeof(TestClass),null);
            test.Assert(2 == cases.Count);
		};

		_ test_cases_in_nested_classes = test=>{
            var cases = TestCaseFinder.FindCases(typeof(Wrapper),null);
            test.Assert(1 == cases.Count);
		};


    }
}
