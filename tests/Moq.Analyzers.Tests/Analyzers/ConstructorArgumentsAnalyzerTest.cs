﻿//-----------------------------------------------------------------------
// <copyright file="ConstructorArgumentsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<ConstructorArgumentsAnalyzer>;

    public class ConstructorArgumentsAnalyzerTest
    {
        [Fact]
        public async Task NoMock()
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

                    public class C
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Interface_WithNotMockableType()
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

                    public delegate void I();
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

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
                new DiagnosticResult(ConstructorArgumentsAnalyzer.ConstructorArgumentsCanBePassedToInterfaceRule)
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
                new DiagnosticResult(ConstructorArgumentsAnalyzer.ConstructorArgumentsCanBePassedToInterfaceRule)
                    .WithLocation(0).WithArguments("1")
                    .WithLocation(1).WithArguments("2"));
        }

        [Fact]
        public async Task Arguments_Empty()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>();
                        }
                    }

                    public class C
                    {
                        public C()
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_Empty_WithPrivateConstructor()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>{|PosInfoMoq2011:()|};
                        }
                    }

                    public class C
                    {
                        private C()
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("1, \"B\"")]
        [InlineData("1, null")]
        [InlineData("default, \"B\"")]
        [InlineData("(int)default, default")]
        [InlineData("default, null, 1234")]
        [InlineData("1, \"An object\", 3, null")]
        [InlineData("1, \"An object\", 3, new System.IO.MemoryStream()")]
        public async Task Arguments_Match(string parameters)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<C>(" + parameters + @");
                        }
                    }

                    public class C
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object b)
                        {
                        }

                        public C(int a, int[] b, int c)
                        {
                        }

                        public C(int a, object b, int c, System.IDisposable d)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("1, \"B\"")]
        [InlineData("1, null")]
        [InlineData("default, \"B\"")]
        [InlineData("(int)default, default")]
        [InlineData("default, null, 1234")]
        [InlineData("1, \"An object\", 3, null")]
        [InlineData("1, \"An object\", 3, new System.IO.MemoryStream()")]
        public async Task Arguments_Match_WithPrivateConstructor(string parameters)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<C>({|PosInfoMoq2011:" + parameters + @"|});
                        }
                    }

                    public class C
                    {
                        private C(int a)
                        {
                        }

                        private C(int a, string b)
                        {
                        }

                        private C(int a, object b)
                        {
                        }

                        private C(int a, int[] b, int c)
                        {
                        }

                        private C(int a, object b, int c, System.IDisposable d)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("abstract class")]
        [InlineData("interface")]
        public async Task Arguments_Match_WithNoConstructor(string type)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<ClassWithNoConstructor>();
                        }
                    }

                    public " + type + @" ClassWithNoConstructor { }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("1, \"B\"")]
        [InlineData("1, \"An object\", 3, null")]
        [InlineData("1, \"An object\", 3, new System.IO.MemoryStream()")]
        public async Task Arguments_Match_WithMockBehavior(string parameters)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<C>(MockBehavior.Strict, " + parameters + @");
                       }
                    }

                    public class C
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object c)
                        {
                        }

                        public C(int a, object b, int c, System.IDisposable d)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("abstract class")]
        [InlineData("interface")]
        public async Task Arguments_Match_WithMockBehavior_WithNoConstructor(string type)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<ClassWithNoConstructor>(MockBehavior.Strict);
                       }
                    }

                    public " + type + @" ClassWithNoConstructor { }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("1, 2, 3")]
        [InlineData("null")]
        [InlineData("\"The string\", 2")]
        [InlineData("1, 2, 3, \"The string\"")]
        public async Task Arguments_NotMatch(string parameters)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<C>({|PosInfoMoq2005:" + parameters + @"|});
                        }
                    }

                    public class C
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object c)
                        {
                        }

                        public C(int a, object b, int c, System.IDisposable d)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_WithDefaultParameters()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<C>{|PosInfoMoq2005:()|};
                        }
                    }

                    public class C
                    {
                        public C(int a = 0, int b = 1, int c = 2, int d = 3)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_NotMatch_WithNoContructor()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<ClassWithNoConstructor>({|PosInfoMoq2005:1, 2|});
                        }
                    }

                    public class ClassWithNoConstructor { }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("1, 2, 3")]
        [InlineData("null")]
        [InlineData("\"The string\", 2")]
        [InlineData("1, 2, 3, \"The string\"")]
        public async Task Arguments_NotMatch_WithMockBehavior(string parameters)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<C>(MockBehavior.Strict, {|PosInfoMoq2005:" + parameters + @"|});
                        }
                    }

                    public class C
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object c)
                        {
                        }
                    }
               }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_NotMatch_WithMockBehavior_WithNoConstructor()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<ClassWithNoConstructor>(MockBehavior.Strict, {|PosInfoMoq2005:1, 2|});
                        }
                    }
 
                    public class ClassWithNoConstructor { }
               }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_CountNotMatch()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>{|PosInfoMoq2005:()|};
                        }
                    }

                    public class C
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object c)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_CountNotMatch_MockBehavior()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>{|PosInfoMoq2005:(MockBehavior.Strict)|};
                        }
                    }

                    public class C
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object c)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(", MockBehavior.Strict")]
        public async Task Arguments_WithFactory(string behavior)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>(() => new C(10)" + behavior + @");
                        }
                    }

                    public class C
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object c)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(", MockBehavior.Strict")]
        public async Task Arguments_WithFactory_NotClass(string behavior)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<{|PosInfoMoq2016:I|}>(() => new C(10)" + behavior + @");
                        }
                    }

                    public interface I
                    {
                    }

                    public class C : I
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object c)
                        {
                        }
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
                            var mock1 = new Mock<C>(MockBehavior.Strict, 1, 2, 3);
                        }
                    }

                    public class C
                    {
                        public C(int a)
                        {
                        }

                        public C(int a, string b)
                        {
                        }

                        public C(int a, object c)
                        {
                        }
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