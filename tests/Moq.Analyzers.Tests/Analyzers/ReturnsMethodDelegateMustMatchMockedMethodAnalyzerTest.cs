//-----------------------------------------------------------------------
// <copyright file="ReturnsMethodDelegateMustMatchMockedMethodAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<ReturnsMethodDelegateMustMatchMockedMethodAnalyzer>;

    public class ReturnsMethodDelegateMustMatchMockedMethodAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                int TestMethod();

                int TestMethod(string a);

                T TestMethod<T>();

                int GetData();

                int TestProperty { get; set; }
            }

            public interface IAsync
            {
                Task<int> TestMethod();

                Task<int> TestMethod(string a);
            }

            public interface IObject
            {
                object TestMethod();
            }

            public interface IAsyncObject
            {
                Task<object> TestMethod();
            }

            public class CustomMock<T>
            {
                public void Setup() { }
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod())
                .Callback(() => { })
                .Returns(() => { return 1234; });
            """,

            """
            var mock = new Mock<IAsync>();
            mock.Setup(i => i.TestMethod())
                .Callback(() => { })
                .ReturnsAsync(() => { return 1234; });
            """,

            """
            var mock = new Mock<IObject>();
            mock.Setup(i => i.TestMethod())
                .Callback(() => { })
                .Returns(() => { return 1234; });
            """,

            """
            var mock = new Mock<IAsyncObject>();
            mock.Setup(i => i.TestMethod())
                .Callback(() => { })
                .ReturnsAsync(() => { return 1234; });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod<It.IsAnyType>())
                .Callback(() => { })
                .Returns(() => { return 1234; });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod<It.IsAnyType>())
                .Callback(() => { })
                .Returns(() => { return "Foobar"; });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestProperty)
                .Callback(() => { })
                .Returns(1234);
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestProperty)
                .Callback(() => { })
                .Returns(() => { return 1234; });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.GetData())
                .Callback(() => { })
                .Returns(1234);
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod("Ignored"))
                .Callback(() => { })
                .Returns((string z) => { return 1234; });
            """,

            """
            var mock = new Mock<IAsync>();
            mock.Setup(i => i.TestMethod("Ignored"))
                .Callback(() => { })
                .ReturnsAsync((string z) => { return 1234; });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod("Ignored"))
                .Callback(() => { })
                .Returns(() => { return 1234; });
            """,

            """
            var mock = new Mock<IAsync>();
            mock.Setup(i => i.TestMethod("Ignored"))
                .Callback(() => { })
                .ReturnsAsync(() => { return 1234; });
            """,

            """
            var mock = new CustomMock<I>();
            mock.Setup();
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod())
                .Callback(() => { })
                .Returns({|PosInfoMoq2012:() => { return "Foobar"; }|});
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestProperty)
                .Callback(() => { })
                .Returns({|PosInfoMoq2012:() => { return "Foobar"; }|});
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod("Ignored"))
                .Callback(() => { })
                .Returns(({|PosInfoMoq2013:int s|}) => { return 1234; });
            """,

            """
            var mock = new Mock<IAsync>();
            mock.Setup(i => i.TestMethod("Ignored"))
                .Callback(() => { })
                .ReturnsAsync(({|PosInfoMoq2013:int s|}) => { return 1234; });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestMethod("Ignored"))
                .Callback(() => { })
                .Returns({|PosInfoMoq2013:(int s, bool b)|} => { return 1234; });
            """,

            """
            var mock = new Mock<IAsync>();
            mock.Setup(i => i.TestMethod("Ignored"))
                .Callback(() => { })
                .ReturnsAsync({|PosInfoMoq2013:(int s, bool b)|} => { return 1234; });
            """,

            """
            var mock = new Mock<I>();
            mock.Setup(i => i.TestProperty)
                .Callback(() => { })
                .Returns({|PosInfoMoq2013:(int s)|} => { return 1234; });
            """,
        ];

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