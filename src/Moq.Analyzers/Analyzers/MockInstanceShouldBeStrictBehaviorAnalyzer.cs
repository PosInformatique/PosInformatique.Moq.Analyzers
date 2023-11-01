//-----------------------------------------------------------------------
// <copyright file="MockInstanceShouldBeStrictBehaviorAnalyzer.cs" company="P.O.S Informatique">
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
    public class MockInstanceShouldBeStrictBehaviorAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MQ1001";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "The Mock<T> instance behavior should be defined to Strict mode",
            "The Mock<T> instance behavior should be defined to Strict mode",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The Mock<T> instance behavior should be defined to Strict mode.");

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

            // Check there is "new Mock<I>()" statement.
            if (!MockExpressionHelper.IsMockCreation(objectCreation))
            {
                return;
            }

            // Check that the "new Mock<I>()" statement have at least one argument (else Strict is missing...).
            if (objectCreation.ArgumentList is null)
            {
                var diagnostic = Diagnostic.Create(Rule, objectCreation.GetLocation());
                context.ReportDiagnostic(diagnostic);

                return;
            }

            var firstArgument = objectCreation.ArgumentList.Arguments.FirstOrDefault();

            if (firstArgument is null)
            {
                var diagnostic = Diagnostic.Create(Rule, objectCreation.GetLocation());
                context.ReportDiagnostic(diagnostic);

                return;
            }

            // Gets the first argument of "new Mock<I>(...)" and ensures it is a MemberAccessExpressionSyntax
            // (because we searching for MockBehavior.Strict).
            if (firstArgument.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            {
                var diagnostic = Diagnostic.Create(Rule, objectCreation.GetLocation());
                context.ReportDiagnostic(diagnostic);

                return;
            }

            // Find the "MockBehavior" type in the semantic model from Moq library.
            var mockBehaviorType = context.Compilation.GetTypeByMetadataName("Moq.MockBehavior");

            if (mockBehaviorType is null)
            {
                // Moq not installed (dependency of the Moq package missing), so we stop analysis.
                return;
            }

            // Check that the "memberAccessExpression.Expression" is applied on the Moq MockBehavior type.
            var firstArgumentType = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Expression);

            if (!SymbolEqualityComparer.Default.Equals(firstArgumentType.Symbol, mockBehaviorType))
            {
                var diagnostic = Diagnostic.Create(Rule, firstArgument.GetLocation());
                context.ReportDiagnostic(diagnostic);

                return;
            }

            // Find the Strict field in the semantic model
            var strictField = mockBehaviorType.GetMembers("Strict").SingleOrDefault();

            if (strictField is null)
            {
                // The field Strict seam to not exists (dependency of the Moq package missing ? Or something wrong ?), so we stop analysis.
                return;
            }

            // Check that the memberAccessExpression.Name reference the Strict field
            var firstArgumentField = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Name);

            if (!SymbolEqualityComparer.Default.Equals(firstArgumentField.Symbol, strictField))
            {
                var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.Name.GetLocation());
                context.ReportDiagnostic(diagnostic);

                return;
            }
        }
    }
}
