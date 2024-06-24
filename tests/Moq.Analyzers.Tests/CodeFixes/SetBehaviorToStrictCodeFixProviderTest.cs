//-----------------------------------------------------------------------
// <copyright file="SetBehaviorToStrictCodeFixProviderTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verifier = MoqCSharpCodeFixVerifier<MockInstanceShouldBeStrictBehaviorAnalyzer, SetBehaviorToStrictCodeFixProvider>;

    public class SetBehaviorToStrictCodeFixProviderTest
    {
        [Fact]
        public async Task Fix_Loose()
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
        public async Task Fix_MissingBehavior()
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

        [Fact]
        public async Task Fix_MissingBehavior_WithArguments()
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
                            var mock2 = [|new Mock<I>(MockBehavior.Loose, 1, 2, 3)|];
                            var mock3 = [|new Mock<I>(OtherEnum.A, 1, 2, 3)|];
                            var mock4 = [|new Mock<I>(int.MaxValue, 1, 2, 3)|];
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
                            var mock1 = new Mock<I>(MockBehavior.Strict, 1, 2, 3);
                            var mock2 = new Mock<I>(MockBehavior.Strict, 1, 2, 3);
                            var mock3 = new Mock<I>(MockBehavior.Strict, OtherEnum.A, 1, 2, 3);
                            var mock4 = new Mock<I>(MockBehavior.Strict, int.MaxValue, 1, 2, 3);
                        }
                    }

                    public interface I
                    {
                    }

                    public enum OtherEnum { A }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }
    }
}
