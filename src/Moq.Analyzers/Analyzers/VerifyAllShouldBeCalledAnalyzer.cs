﻿//-----------------------------------------------------------------------
// <copyright file="VerifyAllShouldBeCalledAnalyzer.cs" company="P.O.S Informatique">
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
    public class VerifyAllShouldBeCalledAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq1000",
            "VerifyAll() method should be called when instantiate a Mock<T> instances",
            "The VerifyAll() method should be called at the end of the unit test",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "VerifyAll() method should be called at the end of the unit test method.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Design/PosInfoMoq1000.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            var moqSymbols = MoqSymbols.FromCompilation(context.Compilation);

            if (moqSymbols is null)
            {
                return;
            }

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            if (!moqExpressionAnalyzer.IsMockCreation(objectCreation, context.CancellationToken))
            {
                return;
            }

            // Retrieve the variable name
            var variableName = objectCreation.Ancestors().OfType<VariableDeclaratorSyntax>().FirstOrDefault();

            if (variableName is null)
            {
                // No variable set on the left for the "new Mock<T>()". Skip it.
                return;
            }

            var variableNameModel = context.SemanticModel.GetDeclaredSymbol(variableName, context.CancellationToken);

            if (variableNameModel is null)
            {
                return;
            }

            // Check if there is a VerifyAll() invocation in the method's parent block.
            var parentMethod = objectCreation.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            if (parentMethod is null)
            {
                // Parent method not found, skip it.
                return;
            }

            // Retrieve all method invocation expressions.
            var invocationExpressions = parentMethod.DescendantNodes().OfType<InvocationExpressionSyntax>();

            var verifyAllCalled = invocationExpressions.Any(expression => IsMockVerifyAllInvocation(expression, variableNameModel, moqSymbols, context.SemanticModel, context.CancellationToken));

            if (!verifyAllCalled)
            {
                var diagnostic = Diagnostic.Create(Rule, objectCreation.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsMockVerifyAllInvocation(InvocationExpressionSyntax invocation, ISymbol variableNameSymbol, MoqSymbols moqSymbols, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            {
                return false;
            }

            // Check if the invocation expression is a Verify() / VerifyAll() methods.
            var verifyMethod = semanticModel.GetSymbolInfo(memberAccess, cancellationToken);

            if (verifyMethod.Symbol is null)
            {
                return false;
            }

            if (!moqSymbols.IsVerifyAllMethod(verifyMethod.Symbol))
            {
                if (!moqSymbols.IsVerifyAllStaticMethod(verifyMethod.Symbol))
                {
                    return false;
                }

                // Special case, the static method VerifyAll() has been called.
                // In this case, iterate on each arguments of the method called and check if the variableNameSymbol has been passed.
                foreach (var argument in invocation.ArgumentList.Arguments)
                {
                    var argumentSymbol = semanticModel.GetSymbolInfo(argument.Expression, cancellationToken);

                    if (argumentSymbol.Symbol is null)
                    {
                        return false;
                    }

                    if (SymbolEqualityComparer.Default.Equals(argumentSymbol.Symbol, variableNameSymbol))
                    {
                        return true;
                    }
                }

                return false;
            }

            // Gets the variable name symbol.
            var identifierSymbol = semanticModel.GetSymbolInfo(memberAccess.Expression, cancellationToken);

            // If the variable name of .VerifyAll() does not match the variable, so the VerifyAll() was for other Mock instance.
            if (!SymbolEqualityComparer.Default.Equals(identifierSymbol.Symbol, variableNameSymbol))
            {
                return false;
            }

            return true;
        }
    }
}
