//-----------------------------------------------------------------------
// <copyright file="VerifyMustBeUsedOnlyForOverridableMembersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyMustBeUsedOnlyForOverridableMembersAnalyzer>;

    public class VerifyMustBeUsedOnlyForOverridableMembersAnalyzerTest
    {
        [Theory]
        [InlineData("")]
        [InlineData(", \"The message\"")]
        public async Task Overridable_NoDiagnosticReported(string secondArgument)
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
                            mock1.Verify(i => i.TestMethod()" + secondArgument + @");
                            mock1.Verify(i => i.TestProperty" + secondArgument + @");
                            mock1.Verify(i => i.InnerObject.VirtualMethod()" + secondArgument + @");
                            mock1.Verify(i => i.InnerObject.VirtualProperty" + secondArgument + @");
                            mock1.Verify(i => i.InnerObject.AbstractMethod()" + secondArgument + @");
                            mock1.Verify(i => i.InnerObject.AbstractProperty" + secondArgument + @");

                            var mock2 = new Mock<StandardClass>();
                            mock2.Verify(i => i.VirtualMethod()" + secondArgument + @");
                            mock2.Verify(i => i.VirtualProperty" + secondArgument + @");
                            mock2.Verify(i => i.InnerObject.VirtualMethod()" + secondArgument + @");
                            mock2.Verify(i => i.InnerObject.VirtualProperty" + secondArgument + @");
                            mock2.Verify(i => i.InnerObject.AbstractMethod()" + secondArgument + @");
                            mock2.Verify(i => i.InnerObject.AbstractProperty" + secondArgument + @");

                            var mock3 = new Mock<AbstractClass>();
                            mock3.Verify(i => i.VirtualMethod()" + secondArgument + @");
                            mock3.Verify(i => i.VirtualProperty" + secondArgument + @");
                            mock3.Verify(i => i.AbstractMethod()" + secondArgument + @");
                            mock3.Verify(i => i.AbstractProperty" + secondArgument + @");
                            mock3.Verify(i => i.InnerObject.VirtualMethod()" + secondArgument + @");
                            mock3.Verify(i => i.InnerObject.VirtualProperty" + secondArgument + @");
                            mock3.Verify(i => i.InnerObject.AbstractMethod()" + secondArgument + @");
                            mock3.Verify(i => i.InnerObject.AbstractProperty" + secondArgument + @");

                            var mock4 = new Mock<InheritedFromAbstractClass>();
                            mock4.Verify(i => i.VirtualMethod()" + secondArgument + @");
                            mock4.Verify(i => i.VirtualProperty" + secondArgument + @");
                            mock4.Verify(i => i.AbstractMethod()" + secondArgument + @");
                            mock4.Verify(i => i.AbstractProperty" + secondArgument + @");
                            mock4.Verify(i => i.InnerObject.VirtualMethod()" + secondArgument + @");
                            mock4.Verify(i => i.InnerObject.VirtualProperty" + secondArgument + @");
                            mock4.Verify(i => i.InnerObject.AbstractMethod()" + secondArgument + @");
                            mock4.Verify(i => i.InnerObject.AbstractProperty" + secondArgument + @");

                            var mock5 = new Mock<InheritedFromAbstractClassDontOverrideVirtual>();
                            mock5.Verify(i => i.VirtualMethod()" + secondArgument + @");
                            mock5.Verify(i => i.VirtualProperty" + secondArgument + @");
                            mock5.Verify(i => i.AbstractMethod()" + secondArgument + @");
                            mock5.Verify(i => i.AbstractProperty" + secondArgument + @");
                            mock5.Verify(i => i.InnerObject.VirtualMethod()" + secondArgument + @");
                            mock5.Verify(i => i.InnerObject.VirtualProperty" + secondArgument + @");
                            mock5.Verify(i => i.InnerObject.AbstractMethod()" + secondArgument + @");
                            mock5.Verify(i => i.InnerObject.AbstractProperty" + secondArgument + @");
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        string TestProperty { get; }

                        InnerObject InnerObject { get; }
                    }

                    public class StandardClass
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty => null;

                        public virtual InnerObject InnerObject { get => null; }
                    }

                    public abstract class AbstractClass
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty => null;

                        public abstract void AbstractMethod();

                        public abstract string AbstractProperty { get; }

                        public abstract InnerObject InnerObject { get; }
                    }

                    public class InheritedFromAbstractClass : AbstractClass
                    {
                        public override void AbstractMethod() { }

                        public override string AbstractProperty => null;

                        public override void VirtualMethod() { }

                        public override string VirtualProperty => null;

                        public override InnerObject InnerObject => null;
                    }

                    public class InheritedFromAbstractClassDontOverrideVirtual : AbstractClass
                    {
                        public override void AbstractMethod() { }

                        public override string AbstractProperty => null;

                        public override InnerObject InnerObject => null;
                    }

                    public abstract class InnerObject
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty => null;

                        public abstract void AbstractMethod();

                        public abstract string AbstractProperty { get; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(", \"The message\"")]
        public async Task NoOverridable_DiagnosticReported(string secondArgument)
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
                            mock1.Verify(i => i.[|Method|]()" + secondArgument + @");
                            mock1.Verify(i => i.[|Property|]" + secondArgument + @");
                            mock1.Verify(i => i.[|InnerObject|].VirtualMethod()" + secondArgument + @");
                            mock1.Verify(i => i.[|InnerObject|].VirtualProperty" + secondArgument + @");
                            mock1.Verify(i => i.AbstractInnerObject.[|Method|]()" + secondArgument + @");
                            mock1.Verify(i => i.AbstractInnerObject.[|Property|]" + secondArgument + @");
                            mock1.Verify(i => i.[|SealedInnerObject|].VirtualMethod()" + secondArgument + @");
                            mock1.Verify(i => i.[|SealedInnerObject|].VirtualProperty" + secondArgument + @");

                            var mock2 = new Mock<StandardClass>();
                            mock2.Verify(i => StandardClass.[|StaticMethod|]()" + secondArgument + @");

                            var mock3 = new Mock<AbstractClass>();
                            mock3.Verify(i => i.[|Method|]()" + secondArgument + @");
                            mock3.Verify(i => i.[|Property|]" + secondArgument + @");

                            var mock4 = new Mock<I>();
                            mock4.Verify(i => i.[|ExtensionMethod|]()" + secondArgument + @");
                        }
                    }

                    public class StandardClass
                    {
                        public void Method() { }

                        public string Property => null;

                        public static void StaticMethod() { }

                        public InnerObject InnerObject => null;

                        public AbstractInnerObject AbstractInnerObject => null;

                        public SealedInnerObject SealedInnerObject => null;
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

                    public class InnerObject
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty => null;
                    }

                    public abstract class AbstractInnerObject
                    {
                        public void Method() { }

                        public string Property { get => null; }

                        public abstract void VirtualMethod();

                        public abstract string VirtualProperty { get; }
                    }

                    public sealed class SealedInnerObject : AbstractInnerObject
                    {
                        public override void VirtualMethod() { }

                        public override string VirtualProperty => null;
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
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            mock1.Verify(i => i.Method());
                        }
                    }

                    public interface I
                    {
                        void Method();
                    }
                }

                namespace OtherNamespace
                {
                    public class Mock<T>
                    {
                        public Mock(MockBehavior _) { }

                        public void Verify(System.Action<T> _) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}