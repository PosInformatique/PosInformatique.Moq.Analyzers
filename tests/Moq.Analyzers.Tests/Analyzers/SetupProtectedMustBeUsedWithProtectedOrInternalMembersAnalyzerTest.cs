//-----------------------------------------------------------------------
// <copyright file="SetupProtectedMustBeUsedWithProtectedOrInternalMembersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<SetupProtectedMustBeUsedWithProtectedOrInternalMembersAnalyzer>;

    public class SetupProtectedMustBeUsedWithProtectedOrInternalMembersAnalyzerTest
    {
        private const string TestTypes =
            """
            public abstract class C : BaseClass
            {
                // Virtual
                protected virtual void ProtectedVirtualMethod() { }

                protected virtual int ProtectedVirtualMethodReturnValue() { return 10; }

                protected internal virtual void ProtectedInternalVirtualMethod() { }

                protected internal virtual int ProtectedInternalVirtualMethodReturnValue() { return 10; }

                internal virtual void InternalVirtualMethod() { }

                internal virtual int InternalVirtualMethodReturnValue() { return 10; }

                // Abstract
                protected abstract void ProtectedAbstractMethod();

                protected abstract int ProtectedAbstractMethodReturnValue();

                protected internal abstract void ProtectedInternalAbstractMethod();

                protected internal abstract int ProtectedInternalAbstractMethodReturnValue();

                internal abstract void InternalAbstractMethod();

                internal abstract int InternalAbstractMethodReturnValue();

                // Override
                protected override void ProtectedOverrideMethod() { }

                protected override int ProtectedOverrideMethodReturnValue() { return 10; }

                protected internal override void ProtectedInternalOverrideMethod() { }

                protected internal override int ProtectedInternalOverrideMethodReturnValue() { return 10; }

                internal override void InternalOverrideMethod() { }

                internal override int InternalOverrideMethodReturnValue() { return 10; }

                public virtual void TestMethodPublic() { }

                protected override sealed void TestMethod() { }

                protected void NonOverridableProtectedMethod() { }

                protected internal void NonOverridableProtectedInternalMethod() { }

                internal void NonOverridableInternalMethod() { }

                protected virtual int TestMethodProtectedReturnValue() { return 0; }

                protected virtual void TestMethodProtected() { }
            }

            public abstract class BaseClass
            {
                protected virtual void ProtectedOverrideMethod() { }

                protected virtual int ProtectedOverrideMethodReturnValue() { return 10; }

                protected virtual void ProtectedMethod() { }

                protected internal virtual void ProtectedInternalOverrideMethod() { }

                protected internal virtual int ProtectedInternalOverrideMethodReturnValue() { return 10; }

                protected internal virtual void ProtectedInternalMethod() { }

                internal virtual void InternalOverrideMethod() { }

                internal virtual int InternalOverrideMethodReturnValue() { return 10; }

                internal virtual void InternalMethod() { }

                internal virtual int InternalMethodReturnValue() { return 10; }

                protected abstract void TestMethod();
            }

            public interface I
            {
                void TestMethodPublic();
            }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("ProtectedVirtualMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("ProtectedInternalVirtualMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("InternalVirtualMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("ProtectedAbstractMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("ProtectedInternalAbstractMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("InternalAbstractMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("ProtectedMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("ProtectedOverrideMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("ProtectedInternalOverrideMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("ProtectedInternalMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("InternalMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup("InternalOverrideMethod");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("ProtectedVirtualMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("ProtectedInternalVirtualMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("InternalVirtualMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("ProtectedAbstractMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("ProtectedInternalAbstractMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("InternalAbstractMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("ProtectedOverrideMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("ProtectedInternalOverrideMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("InternalOverrideMethodReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>("InternalMethodReturnValue");
            """,
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup({|PosInfoMoq2006:"TestMethodPublic"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>({|PosInfoMoq2006:"TestMethodPublic"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup({|PosInfoMoq2006:"TestMethod"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>({|PosInfoMoq2006:"TestMethod"|});
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Protected().Setup({|PosInfoMoq2006:"TestMethodPublic"|});
            """,

            """
            var mock1 = new Mock<I>();
            mock1.Protected().Setup<int>({|PosInfoMoq2006:"TestMethodPublic"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup({|PosInfoMoq2006:"NonOverridableProtectedMethod"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup({|PosInfoMoq2006:"NonOverridableProtectedInternalMethod"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup({|PosInfoMoq2006:"NonOverridableInternalMethod"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>({|PosInfoMoq2006:"NonOverridableProtectedMethod"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>({|PosInfoMoq2006:"NonOverridableProtectedInternalMethod"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<int>({|PosInfoMoq2006:"NonOverridableInternalMethod"|});
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<{|PosInfoMoq2015:long|}>("TestMethodProtectedReturnValue");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().Setup<{|PosInfoMoq2015:long|}>("TestMethodProtected");
            """,

            """
            var mock1 = new Mock<C>();
            mock1.Protected().{|PosInfoMoq2015:Setup|}("TestMethodProtectedReturnValue");
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
                    using Moq.Protected;

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
                    using FakeMoqLibrary.Protected;

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