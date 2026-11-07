//-----------------------------------------------------------------------
// <copyright file="VerifyStaticMethodsRequiresMockParametersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyStaticMethodsRequiresMockParametersAnalyzer>;

    public class VerifyStaticMethodsRequiresMockParametersAnalyzerTest
    {
        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock1 = new Mock<I>();
            Mock.Verify(mock1);
            """,

            """
            var mock1 = new Mock<I>();
            Mock.VerifyAll(mock1);
            """,

            """
            var mock1 = new Mock<I>();
            var mock2 = new Mock<I>();
            Mock.Verify(mock1, mock2);
            """,

            """
            var mock1 = new Mock<I>();
            var mock2 = new Mock<I>();
            Mock.VerifyAll(mock1, mock2);
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            Mock.{|PosInfoMoq1008:Verify()|};
            """,

            """
            Mock.{|PosInfoMoq1008:VerifyAll()|};
            """,
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

                    public interface I
                    {
                        int TestMethod();
                    }
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

                    public interface I
                    {
                        int TestMethod();
                    }
                }

                {{MoqAnalyzersTests.FakeMoqLibrary}}
                """;

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}