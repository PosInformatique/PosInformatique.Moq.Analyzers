//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer>;

    public class CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzerTest
    {
        [Fact]
        public async Task NoCallBack_DiagnosticReported()
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
                            mock1.Setup(m => m.TestMethod({|#0:It.IsAny<string>()|#0}, {|#1:It.IsAny<int>()|#1}));
                            mock1.Setup(m => m.TestMethod(""Ignored"", {|#2:It.IsAny<int>()|#2}));
                            mock1.Setup(m => m.TestMethod({|#3:It.IsAny<string>()|#3}, 1234));
                            mock1.Setup(m => m.TestMethod({|#4:It.IsAny<string>()|#4}));
                       }
                    }

                    public interface I
                    {
                        void TestMethod(string a);

                        void TestMethod(string a, int b);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                new[]
                {
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackShouldBeUsedRule)
                        .WithSpan(12, 59, 12, 77).WithArguments("a"),
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackShouldBeUsedRule)
                        .WithSpan(12, 79, 12, 94).WithArguments("b"),
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackShouldBeUsedRule)
                        .WithSpan(13, 70, 13, 85).WithArguments("b"),
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackShouldBeUsedRule)
                        .WithSpan(14, 59, 14, 77).WithArguments("a"),
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackShouldBeUsedRule)
                        .WithSpan(15, 59, 15, 77).WithArguments("a"),
                });
        }

        [Fact]
        public async Task WithCallbackAndNoParameters_DiagnosticReported()
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
                            mock1.Setup(m => m.TestMethod(It.IsAny<string>(), It.IsAny<int>()))
                                .Callback(({|#0:string _|#0}, {|#1:int _|#1}) => { });
                            mock1.Setup(m => m.TestMethod(""OK"", It.IsAny<int>()))
                                .Callback((string _, {|#2:int _|#2}) => { });
                            mock1.Setup(m => m.TestMethod(It.IsAny<string>(), 1234))
                                .Callback(({|#3:string _|#3}, int _) => { });
                       }
                    }

                    public interface I
                    {
                        void TestMethod(string a);

                        void TestMethod(string a, int b);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                [
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackParameterShouldNotBeIgnoredRule)
                        .WithSpan(13, 44, 13, 52).WithArguments("a"),
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackParameterShouldNotBeIgnoredRule)
                        .WithSpan(13, 54, 13, 59).WithArguments("b"),
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackParameterShouldNotBeIgnoredRule)
                        .WithSpan(15, 54, 15, 59).WithArguments("b"),
                    new DiagnosticResult(CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.CallbackParameterShouldNotBeIgnoredRule)
                        .WithSpan(17, 44, 17, 52).WithArguments("a"),
                ]);
        }

        [Fact]
        public async Task WithCallbackAndRequiredParameters_NoDiagnosticReported()
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
                            mock1.Setup(m => m.TestMethod(It.IsAny<string>()))
                                .Callback((string a) => { });
                            mock1.Setup(m => m.TestMethod(It.IsAny<string>(), It.IsAny<int>()))
                                .Callback((string a, int b) => { });
                            mock1.Setup(m => m.TestMethod(""OK"", It.IsAny<int>()))
                                .Callback((string _, int b) => { });
                            mock1.Setup(m => m.TestMethod(It.IsAny<string>(), 1234))
                                .Callback((string a, int _) => { });

                            var mock2 = new Mock<I>();
                            mock2.Setup(m => m.TestMethod());

                            var mock3 = new Mock<I>();
                            mock3.Setup(m => m.TestMethod(""OK"", 1234));

                            var o = new object();
                            o.ToString();   // Ignored
                       }
                    }

                    public interface I
                    {
                        void TestMethod();

                        void TestMethod(string a);

                        void TestMethod(string a, int b);
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