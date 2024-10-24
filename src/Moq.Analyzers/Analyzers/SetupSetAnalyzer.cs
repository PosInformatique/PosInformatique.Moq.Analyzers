//-----------------------------------------------------------------------
// <copyright file="SetupSetAnalyzer.cs" company="P.O.S Informatique">
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
    public class SetupSetAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor UseSetupSetWithGenericArgumentRule = new DiagnosticDescriptor(
            "PosInfoMoq1005",
            "Defines the generic argument of the SetupSet() method with the type of the mocked property",
            "Defines the generic argument of the SetupSet() method with the type of the mocked property",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The SetupSet<T>() method must be used when setting up a mocked property.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq1005.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(UseSetupSetWithGenericArgumentRule);

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

            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            // Check if SetupSet() method.
            if (!moqSymbols.IsSetupSetMethod(methodSymbol.Symbol))
            {
                return;
            }

            // Check is SetupSet<T>() method.
            if (!moqSymbols.IsSetupSetMethodWithoutGenericArgument(methodSymbol.Symbol))
            {
                var nameSyntax = ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name;

                context.ReportDiagnostic(UseSetupSetWithGenericArgumentRule, nameSyntax.GetLocation());

                return;
            }
        }
    }
}
