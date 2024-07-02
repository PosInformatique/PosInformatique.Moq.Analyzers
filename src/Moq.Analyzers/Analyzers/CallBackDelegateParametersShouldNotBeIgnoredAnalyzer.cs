//-----------------------------------------------------------------------
// <copyright file="CallBackDelegateParametersShouldNotBeIgnoredAnalyzer.cs" company="P.O.S Informatique">
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
    public class CallBackDelegateParametersShouldNotBeIgnoredAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq1004",
            "The Callback() parameter should not be ignored if it has been setup as an It.IsAny<T>() argument",
            "The '{0}' parameter should not be ignored if it has been setup as an It.IsAny<T>() argument",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The Callback() parameter should not be ignored if it has been setup as an It.IsAny<T>() argument.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Design/PosInfoMoq1004.html");

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

            // Extracts the Setup() linked method.
            var setupMethod = moqExpressionAnalyzer.ExtractSetupMethod(invocationExpression, context.CancellationToken);

            if (setupMethod is null)
            {
                return;
            }

            // Check there is It.Any<T>() parameters.
            var itIsAnyArguments = new Dictionary<int, ChainInvocationArgument>();

            for (var i = 0; i < setupMethod.InvocationArguments.Count; i++)
            {
                var argument = setupMethod.InvocationArguments[i];

                if (moqSymbols.IsItIsAny(argument.Symbol))
                {
                    // The Callback() method is required for the argument, add in the list.
                    itIsAnyArguments.Add(i, argument);
                }
            }

            if (itIsAnyArguments.Any())
            {
                // Here, it is mean Setup() method has been defined with some It.IsAny<T>() parameters.
                // Extracts the Callback() method (if exists).
                var callbackMethod = moqExpressionAnalyzer.ExtractCallBackLambdaExpressionMethod(invocationExpression, out var lambdaExpression, context.CancellationToken);

                if (callbackMethod is null)
                {
                    return;
                }

                // Check each parameter of the Callback() method.
                for (var i = 0; i < callbackMethod.Parameters.Length; i++)
                {
                    if (!itIsAnyArguments.TryGetValue(i, out var itIsAnyArgument))
                    {
                        // The parameter in the Callback() method is not related to a It.IsAny<T>() expression.
                        continue;
                    }

                    if (callbackMethod.Parameters[i].Name == "_")
                    {
                        // Raise warning for the parameter which is not used.
                        var parameterName = setupMethod.InvocationArguments[i].ParameterSymbol.Name;

                        context.ReportDiagnostic(Rule, lambdaExpression!.ParameterList.Parameters[i].GetLocation(), parameterName);
                    }
                }
            }
        }
    }
}
