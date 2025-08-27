//-----------------------------------------------------------------------
// <copyright file="ItArgumentsMustMatchMockedMethodArgumentsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<ItArgumentsMustMatchMockedMethodArgumentsAnalyzer>;

    public class ItArgumentsMustMatchMockedMethodArgumentsAnalyzerTest
    {
        [Fact]
        public async Task Match_NoDiagnosticReported()
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
                            mock1.Setup(m => m.TestMethod(It.IsAny<int?>(), 1, ""Ignored"", C.OtherMethod()));
                            mock1.Setup(m => m.NoParametersMethod());
                       }
                    }

                    public interface I
                    {
                        void NoParametersMethod();

                        void TestMethod(int? a, int b, string c, int d);
                    }

                    public class C
                    {
                        public static int OtherMethod() => default;
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NotMatch_DiagnosticReported()
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
                            mock1.Setup(m => m.TestMethod({|#0:It.IsAny<int>()|#0}, 1, ""Ignored""));
                       }
                    }

                    public interface I
                    {
                        void TestMethod(int? a, int b, string c);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                [
                    new DiagnosticResult(ItArgumentsMustMatchMockedMethodArgumentsAnalyzer.Rule)
                        .WithSpan(12, 59, 12, 74),
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