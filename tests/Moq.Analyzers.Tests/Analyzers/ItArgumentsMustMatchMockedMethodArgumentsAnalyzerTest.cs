//-----------------------------------------------------------------------
// <copyright file="ItArgumentsMustMatchMockedMethodArgumentsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<ItArgumentsMustMatchMockedMethodArgumentsAnalyzer>;

    public class ItArgumentsMustMatchMockedMethodArgumentsAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                void NoParametersMethod();

                void TestMethod();

                void TestMethod(int? a, int b, string c);

                void TestMethod(int? a, int b, string c, int d);

                void TestMethodWithBaseClass(BaseClass a);
            }

            public class C
            {
                public static int OtherMethod() => default;
            }

            public abstract class BaseClass
            {
                public int Property { get; set; }
            }

            public class InheritedClass : BaseClass
            {
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.NoParametersMethod());
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethod(It.IsAny<int?>(), 1, "Ignored", C.OtherMethod()));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethod(It.Is<int?>(p => p == 10), 1, "Ignored", C.OtherMethod()));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethod(It.Is<int?>(10, EqualityComparer<int?>.Default), 1, "Ignored", C.OtherMethod()));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethodWithBaseClass(null));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethodWithBaseClass(It.IsAny<BaseClass>()));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethodWithBaseClass(It.IsAny<InheritedClass>()));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethodWithBaseClass(It.Is<BaseClass>(c => c.Property == 10)));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethodWithBaseClass(It.Is<InheritedClass>(c => c.Property == 10)));
            """,

            """
            var mock1 = new Mock<I>(MockBehavior.Strict);
            mock1.Setup(m => m.TestMethod());
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethod({|PosInfoMoq1006:It.IsAny<int>()|}, 1, "Ignored"));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethod({|PosInfoMoq1006:It.Is<int>(p => p == 10)|}, 1, "Ignored"));
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Setup(m => m.TestMethod({|PosInfoMoq1006:It.Is<int>(10, EqualityComparer<int>.Default)|}, 1, "Ignored"));
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
                    using System.Collections.Generic;

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
                    using System.Collections.Generic;

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