//-----------------------------------------------------------------------
// <copyright file="VerifyMustBeUsedOnlyForOverridableMembersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<VerifyMustBeUsedOnlyForOverridableMembersAnalyzer>;

    public class VerifyMustBeUsedOnlyForOverridableMembersAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                int TestMethod();

                void Method();

                string TestProperty { get; }

                InnerObject InnerObject { get; }
            }

            public class StandardClass
            {
                public virtual void VirtualMethod() { }

                public virtual string VirtualProperty => null;

                public virtual InnerObject InnerObject => null;

                public void Method() { }

                public string Property => null;

                public static void StaticMethod() { }

                public InnerObject NonOverridableInnerObject => null;

                public AbstractInnerObject AbstractInnerObject => null;

                public SealedInnerObject SealedInnerObject => null;
            }

            public abstract class AbstractClass
            {
                public virtual void VirtualMethod() { }

                public virtual string VirtualProperty => null;

                public abstract void AbstractMethod();

                public abstract string AbstractProperty { get; }

                public abstract InnerObject InnerObject { get; }

                public void Method() { }

                public string Property => null;
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

            public abstract class AbstractInnerObject
            {
                public void Method() { }

                public string Property => null;

                public abstract void VirtualMethod();

                public abstract string VirtualProperty { get; }
            }

            public sealed class SealedInnerObject : AbstractInnerObject
            {
                public override void VirtualMethod() { }

                public override string VirtualProperty => null;
            }

            public static class ExtensionsClass
            {
                public static void ExtensionMethod(this I _) { }
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """new Mock<I>().Verify(i => i.TestMethod());""",

            """new Mock<I>().Verify(i => i.TestMethod(), "The message");""",

            """new Mock<I>().Verify(i => i.TestProperty);""",

            """new Mock<I>().Verify(i => i.TestProperty, "The message");""",

            """new Mock<I>().Verify(i => i.InnerObject.VirtualMethod());""",

            """new Mock<I>().Verify(i => i.InnerObject.VirtualMethod(), "The message");""",

            """new Mock<I>().Verify(i => i.InnerObject.VirtualProperty);""",

            """new Mock<I>().Verify(i => i.InnerObject.VirtualProperty, "The message");""",

            """new Mock<I>().Verify(i => i.InnerObject.AbstractMethod());""",

            """new Mock<I>().Verify(i => i.InnerObject.AbstractMethod(), "The message");""",

            """new Mock<I>().Verify(i => i.InnerObject.AbstractProperty);""",

            """new Mock<I>().Verify(i => i.InnerObject.AbstractProperty, "The message");""",

            """new Mock<StandardClass>().Verify(i => i.VirtualMethod());""",

            """new Mock<StandardClass>().Verify(i => i.VirtualMethod(), "The message");""",

            """new Mock<StandardClass>().Verify(i => i.VirtualProperty);""",

            """new Mock<StandardClass>().Verify(i => i.VirtualProperty, "The message");""",

            """new Mock<StandardClass>().Verify(i => i.InnerObject.VirtualMethod());""",

            """new Mock<StandardClass>().Verify(i => i.InnerObject.VirtualMethod(), "The message");""",

            """new Mock<StandardClass>().Verify(i => i.InnerObject.VirtualProperty);""",

            """new Mock<StandardClass>().Verify(i => i.InnerObject.VirtualProperty, "The message");""",

            """new Mock<StandardClass>().Verify(i => i.InnerObject.AbstractMethod());""",

            """new Mock<StandardClass>().Verify(i => i.InnerObject.AbstractMethod(), "The message");""",

            """new Mock<StandardClass>().Verify(i => i.InnerObject.AbstractProperty);""",

            """new Mock<StandardClass>().Verify(i => i.InnerObject.AbstractProperty, "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.VirtualMethod());""",

            """new Mock<AbstractClass>().Verify(i => i.VirtualMethod(), "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.VirtualProperty);""",

            """new Mock<AbstractClass>().Verify(i => i.VirtualProperty, "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.AbstractMethod());""",

            """new Mock<AbstractClass>().Verify(i => i.AbstractMethod(), "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.AbstractProperty);""",

            """new Mock<AbstractClass>().Verify(i => i.AbstractProperty, "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.InnerObject.VirtualMethod());""",

            """new Mock<AbstractClass>().Verify(i => i.InnerObject.VirtualMethod(), "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.InnerObject.VirtualProperty);""",

            """new Mock<AbstractClass>().Verify(i => i.InnerObject.VirtualProperty, "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.InnerObject.AbstractMethod());""",

            """new Mock<AbstractClass>().Verify(i => i.InnerObject.AbstractMethod(), "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.InnerObject.AbstractProperty);""",

            """new Mock<AbstractClass>().Verify(i => i.InnerObject.AbstractProperty, "The message");""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.VirtualMethod());""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.VirtualMethod(), "The message");""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.VirtualProperty);""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.VirtualProperty, "The message");""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.AbstractMethod());""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.AbstractMethod(), "The message");""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.AbstractProperty);""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.AbstractProperty, "The message");""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.InnerObject.VirtualMethod());""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.InnerObject.VirtualMethod(), "The message");""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.InnerObject.VirtualProperty);""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.InnerObject.VirtualProperty, "The message");""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.InnerObject.AbstractMethod());""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.InnerObject.AbstractMethod(), "The message");""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.InnerObject.AbstractProperty);""",

            """new Mock<InheritedFromAbstractClass>().Verify(i => i.InnerObject.AbstractProperty, "The message");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.VirtualMethod());""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.VirtualMethod(), "The message");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.VirtualProperty);""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.VirtualProperty, "The message");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.AbstractMethod());""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.AbstractMethod(), "The message");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.AbstractProperty);""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.AbstractProperty, "The message");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.InnerObject.VirtualMethod());""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.InnerObject.VirtualMethod(), "The message");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.InnerObject.VirtualProperty);""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.InnerObject.VirtualProperty, "The message");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.InnerObject.AbstractMethod());""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.InnerObject.AbstractMethod(), "The message");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.InnerObject.AbstractProperty);""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Verify(i => i.InnerObject.AbstractProperty, "The message");""",

            """new Mock<I>(MockBehavior.Strict).Verify(i => i.Method());""",
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:Method|}());""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:Method|}(), "The message");""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:Property|});""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:Property|}, "The message");""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:NonOverridableInnerObject|}.VirtualMethod());""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:NonOverridableInnerObject|}.VirtualMethod(), "The message");""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:NonOverridableInnerObject|}.VirtualProperty);""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:NonOverridableInnerObject|}.VirtualProperty, "The message");""",

            """new Mock<StandardClass>().Verify(i => i.AbstractInnerObject.{|PosInfoMoq2008:Method|}());""",

            """new Mock<StandardClass>().Verify(i => i.AbstractInnerObject.{|PosInfoMoq2008:Method|}(), "The message");""",

            """new Mock<StandardClass>().Verify(i => i.AbstractInnerObject.{|PosInfoMoq2008:Property|});""",

            """new Mock<StandardClass>().Verify(i => i.AbstractInnerObject.{|PosInfoMoq2008:Property|}, "The message");""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:SealedInnerObject|}.VirtualMethod());""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:SealedInnerObject|}.VirtualMethod(), "The message");""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:SealedInnerObject|}.VirtualProperty);""",

            """new Mock<StandardClass>().Verify(i => i.{|PosInfoMoq2008:SealedInnerObject|}.VirtualProperty, "The message");""",

            """new Mock<StandardClass>().Verify(i => StandardClass.{|PosInfoMoq2008:StaticMethod|}());""",

            """new Mock<StandardClass>().Verify(i => StandardClass.{|PosInfoMoq2008:StaticMethod|}(), "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.{|PosInfoMoq2008:Method|}());""",

            """new Mock<AbstractClass>().Verify(i => i.{|PosInfoMoq2008:Method|}(), "The message");""",

            """new Mock<AbstractClass>().Verify(i => i.{|PosInfoMoq2008:Property|});""",

            """new Mock<AbstractClass>().Verify(i => i.{|PosInfoMoq2008:Property|}, "The message");""",

            """new Mock<I>().Verify(i => i.{|PosInfoMoq2008:ExtensionMethod|}());""",

            """new Mock<I>().Verify(i => i.{|PosInfoMoq2008:ExtensionMethod|}(), "The message");""",
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