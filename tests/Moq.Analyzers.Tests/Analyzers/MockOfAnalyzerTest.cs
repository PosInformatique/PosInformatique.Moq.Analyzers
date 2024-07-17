//-----------------------------------------------------------------------
// <copyright file="MockOfAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<MockOfAnalyzer>;

    public class MockOfAnalyzerTest
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
                            var mock = Mock.Of<StandardClass>();
                        }
                    }

                    public class StandardClass
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoSealedClass_WithMultipleConstructors_NoDiagnosticReported()
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
                            var mock = Mock.Of<StandardClass>();
                        }
                    }

                    public class StandardClass
                    {
                        public StandardClass()
                        {
                        }

                        public StandardClass(int a)
                        {
                        }

                        public StandardClass(string b)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoSealedClass_WithExpression_NoDiagnosticReported()
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
                            var mock = Mock.Of<StandardClass>(sc => sc.Property == 1234);
                        }
                    }

                    public class StandardClass
                    {
                        public int Property { get; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoSealedClass_WithExpression_WithMultipleConstructors_NoDiagnosticReported()
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
                            var mock = Mock.Of<StandardClass>(sc => sc.Property == 1234);
                        }
                    }

                    public class StandardClass
                    {
                        public StandardClass()
                        {
                        }

                        public StandardClass(int a)
                        {
                        }

                        public StandardClass(string b)
                        {
                        }

                        public int Property { get; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Interface_NoDiagnosticReported()
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
                            var mock = Mock.Of<I>();
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Interface_WithExpression_NoDiagnosticReported()
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
                            var mock = Mock.Of<I>(sc => sc.Property == 1234);
                        }
                    }

                    public interface I
                    {
                        int Property { get; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData("MockBehavior.Strict")]
        public async Task AbstractClass_NoDiagnosticReported(string mockBehavior)
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
                            var mock = Mock.Of<AbstractClass>(" + mockBehavior + @");
                        }
                    }

                    public abstract class AbstractClass
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData("MockBehavior.Strict")]
        public async Task AbstractClass_WithMultipleConstructors_NoDiagnosticReported(string mockBehavior)
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
                            var mock = Mock.Of<AbstractClass>(" + mockBehavior + @");
                        }
                    }

                    public abstract class AbstractClass
                    {
                        public AbstractClass()
                        {
                        }

                        public AbstractClass(int a)
                        {
                        }

                        public AbstractClass(string b)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(", MockBehavior.Strict")]
        public async Task AbstractClass_WithExpression_NoDiagnosticReported(string mockBehavior)
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
                            var mock = Mock.Of<AbstractClass>(sc => sc.Property == 1234" + mockBehavior + @");
                        }
                    }

                    public abstract class AbstractClass
                    {
                        public int Property { get; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(", MockBehavior.Strict")]
        public async Task AbstractClass_WithExpression_WithMultipleConstructors_NoDiagnosticReported(string mockBehavior)
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
                            var mock = Mock.Of<AbstractClass>(sc => sc.Property == 1234" + mockBehavior + @");
                        }
                    }

                    public abstract class AbstractClass
                    {
                        public int Property { get; }

                        public AbstractClass()
                        {
                        }

                        public AbstractClass(int a)
                        {
                        }

                        public AbstractClass(string b)
                        {
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoMockOfUsage_NoDiagnosticReported()
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
                            string.Format(""Ignored"");     // Ignored by the analysis.
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task SealedClass_DiagnosticReported()
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
                            var mock = Mock.Of<{|PosInfoMoq2009:SealedClass|}>();
                        }
                    }

                    public sealed class SealedClass
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task SealedClass_WithExpression_DiagnosticReported()
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
                            var mock = Mock.Of<{|PosInfoMoq2009:SealedClass|]>(sc => sc.Property == 1234);
                        }
                    }

                    public sealed class SealedClass
                    {
                        public int Property { get; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData("MockBehavior.Strict")]
        public async Task Struct_DiagnosticReported(string mockBehavior)
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
                            var mock = Mock.Of<{|PosInfoMoq2009:string|]>(" + mockBehavior + @");
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(", MockBehavior.Strict")]
        public async Task Struct_WithExpression_DiagnosticReported(string mockBehavior)
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
                            var mock = Mock.Of<{|PosInfoMoq2009:string|]>(sc => sc.Length == 1234" + mockBehavior + @");
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(", MockBehavior.Strict")]
        public async Task Struct_WithExpressionAndTypeInLambdaParameter_DiagnosticReported(string mockBehavior)
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
                            var mock = Mock.Of(({|PosInfoMoq2009:string|] sc) => sc.Length == 1234" + mockBehavior + @");
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abstract")]
        public async Task NoParameterLessConstructor_DiagnosticReported(string classModifiers)
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
                            var mock = Mock.Of<{|PosInfoMoq2010:MockClass|}>();
                        }
                    }

                    public " + classModifiers + @" class MockClass
                    {
                        public MockClass(int a) { }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abstract")]
        public async Task NoParameterLessConstructor_WithExpression_DiagnosticReported(string classModifiers)
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
                            var mock = Mock.Of(({|PosInfoMoq2010:MockClass|} m) => true);
                        }
                    }

                    public " + classModifiers + @" class MockClass
                    {
                        public MockClass(int a) { }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abstract")]
        public async Task PrivateParameterLessConstructor_DiagnosticReported(string classModifiers)
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
                            var mock = Mock.Of<{|PosInfoMoq2010:MockClass|}>();
                        }
                    }

                    public " + classModifiers + @" class MockClass
                    {
                        private MockClass() { }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abstract")]
        public async Task PrivateParameterLessConstructor_WithExpression_DiagnosticReported(string classModifiers)
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
                            var mock = Mock.Of(({|PosInfoMoq2010:MockClass|} m) => true);
                        }
                    }

                    public " + classModifiers + @" class MockClass
                    {
                        private MockClass() { }
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
                            var mock = Mock.Of<I>();
                        }
                    }

                    public interface I
                    {
                    }
                }

                namespace OtherNamespace
                {
                    public static class Mock
                    {
                        public static T Of<T>() { return default; }
                    }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}