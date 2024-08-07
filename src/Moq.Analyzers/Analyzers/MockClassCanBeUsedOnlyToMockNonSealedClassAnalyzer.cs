﻿//-----------------------------------------------------------------------
// <copyright file="MockClassCanBeUsedOnlyToMockNonSealedClassAnalyzer.cs" company="P.O.S Informatique">
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
    public class MockClassCanBeUsedOnlyToMockNonSealedClassAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2002",
            "Mock<T> class can be used only to mock non-sealed class",
            "Mock<T> class can be used only to mock non-sealed class",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Mock<T> class can be used only to mock non-sealed class.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2002.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            var moqSymbols = MoqSymbols.FromCompilation(context.Compilation);

            if (moqSymbols is null)
            {
                return;
            }

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            // Check the expression is a Mock<T> instance creation.
            var mockedType = moqExpressionAnalyzer.GetMockedType(objectCreationExpression, out var typeExpression, context.CancellationToken);

            if (mockedType is null)
            {
                return;
            }

            if (moqSymbols.IsMockable(mockedType))
            {
                return;
            }

            // No returns method has been specified with Strict mode. Report the diagnostic issue.
            var diagnostic = Diagnostic.Create(Rule, typeExpression!.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
