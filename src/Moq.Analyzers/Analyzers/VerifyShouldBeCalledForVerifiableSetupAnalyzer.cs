//-----------------------------------------------------------------------
// <copyright file="VerifyShouldBeCalledForVerifiableSetupAnalyzer.cs" company="P.O.S Informatique">
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
    public class VerifyShouldBeCalledForVerifiableSetupAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq1002",
            "Verify() methods should be called when Verifiable() has been setup",
            "The Verify() methods should be called at the end of the unit tests when Verifiable() has been setup",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The Verify() methods should be called at the end of the unit tests when Verifiable() has been setup.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Design/PosInfoMoq1002.html");

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

            // Check it is a Verifiable() method.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (!moqSymbols.IsVerifiableMethod(methodSymbol.Symbol))
            {
                return;
            }

            // Retrieve the setup method
            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            var setupMethod = moqExpressionAnalyzer.ExtractSetupMethod(invocationExpression, context.CancellationToken);

            if (setupMethod is null)
            {
                return;
            }

            // Gets the variable of the mocked instance.
            var variableNameModel = moqExpressionAnalyzer.GetMockVariable(invocationExpression, out var localVariableExpression, context.CancellationToken);

            if (variableNameModel is null)
            {
                return;
            }

            // Check if there is a Verify() invocation in the method's parent block.
            var parentMethod = localVariableExpression!.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            if (parentMethod is null)
            {
                // Parent method not found, skip it.
                return;
            }

            // Retrieve all method invocation expressions.
            var invocationExpressions = parentMethod.DescendantNodes().OfType<InvocationExpressionSyntax>();

            var verifyCalled = invocationExpressions.Any(expression => IsMockVerifyInvocation(expression, moqExpressionAnalyzer, variableNameModel, setupMethod, moqSymbols, context.SemanticModel, context.CancellationToken));

            if (!verifyCalled)
            {
                if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
                {
                    return;
                }

                var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.Name.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsMockVerifyInvocation(InvocationExpressionSyntax invocation, MoqExpressionAnalyzer moqExpressionAnalyzer, ISymbol variableNameSymbol, ChainMembersInvocation setupMethod, MoqSymbols moqSymbols, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            {
                return false;
            }

            // Check if the invocation expression is a Verify() method.
            var verifyMethodSymbol = semanticModel.GetSymbolInfo(memberAccess, cancellationToken);

            if (!moqSymbols.IsVerifyMethod(verifyMethodSymbol.Symbol))
            {
                if (!moqSymbols.IsVerifyStaticMethod(verifyMethodSymbol.Symbol))
                {
                    return false;
                }

                // Special case, the static method Verify() has been called.
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

            // Here we called the myMock.Verify() method. So check the variable is related to the variable of the invoked expression.
            var identifierSymbol = semanticModel.GetSymbolInfo(memberAccess.Expression, cancellationToken);

            // If the variable name of .Verify() does not match the variable, so the Verify() was for other Mock instance.
            if (!SymbolEqualityComparer.Default.Equals(identifierSymbol.Symbol, variableNameSymbol))
            {
                return false;
            }

            // The variable "myMock.Verify()" match the "myMock.Setup()", so now check the Verify() expression.
            // First check if there is no argument to the verify method (in this case, the developer call the Verify() method, so it is OK).
            if (invocation.ArgumentList.Arguments.Count == 0)
            {
                return true;
            }

            // Else extract the members chain the Verify(xxx) lambda expression.
            // Compare the members chain between the Setup() and the Verify() method.
            var verifyMethod = moqExpressionAnalyzer.ExtractChainedMembersInvocationFromLambdaExpression(invocation, cancellationToken);

            if (verifyMethod is null)
            {
                return false;
            }

            // Members comparisons
            if (!verifyMethod.HasSameMembers(setupMethod))
            {
                return false;
            }

            return true;
        }
    }
}
