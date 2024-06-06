//-----------------------------------------------------------------------
// <copyright file="ConstructorArgumentCannotBeParsedForInterfaceAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        ConstructorArgumentCannotBeParsedForInterfaceAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

    public class ConstructorArgumentCannotBeParsedForInterfaceAnalyzerTest
    {
        [Fact]
        public async Task Interface_NoMock()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var obj = new object();
                        }
                    }

                    public interface I
                    {
                    }
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Interface_WithNoArgument()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                        }
                    }

                    public interface I
                    {
                    }
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Interface_WithoutBehaviorStrict()
        {
            var context = new CSharpAnalyzerTest<ConstructorArgumentCannotBeParsedForInterfaceAnalyzer, DefaultVerifier>();

            context.TestCode = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>({|#0:1|}, {|#1:2|});
                        }
                    }

                    public interface I
                    {
                    }
                }" + MoqLibrary.Code;

            context.ExpectedDiagnostics.Add(new DiagnosticResult(ConstructorArgumentCannotBeParsedForInterfaceAnalyzer.Rule)
                .WithLocation(0).WithArguments("1")
                .WithLocation(1).WithArguments("2"));

            await context.RunAsync();
        }

        [Fact]
        public async Task Interface_WithBehaviorStrict()
        {
            var context = new CSharpAnalyzerTest<ConstructorArgumentCannotBeParsedForInterfaceAnalyzer, DefaultVerifier>();

            context.TestCode = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict, {|#0:1|}, {|#1:2|});
                        }
                    }

                    public interface I
                    {
                    }
                }" + MoqLibrary.Code;

            context.ExpectedDiagnostics.Add(new DiagnosticResult(ConstructorArgumentCannotBeParsedForInterfaceAnalyzer.Rule)
                .WithLocation(0).WithArguments("1")
                .WithLocation(1).WithArguments("2"));

            await context.RunAsync();
        }

        [Fact]
        public async Task Class()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>(1, 2, 3);
                        }
                    }

                    public abstract class C
                    {
                    }
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Interface_NoMoqLibrary()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using OtherNamespace;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict, 1, 2, 3);
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
                        public Mock(MockBehavior _, int a, int b, int c) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}