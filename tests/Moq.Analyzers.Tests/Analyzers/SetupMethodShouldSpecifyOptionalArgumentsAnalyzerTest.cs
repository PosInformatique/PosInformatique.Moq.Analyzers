//-----------------------------------------------------------------------
// <copyright file="SetupMethodShouldSpecifyOptionalArgumentsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<SetupMethodShouldSpecifyOptionalArgumentsAnalyzer>;

    public class SetupMethodShouldSpecifyOptionalArgumentsAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                void TestMethod();

                void TestMethod(string a);

                void TestMethod(string a, int b);

                void TestMethodWithDefaultParameters(string a, int b, int c = 42, string d = "default");
            }

            public static class MoqExtensions
            {
                public static ISetup<T> MySetup<T>(this Mock<T> mock, Expression<Action<T>> exp)
                    where T : class
                {
                    return mock.Setup(exp);
                }
            }

            public interface IRepository
            {
                void Get(string s);
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod())
                .Callback(() => { })
                .Throws(new Exception());
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod("A"))
                .Callback((string x) => { })
                .Throws(new Exception());
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethod("A", 10))
                .Callback((string x, int y) => { })
                .Throws(new Exception());
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.TestMethodWithDefaultParameters("A", 10, 20, "B"))
                .Callback((string x, int y, int z, string w) => { })
                .Throws(new Exception());
            """,

            """
            var sequence = new MockSequence();
            var mock = new Mock<I>();
            mock.InSequence(sequence).Setup(m => m.TestMethod())
                .Callback(() => { })
                .Throws(new Exception());
            """,

            """
            var sequence = new MockSequence();
            var mock = new Mock<I>();
            mock.InSequence(sequence).Setup(m => m.TestMethod("A"))
                .Callback((string x) => { })
                .Throws(new Exception());
            """,

            """
            var sequence = new MockSequence();
            var mock = new Mock<I>();
            mock.InSequence(sequence).Setup(m => m.TestMethod("A", 10))
                .Callback((string x, int y) => { })
                .Throws(new Exception());
            """,

            """
            var sequence = new MockSequence();
            var mock = new Mock<I>();
            mock.InSequence(sequence).Setup(m => m.TestMethodWithDefaultParameters("A", 10, 20, "B"))
                .Callback((string x, int y, int z, string w) => { })
                .Throws(new Exception());
            """,

            """
            var mock = new Mock<IRepository>(MockBehavior.Strict);
            mock.MySetup(i => i.Get("Foobar"));
            """,

            """
            var mock = new Mock<I>(MockBehavior.Strict);
            mock.Setup(m => m.TestMethod());
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.Setup(m => m.{|PosInfoMoq1011:TestMethodWithDefaultParameters|}("A", 10))
                .Callback(() => { })
                .Throws(new Exception());
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(m => m.{|PosInfoMoq1011:TestMethodWithDefaultParameters|}("A", 10, 20))
                .Callback(() => { })
                .Throws(new Exception());
            """,

            """
            var sequence = new MockSequence();
            var mock = new Mock<I>();
            mock.InSequence(sequence).Setup(m => m.{|PosInfoMoq1011:TestMethodWithDefaultParameters|}("A", 10))
                .Callback(() => { })
                .Throws(new Exception());
            """,

            """
            var sequence = new MockSequence();
            var mock = new Mock<I>();
            mock.InSequence(sequence).Setup(m => m.{|PosInfoMoq1011:TestMethodWithDefaultParameters|}("A", 10, 20))
                .Callback(() => { })
                .Throws(new Exception());
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
                    using Moq.Language.Flow;
                    using System;
                    using System.Linq.Expressions;

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
                    using FakeMoqLibrary.Language.Flow;
                    using System;
                    using System.Linq.Expressions;

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