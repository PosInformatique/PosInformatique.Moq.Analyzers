//-----------------------------------------------------------------------
// <copyright file="SetupMethodShouldSpecifyOptionalArgumentsAnalyzer.cs" company="P.O.S Informatique">
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
    public class SetupMethodShouldSpecifyOptionalArgumentsAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor SetupMustExplicitlySpecifyOptionalArgumentsRule = new DiagnosticDescriptor(
            "PosInfoMoq1011",
            "The Setup() expression must explicitly specify optional argument values",
            "The Setup() expression must explicitly specify optional argument values",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Optional arguments in Setup() must be written explicitly, even when the default value is used.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Design/PosInfoMoq1011.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [SetupMustExplicitlySpecifyOptionalArgumentsRule];

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

            // Check it is a Setup() method.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (!moqSymbols.IsSetupMethod(methodSymbol.Symbol))
            {
                return;
            }

            // Extracts the setup method.
            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            var setupMethod = moqExpressionAnalyzer.ExtractSetupMethod(invocationExpression, context.CancellationToken);

            if (setupMethod is null)
            {
                return;
            }

            foreach (var parameter in setupMethod.Parameters)
            {
                if (!parameter.IsOptional)
                {
                    continue;
                }

                var invocationArgument = setupMethod.GetInvocationArgument(parameter);

                if (invocationArgument is null)
                {
                    context.ReportDiagnostic(SetupMustExplicitlySpecifyOptionalArgumentsRule, setupMethod.InvocationExpression.GetLocation());
                    return;
                }
            }
        }
    }
}
