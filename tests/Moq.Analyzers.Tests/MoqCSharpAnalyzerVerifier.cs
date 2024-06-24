//-----------------------------------------------------------------------
// <copyright file="MoqCSharpAnalyzerVerifier.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;

    internal static class MoqCSharpAnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expectedDiagnostics)
        {
            var context = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>();
            context.ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard20
                .AddPackages([new PackageIdentity("Moq", "4.20.70")]);

            context.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            context.TestCode = source;

            await context.RunAsync();
        }

        public static async Task VerifyAnalyzerWithNoMoqLibraryAsync(string source)
        {
            var context = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>();
            context.ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard20;

            context.TestCode = source;

            await context.RunAsync();
        }
    }
}
