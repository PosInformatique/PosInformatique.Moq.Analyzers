//-----------------------------------------------------------------------
// <copyright file="SetGenericArgumentSetupSetCodeFixProviderTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpCodeFixVerifier<SetupSetAnalyzer, SetGenericArgumentSetupSetCodeFixProvider>;

    public class SetGenericArgumentSetupSetCodeFixProviderTest
    {
        [Fact]
        public async Task SetupSet_Fix()
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
                            mock1.[|SetupSet|](i => i.TestPropertyInt32 = 1234);
                            mock1.[|SetupSet|](i => i.TestPropertyString = ""Foobard"");

                            // No changes
                            mock1.SetupSet<int>(i => i.TestPropertyInt32 = 1234);
                            mock1.SetupSet<string>(i => i.TestPropertyString = ""Foobard"");
                        }
                    }

                    public interface I
                    {
                        int TestPropertyInt32 { get; set; }

                        string TestPropertyString { get; set; }
                    }
                }";

            var expectedFixedSource =
            @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.SetupSet<int>(i => i.TestPropertyInt32 = 1234);
                            mock1.SetupSet<string>(i => i.TestPropertyString = ""Foobard"");

                            // No changes
                            mock1.SetupSet<int>(i => i.TestPropertyInt32 = 1234);
                            mock1.SetupSet<string>(i => i.TestPropertyString = ""Foobard"");
                        }
                    }

                    public interface I
                    {
                        int TestPropertyInt32 { get; set; }

                        string TestPropertyString { get; set; }
                    }
                }";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }
    }
}
