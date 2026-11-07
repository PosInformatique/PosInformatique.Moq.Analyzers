//-----------------------------------------------------------------------
// <copyright file="MockClassCanBeUsedOnlyToMockNonSealedClassAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<MockClassCanBeUsedOnlyToMockNonSealedClassAnalyzer>;

    public class MockClassCanBeUsedOnlyToMockNonSealedClassAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I { }

            public class StandardClass { }

            public abstract class AbstractClass { }

            public sealed class SealedClass { }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """var mock = new Mock<I>();""",

            """var mock = new Mock<I>(MockBehavior.Strict);""",

            """var mock = new Mock<StandardClass>();""",

            """var mock = new Mock<AbstractClass>();""",

            """var obj = new object();""",
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """var mock = new Mock<{|PosInfoMoq2002:SealedClass|}>();""",

            """var mock = new Mock<{|PosInfoMoq2002:string|}>();""",
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