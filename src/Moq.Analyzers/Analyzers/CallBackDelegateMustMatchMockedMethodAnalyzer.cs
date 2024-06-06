//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateMustMatchMockedMethodAnalyzer.cs" company="P.O.S Informatique">
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
    public class CallBackDelegateMustMatchMockedMethodAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2003",
            "The Callback() delegate expression must match the signature of the mocked method",
            "The Callback() delegate expression must match the signature of the mocked method",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Callback() delegate expression must match the signature of the mocked method.");

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

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(context.SemanticModel);

            // Try to determine if the invocation expression is a Callback() expression.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (!moqSymbols.IsCallback(methodSymbol.Symbol))
            {
                return;
            }

            // If yes, we extract the lambda expression of it.
            var callBackLambdaExpressionSymbol = moqExpressionAnalyzer.ExtractCallBackLambdaExpressionMethod(invocationExpression, out var lambdaExpression, context.CancellationToken);

            if (callBackLambdaExpressionSymbol is null)
            {
                return;
            }

            // Check each CallBack() method for the following calls.
            var followingMethods = invocationExpression.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var followingMethod in followingMethods)
            {
                // Find the symbol of the mocked method (if not symbol found, it is mean we Setup() method that not currently compile)
                // so we skip the analysis.
                var mockedMethod = moqExpressionAnalyzer.ExtractSetupMethod(followingMethod, out var _, context.CancellationToken);

                if (mockedMethod is null)
                {
                    continue;
                }

                // Compare the parameters between the mocked method and lambda expression in the CallBack() method.
                // 1- Compare the number of the parameters
                if (callBackLambdaExpressionSymbol.Parameters.Length != mockedMethod.Parameters.Length)
                {
                    var diagnostic = Diagnostic.Create(Rule, lambdaExpression!.ParameterList.GetLocation());
                    context.ReportDiagnostic(diagnostic);

                    continue;
                }

                // 2- Iterate for each parameter
                for (var i = 0; i < callBackLambdaExpressionSymbol.Parameters.Length; i++)
                {
                    // Special case, if the argument is IsAnyType
                    if (moqSymbols.IsAnyType(mockedMethod.Parameters[i].Type))
                    {
                        // The callback parameter associated must be an object.
                        if (callBackLambdaExpressionSymbol.Parameters[i].Type.SpecialType != SpecialType.System_Object)
                        {
                            var diagnostic = Diagnostic.Create(Rule, lambdaExpression!.ParameterList.Parameters[i].GetLocation());
                            context.ReportDiagnostic(diagnostic);

                            continue;
                        }
                    }
                    else if (!SymbolEqualityComparer.Default.Equals(callBackLambdaExpressionSymbol.Parameters[i].Type, mockedMethod.Parameters[i].Type))
                    {
                        var diagnostic = Diagnostic.Create(Rule, lambdaExpression!.ParameterList.Parameters[i].GetLocation());
                        context.ReportDiagnostic(diagnostic);

                        continue;
                    }
                }
            }
        }
    }
}
