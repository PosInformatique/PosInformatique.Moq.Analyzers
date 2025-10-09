//-----------------------------------------------------------------------
// <copyright file="VerifyStaticMethodsRequiresMockParametersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyStaticMethodsRequiresMockParametersAnalyzer>;

    public class VerifyStaticMethodsRequiresMockParametersAnalyzerTest
    {
        [Fact]
        public async Task StaticVerifyAndVerifyAllWithParameters_NoDiagnosticReported()
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
                            var mock2 = new Mock<I>();

                            Mock.Verify(mock1);
                            Mock.VerifyAll(mock1);
                            Mock.Verify(mock1, mock2);
                            Mock.VerifyAll(mock1, mock2);
                        }
                    }

                    public interface I
                    {
                        int TestMethod();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task StaticVerifyAndVerifyAllWithNoParameters_DiagnosticReported()
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
                            Mock.{|#0:Verify()|#0};
                            Mock.{|#1:VerifyAll()|#1};
                        }
                    }

                    public interface I
                    {
                        int TestMethod();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                [
                    new DiagnosticResult(VerifyStaticMethodsRequiresMockParametersAnalyzer.Rule)
                        .WithLocation(0)
                        .WithArguments("Verify"),
                    new DiagnosticResult(VerifyStaticMethodsRequiresMockParametersAnalyzer.Rule)
                        .WithLocation(1)
                        .WithArguments("VerifyAll"),
                ]);
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
                        }
                    }

                    public interface I
                    {
                        void Method();
                    }
                }

                namespace OtherNamespace
                {
                    public class Mock<T>
                    {
                        public Mock(MockBehavior _) { }

                        public void Verify(System.Action<T> _) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}