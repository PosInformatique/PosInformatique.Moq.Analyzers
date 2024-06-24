//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateMustMatchMockedMethodAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<CallBackDelegateMustMatchMockedMethodAnalyzer>;

    public class CallBackDelegateMustMatchMockedMethodAnalyzerTest
    {
        [Fact]
        public async Task CallBackSignatureMatch_NoDiagnosticReported()
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
                            var mock1 = new Mock<I>();

                            mock1.Setup(m => m.TestMethod())
                                .Callback(() => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestMethod(default))
                                .Callback((string x) => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestMethod(default, default))
                                .Callback((string x, int y) => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestGenericMethod(1234))
                                .Callback((int x) => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                                .Callback((object x) => { })
                                .Throws(new Exception());

                            mock1.Setup(m => m.TestMethodReturn())
                                .Callback(() => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default))
                                .Callback((string x) => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default, default))
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

        [Fact]
        public async Task CallBackSignatureNotMatch_DiagnosticReported()
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
                            var mock1 = new Mock<I>();

                            mock1.Setup(m => m.TestMethod())
                                .Callback([|(int too, int much, int parameters)|] => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestMethod(default))
                                .Callback([|()|] => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestMethod(default))
                                .Callback(([|int otherType|]) => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestMethod(default))
                                .Callback([|(int too, int much, int parameters)|] => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestGenericMethod(1234))
                                .Callback(([|string x|]) => { })
                                .Throws(new Exception());
                            mock1.Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                                .Callback(([|string x|]) => { })
                                .Throws(new Exception());

                            mock1.Setup(m => m.TestMethodReturn())
                                .Callback([|(int too, int much, int parameters)|] => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default))
                                .Callback([|()|] => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default))
                                .Callback(([|int otherType|]) => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default))
                                .Callback([|(int too, int much, int parameters)|] => { })
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