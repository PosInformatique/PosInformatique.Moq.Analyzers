//-----------------------------------------------------------------------
// <copyright file="SetBehaviorToStrictCodeFixProviderTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpCodeFixVerifier<MockInstanceShouldBeStrictBehaviorAnalyzer, SetBehaviorToStrictCodeFixProvider>;

    public class SetBehaviorToStrictCodeFixProviderTest
    {
        [Fact]
        public async Task NewMock_Fix_Loose()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = [|new Mock<I>(MockBehavior.Loose)|];
                        }
                    }

                    public interface I
                    {
                    }
                }";

            var expectedFixedSource =
            @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<I>(MockBehavior.Strict);
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Fact]
        public async Task NewMock_Fix_MissingBehavior()
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
                            var mock2 = [|new Mock<I> { }|];
                        }
                    }

                    public interface I
                    {
                    }
                }";

            var expectedFixedSource =
            @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            var mock2 = new Mock<I>(MockBehavior.Strict) { };
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Theory]
        [InlineData("1, 2, 3", "MockBehavior.Strict, 1, 2, 3")]
        [InlineData("MockBehavior.Loose, 1, 2, 3", "MockBehavior.Strict, 1, 2, 3")]
        [InlineData("MockBehavior.Default, 1, 2, 3", "MockBehavior.Strict, 1, 2, 3")]
        [InlineData("OtherEnum.A, 1, 2, 3", "MockBehavior.Strict, OtherEnum.A, 1, 2, 3")]
        [InlineData("int.MaxValue, 1, 2, 3", "MockBehavior.Strict, int.MaxValue, 1, 2, 3")]
        public async Task NewMock_Fix_MissingBehavior_WithArguments(string arguments, string expectedArguments)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = [|new Mock<I>(" + arguments + @")|];
                        }
                    }

                    public interface I
                    {
                    }

                    public enum OtherEnum { A }
                }";

            var expectedFixedSource =
            @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<I>(" + expectedArguments + @");
                        }
                    }

                    public interface I
                    {
                    }

                    public enum OtherEnum { A }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Fact]
        public async Task MockOf_Fix_Loose()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = [|Mock.Of<I>(MockBehavior.Loose)|];
                        }
                    }

                    public interface I
                    {
                    }
                }";

            var expectedFixedSource =
            @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = Mock.Of<I>(MockBehavior.Strict);
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Fact]
        public async Task MockOf_Fix_MissingBehavior()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = [|Mock.Of<I>()|];
                        }
                    }

                    public interface I
                    {
                    }
                }";

            var expectedFixedSource =
            @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = Mock.Of<I>(MockBehavior.Strict);
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Theory]
        [InlineData("", "MockBehavior.Strict")]
        [InlineData("MockBehavior.Loose", "MockBehavior.Strict")]
        [InlineData("i => true", "i => true, MockBehavior.Strict")]
        [InlineData("i => true, MockBehavior.Default", "i => true, MockBehavior.Strict")]
        [InlineData("i => true, MockBehavior.Loose", "i => true, MockBehavior.Strict")]
        public async Task MockOf_Fix_MissingBehavior_WithArguments(string arguments, string expectedArguments)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = [|Mock.Of<I>(" + arguments + @")|];
                        }
                    }

                    public interface I
                    {
                    }
                }";

            var expectedFixedSource =
            @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = Mock.Of<I>(" + expectedArguments + @");
                        }
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }
    }
}
