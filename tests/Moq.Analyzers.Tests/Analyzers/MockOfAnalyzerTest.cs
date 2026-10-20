//-----------------------------------------------------------------------
// <copyright file="MockOfAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<MockOfAnalyzer>;

    public class MockOfAnalyzerTest
    {
        private const string TestTypes =
            """
            public class StandardClass
            {
                public int Property { get; }
            }

            public class StandardClassWithMultipleConstructors
            {
                public StandardClassWithMultipleConstructors() { }

                public StandardClassWithMultipleConstructors(int a) { }

                public StandardClassWithMultipleConstructors(string b) { }

                public int Property { get; }
            }

            public interface I
            {
                int Property { get; }
            }

            public abstract class AbstractClass
            {
                public int Property { get; }
            }

            public abstract class AbstractClassWithMultipleConstructors
            {
                public AbstractClassWithMultipleConstructors() { }

                public AbstractClassWithMultipleConstructors(int a) { }

                public AbstractClassWithMultipleConstructors(string b) { }

                public int Property { get; }
            }

            public sealed class SealedClass
            {
                public int Property { get; }
            }

            public class NoParameterlessConstructorClass
            {
                public NoParameterlessConstructorClass(int a) { }
            }

            public abstract class AbstractNoParameterlessConstructorClass
            {
                public AbstractNoParameterlessConstructorClass(int a) { }
            }

            public class PrivateParameterlessConstructorClass
            {
                private PrivateParameterlessConstructorClass() { }
            }

            public abstract class AbstractPrivateParameterlessConstructorClass
            {
                private AbstractPrivateParameterlessConstructorClass() { }
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock = Mock.Of<StandardClass>();
            """,

            """
            var mock = Mock.Of<StandardClassWithMultipleConstructors>();
            """,

            """
            var mock = Mock.Of<StandardClass>(sc => sc.Property == 1234);
            """,

            """
            var mock = Mock.Of<StandardClassWithMultipleConstructors>(sc => sc.Property == 1234);
            """,

            """
            var mock = Mock.Of<I>();
            """,

            """
            var mock = Mock.Of<I>(sc => sc.Property == 1234);
            """,

            """
            var mock = Mock.Of<AbstractClass>();
            """,

            """
            var mock = Mock.Of<AbstractClass>(MockBehavior.Strict);
            """,

            """
            var mock = Mock.Of<AbstractClassWithMultipleConstructors>();
            """,

            """
            var mock = Mock.Of<AbstractClassWithMultipleConstructors>(MockBehavior.Strict);
            """,

            """
            var mock = Mock.Of<AbstractClass>(sc => sc.Property == 1234);
            """,

            """
            var mock = Mock.Of<AbstractClass>(sc => sc.Property == 1234, MockBehavior.Strict);
            """,

            """
            var mock = Mock.Of<AbstractClassWithMultipleConstructors>(sc => sc.Property == 1234);
            """,

            """
            var mock = Mock.Of<AbstractClassWithMultipleConstructors>(sc => sc.Property == 1234, MockBehavior.Strict);
            """,

            // Ignored by the analysis because it is not a Mock.Of() call.
            """
            string.Format("Ignored");
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock = Mock.Of<{|PosInfoMoq2009:SealedClass|}>();
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2009:SealedClass|}>(sc => sc.Property == 1234);
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2009:string|}>();
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2009:string|}>(MockBehavior.Strict);
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2009:string|}>(sc => sc.Length == 1234);
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2009:string|}>(sc => sc.Length == 1234, MockBehavior.Strict);
            """,

            """
            var mock = Mock.Of(({|PosInfoMoq2009:string|} sc) => sc.Length == 1234);
            """,

            """
            var mock = Mock.Of(({|PosInfoMoq2009:string|} sc) => sc.Length == 1234, MockBehavior.Strict);
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2010:NoParameterlessConstructorClass|}>();
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2010:AbstractNoParameterlessConstructorClass|}>();
            """,

            """
            var mock = Mock.Of(({|PosInfoMoq2010:NoParameterlessConstructorClass|} m) => true);
            """,

            """
            var mock = Mock.Of(({|PosInfoMoq2010:AbstractNoParameterlessConstructorClass|} m) => true);
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2010:PrivateParameterlessConstructorClass|}>();
            """,

            """
            var mock = Mock.Of<{|PosInfoMoq2010:AbstractPrivateParameterlessConstructorClass|}>();
            """,

            """
            var mock = Mock.Of(({|PosInfoMoq2010:PrivateParameterlessConstructorClass|} m) => true);
            """,

            """
            var mock = Mock.Of(({|PosInfoMoq2010:AbstractPrivateParameterlessConstructorClass|} m) => true);
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