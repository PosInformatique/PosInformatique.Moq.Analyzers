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
        [Theory]
        [InlineData("")]
        [InlineData("MockBehavior.Loose")]
        [InlineData("MockBehavior.Default")]
        public async Task NewMock_Fix(string behavior)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = [|new Mock<I>(" + behavior + @")|];
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

        [Theory]
        [InlineData("")]
        [InlineData(", MockBehavior.Loose")]
        [InlineData(", MockBehavior.Default")]
        public async Task NewMock_Fix_WithLambdaExpression(string behavior)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = [|new Mock<C>(() => new C()" + behavior + @")|];
                        }
                    }

                    public class C
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
                            var mock = new Mock<C>(() => new C(), MockBehavior.Strict);
                        }
                    }

                    public class C
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
        public async Task NewMock_Fix_WithArguments(string arguments, string expectedArguments)
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

        [Theory]
        [InlineData("")]
        [InlineData("MockBehavior.Loose")]
        [InlineData("MockBehavior.Default")]
        public async Task MockOf_Fix(string behavior)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = [|Mock.Of<I>(" + behavior + @")|];
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

        [Theory]
        [InlineData("", "MockBehavior.Strict")]
        [InlineData("MockBehavior.Loose", "MockBehavior.Strict")]
        [InlineData("i => true", "i => true, MockBehavior.Strict")]
        [InlineData("i => true, MockBehavior.Default", "i => true, MockBehavior.Strict")]
        [InlineData("i => true, MockBehavior.Loose", "i => true, MockBehavior.Strict")]
        public async Task MockOf_Fix_WithArguments(string arguments, string expectedArguments)
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

        [Theory]
        [InlineData("")]
        [InlineData("MockBehavior.Loose")]
        [InlineData("MockBehavior.Default")]
        public async Task MockOf_InConstructor_Fix(string behavior)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new C([|Mock.Of<I>(" + behavior + @")|], [|Mock.Of<I>(" + behavior + @")|]);
                        }
                    }

                    public interface I
                    {
                    }

                    public class C
                    {
                        public C(I r1, I r2)
                        {
                        }
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
                            var mock = new C(Mock.Of<I>(MockBehavior.Strict), Mock.Of<I>(MockBehavior.Strict));
                        }
                    }

                    public interface I
                    {
                    }

                    public class C
                    {
                        public C(I r1, I r2)
                        {
                        }
                    }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }
    }
}
