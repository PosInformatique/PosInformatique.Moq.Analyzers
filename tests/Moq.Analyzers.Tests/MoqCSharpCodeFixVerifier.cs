//-----------------------------------------------------------------------
// <copyright file="MoqCSharpCodeFixVerifier.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;

    internal static class MoqCSharpCodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        public static async Task VerifyCodeFixAsync(string source, string expectedFixedSource, params DiagnosticResult[] expectedDiagnostics)
        {
            var context = new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>();
            context.ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard20
                .AddPackages([new PackageIdentity("Moq", "4.20.70")]);

            context.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            context.TestCode = source;

            await context.RunAsync();
        }
    }
}
