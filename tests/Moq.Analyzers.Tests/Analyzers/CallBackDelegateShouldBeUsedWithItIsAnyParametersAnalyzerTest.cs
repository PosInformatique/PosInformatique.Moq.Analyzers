//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer>;

    public class CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzerTest
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
            mock.Setup(m => m.TestMethod(It.IsAny<string>()))
                .Callback(() => { });
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod(It.IsAny<string>()))
                .Callback(() => { });
            """,

            """
            mock.Setup(m => m.TestMethod(It.IsAny<string>(), It.IsAny<int>()))
                .Callback(() => { });
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod(It.IsAny<string>(), It.IsAny<int>()))
                .Callback(() => { });
            """,

            """
            mock.Setup(m => m.TestMethod("OK", It.IsAny<int>()))
                .Callback(() => { });
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod("OK", It.IsAny<int>()))
                .Callback(() => { });
            """,

            """
            mock.Setup(m => m.TestMethod(It.IsAny<string>(), 1234))
                .Callback(() => { });
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod(It.IsAny<string>(), 1234))
                .Callback(() => { });
            """,

            """mock.Setup(m => m.TestMethod());""",

            """mock.InSequence(sequence).Setup(m => m.TestMethod());""",

            """mock.Setup(m => m.TestMethod("OK", 1234));""",

            """mock.InSequence(sequence).Setup(m => m.TestMethod("OK", 1234));""",

            """
            var strictMock = new Mock<I>(MockBehavior.Strict);
            strictMock.Setup(m => m.TestMethod());
            """,

            """
            var o = new object();
            o.ToString();
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """mock.Setup(m => m.TestMethod({|PosInfoMoq1003:It.IsAny<string>()|}, {|PosInfoMoq1003:It.IsAny<int>()|}));""",

            """mock.InSequence(sequence).Setup(m => m.TestMethod({|PosInfoMoq1003:It.IsAny<string>()|}, {|PosInfoMoq1003:It.IsAny<int>()|}));""",

            """mock.Setup(m => m.TestMethod("Ignored", {|PosInfoMoq1003:It.IsAny<int>()|}));""",

            """mock.InSequence(sequence).Setup(m => m.TestMethod("Ignored", {|PosInfoMoq1003:It.IsAny<int>()|}));""",

            """mock.Setup(m => m.TestMethod({|PosInfoMoq1003:It.IsAny<string>()|}, 1234));""",

            """mock.InSequence(sequence).Setup(m => m.TestMethod({|PosInfoMoq1003:It.IsAny<string>()|}, 1234));""",

            """mock.Setup(m => m.TestMethod({|PosInfoMoq1003:It.IsAny<string>()|}));""",

            """mock.InSequence(sequence).Setup(m => m.TestMethod({|PosInfoMoq1003:It.IsAny<string>()|}));""",
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
                            var sequence = new MockSequence();

                            var mock = new Mock<I>();
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
                            var sequence = new MockSequence();
                            var mock = new Mock<I>();
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