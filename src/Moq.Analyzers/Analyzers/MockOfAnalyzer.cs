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
        private static readonly DiagnosticDescriptor MustBeUsedOnlyToMockNonSealedClassRule = new DiagnosticDescriptor(
            "PosInfoMoq2009",
            "Mock.Of<T> method must be used only to mock non-sealed class",
            "Mock.Of<T> method must be used only to mock non-sealed class",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Mock<T> method must be used only to mock non-sealed class.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2009.html");

        private static readonly DiagnosticDescriptor MustContainsParameterlessContructorRule = new DiagnosticDescriptor(
            "PosInfoMoq2010",
            "Mock.Of<T> method must be used only with types that contains parameterless contructor",
            "Mock.Of<T> method must be used only with types that contains parameterless contructor",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Mock.Of<T> method must be used only with types that contains parameterless contructor.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2010.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(MustBeUsedOnlyToMockNonSealedClassRule, MustContainsParameterlessContructorRule);

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

            // Gets the symbol of the mocked type
            var symbol = context.SemanticModel.GetSymbolInfo(mockedTypeExpression, context.CancellationToken).Symbol;

            if (symbol is not INamedTypeSymbol mockedType)
            {
                return;
            }

            // Check if the type is mockable.
            if (!moqSymbols.IsMockable(mockedType))
            {
                // The mocked type is not mockabke. Report the diagnostic issue.
                context.ReportDiagnostic(MustBeUsedOnlyToMockNonSealedClassRule, mockedTypeExpression.GetLocation());
                return;
            }

            // Check if the type contains a parameterless constructor (ignore this check for the interfaces).
            if (mockedType.TypeKind == TypeKind.Interface)
            {
                return;
            }

            foreach (var contructor in mockedType.Constructors)
            {
                if (contructor.DeclaredAccessibility != Accessibility.Private && contructor.Parameters.Length == 0)
                {
                    return;
                }
            }

            // No parameter less constructor has been found. Report the diagnostic issue.
            context.ReportDiagnostic(MustContainsParameterlessContructorRule, mockedTypeExpression.GetLocation());
        }
    }
}
