//-----------------------------------------------------------------------
// <copyright file="ConstructorArgumentsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<ConstructorArgumentsAnalyzer>;

    public class ConstructorArgumentsAnalyzerTest
    {
        private const string TestTypes =
            """
            public delegate void D();

            public interface I { }

            public interface InterfaceWithoutConstructor { }

            public class WithoutDeclaredConstructor { }

            public abstract class AbstractWithoutDeclaredConstructor { }

            public class WithParameterlessConstructor
            {
                public WithParameterlessConstructor() { }
            }

            public class WithPrivateParameterlessConstructor
            {
                private WithPrivateParameterlessConstructor() { }
            }

            public class MatchingConstructors
            {
                public MatchingConstructors(int a) { }

                public MatchingConstructors(int a, string b) { }

                public MatchingConstructors(int a, object b) { }

                public MatchingConstructors(int a, int[] b, int c) { }

                public MatchingConstructors(int a, object b, int c, System.IDisposable d) { }

                public MatchingConstructors(string[] array) { }
            }

            public class WithPrivateConstructors
            {
                private WithPrivateConstructors(int a) { }

                private WithPrivateConstructors(int a, string b) { }

                private WithPrivateConstructors(int a, object b) { }

                private WithPrivateConstructors(int a, int[] b, int c) { }

                private WithPrivateConstructors(int a, object b, int c, System.IDisposable d) { }
            }

            public class StrictMatchingConstructors
            {
                public StrictMatchingConstructors(int a) { }

                public StrictMatchingConstructors(int a, string b) { }

                public StrictMatchingConstructors(int a, object c) { }

                public StrictMatchingConstructors(int a, object b, int c, System.IDisposable d) { }

                public StrictMatchingConstructors(string[] array) { }
            }

            public class MismatchingConstructors
            {
                public MismatchingConstructors(int a) { }

                public MismatchingConstructors(int a, string b) { }

                public MismatchingConstructors(int a, object c) { }

                public MismatchingConstructors(int a, object b, int c, System.IDisposable d) { }

                public MismatchingConstructors(string[] array, int b) { }
            }

            public class StrictMismatchingConstructors
            {
                public StrictMismatchingConstructors(int a) { }

                public StrictMismatchingConstructors(int a, string b) { }

                public StrictMismatchingConstructors(int a, object c) { }

                public StrictMismatchingConstructors(string[] array, int b) { }
            }

            public class WithOptionalParameters
            {
                public WithOptionalParameters(int a = 0, int b = 1, int c = 2, int d = 3) { }
            }

            public class WithRequiredArguments : I
            {
                public WithRequiredArguments(int a) { }

                public WithRequiredArguments(int a, string b) { }

                public WithRequiredArguments(int a, object c) { }
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var obj = new object();
            """,

            """
            var mock = new Mock<D>();
            """,

            """
            var mock = new Mock<I>();
            """,

            """
            var mock = new Mock<WithParameterlessConstructor>();
            """,

            """
            var mock = new Mock<MatchingConstructors>(1);
            """,

            """
            var mock = new Mock<MatchingConstructors>(1, "B");
            """,

            """
            var mock = new Mock<MatchingConstructors>(1, null);
            """,

            """
            var mock = new Mock<MatchingConstructors>(default, "B");
            """,

            """
            var mock = new Mock<MatchingConstructors>((int)default, default);
            """,

            """
            var mock = new Mock<MatchingConstructors>(default, null, 1234);
            """,

            """
            var mock = new Mock<MatchingConstructors>(1, "An object", 3, null);
            """,

            """
            var mock = new Mock<MatchingConstructors>(1, "An object", 3, new System.IO.MemoryStream());
            """,

            """
            var mock = new Mock<MatchingConstructors>([new string[] { "A", "B" }]);
            """,

            """
            var mock = new Mock<MatchingConstructors>(new object[] { new string[] { "A", "B" } });
            """,

            """
            var mock = new Mock<MatchingConstructors>([1]);
            """,

            """
            var mock = new Mock<MatchingConstructors>([1, "A"]);
            """,

            """
            var mock = new Mock<WithoutDeclaredConstructor>();
            """,

            """
            var mock = new Mock<AbstractWithoutDeclaredConstructor>();
            """,

            """
            var mock = new Mock<InterfaceWithoutConstructor>();
            """,

            """
            var mock = new Mock<StrictMatchingConstructors>(MockBehavior.Strict, 1, "B");
            """,

            """
            var mock = new Mock<StrictMatchingConstructors>(MockBehavior.Strict, 1, "An object", 3, null);
            """,

            """
            var mock = new Mock<StrictMatchingConstructors>(MockBehavior.Strict, 1, "An object", 3, new System.IO.MemoryStream());
            """,

            """
            var mock = new Mock<StrictMatchingConstructors>(MockBehavior.Strict, [1]);
            """,

            """
            var mock = new Mock<StrictMatchingConstructors>(MockBehavior.Strict, [1, "A"]);
            """,

            """
            var mock = new Mock<WithoutDeclaredConstructor>(MockBehavior.Strict);
            """,

            """
            var mock = new Mock<AbstractWithoutDeclaredConstructor>(MockBehavior.Strict);
            """,

            """
            var mock = new Mock<InterfaceWithoutConstructor>(MockBehavior.Strict);
            """,

            """
            var mock = new Mock<WithRequiredArguments>(() => new WithRequiredArguments(10));
            """,

            """
            var mock = new Mock<WithRequiredArguments>(() => new WithRequiredArguments(10), MockBehavior.Strict);
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock = new Mock<I>({|PosInfoMoq2004:1|}, 2);
            """,

            """
            var mock = new Mock<I>(MockBehavior.Strict, {|PosInfoMoq2004:1|}, 2);
            """,

            """
            var mock = new Mock<WithPrivateParameterlessConstructor>{|PosInfoMoq2011:()|};
            """,

            """
            var mock = new Mock<WithPrivateConstructors>({|PosInfoMoq2011:1, "B"|});
            """,

            """
            var mock = new Mock<WithPrivateConstructors>({|PosInfoMoq2011:1, null|});
            """,

            """
            var mock = new Mock<WithPrivateConstructors>({|PosInfoMoq2011:default, "B"|});
            """,

            """
            var mock = new Mock<WithPrivateConstructors>({|PosInfoMoq2011:(int)default, default|});
            """,

            """
            var mock = new Mock<WithPrivateConstructors>({|PosInfoMoq2011:default, null, 1234|});
            """,

            """
            var mock = new Mock<WithPrivateConstructors>({|PosInfoMoq2011:1, "An object", 3, null|});
            """,

            """
            var mock = new Mock<WithPrivateConstructors>({|PosInfoMoq2011:1, "An object", 3, new System.IO.MemoryStream()|});
            """,

            """
            var mock = new Mock<MismatchingConstructors>({|PosInfoMoq2005:1, 2, 3|});
            """,

            """
            var mock = new Mock<MismatchingConstructors>({|PosInfoMoq2005:null|});
            """,

            """
            var mock = new Mock<MismatchingConstructors>({|PosInfoMoq2005:"The string", 2|});
            """,

            """
            var mock = new Mock<MismatchingConstructors>({|PosInfoMoq2005:1, 2, 3, "The string"|});
            """,

            """
            var mock = new Mock<MismatchingConstructors>({|PosInfoMoq2005:new int[] { 1, 2 }, 1000|});
            """,

            """
            var mock = new Mock<MismatchingConstructors>({|PosInfoMoq2005:new object[] { "A", "B" }, 1000|});
            """,

            """
            var mock = new Mock<StrictMismatchingConstructors>{|PosInfoMoq2005:([])|};
            """,

            """
            var mock = new Mock<WithOptionalParameters>{|PosInfoMoq2005:()|};
            """,

            """
            var mock = new Mock<WithoutDeclaredConstructor>({|PosInfoMoq2005:1, 2|});
            """,

            """
            var mock = new Mock<StrictMismatchingConstructors>(MockBehavior.Strict, {|PosInfoMoq2005:1, 2, 3|});
            """,

            """
            var mock = new Mock<StrictMismatchingConstructors>(MockBehavior.Strict, {|PosInfoMoq2005:null|});
            """,

            """
            var mock = new Mock<StrictMismatchingConstructors>(MockBehavior.Strict, {|PosInfoMoq2005:"The string", 2|});
            """,

            """
            var mock = new Mock<StrictMismatchingConstructors>(MockBehavior.Strict, {|PosInfoMoq2005:1, 2, 3, "The string"|});
            """,

            """
            var mock = new Mock<StrictMismatchingConstructors>(MockBehavior.Strict, {|PosInfoMoq2005:new int[] { 1, 2 }, 1000|});
            """,

            """
            var mock = new Mock<StrictMismatchingConstructors>{|PosInfoMoq2005:(MockBehavior.Strict, [])|};
            """,

            """
            var mock = new Mock<WithoutDeclaredConstructor>(MockBehavior.Strict, {|PosInfoMoq2005:1, 2|});
            """,

            """
            var mock = new Mock<WithRequiredArguments>{|PosInfoMoq2005:()|};
            """,

            """
            var mock = new Mock<WithRequiredArguments>{|PosInfoMoq2005:(MockBehavior.Strict)|};
            """,

            """
            var mock = new Mock<{|PosInfoMoq2016:I|}>(() => new WithRequiredArguments(10));
            """,

            """
            var mock = new Mock<{|PosInfoMoq2016:I|}>(() => new WithRequiredArguments(10), MockBehavior.Strict);
            """,

            """
            var mock = new Mock<WithRequiredArguments>(MockBehavior.Strict, {|PosInfoMoq2005:1, 2, 3|});
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