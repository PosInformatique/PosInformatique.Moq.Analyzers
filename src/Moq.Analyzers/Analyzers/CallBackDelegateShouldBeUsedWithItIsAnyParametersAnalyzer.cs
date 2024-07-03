//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer.cs" company="P.O.S Informatique">
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
    public class CallBackDelegateShouldBeUsedWithItIsAnyParametersAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq1003",
            "The Callback() method should be used to check the parameters when mocking a method with It.IsAny<T>() arguments",
            "The Callback() method should be used to check the '{0}' parameter when setup the method with It.IsAny<T>()",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The Callback() method should be used to check the parameters when mocking a method with a It.IsAny<T>() arguments.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Design/PosInfoMoq1003.html");

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

            // Check it is a Setup() method.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (!moqSymbols.IsSetupMethod(methodSymbol.Symbol))
            {
                return;
            }

            // Extracts the setup method.
            var setupMethod = moqExpressionAnalyzer.ExtractSetupMethod(invocationExpression, context.CancellationToken);

            if (setupMethod is null)
            {
                return;
            }

            // Check each parameter if it is an It.Any<T>() parameter.
            var itIsAnyArguments = new List<ChainInvocationArgument>();

            foreach (var argument in setupMethod.InvocationArguments)
            {
                if (moqSymbols.IsItIsAny(argument.Symbol))
                {
                    // The Callback() method is required for the argument, add in the list.
                    itIsAnyArguments.Add(argument);
                }
            }

            if (itIsAnyArguments.Any())
            {
                // Retrieve the Callback() method in the invocation expression
                var followingMethods = invocationExpression.Ancestors().OfType<InvocationExpressionSyntax>();

                foreach (var followingMethod in followingMethods)
                {
                    var callbackMethod = moqExpressionAnalyzer.ExtractCallBackLambdaExpressionMethod(followingMethod, out var lambdaExpression, context.CancellationToken);

                    if (callbackMethod is not null)
                    {
                        // Callback() method exists and parameters has been verified, exit the analysis.
                        return;
                    }
                }

                // Report the warning PosInfo1003 for missing Callback() method.
                // Do it for each It.IsAny<T>() arguments.
                foreach (var itIsAnyArgument in itIsAnyArguments)
                {
                    context.ReportDiagnostic(Rule, itIsAnyArgument.Syntax.GetLocation(), itIsAnyArgument.ParameterSymbol.Name);
                }
            }
        }
    }
}
