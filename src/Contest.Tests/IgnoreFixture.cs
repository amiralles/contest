
namespace Contest.Test {
    using System.Linq;
    using NUnit.Framework;
    using Core;
    using Unbinder;

    public class IgnoreFixture {

        [TestFixture]
        class ReadIgnoreFile {
            [Test]
            public void EmptyFile() {
                IgnoreFileReader.ReadAllLines = () => null;
                Assert.AreEqual(new string[0], new TestCaseFinder().GetIgnoredPatternsFromFile());
            }

            [Test]
            public void CommaSepValues() {
                IgnoreFileReader.ReadAllLines = () =>
                    new[] { "*foo, *bar*, test*" };

                Assert.AreEqual(3, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
            }

            [Test]
            public void ColonSepValues() {
                IgnoreFileReader.ReadAllLines = () =>
                    new[] { "*foo; *bar*; test*" };

                Assert.AreEqual(3, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
            }

            [Test]
            public void SpaceSepValues() {
                IgnoreFileReader.ReadAllLines = () =>
                    new[] { "*foo *bar* test*" };

                Assert.AreEqual(3, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
            }

            [Test]
            public void NewLineSepValues() {
                IgnoreFileReader.ReadAllLines = () =>
                    "*foo\n *bar*\n test*\n".Split('\n');

                Assert.AreEqual(3, new TestCaseFinder().GetIgnoredPatternsFromFile().Length);
            }

            //=============================================================
            //Misc.
            [Test]
            public void WhenSplitTrimWhiteSpaces() {
                IgnoreFileReader.ReadAllLines = () =>
                    new[] { "  *foo,    *bar*,    test*  " };
                var patterns = new TestCaseFinder().GetIgnoredPatternsFromFile();
                Assert.AreEqual(0, patterns.Count(p => p.Contains(' ')));
            }
            //=============================================================

        }

        [TestFixture]
        class PatternsFromIgnoreFile {

            [Test]
            public void ignore_all_cases() {
                var cases = Contest.FindCases(
                    new TestCaseFinder(getIgnoredFromFile: () => new[] { "*" }),
                    typeof(TestClass),
                    null);

                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(2, runner.IgnoreCount, "Fail IgnoreCount");
            }


            [Test]
            public void ignore_cases_ending_with() {

                var cases = Contest.FindCases(
                    new TestCaseFinder(() => new[] { "*AnotherTest" }),
                    typeof(TestClass),
                    null);

                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
            }


            [Test]
            public void ignore_cases_starting_with() {

                var cases = Contest.FindCases(
                    new TestCaseFinder(getIgnoredFromFile: () => new[] {
					    "Contest.Test.TestClass.ThisIsAT*"
					}),
                    typeof(TestClass),
                    null);

                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");

            }
            [Test]
            public void ignore_cases_when_contains() {

                var cases = Contest.FindCases(
                    new TestCaseFinder(getIgnoredFromFile: () => new[] { "*Tes*" }),
                    typeof(TestClass),
                    null);

                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(2, runner.IgnoreCount, "Fail IgnoreCount");

            }

            [Test]
            public void ignore_no_cases() {

                var cases = Contest.FindCases(
                    new TestCaseFinder(getIgnoredFromFile: () => null),
                    typeof(TestClass),
                    null);

                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(0, runner.IgnoreCount, "Fail IgnoreCount");
            }

        }

        [TestFixture]
        class PatternsFromCmdLine {

            [Test]
            public void ignore_all_cases() {
                var cases = Contest.FindCases(
                    new TestCaseFinder(),
                    typeof(TestClass),
                    "*");

                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(2, runner.IgnoreCount, "Fail IgnoreCount");
            }

            [Test]
            public void ignore_cases_ending_with() {
                var cases = Contest.FindCases(
                    new TestCaseFinder(),
                    typeof(TestClass),
                    "*AnotherTest");

                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
            }

            [Test]
            public void ignore_cases_starting_with() {

                var cases = Contest.FindCases(
                    new TestCaseFinder(),
                    typeof(TestClass),
                    "Contest.Test.TestClass.ThisIsAT*");

                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(1, runner.IgnoreCount, "Fail IgnoreCount");
            }

            [Test]
            public void ignore_cases_when_contains() {

                var cases = Contest.FindCases(new TestCaseFinder(), typeof(TestClass), "*Tes*");
                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(2, runner.IgnoreCount, "Fail IgnoreCount");

            }

            [Test]
            public void ignore_no_cases() {

                var cases = Contest.FindCases(new TestCaseFinder(), typeof(TestClass), null);
                var runner = new Runner();
                runner.Run(cases);

                Assert.AreEqual(2, runner.TestCount, "Fail TestCount");
                Assert.AreEqual(0, runner.IgnoreCount, "Fail IgnoreCount");
            }

        }
    }
}
