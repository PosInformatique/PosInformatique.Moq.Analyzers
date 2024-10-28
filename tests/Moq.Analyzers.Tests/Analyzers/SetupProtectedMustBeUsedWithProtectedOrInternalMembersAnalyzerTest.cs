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

                            mock1.Protected().Setup<int>(""ProtectedVirtualMethodReturnValue"");
                            mock1.Protected().Setup<int>(""ProtectedInternalVirtualMethodReturnValue"");
                            mock1.Protected().Setup<int>(""InternalVirtualMethodReturnValue"");
                            mock1.Protected().Setup<int>(""ProtectedAbstractMethodReturnValue"");
                            mock1.Protected().Setup<int>(""ProtectedInternalAbstractMethodReturnValue"");
                            mock1.Protected().Setup<int>(""InternalAbstractMethodReturnValue"");
                            mock1.Protected().Setup<int>(""ProtectedOverrideMethodReturnValue"");
                            mock1.Protected().Setup<int>(""ProtectedInternalOverrideMethodReturnValue"");
                            mock1.Protected().Setup<int>(""InternalOverrideMethodReturnValue"");
                            mock1.Protected().Setup<int>(""InternalMethodReturnValue"");
                        }
                    }

                    public abstract class C : BaseClass
                    {
                        // Virtual
                        protected virtual void ProtectedVirtualMethod() { }

                        protected virtual int ProtectedVirtualMethodReturnValue() { return 10;}

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
                            mock1.Protected().Setup({|PosInfoMoq2006:""TestMethodPublic""|});
                            mock1.Protected().Setup<int>({|PosInfoMoq2006:""TestMethodPublic""|});
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
                            mock1.Protected().Setup({|PosInfoMoq2006:""TestMethod""|});
                            mock1.Protected().Setup<int>({|PosInfoMoq2006:""TestMethod""|});
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
                            mock1.Protected().Setup({|PosInfoMoq2006:""TestMethodPublic""|});
                            mock1.Protected().Setup<int>({|PosInfoMoq2006:""TestMethodPublic""|});
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
                            mock1.Protected().Setup({|PosInfoMoq2006:""NonOverridableProtectedMethod""|});
                            mock1.Protected().Setup({|PosInfoMoq2006:""NonOverridableProtectedInternalMethod""|});
                            mock1.Protected().Setup({|PosInfoMoq2006:""NonOverridableInternalMethod""|});

                            mock1.Protected().Setup<int>({|PosInfoMoq2006:""NonOverridableProtectedMethod""|});
                            mock1.Protected().Setup<int>({|PosInfoMoq2006:""NonOverridableProtectedInternalMethod""|});
                            mock1.Protected().Setup<int>({|PosInfoMoq2006:""NonOverridableInternalMethod""|});
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
        public async Task ReturnMethodTypeNotMatching_DiagnosticReported()
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
                            mock1.Protected().Setup<{|PosInfoMoq2015:long|}>(""TestMethodProtectedReturnValue"");
                            mock1.Protected().Setup<{|PosInfoMoq2015:long|}>(""TestMethodProtected"");
                            mock1.Protected().{|PosInfoMoq2015:Setup|}(""TestMethodProtectedReturnValue"");
                        }
                    }

                    public class C
                    {
                        protected virtual int TestMethodProtectedReturnValue() { return 0; }

                        protected virtual void TestMethodProtected() { }
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