namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        VerifyAllShouldBeCalledAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

    public class VerifyAllShouldBeCalledAnalyzerTest
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
                            
                            var o = new object();
                            o.ToString();

                            var mock2 = new Mock<I>();

                            new Mock<I>();  // No variable (ignored)

                            new System.Action(() => { })();     // Ignored

                            mock1.Property = 1234;  // Property access are ignored.

                            mock1.VerifyAll();
                            mock2.Verify(1, 2);
                        }
                    }

                    public interface I
                    {
                    }
                }

                namespace Moq
                {
                    public class Mock<T>
                    {
                        public void VerifyAll() { }
                        public void Verify(int a, int b) { }
                        public object Property { get; set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Verify_Missing_Calls()
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

                            mock1.Property = 1234;  // Property access are ignored.
                        }
                    }

                    public interface I
                    {
                    }
                }

                namespace Moq
                {
                    public class Mock<T>
                    {
                        public object Property { get; set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Verify_Missing_Calls_WithArguments()
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
                }

                namespace Moq
                {
                    public class Mock<T>
                    {
                        public Mock(params object[] args)
                        {
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(source);
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

            await Verify.VerifyAnalyzerAsync(source);
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

            await Verify.VerifyAnalyzerAsync(source);
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
                }

                namespace Moq
                {
                    public class Mock<T>
                    {
                        public object Property { get; set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}
