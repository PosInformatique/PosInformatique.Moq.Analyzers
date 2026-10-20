//-----------------------------------------------------------------------
// <copyright file="VerifyShouldBeCalledForVerifiableSetupAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyShouldBeCalledForVerifiableSetupAnalyzer>;

    public class VerifyShouldBeCalledForVerifiableSetupAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                void Method();

                void OtherMethod();

                void Method(int a, int b);

                void Method(string c, int d);
            }

            public class CustomMock<T>
            {
                public void Setup() { }
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.Setup(m => m.Method());
            """,

            """
            var mock = new CustomMock<I>();
            mock.Setup();
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.Setup(m => m.Method()).{|PosInfoMoq1009:Verifiable|}();
            mock.Verify();
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.Method()).{|PosInfoMoq1009:Verifiable|}();
            var mock2 = new Mock<I>();
            mock2.Setup(m => m.Method()).{|PosInfoMoq1009:Verifiable|}();
            Mock.Verify(mock1, mock2, default);
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.Method()).{|PosInfoMoq1009:Verifiable|}();
            mock.Setup(m => m.OtherMethod()).{|PosInfoMoq1009:Verifiable|}();
            mock.Verify(m => m.Method());
            mock.Verify(m => m.OtherMethod());
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.Method(1, 2)).{|PosInfoMoq1009:Verifiable|}();
            mock.Verify(m => m.Method(1, 2));
            mock.Verify(m => m.Method("A", 3));
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.Method()).{|PosInfoMoq1002:{|PosInfoMoq1009:Verifiable|}|}();
            var o = new object();
            var s = o.ToString();
            var length = s.Length;
            Mock.Verify();
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.Method()).{|PosInfoMoq1009:Verifiable|}();
            mock.Setup(m => m.OtherMethod()).{|PosInfoMoq1002:{|PosInfoMoq1009:Verifiable|}|}();
            mock.Verify(m => m.Method());
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.Method(1, 2)).{|PosInfoMoq1002:{|PosInfoMoq1009:Verifiable|}|}();
            mock.Verify(m => m.Method());
            mock.Verify(m => m.Method("A", 3));
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.Method()).{|PosInfoMoq1002:{|PosInfoMoq1009:Verifiable|}|}();
            mock.Verify(null);
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(null).{|PosInfoMoq1009:Verifiable|}();
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
