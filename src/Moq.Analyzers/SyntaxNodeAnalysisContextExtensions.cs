//-----------------------------------------------------------------------
// <copyright file="SyntaxNodeAnalysisContextExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class SyntaxNodeAnalysisContextExtensions
    {
        public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params object[]? messageArgs)
        {
            var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
            context.ReportDiagnostic(diagnostic);
        }

        public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, IEnumerable<Location> locations, params object[]? messageArgs)
        {
            var diagnostic = Diagnostic.Create(descriptor, locations.First(), locations.Skip(1), messageArgs);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
