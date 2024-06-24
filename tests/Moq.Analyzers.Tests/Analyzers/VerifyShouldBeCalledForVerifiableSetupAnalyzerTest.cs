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
                                .Verifiable();
                            
                            var mock2 = new Mock<I>();
                            mock2.Setup(m => m.Method())
                                .Verifiable();
                                 
                            var mock3 = new Mock<I>();
                            mock3.Setup(m => m.Method())
                                .Verifiable();
                           
                            var mock9 = new Mock<I>();
                            mock9.Setup(m => m.Method());   // No Verifiable()
                 
                            var o = new object();
                            var s = o.ToString();       // Ignored

                            var l = s.Length;   // Ignored

                            mock1.Verify();
                            Mock.Verify(mock2, mock3, default);
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
        public async Task Verify_NotCalled()
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
                                .[|Verifiable|]();
                            
                            var o = new object();
                            o.ToString();       // Ignored
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
