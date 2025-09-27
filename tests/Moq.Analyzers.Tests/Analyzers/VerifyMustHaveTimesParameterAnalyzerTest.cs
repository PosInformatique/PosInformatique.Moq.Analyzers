//-----------------------------------------------------------------------
// <copyright file="VerifyMustHaveTimesParameterAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyMustHaveTimesParameterAnalyzer>;

    public class VerifyMustHaveTimesParameterAnalyzerTest
    {
        [Fact]
        public async Task TimesExplicitelySet_NoDiagnosticReported()
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

                            mock.[|Verify|]();
                            mock.[|Verify|](i => i.TestMethod());
                            mock.[|Verify|](i => i.TestMethod(), ""Failed message ignored"");
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