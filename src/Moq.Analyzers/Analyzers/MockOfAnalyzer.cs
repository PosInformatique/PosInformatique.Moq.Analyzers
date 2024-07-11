//-----------------------------------------------------------------------
// <copyright file="MockOfAnalyzer.cs" company="P.O.S Informatique">
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
    public class MockOfAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2009",
            "Mock.Of<T> method must be used only to mock non-sealed class",
            "Mock.Of<T> method must be used only to mock non-sealed class",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Mock<T> method must be used only to mock non-sealed class.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2009.html");

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

            // Check the expression is a Mock.Of method.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken).Symbol;

            if (!moqSymbols.IsMockOfMethod(methodSymbol))
            {
                return;
            }

            // Extract the generic parameter in the Mock.Of<T>() expression.
            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            {
                return;
            }

            TypeSyntax mockedTypeExpression;

            if (memberAccessExpression.Name is not GenericNameSyntax genericNameExpression)
            {
                // Special case here, we use the Mock.Of((MyMockedClass) => ...) overload.
                var lambdaExpressionArgument = invocationExpression.ArgumentList.Arguments[0];

                if (lambdaExpressionArgument.Expression is not ParenthesizedLambdaExpressionSyntax lambdaExpressionSyntax)
                {
                    return;
                }

                var parameter = lambdaExpressionSyntax.ParameterList.Parameters[0];

                if (parameter.Type is null)
                {
                    return;
                }

                mockedTypeExpression = parameter.Type;
            }
            else
            {
                mockedTypeExpression = genericNameExpression.TypeArgumentList.Arguments[0];
            }

            // Check the expression is a Mock<T> instance creation.
            var symbol = context.SemanticModel.GetSymbolInfo(mockedTypeExpression, context.CancellationToken).Symbol;

            if (symbol is not ITypeSymbol mockedType)
            {
                return;
            }

            if (moqSymbols.IsMockable(mockedType))
            {
                return;
            }

            // The mocked type is not mockabke. Report the diagnostic issue.
            var diagnostic = Diagnostic.Create(Rule, mockedTypeExpression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
