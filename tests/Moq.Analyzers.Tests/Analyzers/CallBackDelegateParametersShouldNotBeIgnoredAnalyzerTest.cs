//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateParametersShouldNotBeIgnoredAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<CallBackDelegateParametersShouldNotBeIgnoredAnalyzer>;

    public class CallBackDelegateParametersShouldNotBeIgnoredAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                void TestMethod();

                void TestMethod(string a);

                void TestMethod(string a, int b);
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod(It.IsAny<string>()))
                .Callback((string a) => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod(It.IsAny<string>(), It.IsAny<int>()))
                .Callback((string a, int b) => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod("OK", It.IsAny<int>()))
                .Callback((string _, int b) => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod(It.IsAny<string>(), 1234))
                .Callback((string a, int _) => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod());
            """,

            """
            var mock = new Mock<I>(MockBehavior.Strict);
            mock.Setup(m => m.TestMethod());
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod("OK", 1234));
            """,

            """
            var o = new object();
            o.ToString();
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod(It.IsAny<string>(), It.IsAny<int>()))
                .Callback(({|PosInfoMoq1004:string _|}, {|PosInfoMoq1004:int _|}) => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod("OK", It.IsAny<int>()))
                .Callback((string _, {|PosInfoMoq1004:int _|}) => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod(It.IsAny<string>(), 1234))
                .Callback(({|PosInfoMoq1004:string _|}, int _) => { });
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