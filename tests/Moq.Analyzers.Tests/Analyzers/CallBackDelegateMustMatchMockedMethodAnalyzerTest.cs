//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateMustMatchMockedMethodAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        CallBackDelegateMustMatchMockedMethodAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

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
                                .Throws()
                                .Callback(() => { });
                            mock1.Setup(m => m.TestMethod(default))
                                .Callback((string x) => { })
                                .Throws()
                                .Callback((string x) => { });
                            mock1.Setup(m => m.TestMethod(default, default))
                                .Callback((string x, int y) => { })
                                .Throws()
                                .Callback((string x, int y) => { });
                            mock1.Setup(m => m.TestGenericMethod(1234))
                                .Callback((int x) => { })
                                .Throws()
                                .Callback((int x) => { });
                            mock1.Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                                .Callback((object x) => { })
                                .Throws()
                                .Callback((object x) => { });

                            mock1.Setup(m => m.TestMethodReturn())
                                .Callback(() => { })
                                .Throws()
                                .Callback(() => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default))
                                .Callback((string x) => { })
                                .Throws()
                                .Callback((string x) => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default, default))
                                .Callback((string x, int y) => { })
                                .Throws()
                                .Callback((string x, int y) => { })
                                .Returns(1234);

                            mock1.Setup(m => m.TestMethod(default))
                                .Callback()
                                .Throws()
                                .Callback();
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
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                                .Throws()
                                .Callback([|(int too, int much, int parameters)|] => { });
                            mock1.Setup(m => m.TestMethod(default))
                                .Callback([|()|] => { })
                                .Throws()
                                .Callback([|()|] => { });
                            mock1.Setup(m => m.TestMethod(default))
                                .Callback(([|int otherType|]) => { })
                                .Throws()
                                .Callback(([|int otherType|]) => { });
                            mock1.Setup(m => m.TestMethod(default))
                                .Callback([|(int too, int much, int parameters)|] => { })
                                .Throws()
                                .Callback([|(int too, int much, int parameters)|] => { });
                            mock1.Setup(m => m.TestGenericMethod(1234))
                                .Callback(([|string x|]) => { })
                                .Throws()
                                .Callback(([|string x|]) => { });
                            mock1.Setup(m => m.TestGenericMethod(It.IsAny<It.IsAnyType>()))
                                .Callback(([|string x|]) => { })
                                .Throws()
                                .Callback(([|string x|]) => { });

                            mock1.Setup(m => m.TestMethodReturn())
                                .Callback([|(int too, int much, int parameters)|] => { })
                                .Throws()
                                .Callback([|(int too, int much, int parameters)|] => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default))
                                .Callback([|()|] => { })
                                .Throws()
                                .Callback([|()|] => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default))
                                .Callback(([|int otherType|]) => { })
                                .Throws()
                                .Callback(([|int otherType|]) => { })
                                .Returns(1234);
                            mock1.Setup(m => m.TestMethodReturn(default))
                                .Callback([|(int too, int much, int parameters)|] => { })
                                .Throws()
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
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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

            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}