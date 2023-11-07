//-----------------------------------------------------------------------
// <copyright file="NoSealedClassAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        NoSealedClassAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

    public class NoSealedClassAnalyzerTest
    {
        [Fact]
        public async Task NoSealedClass_NoDiagnosticReported()
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

                            var mock2 = new Mock<StandardClass>();

                            var mock3 = new Mock<AbstractClass>();

                            var obj = new object();     // Ignored by the analysis.
                        }
                    }

                    public interface I
                    {
                    }

                    public class StandardClass
                    {
                    }

                    public abstract class AbstractClass
                    {
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task SealedClass_NoDiagnosticReported()
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
                            var mock1 = new Mock<[|SealedClass|]>();
                            var mock2 = new Mock<[|string|]>();
                            var mock3 = new Mock<[|int|]>();
                        }
                    }

                    public sealed class SealedClass
                    {
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
                        }
                    }

                    public interface I
                    {
                    }
                }

                namespace OtherNamespace
                {
                    public class Mock<T>
                    {
                        public Mock(MockBehavior _) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}