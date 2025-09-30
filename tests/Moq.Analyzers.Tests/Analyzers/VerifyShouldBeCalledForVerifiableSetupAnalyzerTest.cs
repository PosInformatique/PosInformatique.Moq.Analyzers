//-----------------------------------------------------------------------
// <copyright file="VerifyShouldBeCalledForVerifiableSetupAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyShouldBeCalledForVerifiableSetupAnalyzer>;

    public class VerifyShouldBeCalledForVerifiableSetupAnalyzerTest
    {
        [Fact]
        public async Task Verifiable_NotUsed()
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
                            mock1.Setup(m => m.Method());
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
        public async Task Verify_Called()
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
                            mock1.Setup(m => m.Method())
                                .{|PosInfoMoq1009:Verifiable|}();
                            
                            var mock2 = new Mock<I>();
                            mock2.Setup(m => m.Method())
                                .{|PosInfoMoq1009:Verifiable|}();
                                 
                            var mock3 = new Mock<I>();
                            mock3.Setup(m => m.Method())
                                .{|PosInfoMoq1009:Verifiable|}();

                            var mock4 = new Mock<I>();
                            mock4.Setup(m => m.Method())
                                .{|PosInfoMoq1009:Verifiable|}();
                            mock4.Setup(m => m.OtherMethod())
                                .{|PosInfoMoq1009:Verifiable|}();

                            var mock5 = new Mock<I>();
                            mock5.Setup(m => m.Method(1, 2))
                                .{|PosInfoMoq1009:Verifiable|}();
                      
                            var mock9 = new Mock<I>();
                            mock9.Setup(m => m.Method());   // No Verifiable()
                 
                            var o = new object();
                            var s = o.ToString();       // Ignored

                            var l = s.Length;   // Ignored

                            mock1.Verify();
                            Mock.Verify(mock2, mock3, default);

                            mock4.Verify(m => m.Method());
                            mock4.Verify(m => m.OtherMethod());

                            mock5.Verify(m => m.Method(1, 2));
                            mock5.Verify(m => m.Method(""A"", 3));  // Not used
                       }
                    }

                    public interface I
                    {
                        void Method();

                        void OtherMethod();

                        void Method(int a, int b);

                        void Method(string c, int d);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Verify_Missing()
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
                            mock1.Setup(m => m.Method())
                                .{|PosInfoMoq1002:{|PosInfoMoq1009:Verifiable|}|}();

                            var mock4 = new Mock<I>();
                            mock4.Setup(m => m.Method())
                                .{|PosInfoMoq1009:Verifiable|}();
                            mock4.Setup(m => m.OtherMethod())
                                .{|PosInfoMoq1002:{|PosInfoMoq1009:Verifiable|}|}();

                            var mock5 = new Mock<I>();
                            mock5.Setup(m => m.Method(1, 2))
                                .{|PosInfoMoq1002:{|PosInfoMoq1009:Verifiable|}|}();
                            
                            var o = new object();
                            o.ToString();       // Ignored

                            mock4.Verify(m => m.Method());

                            mock5.Verify(m => m.Method());
                            mock5.Verify(m => m.Method(""A"", 3));

                            Mock.Verify();      // Do nothing
                      }
                    }

                    public interface I
                    {
                        void Method();

                        void OtherMethod();

                        void Method(int a, int b);

                        void Method(string c, int d);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Verify_MissingWithNullLambdaExpression()
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
                            mock1.Setup(m => m.Method())
                                .{|PosInfoMoq1002:{|PosInfoMoq1009:Verifiable|}|}();

                            mock1.Verify(null);
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
        public async Task Verify_SetupNullArgument()
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
                            mock1.Setup(null)
                                .{|PosInfoMoq1009:Verifiable|}();
                       }
                    }

                    public interface I
                    {
                        void Method();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source);
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
