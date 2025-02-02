﻿//-----------------------------------------------------------------------
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
        internal static readonly DiagnosticDescriptor CallbackMustMatchSignature = new DiagnosticDescriptor(
            "PosInfoMoq2003",
            "The Callback() delegate expression must match the signature of the mocked method",
            "The Callback() delegate expression must match the signature of the mocked method",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Callback() delegate expression must match the signature of the mocked method.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2003.html");

        internal static readonly DiagnosticDescriptor CallbackMustNotReturnValue = new DiagnosticDescriptor(
            "PosInfoMoq2014",
            "The Callback() delegate expression must not return a value",
            "The Callback() delegate expression must not return a value",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Callback() delegate expression must not return a value.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2014.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CallbackMustMatchSignature, CallbackMustNotReturnValue);

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

            // Extract the lambda expression.
            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            var callBackLambdaExpressionSymbol = moqExpressionAnalyzer.ExtractCallBackLambdaExpressionMethod(invocationExpression, out var lambdaExpression, context.CancellationToken);

            if (callBackLambdaExpressionSymbol is null)
            {
                return;
            }

            if (!callBackLambdaExpressionSymbol.ReturnsVoid)
            {
                var allReturnsStatements = ReturnStatementSyntaxFinder.GetAllReturnStatements(lambdaExpression!.Block!);

                var allReturnsStatementsLocations = allReturnsStatements.Select(statement => statement.GetLocation());

                context.ReportDiagnostic(CallbackMustNotReturnValue, allReturnsStatementsLocations);
            }

            // Extracts the setup method from the Callback() method call.
            var setupMethod = moqExpressionAnalyzer.ExtractSetupMethod(invocationExpression, context.CancellationToken);

            if (setupMethod is null)
            {
                return;
            }

            // Compare the parameters between the mocked method and lambda expression in the CallBack() method.
            // 1- Compare the number of the parameters
            if (callBackLambdaExpressionSymbol.Parameters.Length != setupMethod.InvocationArguments.Count)
            {
                var diagnostic = Diagnostic.Create(CallbackMustMatchSignature, lambdaExpression!.ParameterList.GetLocation());
                context.ReportDiagnostic(diagnostic);

                return;
            }

            // 2- Iterate for each parameter
            for (var i = 0; i < callBackLambdaExpressionSymbol.Parameters.Length; i++)
            {
                // Special case, if the argument is IsAnyType
                if (moqSymbols.IsAnyType(setupMethod.InvocationArguments[i].ParameterSymbol.Type))
                {
                    // The callback parameter associated must be an object.
                    if (callBackLambdaExpressionSymbol.Parameters[i].Type.SpecialType != SpecialType.System_Object)
                    {
                        var diagnostic = Diagnostic.Create(CallbackMustMatchSignature, lambdaExpression!.ParameterList.Parameters[i].GetLocation());
                        context.ReportDiagnostic(diagnostic);

                        continue;
                    }
                }
                else if (!SymbolEqualityComparer.Default.Equals(callBackLambdaExpressionSymbol.Parameters[i].Type, setupMethod.InvocationArguments[i].ParameterSymbol.Type))
                {
                    var diagnostic = Diagnostic.Create(CallbackMustMatchSignature, lambdaExpression!.ParameterList.Parameters[i].GetLocation());
                    context.ReportDiagnostic(diagnostic);

                    continue;
                }
            }
        }

        private sealed class ReturnStatementSyntaxFinder : CSharpSyntaxWalker
        {
            private readonly List<ReturnStatementSyntax> returnStatements;

            private ReturnStatementSyntaxFinder()
            {
                this.returnStatements = new List<ReturnStatementSyntax>();
            }

            public static IList<ReturnStatementSyntax> GetAllReturnStatements(BlockSyntax blockSyntax)
            {
                var finder = new ReturnStatementSyntaxFinder();

                finder.Visit(blockSyntax);

                return finder.returnStatements;
            }

            public override void VisitReturnStatement(ReturnStatementSyntax node)
            {
                this.returnStatements.Add(node);

                base.VisitReturnStatement(node);
            }
        }
    }
}
