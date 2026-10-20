//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateMustMatchMockedMethodAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<CallBackDelegateMustMatchMockedMethodAnalyzer>;

    public class CallBackDelegateMustMatchMockedMethodAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                void TestMethod();

                void TestMethod(string a);

                void TestMethod(string a, int b);

                void TestGenericMethod<T>(T value);

                void TestMethodWithDefaultParameters(string a, int b, int c = 42, string d = "default");

                int TestMethodReturn();

                int TestMethodReturn(string a);

                int TestMethodReturn(string a, int b);
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
            mock.Setup(m => m.TestMethod())
                .Callback(() => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod())
                .Callback(() => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethod(default))
                .Callback((string x) => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod(default))
                .Callback((string x) => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethod(default, default))
                .Callback((string x, int y) => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod(default, default))
                .Callback((string x, int y) => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestGenericMethod(1234))
                .Callback((int x) => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestGenericMethod(1234))
                .Callback((int x) => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                .Callback((object x) => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                .Callback((object x) => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethodWithDefaultParameters("A", 10))
                .Callback((string x, int y, int z, string w) => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodWithDefaultParameters("A", 10))
                .Callback((string x, int y, int z, string w) => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethodReturn())
                .Callback(() => { })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn())
                .Callback(() => { })
                .Returns(1234);
            """,

            """
            mock.Setup(m => m.TestMethodReturn(default))
                .Callback((string x) => { })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn(default))
                .Callback((string x) => { })
                .Returns(1234);
            """,

            """
            mock.Setup(m => m.TestMethodReturn(default, default))
                .Callback((string x, int y) => { })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn(default, default))
                .Callback((string x, int y) => { })
                .Returns(1234);
            """,

            """
            var innerMock = new Mock<I>();
            mock.Setup(m => m.TestMethod())
                .Callback(() => { innerMock.Verify(im => im.TestMethod("a")); });
            """,

            """
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            repositoryMock.MySetup(i => i.Get("Foobar"))
                .Callback(() =>
                {
                });
            """,

            """
            var strictMock = new Mock<I>(MockBehavior.Strict);
            strictMock.Setup(m => m.TestMethod());
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            mock.Setup(m => m.TestMethod())
                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod())
                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethod(default))
                .Callback({|PosInfoMoq2003:()|} => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod(default))
                .Callback({|PosInfoMoq2003:()|} => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethod(default))
                .Callback(({|PosInfoMoq2003:int otherType|}) => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod(default))
                .Callback(({|PosInfoMoq2003:int otherType|}) => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethod(default))
                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethod(default))
                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestGenericMethod(1234))
                .Callback(({|PosInfoMoq2003:string x|}) => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestGenericMethod(1234))
                .Callback(({|PosInfoMoq2003:string x|}) => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                .Callback(({|PosInfoMoq2003:string x|}) => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                .Callback(({|PosInfoMoq2003:string x|}) => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethodWithDefaultParameters("A", 10))
                .Callback({|PosInfoMoq2003:(string x, int y)|} => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodWithDefaultParameters("A", 10))
                .Callback({|PosInfoMoq2003:(string x, int y)|} => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethodWithDefaultParameters("A", 10))
                .Callback({|PosInfoMoq2003:(string x, int y, string z)|} => { })
                .Throws(new Exception());
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodWithDefaultParameters("A", 10))
                .Callback({|PosInfoMoq2003:(string x, int y, string z)|} => { })
                .Throws(new Exception());
            """,

            """
            mock.Setup(m => m.TestMethodReturn())
                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn())
                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                .Returns(1234);
            """,

            """
            mock.Setup(m => m.TestMethodReturn(default))
                .Callback({|PosInfoMoq2003:()|} => { })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn(default))
                .Callback({|PosInfoMoq2003:()|} => { })
                .Returns(1234);
            """,

            """
            mock.Setup(m => m.TestMethodReturn(default))
                .Callback(({|PosInfoMoq2003:int otherType|}) => { })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn(default))
                .Callback(({|PosInfoMoq2003:int otherType|}) => { })
                .Returns(1234);
            """,

            """
            mock.Setup(m => m.TestMethodReturn(default))
                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn(default))
                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                .Returns(1234);
            """,

            """
            mock.Setup(m => m.TestMethodReturn())
                .Callback(() => { {|PosInfoMoq2014:return false;|} })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn())
                .Callback(() => { {|PosInfoMoq2014:return false;|} })
                .Returns(1234);
            """,

            """
            mock.Setup(m => m.TestMethodReturn())
                .Callback(() =>
                {
                    var b = false;
                    if (b)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (i == 0) {|#0:return false;|};
                        }

                        {|#1:return false;|}
                    }

                    {|#2:return true;|}
                })
                .Returns(1234);
            """,

            """
            mock.InSequence(sequence).Setup(m => m.TestMethodReturn())
                .Callback(() =>
                {
                    var b = false;
                    if (b)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (i == 0) {|#0:return false;|};
                        }

                        {|#1:return false;|}
                    }

                    {|#2:return true;|}
                })
                .Returns(1234);
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
                            var mock = new Mock<I>();
                            var sequence = new MockSequence();
                            {{code}}
                        }
                    }

                    {{TestTypes}}
                }
                """;

            if (code.Contains("{|#0:", StringComparison.Ordinal))
            {
                await Verifier.VerifyAnalyzerAsync(
                    source,
                    new DiagnosticResult(CallBackDelegateMustMatchMockedMethodAnalyzer.CallbackMustNotReturnValue)
                        .WithLocation(0)
                        .WithLocation(1)
                        .WithLocation(2));
            }
            else
            {
                await Verifier.VerifyAnalyzerAsync(source);
            }
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
                            var mock = new Mock<I>();
                            var sequence = new MockSequence();
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