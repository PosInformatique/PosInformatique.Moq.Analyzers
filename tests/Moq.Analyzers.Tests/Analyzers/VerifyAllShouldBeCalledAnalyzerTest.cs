//-----------------------------------------------------------------------
// <copyright file="VerifyAllShouldBeCalledAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyAllShouldBeCalledAnalyzer>;

    public class VerifyAllShouldBeCalledAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                void Method();
            }

            public class Generic<T> { }

            public class MockHolder
            {
                public Mock<I> mock = new Mock<I>();
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.VerifyAll();
            """,

            """
            var mock1 = new Mock<I>();
            var mock2 = new Mock<I>();
            Mock.VerifyAll(mock1, mock2);
            """,

            """
            var mock1 = new Mock<I>();
            var mock2 = new Mock<I>();
            var mock3 = new Mock<I>();
            mock1.VerifyAll();
            Mock.VerifyAll(mock2, mock3);
            """,

            """
            var mock = new Mock<I>();
            mock.CallBase = false;
            mock.VerifyAll();
            """,

            """new Mock<I>();""",

            """new System.Action(() => { })();""",

            """
            var o = new object();
            o.ToString();
            """,

            """
            var o = new Generic<I>();
            o.ToString();
            """,

            """var holder = new MockHolder();""",
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """var mock = {|PosInfoMoq1000:new Mock<I>()|};""",

            """var mock = {|PosInfoMoq1000:new Mock<I>(MockBehavior.Strict)|};""",

            """
            var mock1 = {|PosInfoMoq1000:new Mock<I>()|};
            var mock2 = {|PosInfoMoq1000:new Mock<I>()|};
            new Mock<I>();
            """,

            """var mock = {|PosInfoMoq1000:new Mock<I>(1, 2, 3)|};""",

            """var mock = {|PosInfoMoq1000:new Mock<I>("A", "B", "C")|};""",
        ];

        [Theory]
        [MemberData(nameof(ValidTestCases))]
        [MemberData(nameof(InvalidTestCases))]
        public async Task Analyze(string code)
        {
            var source =
                $$"""
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            {{code}}
                        }
                    }

                    {{TestTypes}}
                }
                """;

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [MemberData(nameof(ValidTestCases))]
        [MemberData(nameof(InvalidTestCases))]
        public async Task NoMoqLibrary(string code)
        {
            var source =
                $$"""
                namespace ConsoleApplication1
                {
                    using FakeMoqLibrary;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            {{code}}
                        }
                    }

                    {{TestTypes}}
                }

                {{MoqAnalyzersTests.FakeMoqLibrary}}
                """;

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}
