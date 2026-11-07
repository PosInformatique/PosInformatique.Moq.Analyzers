//-----------------------------------------------------------------------
// <copyright file="AsMustBeUsedWithInterfaceAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<AsMustBeUsedWithInterfaceAnalyzer>;

    public class AsMustBeUsedWithInterfaceAnalyzerTest
    {
        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock1 = new Mock<C>();
            mock1.As<I>();
            """,

            // No As<T>() method call.
            """
            var mock1 = new Mock<C>();
            mock1.Setup(m => m.Method());
            """,

            // Ignored by ExtractAsMethodType() because it is not a MemberAccessExpressionSyntax.
            """
            var action = new System.Action(() => { });
            action();
            """,

            // Ignored by ExtractAsMethodType() because it is not an As<T>() symbol.
            """
            var c = new C();
            c.GenericMethodNotAs<I>();
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock1 = new Mock<C>();
            mock1.As<{|PosInfoMoq2007:C|}>();
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

                    public class C
                    {
                        public virtual void Method() { }

                        public void GenericMethodNotAs<T>() { }
                    }

                    public interface I
                    {
                    }
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

                    public class C
                    {
                        public virtual void Method() { }
                
                        public void GenericMethodNotAs<T>() { }
                    }
                
                    public interface I
                    {
                    }
                }

                {{MoqAnalyzersTests.FakeMoqLibrary}}
                """;

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}