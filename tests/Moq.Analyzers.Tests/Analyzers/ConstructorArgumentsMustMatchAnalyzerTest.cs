﻿//-----------------------------------------------------------------------
// <copyright file="ConstructorArgumentsMustMatchAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        ConstructorArgumentsMustMatchAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

    public class ConstructorArgumentsMustMatchAnalyzerTest
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
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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

                    public struct I
                    {
                    }
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_Match()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>(1, ""B"");
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
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_Match_WithMockBehavior()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>(MockBehavior.Strict, 1, ""B"");
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
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_NotMatch()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>([|1, 2|]);
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
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Arguments_NotMatch_WithMockBehavior()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>(MockBehavior.Strict, [|1, 2|]);
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
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                            var mock1 = new Mock<C>[|()|];
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
                }" + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                            var mock1 = new Mock<C>[|(MockBehavior.Strict)|];
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
                }" + MoqLibrary.Code;

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

            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}