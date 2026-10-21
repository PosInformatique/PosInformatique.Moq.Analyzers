//-----------------------------------------------------------------------
// <copyright file="CallBaseMustBeUsedOnlyForOverridableMembersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<CallBaseMustBeUsedOnlyForOverridableMembersAnalyzer>;

    public class CallBaseMustBeUsedOnlyForOverridableMembersAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                void Method();

                string Property { get; }
            }

            public abstract class BaseClass
            {
                public abstract void AbstractMethod();

                public virtual void VirtualMethod() { }

                public virtual string VirtualProperty => null;
            }

            public class StandardClass : BaseClass
            {
                public override void AbstractMethod() { }

                public sealed override void VirtualMethod() { }

                public void Method() { }

                public string Property => null;

                public virtual int VirtualMethodWithResult() => 0;

                public virtual string VirtualSetProperty { get; set; }

                public virtual StandardClass Inner => null;

                public static void StaticMethod() { }
            }

            public class Derived : BaseClass
            {
                public override void AbstractMethod() { }

                public override string VirtualProperty => null;
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            "new Mock<StandardClass>().Setup(c => c.VirtualMethodWithResult()).CallBase();",
            "new Mock<StandardClass>().Setup(c => c.VirtualProperty).CallBase();",
            "new Mock<StandardClass>().Setup(c => c.Inner.VirtualMethodWithResult()).CallBase();",
            "new Mock<Derived>().Setup(c => c.AbstractMethod()).CallBase();",
            "new Mock<Derived>().Setup(c => c.VirtualProperty).CallBase();",
            "new Mock<Derived>().Setup(c => c.VirtualMethod()).CallBase();",

            // No CallBase() call.
            "new Mock<StandardClass>().Setup(c => c.Method());",
            "new Mock<I>().Setup(i => i.Method());",

            // Mock<T>.CallBase property (not a method).
            "new Mock<StandardClass>().CallBase = true;",
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            "new Mock<StandardClass>().Setup(c => c.Method()).{|PosInfoMoq2020:CallBase|}();",
            "new Mock<StandardClass>().Setup(c => c.Property).{|PosInfoMoq2020:CallBase|}();",
            "new Mock<StandardClass>().Setup(c => c.VirtualMethod()).{|PosInfoMoq2020:CallBase|}();",
            "new Mock<StandardClass>().Setup(c => c.Inner.Method()).{|PosInfoMoq2020:CallBase|}();",
            "new Mock<BaseClass>().Setup(c => c.AbstractMethod()).{|PosInfoMoq2020:CallBase|}();",
            "new Mock<I>().Setup(i => i.Method()).{|PosInfoMoq2020:CallBase|}();",
            "new Mock<I>().Setup(i => i.Property).{|PosInfoMoq2020:CallBase|}();",
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
