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
        [Fact]
        public async Task NoDiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using Moq.Protected;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>();
                            mock1.Protected().Setup(""ProtectedVirtualMethod"");
                            mock1.Protected().Setup(""ProtectedInternalVirtualMethod"");
                            mock1.Protected().Setup(""InternalVirtualMethod"");
                            mock1.Protected().Setup(""ProtectedAbstractMethod"");
                            mock1.Protected().Setup(""ProtectedInternalAbstractMethod"");
                            mock1.Protected().Setup(""InternalAbstractMethod"");
                            mock1.Protected().Setup(""ProtectedMethod"");
                            mock1.Protected().Setup(""ProtectedOverrideMethod"");
                            mock1.Protected().Setup(""ProtectedInternalOverrideMethod"");
                            mock1.Protected().Setup(""ProtectedInternalMethod"");
                            mock1.Protected().Setup(""InternalMethod"");
                            mock1.Protected().Setup(""InternalOverrideMethod"");

                            mock1.Protected().Setup<int>(""ProtectedVirtualMethod"");
                            mock1.Protected().Setup<int>(""ProtectedInternalVirtualMethod"");
                            mock1.Protected().Setup<int>(""InternalVirtualMethod"");
                            mock1.Protected().Setup<int>(""ProtectedAbstractMethod"");
                            mock1.Protected().Setup<int>(""ProtectedInternalAbstractMethod"");
                            mock1.Protected().Setup<int>(""InternalAbstractMethod"");
                            mock1.Protected().Setup<int>(""ProtectedOverrideMethod"");
                            mock1.Protected().Setup<int>(""ProtectedInternalOverrideMethod"");
                            mock1.Protected().Setup<int>(""InternalOverrideMethod"");
                        }
                    }

                    public abstract class C : BaseClass
                    {
                        // Virtual
                        protected virtual void ProtectedVirtualMethod() { }

                        protected internal virtual void ProtectedInternalVirtualMethod() { }

                        internal virtual void InternalVirtualMethod() { }

                        // Abstract
                        protected abstract void ProtectedAbstractMethod();

                        protected internal abstract void ProtectedInternalAbstractMethod();

                        internal abstract void InternalAbstractMethod();

                        // Override
                        protected override void ProtectedOverrideMethod() { }

                        protected internal override void ProtectedInternalOverrideMethod() { }

                        internal override void InternalOverrideMethod() { }
                    }

                    public abstract class BaseClass
                    {
                        protected virtual void ProtectedOverrideMethod() { }

                        protected virtual void ProtectedMethod() { }

                        protected internal virtual void ProtectedInternalOverrideMethod() { }

                        protected internal virtual void ProtectedInternalMethod() { }

                        internal virtual void InternalOverrideMethod() { }

                        internal virtual void InternalMethod() { }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task PublicMethod_DiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using Moq.Protected;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>();
                            mock1.Protected().Setup([|""TestMethodPublic""|]);
                            mock1.Protected().Setup<int>([|""TestMethodPublic""|]);
                        }
                    }

                    public class C
                    {
                        public virtual void TestMethodPublic() { }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task SealedMethod_DiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using Moq.Protected;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>();
                            mock1.Protected().Setup([|""TestMethod""|]);
                            mock1.Protected().Setup<int>([|""TestMethod""|]);
                        }
                    }

                    public class C : BaseClass
                    {
                        protected override sealed void TestMethod() { }
                    }

                    public abstract class BaseClass
                    {
                        protected abstract void TestMethod();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Interface_DiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using Moq.Protected;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Protected().Setup([|""TestMethodPublic""|]);
                            mock1.Protected().Setup<int>([|""TestMethodPublic""|]);
                        }
                    }

                    public interface I
                    {
                        void TestMethodPublic();
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NonOverridableMethod_DiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using Moq.Protected;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<C>();
                            mock1.Protected().Setup([|""NonOverridableProtectedMethod""|]);
                            mock1.Protected().Setup([|""NonOverridableProtectedInternalMethod""|]);
                            mock1.Protected().Setup([|""NonOverridableInternalMethod""|]);

                            mock1.Protected().Setup<int>([|""NonOverridableProtectedMethod""|]);
                            mock1.Protected().Setup<int>([|""NonOverridableProtectedInternalMethod""|]);
                            mock1.Protected().Setup<int>([|""NonOverridableInternalMethod""|]);
                        }
                    }

                    public class C
                    {
                        protected void NonOverridableProtectedMethod() { }
                        protected internal void NonOverridableProtectedInternalMethod() { }
                        internal void NonOverridableInternalMethod() { }
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
                            var mock1 = new Mock<C>();
                            mock1.Protected().Setup(""NonOverridableProtectedMethod"");
                        }
                    }

                    public class C
                    {
                        protected virtual void NonOverridableProtectedMethod() { }
                    }
                }

                namespace OtherNamespace
                {
                    public class Mock<T>
                    {
                        public Mock() { }

                        public IProtected Protected() { return null; }
                    }

                    public interface IProtected
                    {
                        void Setup(string _);
                    }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}