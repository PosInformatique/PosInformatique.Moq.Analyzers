//-----------------------------------------------------------------------
// <copyright file="VerifyMustHaveTimesParameterAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyMustHaveTimesParameterAnalyzer>;

    public class VerifyMustHaveTimesParameterAnalyzerTest
    {
        public static TheoryData<string> ValidTestCases =>
        [
            """
            mock.Verify();
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.AtLeast(15));
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Exactly(1));
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Never);
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Never());
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Once);
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Once());
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.AtLeast(15), "Failed message ignored");
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Exactly(1), "Failed message ignored");
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Never, "Failed message ignored");
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Never(), "Failed message ignored");
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Once, "Failed message ignored");
            """,

            """
            mock.Verify(i => i.TestMethod(), Times.Once(), "Failed message ignored");
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.AtLeast(15));
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Exactly(1));
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Never);
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Never());
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Once);
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Once());
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.AtLeast(15), "Failed message ignored");
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Exactly(1), "Failed message ignored");
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Never, "Failed message ignored");
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Never(), "Failed message ignored");
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Once, "Failed message ignored");
            """,

            """
            mock.Setup(i => i.TestMethod()).Verifiable(Times.Once(), "Failed message ignored");
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            mock.{|PosInfoMoq1007:Verify|}(i => i.TestMethod());
            """,

            """
            mock.{|PosInfoMoq1007:Verify|}(i => i.TestMethod(), "Failed message ignored");
            """,

            """
            mock.Setup(m => m.TestMethod()).{|PosInfoMoq1007:Verifiable|}();
            """,

            """
            mock.Setup(m => m.TestMethod()).{|PosInfoMoq1007:Verifiable|}("Failed message ignored");
            """,

            """
            var strictMock = new Mock<I>(MockBehavior.Strict);
            strictMock.{|PosInfoMoq1007:Verify|}(i => i.Method());
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
                            var mock = new Mock<I>();

                            {{code}}
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        void Method();
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
                            var mock = new Mock<I>();

                            {{code}}
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        void Method();
                    }
                }

                {{MoqAnalyzersTests.FakeMoqLibrary}}
                """;

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}