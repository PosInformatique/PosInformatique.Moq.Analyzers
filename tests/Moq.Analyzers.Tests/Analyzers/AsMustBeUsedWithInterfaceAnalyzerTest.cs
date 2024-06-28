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
        [Fact]
        public async Task AsMethod_WithInterface()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>();
                            mock1.As<I>();
                        }
                    }

                    public class C
                    {
                    }

                    public interface I
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task AsMethod_WithClass()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>();
                            mock1.As<[|C2|]>();
                        }
                    }

                    public class C
                    {
                    }

                    public class C2
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoAsMethod()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;

                    public class TestClass
                    {

                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>();
                            mock1.Setup(m => m.Method());

                            var action = new System.Action(() => { });
                            action();   // Ignored by the ExtractAsMethodType() method because not a MemberAccessExpressionSyntax.

                            var otherClass = new OtherClass();
                            otherClass.GenericMethodNotAs<C>(); // Ignored by the ExtractAsMethodType() method because it not As<T>() symbol.
                        }
                    }

                    public class C
                    {
                        public virtual void Method() { }
                    }

                    public class OtherClass
                    {
                        public void GenericMethodNotAs<T>() { }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoMoqLibrary()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using OtherNamespace;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.As<I>();
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
                        public void As<TInterface>() { }
                    }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}