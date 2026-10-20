//-----------------------------------------------------------------------
// <copyright file="SetupSetAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<SetupSetAnalyzer>;

    public class SetupSetAnalyzerTest
    {
        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock1 = new Mock<I>();
            mock1.SetupSet<int>(i => i.TestProperty = 1234);
            """,

            // Ignored because obsolete by Moq.
            """
            var mock1 = new Mock<I>();
            mock1.SetupSet(i => i.TestProperty);
            """,

            // Invocation should be ignored.
            """
            var s = "The string";
            s.ToString();
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock1 = new Mock<I>();
            mock1.{|PosInfoMoq1005:SetupSet|}(i => i.TestProperty = 1234);
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
                        int TestProperty { get; set; }
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
                        int TestProperty { get; set; }
                    }
                }

                {{MoqAnalyzersTests.FakeMoqLibrary}}
                """;

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}