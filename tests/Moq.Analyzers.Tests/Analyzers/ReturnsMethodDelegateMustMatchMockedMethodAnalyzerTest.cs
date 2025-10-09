//-----------------------------------------------------------------------
// <copyright file="ReturnsMethodDelegateMustMatchMockedMethodAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<ReturnsMethodDelegateMustMatchMockedMethodAnalyzer>;

    public class ReturnsMethodDelegateMustMatchMockedMethodAnalyzerTest
    {
        [Theory]
        [InlineData("Returns", "int")]
        [InlineData("ReturnsAsync", "int")]
        [InlineData("Returns", "object")]
        [InlineData("ReturnsAsync", "object")]
        public async Task Returns_ValidReturnsType_Method_NoDiagnosticReported(string methodName, string returnType)
        {
            if (methodName == "ReturnsAsync")
            {
                returnType = "Task<" + returnType + ">";
            }

            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Setup(i => i.TestMethod())
                                .Callback(() => { })
                                ." + methodName + @"(() => { return 1234; });
                        }
                    }

                    public interface I
                    {
                        " + returnType + @" TestMethod();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("\"Foobar\"")]
        public async Task Returns_ValidReturnsType_Method_Generic_NoDiagnosticReported(string value)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Setup(i => i.TestMethod<It.IsAnyType>())
                                .Callback(() => { })
                                .Returns(() => { return " + value + @"; });
                        }
                    }

                    public interface I
                    {
                        T TestMethod<T>();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Returns_InvalidReturnsType_Method_DiagnosticReported()
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
                            mock1.Setup(i => i.TestMethod())
                                .Callback(() => { })
                                .Returns({|#0:() => { return ""Foobar""; }|#0});
                        }
                    }

                    public interface I
                    {
                        int TestMethod();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(ReturnsMethodDelegateMustMatchMockedMethodAnalyzer.ReturnValueMustMatchRule)
                    .WithLocation(0)
                    .WithArguments("Int32"));
        }

        [Fact]
        public async Task Returns_InvalidReturnsType_Property_DiagnosticReported()
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
                            mock1.Setup(i => i.TestProperty)
                                .Callback(() => { })
                                .Returns({|#0:() => { return ""Foobar""; }|#0});
                        }
                    }

                    public interface I
                    {
                        int TestProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(ReturnsMethodDelegateMustMatchMockedMethodAnalyzer.ReturnValueMustMatchRule)
                    .WithLocation(0)
                    .WithArguments("Int32"));
        }

        [Theory]
        [InlineData("TestProperty")]
        [InlineData("GetData()")]
        public async Task Returns_ValidReturnsValue_NoDiagnosticReported(string member)
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
                            mock1.Setup(i => i." + member + @")
                                .Callback(() => { })
                                .Returns(1234);
                        }
                    }

                    public interface I
                    {
                        int GetData();

                        int TestProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Returns_ValidReturnsType_Property_NoDiagnosticReported()
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
                            mock1.Setup(i => i.TestProperty)
                                .Callback(() => { })
                                .Returns(() => { return 1234; });
                        }
                    }

                    public interface I
                    {
                        int TestProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("Returns")]
        [InlineData("ReturnsAsync")]
        public async Task Returns_InvalidArgumentType_DiagnosticReported(string methodName)
        {
            var returnType = "int";

            if (methodName == "ReturnsAsync")
            {
                returnType = "Task<" + returnType + ">";
            }

            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Setup(i => i.TestMethod(""Ignored""))
                                .Callback(() => { })
                                ." + methodName + @"(({|PosInfoMoq2013:int s|}) => { return 1234; });
                        }
                    }

                    public interface I
                    {
                        " + returnType + @" TestMethod(string a);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("Returns")]
        [InlineData("ReturnsAsync")]
        public async Task Returns_DifferentArgumentCount_DiagnosticReported(string methodName)
        {
            var returnType = "int";

            if (methodName == "ReturnsAsync")
            {
                returnType = "Task<" + returnType + ">";
            }

            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Setup(i => i.TestMethod(""Ignored""))
                                .Callback(() => { })
                                ." + methodName + @"({|PosInfoMoq2013:(int s, bool b)|} => { return 1234; });
                        }
                    }

                    public interface I
                    {
                        " + returnType + @" TestMethod(string a);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("Returns")]
        [InlineData("ReturnsAsync")]
        public async Task Returns_ValidArgumentType_NoDiagnosticReported(string methodName)
        {
            var returnType = "int";

            if (methodName == "ReturnsAsync")
            {
                returnType = "Task<" + returnType + ">";
            }

            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Setup(i => i.TestMethod(""Ignored""))
                                .Callback(() => { })
                                ." + methodName + @"((string z) => { return 1234; });
                        }
                    }

                    public interface I
                    {
                        " + returnType + @" TestMethod(string a);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("Returns")]
        [InlineData("ReturnsAsync")]
        public async Task Returns_WithNoArgument_NoDiagnosticReported(string methodName)
        {
            var returnType = "int";

            if (methodName == "ReturnsAsync")
            {
                returnType = "Task<" + returnType + ">";
            }

            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Setup(i => i.TestMethod(""Ignored""))
                                .Callback(() => { })
                                ." + methodName + @"(() => { return 1234; });
                        }
                    }

                    public interface I
                    {
                        " + returnType + @" TestMethod(string a);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Returns_WithArgumentsForProperty_DiagnosticReported()
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
                            mock1.Setup(i => i.TestProperty)
                                .Callback(() => { })
                                .Returns({|PosInfoMoq2013:(int s)|} => { return 1234; });
                        }
                    }

                    public interface I
                    {
                        int TestProperty { get; set; }
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
                            var mock1 = new Mock<I>();
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
                        public Mock() { }

                        public void Setup() { }
                    }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}