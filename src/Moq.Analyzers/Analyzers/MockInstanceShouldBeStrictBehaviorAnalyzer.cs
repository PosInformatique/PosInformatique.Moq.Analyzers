//-----------------------------------------------------------------------
// <copyright file="MockInstanceShouldBeStrictBehaviorAnalyzer.cs" company="P.O.S Informatique">
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
    public class MockInstanceShouldBeStrictBehaviorAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PosInfoMoq1001";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "The mocked instances behavior should be defined to Strict mode",
            "The mocked instances behavior should be defined to Strict mode",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The mocked instances behavior should be defined to Strict mode.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Design/PosInfoMoq1001.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            var moqSymbols = MoqSymbols.FromCompilation(context.Compilation);

            if (moqSymbols is null)
            {
                return;
            }

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            // Check there is "new Mock<I>()" statement.
            if (!moqExpressionAnalyzer.IsMockCreation(objectCreation, context.CancellationToken))
            {
                return;
            }

            if (!moqExpressionAnalyzer.IsMockCreationStrictBehavior(objectCreation, context.CancellationToken))
            {
                var diagnostic = Diagnostic.Create(Rule, objectCreation.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var moqSymbols = MoqSymbols.FromCompilation(context.Compilation);

            if (moqSymbols is null)
            {
                return;
            }

            // Check the expression is a Mock.Of method.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken).Symbol;

            if (!moqSymbols.IsMockOfMethod(methodSymbol))
            {
                return;
            }

            // Extract the type
            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            if (!moqExpressionAnalyzer.IsMockOfStrictBehavior(invocationExpression.ArgumentList, context.CancellationToken))
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
