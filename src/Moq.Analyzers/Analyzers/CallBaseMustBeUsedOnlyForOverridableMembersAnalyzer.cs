//-----------------------------------------------------------------------
// <copyright file="CallBaseMustBeUsedOnlyForOverridableMembersAnalyzer.cs" company="P.O.S Informatique">
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
    public class CallBaseMustBeUsedOnlyForOverridableMembersAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2020",
            "The CallBase() method must be used only on virtual or override members",
            "The CallBase() method must be used only on virtual or override members which are not abstract or sealed",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The CallBase() method can be used only on virtual or override members which provide an implementation (not abstract, not sealed).",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2020.html");

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

            // Quick syntactic check on the CallBase() method name.
            if (invocationExpression.Expression is not MemberAccessExpressionSyntax callBaseAccess ||
                callBaseAccess.Name.Identifier.ValueText != "CallBase")
            {
                return;
            }

            var moqSymbols = MoqSymbols.FromCompilation(context.Compilation);

            if (moqSymbols is null)
            {
                return;
            }

            var callBaseSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (callBaseSymbol.Symbol is not IMethodSymbol callBaseMethod || !callBaseMethod.ContainingNamespace.ToDisplayString().StartsWith("Moq", StringComparison.Ordinal))
            {
                return;
            }

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            var setupMethod = moqExpressionAnalyzer.ExtractSetupMethod(invocationExpression, context.CancellationToken);

            if (setupMethod is null || setupMethod.Members.Count == 0)
            {
                return;
            }

            // The first member is the mocked member (the last in the chain).
            var member = setupMethod.Members[0];

            if (IsVirtualOrOverride(member.Symbol))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, callBaseAccess.Name.GetLocation()));
        }

        private static bool IsVirtualOrOverride(ISymbol symbol)
        {
            if (symbol.IsAbstract || symbol.IsSealed)
            {
                return false;
            }

            return symbol.IsVirtual || symbol.IsOverride;
        }
    }
}
