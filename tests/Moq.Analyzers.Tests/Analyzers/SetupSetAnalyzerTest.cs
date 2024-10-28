//-----------------------------------------------------------------------
// <copyright file="SetupSetAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<SetupSetAnalyzer>;

    public class SetupSetAnalyzerTest
    {
        [Fact]
        public async Task SetupSetWithGenericArgument_NoDiagnosticReported()
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
                            mock1.SetupSet<int>(i => i.TestProperty = 1234);
                            mock1.SetupSet(i => i.TestProperty);   // Ignored because Obsolete by Moq

                            var s = ""The string"";
                            s.ToString();       // Invocation should be ignored
                        }
                    }

                    public interface I
                    {
                        int TestProperty { get; set; }
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task SetupSetWithNoGenericArgument_DiagnosticReported()
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
                            mock1.[|SetupSet|](i => i.TestProperty = 1234);

                            var s = ""The string"";
                            s.ToString();       // Invocation should be ignored
                        }
                    }

                    public interface I
                    {
                        int TestProperty { get; set; }
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
                            var mock1 = new Mock<I>();
                            mock1.SetupSet(i => i.Property = 1234);
                        }
                    }

                    public interface I
                    {
                        int Property { get; set; }
                    }
                }

                namespace OtherNamespace
                {
                    public class Mock<T>
                    {
                        public void SetupSet(System.Action<T> _) { }
                    }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}