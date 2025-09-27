//-----------------------------------------------------------------------
// <copyright file="VerifyMustHaveTimesParameterAnalyzer.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VerifyMustHaveTimesParameterAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq1007",
            "The Verify() method must specify the Times argument",
            "The Verify() method must specify the Times argument",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Using the Verify() method with an explicit Times argument makes the test intention clear and avoids ambiguity.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq1007.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var moqSymbols = MoqSymbols.FromCompilation(context.Compilation);

            if (moqSymbols is null)
            {
                return;
            }

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            // Check is Verify() method.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (!moqSymbols.IsVerifyMethod(methodSymbol.Symbol))
            {
                return;
            }

            // Check if the Verify() method contains the Times parameter.
            if (!moqSymbols.ContainsTimesParameters((IMethodSymbol)methodSymbol.Symbol!))
            {
                // The member is not overridable, raise the error.
                var diagnostic = Diagnostic.Create(Rule, ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name.GetLocation());
                context.ReportDiagnostic(diagnostic);

                return;
            }
        }
    }
}
