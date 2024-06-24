//-----------------------------------------------------------------------
// <copyright file="ConstructorArgumentCannotBePassedForInterfaceAnalyzer.cs" company="P.O.S Informatique">
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
    public class ConstructorArgumentCannotBePassedForInterfaceAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2004",
            "Constructor arguments cannot be passed for interface mocks",
            "Constructor arguments cannot be passed for interface mocks",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Constructor arguments cannot be passed for interface mocks.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2004.html");

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

            // Check there is "new Mock<I>()" statement.
            var mockedType = moqExpressionAnalyzer.GetMockedType(objectCreation, out var typeExpression, context.CancellationToken);
            if (mockedType is null)
            {
                return;
            }

            // Check the mocked type is an interface
            if (mockedType.TypeKind != TypeKind.Interface)
            {
                return;
            }

            // Check that the "new Mock<I>()" statement have at least one argument (else skip analysis).
            if (objectCreation.ArgumentList is null)
            {
                return;
            }

            // Gets the first argument (skip analysis if no argument)
            var firstArgument = objectCreation.ArgumentList.Arguments.FirstOrDefault();

            if (firstArgument is null)
            {
                return;
            }

            // Check if the first argument is MockBehavior argument
            var argumentCheckStart = 0;

            if (moqExpressionAnalyzer.IsStrictBehaviorArgument(firstArgument, out var _, context.CancellationToken))
            {
                argumentCheckStart = 1;
            }

            if (objectCreation.ArgumentList.Arguments.Count > argumentCheckStart)
            {
                var locations = objectCreation.ArgumentList.Arguments
                    .Skip(argumentCheckStart)
                    .Select(a => a.Expression.GetLocation())
                    .ToArray();

                var diagnostic = Diagnostic.Create(Rule, locations[0], locations.Skip(1));
                context.ReportDiagnostic(diagnostic);

                return;
            }
        }
    }
}
