//-----------------------------------------------------------------------
// <copyright file="SetupMustBeUsedOnlyForOverridableMembersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<SetupMustBeUsedOnlyForOverridableMembersAnalyzer>;

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
                            mock1.SetupSet(i => i.TestProperty = ""Foobard"");
                            mock1.SetupSet<string>(i => i.TestProperty = ""Foobard"");
                            mock1.Setup(i => i.InnerObject.VirtualMethod());
                            mock1.Setup(i => i.InnerObject.VirtualProperty);
                            mock1.SetupSet(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock1.SetupSet<string>(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock1.Setup(i => i.InnerObject.AbstractMethod());
                            mock1.Setup(i => i.InnerObject.AbstractProperty);
                            mock1.SetupSet(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock1.SetupSet<string>(i => i.InnerObject.AbstractProperty = ""Foobard"");

                            var mock2 = new Mock<StandardClass>();
                            mock2.Setup(i => i.VirtualMethod());
                            mock2.Setup(i => i.VirtualProperty);
                            mock2.SetupSet(i => i.VirtualProperty = ""Foobard"");
                            mock2.SetupSet<string>(i => i.VirtualProperty = ""Foobard"");
                            mock2.Setup(i => i.InnerObject.VirtualMethod());
                            mock2.Setup(i => i.InnerObject.VirtualProperty);
                            mock2.SetupSet(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock2.SetupSet<string>(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock2.Setup(i => i.InnerObject.AbstractMethod());
                            mock2.Setup(i => i.InnerObject.AbstractProperty);
                            mock2.SetupSet(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock2.SetupSet<string>(i => i.InnerObject.AbstractProperty = ""Foobard"");

                            var mock3 = new Mock<AbstractClass>();
                            mock3.Setup(i => i.VirtualMethod());
                            mock3.Setup(i => i.VirtualProperty);
                            mock3.SetupSet(i => i.VirtualProperty = ""Foobard"");
                            mock3.SetupSet<string>(i => i.VirtualProperty = ""Foobard"");
                            mock3.Setup(i => i.AbstractMethod());
                            mock3.Setup(i => i.AbstractProperty);
                            mock3.SetupSet(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock3.SetupSet<string>(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock3.Setup(i => i.InnerObject.VirtualMethod());
                            mock3.Setup(i => i.InnerObject.VirtualProperty);
                            mock3.SetupSet(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock3.SetupSet<string>(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock3.Setup(i => i.InnerObject.AbstractMethod());
                            mock3.Setup(i => i.InnerObject.AbstractProperty);
                            mock3.SetupSet(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock3.SetupSet<string>(i => i.InnerObject.AbstractProperty = ""Foobard"");

                            var mock4 = new Mock<InheritedFromAbstractClass>();
                            mock4.Setup(i => i.VirtualMethod());
                            mock4.Setup(i => i.VirtualProperty);
                            mock4.SetupSet(i => i.VirtualProperty = ""Foobard"");
                            mock4.SetupSet<string>(i => i.VirtualProperty = ""Foobard"");
                            mock4.Setup(i => i.AbstractMethod());
                            mock4.Setup(i => i.AbstractProperty);
                            mock4.SetupSet(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock4.SetupSet<string>(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock4.Setup(i => i.InnerObject.VirtualMethod());
                            mock4.Setup(i => i.InnerObject.VirtualProperty);
                            mock4.SetupSet(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock4.SetupSet<string>(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock4.Setup(i => i.InnerObject.AbstractMethod());
                            mock4.Setup(i => i.InnerObject.AbstractProperty);
                            mock4.SetupSet(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock4.SetupSet<string>(i => i.InnerObject.AbstractProperty = ""Foobard"");

                            var mock5 = new Mock<InheritedFromAbstractClassDontOverrideVirtual>();
                            mock5.Setup(i => i.VirtualMethod());
                            mock5.Setup(i => i.VirtualProperty);
                            mock5.SetupSet(i => i.VirtualProperty = ""Foobard"");
                            mock5.SetupSet<string>(i => i.VirtualProperty = ""Foobard"");
                            mock5.Setup(i => i.AbstractMethod());
                            mock5.Setup(i => i.AbstractProperty);
                            mock5.SetupSet(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock5.SetupSet<string>(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock5.Setup(i => i.InnerObject.VirtualMethod());
                            mock5.Setup(i => i.InnerObject.VirtualProperty);
                            mock5.SetupSet(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock5.SetupSet<string>(i => i.InnerObject.VirtualProperty = ""Foobard"");
                            mock5.Setup(i => i.InnerObject.AbstractMethod());
                            mock5.Setup(i => i.InnerObject.AbstractProperty);
                            mock5.SetupSet(i => i.InnerObject.AbstractProperty = ""Foobard"");
                            mock5.SetupSet<string>(i => i.InnerObject.AbstractProperty = ""Foobard"");
                        }
                    }

                    public interface I
                    {
                        int TestMethod();

                        string TestProperty { get; set; }

                        InnerObject InnerObject { get; }
                    }

                    public class StandardClass
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty { get => null; set { } }

                        public virtual InnerObject InnerObject { get => null; set { } }
                    }

                    public abstract class AbstractClass
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty { get => null; set { } }

                        public abstract void AbstractMethod();

                        public abstract string AbstractProperty { get; set; }

                        public abstract InnerObject InnerObject { get; }
                    }

                    public class InheritedFromAbstractClass : AbstractClass
                    {
                        public override void AbstractMethod() { }

                        public override string AbstractProperty { get => null; set { } }

                        public override void VirtualMethod() { }

                        public override string VirtualProperty { get => null; set { } }

                        public override InnerObject InnerObject => null;
                    }

                    public class InheritedFromAbstractClassDontOverrideVirtual : AbstractClass
                    {
                        public override void AbstractMethod() { }

                        public override string AbstractProperty { get => null; set { } }

                        public override InnerObject InnerObject => null;
                    }

                    public abstract class InnerObject
                    {
                        public virtual void VirtualMethod() { }

                        public virtual string VirtualProperty { get => null; set { } }

                        public abstract void AbstractMethod();

                        public abstract string AbstractProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
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
                            mock1.SetupSet(i => i.[|Property|] = ""1234"");
                            mock1.SetupSet<string>(i => i.[|Property|] = ""1234"");
                            mock1.Setup(i => i.[|InnerObject|].VirtualMethod());
                            mock1.Setup(i => i.[|InnerObject|].VirtualProperty);
                            mock1.SetupSet(i => i.[|InnerObject|].VirtualProperty = ""1234"");
                            mock1.SetupSet<string>(i => i.[|InnerObject|].VirtualProperty = ""1234"");
                            mock1.Setup(i => i.AbstractInnerObject.[|Method|]());
                            mock1.Setup(i => i.AbstractInnerObject.[|Property|]);
                            mock1.SetupSet(i => i.AbstractInnerObject.[|Property|] = ""1234"");
                            mock1.SetupSet<string>(i => i.AbstractInnerObject.[|Property|] = ""1234"");
                            mock1.Setup(i => i.[|SealedInnerObject|].VirtualMethod());
                            mock1.Setup(i => i.[|SealedInnerObject|].VirtualProperty);
                            mock1.SetupSet(i => i.[|SealedInnerObject|].VirtualProperty = ""1234"");
                            mock1.SetupSet<string>(i => i.[|SealedInnerObject|].VirtualProperty = ""1234"");

                            var mock2 = new Mock<StandardClass>();
                            mock2.Setup(i => StandardClass.[|StaticMethod|]());

                            var mock3 = new Mock<AbstractClass>();
                            mock3.Setup(i => i.[|Method|]());
                            mock3.Setup(i => i.[|Property|]);
                            mock3.SetupSet(i => i.[|Property|] = ""1234"");
                            mock3.SetupSet<string>(i => i.[|Property|] = ""1234"");

                            var mock4 = new Mock<I>();
                            mock4.Setup(i => i.[|ExtensionMethod|]());
                        }
                    }

                    public class StandardClass
                    {
                        public void Method() { }

                        public string Property { get => null; set { } }

                        public static void StaticMethod() { }

                        public InnerObject InnerObject => null;

                        public AbstractInnerObject AbstractInnerObject => null;

                        public SealedInnerObject SealedInnerObject => null;
                    }

                    public abstract class AbstractClass
                    {
                        public void Method() { }

                        public string Property { get => null; set { } }
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

                        public virtual string VirtualProperty { get => null; set { } }
                    }

                    public abstract class AbstractInnerObject
                    {
                        public void Method() { }

                        public string Property { get => null; set { } }

                        public abstract void VirtualMethod();

                        public abstract string VirtualProperty { get; set; }
                    }

                    public sealed class SealedInnerObject : AbstractInnerObject
                    {
                        public override void VirtualMethod() { }

                        public override string VirtualProperty { get => null; set { } }
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
                            mock1.Setup(i => i.Method());
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

                        public void Setup(System.Action<T> _) { }
                    }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}