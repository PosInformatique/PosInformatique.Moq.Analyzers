//-----------------------------------------------------------------------
// <copyright file="SetupProtectedMustBeUsedWithProtectedOrInternalMembersAnalyzer.cs" company="P.O.S Informatique">
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
    public class SetupProtectedMustBeUsedWithProtectedOrInternalMembersAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2006",
            "The Protected().Setup() method must be use with overridable protected or internal methods",
            "The Protected().Setup() method must be use with overridable protected or internal methods",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Protected().Setup() method must be use with overridable protected or internal methods.");

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

            // Check is Protected() method.
            if (!moqExpressionAnalyzer.IsMockSetupMethodProtected(invocationExpression, out var localVariableExpression, context.CancellationToken))
            {
                return;
            }

            // Gets the first argument to retrieve the name of the method.
            if (invocationExpression.ArgumentList is null)
            {
                return;
            }

            if (invocationExpression.ArgumentList.Arguments.Count == 0)
            {
                return;
            }

            if (invocationExpression.ArgumentList.Arguments[0].Expression is not LiteralExpressionSyntax literalExpression)
            {
                return;
            }

            var methodName = literalExpression.Token.ValueText;

            // Gets the mocked type
            var mockedType = moqExpressionAnalyzer.GetMockedType(localVariableExpression!, context.CancellationToken);

            if (mockedType is null)
            {
                return;
            }

            // Check if a method exists with the specified name
            foreach (var method in mockedType.GetMembers(methodName).OfType<IMethodSymbol>())
            {
                if (!method.IsAbstract && !method.IsVirtual && !method.IsOverride)
                {
                    continue;
                }

                if (method.IsSealed)
                {
                    continue;
                }

                if (method.DeclaredAccessibility == Accessibility.Protected)
                {
                    return;
                }

                if (method.DeclaredAccessibility == Accessibility.Internal)
                {
                    return;
                }

                if (method.DeclaredAccessibility == Accessibility.ProtectedOrInternal)
                {
                    return;
                }
            }

            // No returns method has been specified with Strict mode. Report the diagnostic issue.
            var diagnostic = Diagnostic.Create(Rule, literalExpression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
