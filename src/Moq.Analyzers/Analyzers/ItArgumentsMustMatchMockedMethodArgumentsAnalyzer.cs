//-----------------------------------------------------------------------
// <copyright file="ItArgumentsMustMatchMockedMethodArgumentsAnalyzer.cs" company="P.O.S Informatique">
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
    public class ItArgumentsMustMatchMockedMethodArgumentsAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq1006",
            "The It.IsAny<T>() or It.Is<T>() arguments must match the parameters of the mocked method",
            "The It.IsAny<T>() or It.Is<T>() arguments must match the parameters of the mocked method",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The It.IsAny<T>() or It.Is<T>() arguments must match the parameters of the mocked method.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Design/PosInfoMoq1006.html");

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

            // Extracts the setup method from the Callback() method call.
            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);
            var setupMethod = moqExpressionAnalyzer.ExtractSetupMethod(invocationExpression, context.CancellationToken);

            if (setupMethod is null)
            {
                return;
            }

            // Iterate for each parameter
            for (var i = 0; i < setupMethod.InvocationArguments.Count; i++)
            {
                var invocationArgument = setupMethod.InvocationArguments[i];

                // Check if the parameter is "It.IsAny<xxx>()"
                var itIsAnyType = moqSymbols.GetItIsAnyType(invocationArgument.Symbol);
                if (itIsAnyType is not null)
                {
                    if (!SymbolEqualityComparer.Default.Equals(itIsAnyType, invocationArgument.ParameterSymbol.Type))
                    {
                        context.ReportDiagnostic(Rule, invocationArgument.Syntax.GetLocation());
                        continue;
                    }
                }

                // Check if the parameter is "It.Is<xxx>()"
                var itIsType = moqSymbols.GetItIsType(invocationArgument.Symbol);
                if (itIsType is not null)
                {
                    if (!SymbolEqualityComparer.Default.Equals(itIsType, invocationArgument.ParameterSymbol.Type))
                    {
                        context.ReportDiagnostic(Rule, invocationArgument.Syntax.GetLocation());
                        continue;
                    }
                }
            }
        }
    }
}
