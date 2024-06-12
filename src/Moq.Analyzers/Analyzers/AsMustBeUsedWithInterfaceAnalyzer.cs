﻿//-----------------------------------------------------------------------
// <copyright file="AsMustBeUsedWithInterfaceAnalyzer.cs" company="P.O.S Informatique">
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
    public class AsMustBeUsedWithInterfaceAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2007",
            "The As<T>() method can be used only with interfaces",
            "The As<T>() method can be used only with interfaces",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The As<T>() method can be used only with interfaces.");

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

            var asMethodType = moqExpressionAnalyzer.ExtractAsMethodType(moqSymbols, invocationExpression, out var typeSyntax, context.CancellationToken);

            if (asMethodType is null)
            {
                return;
            }

            if (asMethodType.TypeKind == TypeKind.Interface)
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, typeSyntax!.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
