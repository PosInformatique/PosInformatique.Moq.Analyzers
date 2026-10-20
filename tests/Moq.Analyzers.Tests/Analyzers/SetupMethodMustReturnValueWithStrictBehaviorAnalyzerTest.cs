//-----------------------------------------------------------------------
// <copyright file="SetupMethodMustReturnValueWithStrictBehaviorAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<SetupMethodMustReturnValueWithStrictBehaviorAnalyzer>;

    public class SetupMethodMustReturnValueWithStrictBehaviorAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                int TestMethod();

                Task<int> TestAsyncMethod();

                int TestProperty { get; set; }

                int Property { get; set; }
            }

            public interface IVoid
            {
                void TestMethod();
            }

            public enum OtherEnum { A, B }

            public class CustomMock<T>
            {
                public CustomMock(MockBehavior behavior) { }

                public void Setup() { }
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock = new Mock<I>(MockBehavior.Strict);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { })
                .Returns(1234);
            """,

            """
            var mock = new Mock<I>(MockBehavior.Strict);
            mock.Setup(i => i.TestAsyncMethod())
                .Callback(() => { })
                .ReturnsAsync(1234);
            """,

            """
            var mock = new Mock<I>(MockBehavior.Strict);
            mock.Setup(i => i.TestProperty)
                .Callback(() => { })
                .Returns(1234);
            """,

            """var mock = new Mock<I>(MockBehavior.Strict);""",

            """
            Mock<I> mock;
            mock = new Mock<I>(MockBehavior.Strict);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var original = new Mock<I>(MockBehavior.Strict);
            Mock<I> mock = original;
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            Mock<I> mock = new Mock<I>(MockBehavior.Strict);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { })
                .Throws(new Exception());
            """,

            """
            Mock<I> mock = new Mock<I>(MockBehavior.Strict);
            mock.Setup(i => i.TestAsyncMethod())
                .Callback(() => { })
                .ThrowsAsync(new Exception());
            """,

            """
            var obj = new object();
            obj.ToString();
            """,

            """
            var action = new Action(() => { });
            action();
            """,

            """
            var mock = new Mock<I>(MockBehavior.Loose);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(MockBehavior.Loose);
            mock.Setup(i => i.TestProperty)
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestProperty)
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(1, 2);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(1, 2);
            mock.Setup(i => i.TestProperty)
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(OtherEnum.A, 1, 2);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(OtherEnum.A, 1, 2);
            mock.Setup(i => i.TestProperty)
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(MockBehavior.Loose, 1, 2);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(MockBehavior.Loose, 1, 2);
            mock.Setup(i => i.TestProperty)
                .Callback(() => { });
            """,

            """
            var mock = new Mock<IVoid>(MockBehavior.Strict);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<IVoid>(MockBehavior.Strict, 1, 2);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<IVoid>(MockBehavior.Loose);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<IVoid>(MockBehavior.Loose, 1, 2);
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<IVoid>();
            mock.Setup(i => i.TestMethod())
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(MockBehavior.Loose);
            mock.Setup(i => i.Property)
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(MockBehavior.Loose, 1, 2);
            mock.Setup(i => i.Property)
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.Property)
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>();
            mock.Verify();
            """,

            """
            var mock = new CustomMock<I>(MockBehavior.Strict);
            mock.Setup();
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock = new Mock<I>(MockBehavior.Strict);
            {|PosInfoMoq2000:mock.Setup(i => i.TestMethod())|}
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>(MockBehavior.Strict);
            {|PosInfoMoq2000:mock.Setup(i => i.TestProperty)|}
                .Callback(() => { });
            """,

            """
            var mock = new Mock<I>();
            var mock1 = new Mock<I>(MockBehavior.Strict);
            var mock2 = new Mock<I>(MockBehavior.Strict);
            {
                {
                    {|PosInfoMoq2000:mock1.Setup(i => i.TestMethod())|}
                        .Callback(() => { });

                    {|PosInfoMoq2000:mock1.Setup(i => i.TestProperty)|}
                        .Callback(() => { });
                }
            }

            {|PosInfoMoq2000:mock2.Setup(i => i.TestMethod())|}
                .Callback(() => { });

            {|PosInfoMoq2000:mock2.Setup(i => i.TestProperty)|}
                .Callback(() => { });
            """,
        ];

        [Fact]
        public async Task Returns_StrictMode_InvalidReturnsType_NoDiagnosticReported()
        {
            var source =
                $$"""
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            mock1.Setup(i => i.TestMethod())
                                .Callback(() => { })
                                .Returns({|#0:"Foobar"|});
                            mock1.Setup(i => i.TestProperty)
                                .Callback(() => { })
                                .Returns({|#1:"Foobar"|});
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        int TestProperty { get; set; }
                    }
                }
                """;

            await Verifier.VerifyAnalyzerAsync(
                source,
                DiagnosticResult.CompilerError("CS1503").WithLocation(0).WithArguments("1", "string", "int"),
                DiagnosticResult.CompilerError("CS1503").WithLocation(1).WithArguments("1", "string", "int"));
        }

        [Theory]
        [MemberData(nameof(ValidTestCases))]
        [MemberData(nameof(InvalidTestCases))]
        public async Task Analyze(string code)
        {
            var source =
                $$"""
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            {{code}}
                        }
                    }

                    {{TestTypes}}
                }
                """;

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [MemberData(nameof(ValidTestCases))]
        [MemberData(nameof(InvalidTestCases))]
        public async Task NoMoqLibrary(string code)
        {
            var source =
                $$"""
                namespace ConsoleApplication1
                {
                    using FakeMoqLibrary;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            {{code}}
                        }
                    }

                    {{TestTypes}}
                }

                {{MoqAnalyzersTests.FakeMoqLibrary}}
                """;

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}