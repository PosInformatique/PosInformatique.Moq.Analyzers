//-----------------------------------------------------------------------
// <copyright file="ReturnsMethodDelegateMustMatchMockedMethodAnalyzer.cs" company="P.O.S Informatique">
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
    public class ReturnsMethodDelegateMustMatchMockedMethodAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor ReturnValueMustMatchRule = new DiagnosticDescriptor(
            "PosInfoMoq2012",
            "The delegate in the argument of the Returns() method must return a value with same type of the mocked method/property",
            "The delegate in the argument of the Returns() method must return a '{0}' type value",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The delegate in the argument of the Returns() method must return a value with same type of the mocked method/property.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2012.html");

        private static readonly DiagnosticDescriptor ArgumentMustMatchRule = new DiagnosticDescriptor(
            "PosInfoMoq2013",
            "The delegate in the argument of the Returns()/ReturnsAsync() method must have the same parameter types of the mocked method/property",
            "The delegate in the argument of the Returns()/ReturnsAsync() method must have the same parameter types of the mocked method/property",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The delegate in the argument of the Returns()/ReturnsAsync() method must have the same parameter types of the mocked method/property.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2012.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ReturnValueMustMatchRule, ArgumentMustMatchRule);

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

            var invocationExpressionSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (invocationExpressionSymbol.Symbol is not IMethodSymbol methodSymbol)
            {
                return;
            }

            if (!moqSymbols.IsReturnsMethod(methodSymbol) && !moqSymbols.IsReturnsAsyncMethod(methodSymbol))
            {
                return;
            }

            // Gets the first argument of the Returns() method.
            if (invocationExpression.ArgumentList.Arguments.Count != 1)
            {
                return;
            }

            var firstArgumentExpression = invocationExpression.ArgumentList.Arguments[0].Expression;

            if (firstArgumentExpression is not ParenthesizedLambdaExpressionSyntax delegateMethodSyntax)
            {
                return;
            }

            var firstArgumentSymbol = context.SemanticModel.GetSymbolInfo(firstArgumentExpression, context.CancellationToken);

            if (firstArgumentSymbol.Symbol is not IMethodSymbol delegateMethodSymbol)
            {
                return;
            }

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            // Extract the Setup() method.
            var setupMethod = moqExpressionAnalyzer.ExtractSetupMethod(invocationExpression, context.CancellationToken);

            if (setupMethod is null)
            {
                return;
            }

            // Check the return type
            if (!moqSymbols.IsReturnsAsyncMethod(methodSymbol))
            {
                var expectedReturnType = setupMethod.ReturnType;

                if (!SymbolEqualityComparer.Default.Equals(delegateMethodSymbol.ReturnType, expectedReturnType))
                {
                    context.ReportDiagnostic(ReturnValueMustMatchRule, firstArgumentExpression.GetLocation(), expectedReturnType.Name);
                }
            }

            // Check the argument types.
            if (setupMethod.IsProperty)
            {
                if (delegateMethodSymbol.Parameters.Length > 0)
                {
                    // With property, the Returns() method must have no arguments.
                    context.ReportDiagnostic(ArgumentMustMatchRule, delegateMethodSyntax.ParameterList.GetLocation());
                }

                return;
            }

            if (delegateMethodSymbol.Parameters.Length == 0)
            {
                // No argument in the delegate method, Moq accept it.
                return;
            }

            if (delegateMethodSymbol.Parameters.Length != setupMethod.InvocationArguments.Count)
            {
                context.ReportDiagnostic(ArgumentMustMatchRule, delegateMethodSyntax.ParameterList.GetLocation());
                return;
            }

            for (var i = 0; i < delegateMethodSymbol.Parameters.Length; i++)
            {
                if (!SymbolEqualityComparer.Default.Equals(delegateMethodSymbol.Parameters[i].Type, setupMethod.InvocationArguments[i].ParameterSymbol.Type))
                {
                    context.ReportDiagnostic(ArgumentMustMatchRule, delegateMethodSyntax.ParameterList.Parameters[i].GetLocation());
                }
            }
        }
    }
}
