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
        private static readonly DiagnosticDescriptor SetupMustBeOnOverridableMethods = new DiagnosticDescriptor(
            "PosInfoMoq2006",
            "The Protected().Setup() method must be use with overridable protected or internal methods",
            "The Protected().Setup() method must be use with overridable protected or internal methods",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Protected().Setup() method must be use with overridable protected or internal methods.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2006.html");

        private static readonly DiagnosticDescriptor SetupReturnTypeMustMatch = new DiagnosticDescriptor(
            "PosInfoMoq2015",
            "The Protected().Setup() method must match the return type of the mocked method",
            "The Protected().Setup() method must match the return type of the mocked method",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Protected().Setup() method must match the return type of the mocked method.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2015.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(SetupMustBeOnOverridableMethods, SetupReturnTypeMustMatch);

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
            IMethodSymbol? methodMatch = null;

            foreach (var method in mockedType.GetAllMembers(methodName).OfType<IMethodSymbol>())
            {
                if (!method.IsAbstract && !method.IsVirtual && !method.IsOverride)
                {
                    continue;
                }

                if (method.IsSealed)
                {
                    break;
                }

                if (method.DeclaredAccessibility == Accessibility.Protected)
                {
                    methodMatch = method;
                    break;
                }

                if (method.DeclaredAccessibility == Accessibility.Internal)
                {
                    methodMatch = method;
                    break;
                }

                if (method.DeclaredAccessibility == Accessibility.ProtectedOrInternal)
                {
                    methodMatch = method;
                    break;
                }
            }

            if (methodMatch is null)
            {
                // No method match, raise an error.
                context.ReportDiagnostic(SetupMustBeOnOverridableMethods, literalExpression.GetLocation());
                return;
            }

            // Else check the argument type of the Setup<T>() method and the method found.
            var setupMethodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (setupMethodSymbol.Symbol is not IMethodSymbol setupMethod)
            {
                return;
            }

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return;
            }

            if (methodMatch.ReturnsVoid)
            {
                if (setupMethod.TypeArguments.Length > 0)
                {
                    if (memberAccessExpressionSyntax.Name is not GenericNameSyntax genericNameSyntax)
                    {
                        return;
                    }

                    // The method mocked is void and no generic arguments has been specified in the Setup<T>() method.
                    context.ReportDiagnostic(SetupReturnTypeMustMatch, genericNameSyntax.TypeArgumentList.Arguments[0].GetLocation());
                }

                return;
            }

            if (setupMethod.TypeArguments.Length != 1)
            {
                // No generic type has been specified in the Setup<T>().
                context.ReportDiagnostic(SetupReturnTypeMustMatch, memberAccessExpressionSyntax.Name.GetLocation());
                return;
            }

            if (!SymbolEqualityComparer.Default.Equals(setupMethod.TypeArguments[0], methodMatch.ReturnType))
            {
                if (memberAccessExpressionSyntax.Name is not GenericNameSyntax genericNameSyntax)
                {
                    return;
                }

                // The method mocked return a type which does not match the argument type of the Setup<T>() method.
                context.ReportDiagnostic(SetupReturnTypeMustMatch, genericNameSyntax.TypeArgumentList.Arguments[0].GetLocation());

                return;
            }
        }
    }
}
