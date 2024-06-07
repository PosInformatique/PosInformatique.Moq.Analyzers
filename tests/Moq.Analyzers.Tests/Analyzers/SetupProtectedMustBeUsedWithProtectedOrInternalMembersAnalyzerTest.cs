//-----------------------------------------------------------------------
// <copyright file="SetupProtectedMustBeUsedWithProtectedOrInternalMembersAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using System.Threading.Tasks;
    using Xunit;
    using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
        SetupProtectedMustBeUsedWithProtectedOrInternalMembersAnalyzer,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

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

                            mock1.Protected().Setup<int>(""ProtectedVirtualMethod"");
                            mock1.Protected().Setup<int>(""ProtectedInternalVirtualMethod"");
                            mock1.Protected().Setup<int>(""InternalVirtualMethod"");
                            mock1.Protected().Setup<int>(""ProtectedAbstractMethod"");
                            mock1.Protected().Setup<int>(""ProtectedInternalAbstractMethod"");
                            mock1.Protected().Setup<int>(""InternalAbstractMethod"");
                        }
                    }

                    public abstract class C
                    {
                        protected virtual void ProtectedVirtualMethod() { }

                        protected internal virtual void ProtectedInternalVirtualMethod() { }

                        internal virtual void InternalVirtualMethod() { }

                        protected abstract void ProtectedAbstractMethod();

                        protected internal abstract void ProtectedInternalAbstractMethod();

                        internal abstract void InternalAbstractMethod();
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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
                        void TestMethodPublic() { }
                    }
                }
                " + MoqLibrary.Code;

            await Verify.VerifyAnalyzerAsync(source);
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

            await Verify.VerifyAnalyzerAsync(source);
        }
    }
}