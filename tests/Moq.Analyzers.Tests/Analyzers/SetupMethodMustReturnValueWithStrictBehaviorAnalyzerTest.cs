//-----------------------------------------------------------------------
// <copyright file="SetupMethodMustReturnValueWithStrictBehaviorAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        SetupMethodMustReturnValueWithStrictBehaviorAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

    public class SetupMethodMustReturnValueWithStrictBehaviorAnalyzerTest
    {
        [Fact]
        public async Task Returns_NoDiagnosticReported()
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
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            mock1.Setup(i => i.TestMethod())
                                .Callback()
                                .Callback()
                                .Property
                                .Returns();

                            var mock2 = new Mock<I>(MockBehavior.Strict);
                            mock2.Setup(i => i.TestMethod())
                                .Callback()
                                .Callback()
                                .Property
                                .ReturnsAsync();

                            var mock3 = new Mock<I>(MockBehavior.Strict);

                            var obj = new object();     // Ignored because not a Mock<T>
                            obj.ToString();

                            var action = new Action(() => { });
                            action();                   // InvocationExpression ignored
                        }
                    }

                    public interface I
                    {
                        int TestMethod();
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                                        .Callback()
                                        .Callback();
                                }
                            }

                            [|mock2.Setup(i => i.TestMethod())|]
                                .Callback()
                                .Callback();
                        }
                    }

                    public interface I
                    {
                        int TestMethod();
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                                .Callback()
                                .Callback();
                        }
                    }

                    public interface I
                    {
                        int TestMethod();
                    }
                }
                "
                + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                                .Callback()
                                .Callback();
                        }
                    }

                    public interface I
                    {
                        int TestMethod();
                    }

                    public enum OtherEnum { A, B }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                                .Callback()
                                .Callback();
                        }
                    }

                    public interface I
                    {
                        void TestMethod();
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("MockBehavior.Strict")]
        [InlineData("MockBehavior.Strict, 1, 2")]
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
                            mock1.Setup(i => i.Property = 10)
                                .Callback()
                                .Callback();
                        }
                    }

                    public interface I
                    {
                        int Property { get; set; }
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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