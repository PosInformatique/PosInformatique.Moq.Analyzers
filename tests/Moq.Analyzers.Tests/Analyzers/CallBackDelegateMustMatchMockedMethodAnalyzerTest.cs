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
        [Theory]
        [InlineData("")]
        [InlineData(".InSequence(sequence)")]
        public async Task CallBackSignatureMatch_NoDiagnosticReported(string sequence)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var sequence = new MockSequence();

                            var mock1 = new Mock<I>();

                            mock1" + sequence + @".Setup(m => m.TestMethod())
                                .Callback(() => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestMethod(default))
                                .Callback((string x) => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestMethod(default, default))
                                .Callback((string x, int y) => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestGenericMethod(1234))
                                .Callback((int x) => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                                .Callback((object x) => { })
                                .Throws(new Exception());

                            mock1" + sequence + @".Setup(m => m.TestMethodReturn())
                                .Callback(() => { })
                                .Returns(1234);
                            mock1" + sequence + @".Setup(m => m.TestMethodReturn(default))
                                .Callback((string x) => { })
                                .Returns(1234);
                            mock1" + sequence + @".Setup(m => m.TestMethodReturn(default, default))
                                .Callback((string x, int y) => { })
                                .Returns(1234);

                            // Special case add a Verify() method inside the Callback() method.
                            var innerMock = new Mock<I>();

                            mock1.Setup(m => m.TestMethod())
                                .Callback(() => { innerMock.Verify(im => im.TestMethod(""a"")); });
                        }
                    }

                    public interface I
                    {
                        void TestMethod();

                        void TestMethod(string a);

                        void TestMethod(string a, int b);

                        void TestGenericMethod<T>(T value);

                        int TestMethodReturn();

                        int TestMethodReturn(string a);

                        int TestMethodReturn(string a, int b);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(".InSequence(sequence)")]
        public async Task CallBackSignatureNotMatch_DiagnosticReported(string sequence)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var sequence = new MockSequence();

                            var mock1 = new Mock<I>();

                            mock1" + sequence + @".Setup(m => m.TestMethod())
                                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestMethod(default))
                                .Callback({|PosInfoMoq2003:()|} => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestMethod(default))
                                .Callback(({|PosInfoMoq2003:int otherType|}) => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestMethod(default))
                                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestGenericMethod(1234))
                                .Callback(({|PosInfoMoq2003:string x|}) => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                                .Callback(({|PosInfoMoq2003:string x|}) => { })
                                .Throws(new Exception());

                            mock1" + sequence + @".Setup(m => m.TestMethodReturn())
                                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                                .Returns(1234);
                            mock1" + sequence + @".Setup(m => m.TestMethodReturn(default))
                                .Callback({|PosInfoMoq2003:()|} => { })
                                .Returns(1234);
                            mock1" + sequence + @".Setup(m => m.TestMethodReturn(default))
                                .Callback(({|PosInfoMoq2003:int otherType|}) => { })
                                .Returns(1234);
                            mock1" + sequence + @".Setup(m => m.TestMethodReturn(default))
                                .Callback({|PosInfoMoq2003:(int too, int much, int parameters)|} => { })
                                .Returns(1234);

                            // Callback with return value
                            mock1" + sequence + @".Setup(m => m.TestMethodReturn())
                                .Callback(() => { {|PosInfoMoq2014:return false;|} })
                                .Returns(1234);
                            mock1" + sequence + @".Setup(m => m.TestMethodReturn())
                                .Callback(() =>
                                {
                                    var b = false;
                                    if (b)
                                    {
                                        for (int i=0; i<10; i++)
                                        {
                                            if (i == 0) {|#0:return false;|};
                                        }

                                        {|#1:return false;|}
                                    }

                                    {|#2:return true;|}
                                })
                                .Returns(1234);
                        }
                    }

                    public interface I
                    {
                        void TestMethod();

                        void TestMethod(string a);

                        void TestMethod(string a, int b);

                        void TestGenericMethod<T>(T value);

                        int TestMethodReturn();

                        int TestMethodReturn(string a);

                        int TestMethodReturn(string a, int b);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                [
                    new DiagnosticResult(CallBackDelegateMustMatchMockedMethodAnalyzer.CallbackMustNotReturnValue)
                        .WithLocation(0)
                        .WithLocation(1)
                        .WithLocation(2),
                ]);
        }

        [Fact]
        public async Task CallBackWithCustomSetupMethod_NoDiagnosticReported()
        {
            var source = @"
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
                            var m = new Mock<IRepository>(MockBehavior.Strict);

                            m.MySetup(i => i.Get(""Foobar""))
                                .Callback(() =>
                                {
                                });
                        }
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
                }
                ";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoMoqLibrary()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using OtherNamespace;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            mock1.Setup(m => m.TestMethod());
                        }
                    }

                    public interface I
                    {
                        void TestMethod();
                    }
                }

                namespace OtherNamespace
                {
                    using System;

                    public class Mock<T>
                    {
                        public Mock(MockBehavior _) { }
 
                        public void Setup(Action<T> act) { }
                   }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}