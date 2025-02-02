﻿//-----------------------------------------------------------------------
// <copyright file="SetupMustBeUsedOnlyForOverridableMembersAnalyzer.cs" company="P.O.S Informatique">
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
    public class SetupMustBeUsedOnlyForOverridableMembersAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2001",
            "The Setup()/SetupSet() method must be used only on overridable members",
            "The Setup()/SetupSet() method must be used only on overridable members",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Setup() method must be used only on overridable members.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2001.html");

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

            // Check is Setup() method.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (!moqSymbols.IsSetupMethod(methodSymbol.Symbol) && !moqSymbols.IsSetupSetMethod(methodSymbol.Symbol))
            {
                return;
            }

            // Extracts the method in the lambda expression of the Setup() method
            var setupMethod = moqExpressionAnalyzer.ExtractChainedMembersInvocationFromLambdaExpression(invocationExpression, context.CancellationToken);

            if (setupMethod is null)
            {
                return;
            }

            // Check if the member is overridable.
            foreach (var member in setupMethod.Members)
            {
                if (!moqSymbols.IsOverridable(member.Symbol))
                {
                    // The member is not overridable, raise the error.
                    var diagnostic = Diagnostic.Create(Rule, member.Syntax.GetLocation());
                    context.ReportDiagnostic(diagnostic);

                    return;
                }
            }
        }
    }
}
