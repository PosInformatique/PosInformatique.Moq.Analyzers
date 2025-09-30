//-----------------------------------------------------------------------
// <copyright file="VerifyMustHaveTimesParameterAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyMustHaveTimesParameterAnalyzer>;

    public class VerifyMustHaveTimesParameterAnalyzerTest
    {
        [Fact]
        public async Task TimesExplicitelySet_Verify_NoDiagnosticReported()
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
                            var mock = new Mock<I>();

                            mock.Verify();

                            mock.Verify(i => i.TestMethod(), Times.AtLeast(15));
                            mock.Verify(i => i.TestMethod(), Times.Exactly(1));
                            mock.Verify(i => i.TestMethod(), Times.Never);
                            mock.Verify(i => i.TestMethod(), Times.Never());
                            mock.Verify(i => i.TestMethod(), Times.Once);
                            mock.Verify(i => i.TestMethod(), Times.Once());

                            mock.Verify(i => i.TestMethod(), Times.AtLeast(15), ""Failed message ignored"");
                            mock.Verify(i => i.TestMethod(), Times.Exactly(1), ""Failed message ignored"");
                            mock.Verify(i => i.TestMethod(), Times.Never, ""Failed message ignored"");
                            mock.Verify(i => i.TestMethod(), Times.Never(), ""Failed message ignored"");
                            mock.Verify(i => i.TestMethod(), Times.Once, ""Failed message ignored"");
                            mock.Verify(i => i.TestMethod(), Times.Once(), ""Failed message ignored"");
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
        public async Task TimesExplicitelySet_Verifiable_NoDiagnosticReported()
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
                            var mock = new Mock<I>();

                            mock.Setup(i => i.TestMethod()).Verifiable(Times.AtLeast(15));
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Exactly(1));
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Never);
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Never());
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Once);
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Once());

                            mock.Setup(i => i.TestMethod()).Verifiable(Times.AtLeast(15), ""Failed message ignored"");
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Exactly(1), ""Failed message ignored"");
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Never, ""Failed message ignored"");
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Never(), ""Failed message ignored"");
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Once, ""Failed message ignored"");
                            mock.Setup(i => i.TestMethod()).Verifiable(Times.Once(), ""Failed message ignored"");
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
        public async Task TimesExplicitelyNotSet_Verify_DiagnosticReported()
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
                            var mock = new Mock<I>();

                            mock.{|#0:Verify|#0}(i => i.TestMethod());
                            mock.{|#1:Verify|#1}(i => i.TestMethod(), ""Failed message ignored"");
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
                    new DiagnosticResult(VerifyMustHaveTimesParameterAnalyzer.Rule)
                        .WithSpan(13, 34, 13, 40).WithArguments("Verify"),
                    new DiagnosticResult(VerifyMustHaveTimesParameterAnalyzer.Rule)
                        .WithSpan(14, 34, 14, 40).WithArguments("Verify"),
                ]);
        }

        [Fact]
        public async Task TimesExplicitelyNotSet_DiagnosticReported()
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
                            var mock = new Mock<I>();

                            mock.Setup(m => m.TestMethod()).{|#0:Verifiable|#0}();
                            mock.Setup(m => m.TestMethod()).{|#1:Verifiable|#1}(""Failed message ignored"");
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
                    new DiagnosticResult(VerifyMustHaveTimesParameterAnalyzer.Rule)
                        .WithSpan(13, 61, 13, 71).WithArguments("Verifiable"),
                    new DiagnosticResult(VerifyMustHaveTimesParameterAnalyzer.Rule)
                        .WithSpan(14, 61, 14, 71).WithArguments("Verifiable"),
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
                            mock1.Verify(i => i.Method());
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