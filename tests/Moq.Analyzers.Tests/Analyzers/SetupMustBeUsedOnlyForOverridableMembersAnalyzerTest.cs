//-----------------------------------------------------------------------
// <copyright file="SetupMustBeUsedOnlyForOverridableMembersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        SetupMustBeUsedOnlyForOverridableMembersAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

    public class SetupMustBeUsedOnlyForOverridableMembersAnalyzerTest
    {
        [Fact]
        public async Task Overridable_NoDiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Setup(i => i.TestMethod());
                            mock1.Setup(i => i.TestProperty);

                            var mock2 = new Mock<StandardClass>();
                            mock2.Setup(i => i.VirtualMethod());
                            mock2.Setup(i => i.VirtualProperty);

                            var mock3 = new Mock<AbstractClass>();
                            mock3.Setup(i => i.VirtualMethod());
                            mock3.Setup(i => i.VirtualProperty);
                            mock3.Setup(i => i.AbstractMethod());
                            mock3.Setup(i => i.AbstractProperty);

                            var mock4 = new Mock<InheritedFromAbstractClass>();
                            mock4.Setup(i => i.VirtualMethod());
                            mock4.Setup(i => i.VirtualProperty);
                            mock4.Setup(i => i.AbstractMethod());
                            mock4.Setup(i => i.AbstractProperty);

                            var mock5 = new Mock<InheritedFromAbstractClassDontOverrideVirtual>();
                            mock5.Setup(i => i.VirtualMethod());
                            mock5.Setup(i => i.VirtualProperty);
                            mock5.Setup(i => i.AbstractMethod());
                            mock5.Setup(i => i.AbstractProperty);
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        string TestProperty { get; }
                    }

                    public class StandardClass
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty => null;
                    }

                    public abstract class AbstractClass
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty => null;

                        public abstract void AbstractMethod();

                        public abstract string AbstractProperty { get; }
                    }

                    public class InheritedFromAbstractClass : AbstractClass
                    {
                        public override void AbstractMethod() { }

                        public override string AbstractProperty => null;

                        public override void VirtualMethod() { }

                        public override string VirtualProperty => null;
                    }

                    public class InheritedFromAbstractClassDontOverrideVirtual : AbstractClass
                    {
                        public override void AbstractMethod() { }

                        public override string AbstractProperty => null;
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoOverridable_DiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<StandardClass>();
                            mock1.Setup(i => i.[|Method|]());
                            mock1.Setup(i => i.[|Property|]);

                            var mock2 = new Mock<StandardClass>();
                            mock2.Setup(i => StandardClass.[|StaticMethod|]());

                            var mock3 = new Mock<AbstractClass>();
                            mock3.Setup(i => i.[|Method|]());
                            mock3.Setup(i => i.[|Property|]);

                            var mock4 = new Mock<I>();
                            mock4.Setup(i => i.[|ExtensionMethod|]());
                        }
                    }

                    public class StandardClass
                    {
                        public void Method() { }

                        public string Property => null;

                        public static void StaticMethod() { }
                    }

                    public abstract class AbstractClass
                    {
                        public void Method() { }

                        public string Property => null;
                    }

                    public interface I
                    {
                        int TestMethod();
                    }

                    public static class ExtensionsClass
                    {
                        public static void ExtensionMethod(this I _) { }
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                            var mock1 = new Mock<I>(MockBehavior.Strict);
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
                        public Mock(MockBehavior _) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}