﻿//-----------------------------------------------------------------------
// <copyright file="VerifyAllShouldBeCalledAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyAllShouldBeCalledAnalyzer>;

    public class VerifyAllShouldBeCalledAnalyzerTest
    {
        [Fact]
        public async Task VerifyAll_Called()
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
                            
                            var o = new object();
                            o.ToString();

                            var mock5 = new Mock<I>();
                            var mock6 = new Mock<I>();

                            new Mock<I>();  // No variable (ignored)

                            new System.Action(() => { })();     // Ignored

                            mock1.CallBase = false;  // Property access are ignored.

                            mock1.VerifyAll();

                            Mock.VerifyAll(mock5, mock6);
                       }
                    }

                    public interface I
                    {
                        void Method();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task VerifyAll_Missing_Calls()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = [|new Mock<I>()|];
                            var mock2 = [|new Mock<I>()|];

                            new Mock<I>();  // No variable (ignored)
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task VerifyAll_Missing_Calls_WithArguments()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = [|new Mock<I>(1, 2, 3)|];
                            var mock2 = [|new Mock<I>(""A"", ""B"", ""C"")|];
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoMock_Instance()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var o = new object();
                            o.ToString();
                        }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoMock_Instance_OtherGenericType()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var o = new Generic<I>();
                            o.ToString();
                        }
                    }

                    public interface I
                    {
                    }

                    public class Generic<T>
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Mock_NotInMethod_Ignored()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public Mock<I> mock1 = new Mock<I>();
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

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}
