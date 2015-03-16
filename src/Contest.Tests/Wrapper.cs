namespace Contest.Test {
    // ReSharper disable UnusedMember.Local
    //This is a trick to reduce syntax noise.
	using _ = System.Action<Core.Runner>;

    class Wrapper {
        class Nested {
            _ ThisIsATest = runner =>
                runner.Assert(1 == 2, "Something went wrong....");

            void ThisIsntATestMethod() { }
        }
    }
}