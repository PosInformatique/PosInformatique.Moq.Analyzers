//-----------------------------------------------------------------------
// <copyright file="ConstructorArgumentCannotBePassedForInterfaceAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<ConstructorArgumentCannotBePassedForInterfaceAnalyzer>;

    public class ConstructorArgumentCannotBePassedForInterfaceAnalyzerTest
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
                }";

            await Verifier.VerifyAnalyzerAsync(source);
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
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Interface_WithoutBehaviorStrict()
        {
            var source = @"
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
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(ConstructorArgumentCannotBePassedForInterfaceAnalyzer.Rule)
                    .WithLocation(0).WithArguments("1")
                    .WithLocation(1).WithArguments("2"));
        }

        [Fact]
        public async Task Interface_WithBehaviorStrict()
        {
            var source = @"
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
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(ConstructorArgumentCannotBePassedForInterfaceAnalyzer.Rule)
                    .WithLocation(0).WithArguments("1")
                    .WithLocation(1).WithArguments("2"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(", MockBehavior.Strict")]
        public async Task Interface_WithFactoryLambdaExpression(string behavior)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(() => new C()" + behavior + @");
                        }
                    }

                    public interface I
                    {
                    }

                    public class C : I
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
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
                }";

            await Verifier.VerifyAnalyzerAsync(source);
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

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}