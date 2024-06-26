﻿//-----------------------------------------------------------------------
// <copyright file="ConstructorArgumentsMustMatchAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<ConstructorArgumentsMustMatchAnalyzer>;

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
                            var mock2 = new Mock<C>(1, null);
                            var mock3 = new Mock<C>(default, ""B"");
                            var mock4 = new Mock<C>((int)default, default);
                            var mock5 = new Mock<C>(default, null, 1234);
                            var mock6 = new Mock<C>(1, ""An object"", 3, null);
                            var mock7 = new Mock<C>(1, ""An object"", 3, new System.IO.MemoryStream());

                            var mock8 = new Mock<ClassWithNoConstructor>();
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

                    public class ClassWithNoConstructor { }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
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
                            var mock6 = new Mock<C>(MockBehavior.Strict, 1, ""An object"", 3, null);
                            var mock7 = new Mock<C>(MockBehavior.Strict, 1, ""An object"", 3, new System.IO.MemoryStream());
 
                            var mock8 = new Mock<ClassWithNoConstructor>(MockBehavior.Strict);
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

                    public class ClassWithNoConstructor { }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
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
                            var mock1 = new Mock<C>([|1, 2, 3|]);
                            var mock2 = new Mock<C>([|null|]);
                            var mock3 = new Mock<C>([|""The string"", 2|]);
                            var mock4 = new Mock<C>([|1, 2, 3, ""The string""|]);
 
                            var mock8 = new Mock<ClassWithNoConstructor>([|1, 2|]);
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

                    public class ClassWithNoConstructor { }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
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
                            var mock1 = new Mock<C>(MockBehavior.Strict, [|1, 2, 3|]);
                            var mock2 = new Mock<C>(MockBehavior.Strict, [|null|]);
                            var mock3 = new Mock<C>(MockBehavior.Strict, [|""The string"", 2|]);
                            var mock4 = new Mock<C>(MockBehavior.Strict, [|1, 2, 3, ""The string""|]);
 
                            var mock8 = new Mock<ClassWithNoConstructor>(MockBehavior.Strict, [|1, 2|]);
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