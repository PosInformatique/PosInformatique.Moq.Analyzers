//-----------------------------------------------------------------------
// <copyright file="MockInstanceShouldBeStrictBehaviorAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        MockInstanceShouldBeStrictBehaviorAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

    public class MockInstanceShouldBeStrictBehaviorAnalyzerTest
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
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task BehaviorStrict()
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
                        public Mock(MockBehavior _) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task BehaviorLoose()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.[|Loose|]);
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
                        public Mock(MockBehavior _) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoBehavior()
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
                        public Mock() { }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoBehavior_WithNullArgumentList()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = [|new Mock<I> { }|];
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
                        public Mock() { }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Behavior_NotMemberAccessExpression()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>([|default|], 1, 2);
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
                        public Mock(MockBehavior _, int a, int b) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoMoqNamespace()
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

        [Fact]
        public async Task MockBehaviorTypeNotFromMoq()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using OtherNamespace;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>([|NotMockBehavior.Strict|]);
                        }
                    }

                    public interface I
                    {
                    }
                }

                namespace OtherNamespace
                {
                    public enum NotMockBehavior { Strict, Loose }
                }

                namespace Moq
                {
                    using OtherNamespace;

                    public class Mock<T>
                    {
                        public Mock(NotMockBehavior _) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoBehaviorStrict_InMoq()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Loose);
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
                        public Mock(MockBehavior _) { }
                    }

                    public enum MockBehavior { Loose }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}