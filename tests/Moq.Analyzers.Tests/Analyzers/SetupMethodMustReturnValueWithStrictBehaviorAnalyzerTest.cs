//-----------------------------------------------------------------------
// <copyright file="SetupMethodMustReturnValueWithStrictBehaviorAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using Verifier = MoqCSharpAnalyzerVerifier<SetupMethodMustReturnValueWithStrictBehaviorAnalyzer>;

    public class SetupMethodMustReturnValueWithStrictBehaviorAnalyzerTest
    {
        [Fact]
        public async Task Returns_StrictMode_NoDiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            mock1.Setup(i => i.TestMethod())
                                .Callback(() => { })
                                .Returns(1234);

                            var mock2 = new Mock<I>(MockBehavior.Strict);
                            mock2.Setup(i => i.TestAsyncMethod())
                                .Callback(() => { })
                                .ReturnsAsync(1234);

                            var mock3 = new Mock<I>(MockBehavior.Strict);

                            // This scenario is not supported because the declaration is separated of the initialization.
                            // So here, no error should be raised (Missing Returns() with Strict behavior).
                            Mock<I> mock4;
                            mock4 = new Mock<I>(MockBehavior.Strict);
                            mock4.Setup(i => i.TestMethod())
                                .Callback(() => { });

                            // This scenario is not supported because the declaration is separated of the initialization.
                            // So here, no error should be raised (Missing Returns() with Strict behavior).
                            Mock<I> mock5 = mock1;
                            mock5.Setup(i => i.TestMethod())
                                .Callback(() => { });

                            Mock<I> mock6 = new Mock<I>(MockBehavior.Strict);
                            mock6.Setup(i => i.TestMethod())
                                .Callback(() => { })
                                .Throws(new Exception());

                            Mock<I> mock7 = new Mock<I>(MockBehavior.Strict);
                            mock7.Setup(i => i.TestAsyncMethod())
                                .Callback(() => { })
                                .ThrowsAsync(new Exception());

                            var mock8 = new Mock<I>(MockBehavior.Strict);
                            mock8.Setup(i => i.TestProperty)
                                .Callback(() => { })
                                .Returns(1234);

                            var obj = new object();     // Ignored because not a Mock<T>
                            obj.ToString();

                            var action = new Action(() => { });
                            action();                   // InvocationExpression ignored
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        Task<int> TestAsyncMethod();

                        int TestProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Returns_StrictMode_InvalidReturnsType_NoDiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            mock1.Setup(i => i.TestMethod())
                                .Callback(() => { })
                                .Returns(""Foobar"");
                            mock1.Setup(i => i.TestProperty)
                                .Callback(() => { })
                                .Returns(""Foobar"");
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        int TestProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                DiagnosticResult.CompilerError("CS1503").WithSpan(13, 42, 13, 50).WithArguments("1", "string", "int"),
                DiagnosticResult.CompilerError("CS1503").WithSpan(16, 42, 16, 50).WithArguments("1", "string", "int"));
        }

        [Fact]
        public async Task NoReturns_WithBlocksAndMixedBeahviors_DiagnosticReported()
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

                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            var mock2 = new Mock<I>(MockBehavior.Strict);
                            {
                                {
                                    [|mock1.Setup(i => i.TestMethod())|]
                                        .Callback(() => { });

                                    [|mock1.Setup(i => i.TestProperty)|]
                                        .Callback(() => { });
                                }
                            }

                            [|mock2.Setup(i => i.TestMethod())|]
                                .Callback(() => { });

                            [|mock2.Setup(i => i.TestProperty)|]
                                .Callback(() => { });
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        int TestProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoReturns_StrictMode_DiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            [|mock1.Setup(i => i.TestMethod())|]
                                .Callback(() => { });
                            [|mock1.Setup(i => i.TestProperty)|]
                                .Callback(() => { });
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        int TestProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("MockBehavior.Loose")]
        [InlineData("")]
        [InlineData("1, 2")]
        [InlineData("OtherEnum.A, 1, 2")]
        [InlineData("MockBehavior.Loose, 1, 2")]
        public async Task NoReturns_OtherMode_NoDiagnosticReported(string mockArguments)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(" + mockArguments + @");
                            mock1.Setup(i => i.TestMethod())
                                .Callback(() => { });

                            var mock2 = new Mock<I>("" + mockArguments + @"");
                            mock2.Setup(i => i.TestProperty)
                                .Callback(() => { });
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        int TestProperty { get; set; }
                    }

                    public enum OtherEnum { A, B }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("MockBehavior.Strict")]
        [InlineData("MockBehavior.Strict, 1, 2")]
        [InlineData("MockBehavior.Loose")]
        [InlineData("MockBehavior.Loose, 1, 2")]
        [InlineData("")]
        public async Task NoReturns_MockSetupMethodVoid_NoDiagnosticReported(string mockArguments)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(" + mockArguments + @");
                            mock1.Setup(i => i.TestMethod())
                                .Callback(() => { });
                        }
                    }

                    public interface I
                    {
                        void TestMethod();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("MockBehavior.Loose")]
        [InlineData("MockBehavior.Loose, 1, 2")]
        [InlineData("")]
        public async Task NoReturns_MockSetupProperty_NoDiagnosticReported(string mockArguments)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(" + mockArguments + @");
                            mock1.Setup(i => i.Property)
                                .Callback(() => { });
                        }
                    }

                    public interface I
                    {
                        int Property { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoSetupMethod()
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
                            mock1.Verify();
                        }
                    }

                    public interface I
                    {
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
                            mock1.Setup();
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

                        public void Setup() { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}