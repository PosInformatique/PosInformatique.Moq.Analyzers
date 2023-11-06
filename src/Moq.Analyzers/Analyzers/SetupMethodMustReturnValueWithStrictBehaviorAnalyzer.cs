//-----------------------------------------------------------------------
// <copyright file="SetupMethodMustReturnValueWithStrictBehaviorAnalyzer.cs" company="P.O.S Informatique">
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
    public class SetupMethodMustReturnValueWithStrictBehaviorAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "Moq2000",
            "The Returns() or ReturnsAsync() method must be called for Strict mocks",
            "The Returns() or ReturnsAsync() method must be called for Strict mocks",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Returns() or ReturnsAsync() method must be called for Strict mocks.");

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

            // Check is Setup() method.
            if (!MockExpressionHelper.IsMockSetupMethod(moqSymbols, context.SemanticModel, invocationExpression, out var localVariableExpression))
            {
                return;
            }

            // Check the mocked method return type.
            var mockedMethodReturnTypeSymbol = MockExpressionHelper.GetSetupMethodReturnSymbol(moqSymbols, context.SemanticModel, invocationExpression);
            if (mockedMethodReturnTypeSymbol is null)
            {
                return;
            }

            if (mockedMethodReturnTypeSymbol.SpecialType == SpecialType.System_Void)
            {
                return;
            }

            // Check the behavior of the mock instance is Strict.
            if (!MockExpressionHelper.IsStrictBehavior(moqSymbols, context.SemanticModel, localVariableExpression))
            {
                return;
            }

            // Check there Returns() method for the following calls.
            var followingMethods = invocationExpression.Ancestors().OfType<InvocationExpressionSyntax>();

            foreach (var followingMethod in followingMethods)
            {
                var methodSymbol = context.SemanticModel.GetSymbolInfo(followingMethod);

                if (moqSymbols.IsReturnsMethod(methodSymbol.Symbol) || moqSymbols.IsReturnsAsyncMethod(methodSymbol.Symbol))
                {
                    return;
                }
            }

            // No returns method has been specified with Strict mode. Report the diagnostic issue.
            var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
