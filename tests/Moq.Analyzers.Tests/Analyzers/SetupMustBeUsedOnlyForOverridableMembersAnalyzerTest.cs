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
        private const string TestTypes =
            """
            public interface I
            {
                int TestMethod();

                void Method();

                string TestProperty { get; set; }

                InnerObject InnerObject { get; }
            }

            public class StandardClass
            {
                public virtual void VirtualMethod() { }

                public virtual string VirtualProperty { get => null; set { } }

                public virtual InnerObject InnerObject { get => null; set { } }

                public void Method() { }

                public string Property { get => null; set { } }

                public static void StaticMethod() { }

                public InnerObject NonOverridableInnerObject => null;

                public AbstractInnerObject AbstractInnerObject => null;

                public SealedInnerObject SealedInnerObject => null;
            }

            public abstract class AbstractClass
            {
                public virtual void VirtualMethod() { }

                public virtual string VirtualProperty { get => null; set { } }

                public abstract void AbstractMethod();

                public abstract string AbstractProperty { get; set; }

                public abstract InnerObject InnerObject { get; }

                public void Method() { }

                public string Property { get => null; set { } }
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

            public static class ExtensionsClass
            {
                public static void ExtensionMethod(this I _) { }
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """new Mock<I>().Setup(i => i.TestMethod());""",
            """new Mock<I>().Setup(i => i.TestProperty);""",
            """new Mock<I>().SetupSequence(i => i.TestMethod());""",
            """new Mock<I>().SetupSequence(i => i.TestProperty);""",
            """new Mock<I>().SetupSet(i => i.TestProperty = "Foobard");""",
            """new Mock<I>().SetupSet<string>(i => i.TestProperty = "Foobard");""",
            """new Mock<I>().Setup(i => i.InnerObject.VirtualMethod());""",
            """new Mock<I>().Setup(i => i.InnerObject.VirtualProperty);""",
            """new Mock<I>().SetupSequence(i => i.InnerObject.VirtualMethod());""",
            """new Mock<I>().SetupSequence(i => i.InnerObject.VirtualProperty);""",
            """new Mock<I>().SetupSet(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<I>().SetupSet<string>(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<I>().Setup(i => i.InnerObject.AbstractMethod());""",
            """new Mock<I>().Setup(i => i.InnerObject.AbstractProperty);""",
            """new Mock<I>().SetupSequence(i => i.InnerObject.AbstractMethod());""",
            """new Mock<I>().SetupSequence(i => i.InnerObject.AbstractProperty);""",
            """new Mock<I>().SetupSet(i => i.InnerObject.AbstractProperty = "Foobard");""",
            """new Mock<I>().SetupSet<string>(i => i.InnerObject.AbstractProperty = "Foobard");""",

            """new Mock<StandardClass>().Setup(i => i.VirtualMethod());""",
            """new Mock<StandardClass>().Setup(i => i.VirtualProperty);""",
            """new Mock<StandardClass>().SetupSequence(i => i.VirtualMethod());""",
            """new Mock<StandardClass>().SetupSequence(i => i.VirtualProperty);""",
            """new Mock<StandardClass>().SetupSet(i => i.VirtualProperty = "Foobard");""",
            """new Mock<StandardClass>().SetupSet<string>(i => i.VirtualProperty = "Foobard");""",
            """new Mock<StandardClass>().Setup(i => i.InnerObject.VirtualMethod());""",
            """new Mock<StandardClass>().Setup(i => i.InnerObject.VirtualProperty);""",
            """new Mock<StandardClass>().SetupSequence(i => i.InnerObject.VirtualMethod());""",
            """new Mock<StandardClass>().SetupSequence(i => i.InnerObject.VirtualProperty);""",
            """new Mock<StandardClass>().SetupSet(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<StandardClass>().SetupSet<string>(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<StandardClass>().Setup(i => i.InnerObject.AbstractMethod());""",
            """new Mock<StandardClass>().Setup(i => i.InnerObject.AbstractProperty);""",
            """new Mock<StandardClass>().SetupSequence(i => i.InnerObject.AbstractMethod());""",
            """new Mock<StandardClass>().SetupSequence(i => i.InnerObject.AbstractProperty);""",
            """new Mock<StandardClass>().SetupSet(i => i.InnerObject.AbstractProperty = "Foobard");""",
            """new Mock<StandardClass>().SetupSet<string>(i => i.InnerObject.AbstractProperty = "Foobard");""",

            """new Mock<AbstractClass>().Setup(i => i.VirtualMethod());""",
            """new Mock<AbstractClass>().Setup(i => i.VirtualProperty);""",
            """new Mock<AbstractClass>().SetupSequence(i => i.VirtualMethod());""",
            """new Mock<AbstractClass>().SetupSequence(i => i.VirtualProperty);""",
            """new Mock<AbstractClass>().SetupSet(i => i.VirtualProperty = "Foobard");""",
            """new Mock<AbstractClass>().SetupSet<string>(i => i.VirtualProperty = "Foobard");""",
            """new Mock<AbstractClass>().Setup(i => i.AbstractMethod());""",
            """new Mock<AbstractClass>().Setup(i => i.AbstractProperty);""",
            """new Mock<AbstractClass>().SetupSequence(i => i.AbstractMethod());""",
            """new Mock<AbstractClass>().SetupSequence(i => i.AbstractProperty);""",
            """new Mock<AbstractClass>().Setup(i => i.InnerObject.VirtualMethod());""",
            """new Mock<AbstractClass>().Setup(i => i.InnerObject.VirtualProperty);""",
            """new Mock<AbstractClass>().SetupSequence(i => i.InnerObject.VirtualMethod());""",
            """new Mock<AbstractClass>().SetupSequence(i => i.InnerObject.VirtualProperty);""",
            """new Mock<AbstractClass>().SetupSet(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<AbstractClass>().SetupSet<string>(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<AbstractClass>().Setup(i => i.InnerObject.AbstractMethod());""",
            """new Mock<AbstractClass>().Setup(i => i.InnerObject.AbstractProperty);""",
            """new Mock<AbstractClass>().SetupSequence(i => i.InnerObject.AbstractMethod());""",
            """new Mock<AbstractClass>().SetupSequence(i => i.InnerObject.AbstractProperty);""",
            """new Mock<AbstractClass>().SetupSet(i => i.InnerObject.AbstractProperty = "Foobard");""",
            """new Mock<AbstractClass>().SetupSet<string>(i => i.InnerObject.AbstractProperty = "Foobard");""",

            """new Mock<InheritedFromAbstractClass>().Setup(i => i.VirtualMethod());""",
            """new Mock<InheritedFromAbstractClass>().Setup(i => i.VirtualProperty);""",
            """new Mock<InheritedFromAbstractClass>().SetupSequence(i => i.VirtualMethod());""",
            """new Mock<InheritedFromAbstractClass>().SetupSequence(i => i.VirtualProperty);""",
            """new Mock<InheritedFromAbstractClass>().SetupSet(i => i.VirtualProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClass>().SetupSet<string>(i => i.VirtualProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClass>().Setup(i => i.AbstractMethod());""",
            """new Mock<InheritedFromAbstractClass>().Setup(i => i.AbstractProperty);""",
            """new Mock<InheritedFromAbstractClass>().SetupSequence(i => i.AbstractMethod());""",
            """new Mock<InheritedFromAbstractClass>().SetupSequence(i => i.AbstractProperty);""",
            """new Mock<InheritedFromAbstractClass>().Setup(i => i.InnerObject.VirtualMethod());""",
            """new Mock<InheritedFromAbstractClass>().Setup(i => i.InnerObject.VirtualProperty);""",
            """new Mock<InheritedFromAbstractClass>().SetupSequence(i => i.InnerObject.VirtualMethod());""",
            """new Mock<InheritedFromAbstractClass>().SetupSequence(i => i.InnerObject.VirtualProperty);""",
            """new Mock<InheritedFromAbstractClass>().SetupSet(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClass>().SetupSet<string>(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClass>().Setup(i => i.InnerObject.AbstractMethod());""",
            """new Mock<InheritedFromAbstractClass>().Setup(i => i.InnerObject.AbstractProperty);""",
            """new Mock<InheritedFromAbstractClass>().SetupSequence(i => i.InnerObject.AbstractMethod());""",
            """new Mock<InheritedFromAbstractClass>().SetupSequence(i => i.InnerObject.AbstractProperty);""",
            """new Mock<InheritedFromAbstractClass>().SetupSet(i => i.InnerObject.AbstractProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClass>().SetupSet<string>(i => i.InnerObject.AbstractProperty = "Foobard");""",

            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Setup(i => i.VirtualMethod());""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Setup(i => i.VirtualProperty);""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSequence(i => i.VirtualMethod());""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSequence(i => i.VirtualProperty);""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSet(i => i.VirtualProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSet<string>(i => i.VirtualProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Setup(i => i.AbstractMethod());""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Setup(i => i.AbstractProperty);""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSequence(i => i.AbstractMethod());""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSequence(i => i.AbstractProperty);""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Setup(i => i.InnerObject.VirtualMethod());""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Setup(i => i.InnerObject.VirtualProperty);""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSequence(i => i.InnerObject.VirtualMethod());""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSequence(i => i.InnerObject.VirtualProperty);""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSet(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSet<string>(i => i.InnerObject.VirtualProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Setup(i => i.InnerObject.AbstractMethod());""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().Setup(i => i.InnerObject.AbstractProperty);""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSequence(i => i.InnerObject.AbstractMethod());""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSequence(i => i.InnerObject.AbstractProperty);""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSet(i => i.InnerObject.AbstractProperty = "Foobard");""",
            """new Mock<InheritedFromAbstractClassDontOverrideVirtual>().SetupSet<string>(i => i.InnerObject.AbstractProperty = "Foobard");""",

            """new Mock<I>(MockBehavior.Strict).Setup(i => i.Method());""",
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """new Mock<StandardClass>().Setup(i => i.{|PosInfoMoq2001:Method|}());""",
            """new Mock<StandardClass>().Setup(i => i.{|PosInfoMoq2001:Property|});""",
            """new Mock<StandardClass>().SetupSequence(i => i.{|PosInfoMoq2001:Method|}());""",
            """new Mock<StandardClass>().SetupSequence(i => i.{|PosInfoMoq2001:Property|});""",
            """new Mock<StandardClass>().SetupSet(i => i.{|PosInfoMoq2001:Property|} = "1234");""",
            """new Mock<StandardClass>().SetupSet<string>(i => i.{|PosInfoMoq2001:Property|} = "1234");""",
            """new Mock<StandardClass>().Setup(i => i.{|PosInfoMoq2001:NonOverridableInnerObject|}.VirtualMethod());""",
            """new Mock<StandardClass>().Setup(i => i.{|PosInfoMoq2001:NonOverridableInnerObject|}.VirtualProperty);""",
            """new Mock<StandardClass>().SetupSequence(i => i.{|PosInfoMoq2001:NonOverridableInnerObject|}.VirtualMethod());""",
            """new Mock<StandardClass>().SetupSequence(i => i.{|PosInfoMoq2001:NonOverridableInnerObject|}.VirtualProperty);""",
            """new Mock<StandardClass>().SetupSet(i => i.{|PosInfoMoq2001:NonOverridableInnerObject|}.VirtualProperty = "1234");""",
            """new Mock<StandardClass>().SetupSet<string>(i => i.{|PosInfoMoq2001:NonOverridableInnerObject|}.VirtualProperty = "1234");""",
            """new Mock<StandardClass>().Setup(i => i.AbstractInnerObject.{|PosInfoMoq2001:Method|}());""",
            """new Mock<StandardClass>().Setup(i => i.AbstractInnerObject.{|PosInfoMoq2001:Property|});""",
            """new Mock<StandardClass>().SetupSequence(i => i.AbstractInnerObject.{|PosInfoMoq2001:Method|}());""",
            """new Mock<StandardClass>().SetupSequence(i => i.AbstractInnerObject.{|PosInfoMoq2001:Property|});""",
            """new Mock<StandardClass>().SetupSet(i => i.AbstractInnerObject.{|PosInfoMoq2001:Property|} = "1234");""",
            """new Mock<StandardClass>().SetupSet<string>(i => i.AbstractInnerObject.{|PosInfoMoq2001:Property|} = "1234");""",
            """new Mock<StandardClass>().Setup(i => i.{|PosInfoMoq2001:SealedInnerObject|}.VirtualMethod());""",
            """new Mock<StandardClass>().Setup(i => i.{|PosInfoMoq2001:SealedInnerObject|}.VirtualProperty);""",
            """new Mock<StandardClass>().SetupSequence(i => i.{|PosInfoMoq2001:SealedInnerObject|}.VirtualMethod());""",
            """new Mock<StandardClass>().SetupSequence(i => i.{|PosInfoMoq2001:SealedInnerObject|}.VirtualProperty);""",
            """new Mock<StandardClass>().SetupSet(i => i.{|PosInfoMoq2001:SealedInnerObject|}.VirtualProperty = "1234");""",
            """new Mock<StandardClass>().SetupSet<string>(i => i.{|PosInfoMoq2001:SealedInnerObject|}.VirtualProperty = "1234");""",

            """new Mock<StandardClass>().Setup(i => StandardClass.{|PosInfoMoq2001:StaticMethod|}());""",
            """new Mock<StandardClass>().SetupSequence(i => StandardClass.{|PosInfoMoq2001:StaticMethod|}());""",

            """new Mock<AbstractClass>().Setup(i => i.{|PosInfoMoq2001:Method|}());""",
            """new Mock<AbstractClass>().Setup(i => i.{|PosInfoMoq2001:Property|});""",
            """new Mock<AbstractClass>().SetupSequence(i => i.{|PosInfoMoq2001:Method|}());""",
            """new Mock<AbstractClass>().SetupSequence(i => i.{|PosInfoMoq2001:Property|});""",
            """new Mock<AbstractClass>().SetupSet(i => i.{|PosInfoMoq2001:Property|} = "1234");""",
            """new Mock<AbstractClass>().SetupSet<string>(i => i.{|PosInfoMoq2001:Property|} = "1234");""",

            """new Mock<I>().Setup(i => i.{|PosInfoMoq2001:ExtensionMethod|}());""",
            """new Mock<I>().SetupSequence(i => i.{|PosInfoMoq2001:ExtensionMethod|}());""",
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