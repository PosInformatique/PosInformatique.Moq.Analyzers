//-----------------------------------------------------------------------
// <copyright file="VerifyStaticMethodsRequiresMockParametersAnalyzer.cs" company="P.O.S Informatique">
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
    using Microsoft.CodeAnalysis.Text;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VerifyStaticMethodsRequiresMockParametersAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq1008",
            "The Mock.Verify() and Mock.VerifyAll() methods must specify at least one mock",
            "The {0} method must be called with at least one mock instance",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Calling Mock.Verify() or Mock.VerifyAll() without specifying any mocks no verification is performed and should be avoided. Always provide at least one mock instance.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq1008.html");

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

            // Check is Mock.VerifyAll() or Mock.Verify() method.
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken);

            if (!moqSymbols.IsVerifyAllStaticMethod(methodSymbol.Symbol) && !moqSymbols.IsVerifyStaticMethod(methodSymbol.Symbol))
            {
                return;
            }

            // Check if the Mock.VerifyAll() or Mock.Verify() methods contains at least one argument.
            if (invocationExpression.ArgumentList.Arguments.Count == 0)
            {
                // The Mock.VerifyAll() or Mock.Verify() contains no arguments.
                var methodInvocation = ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name;
                var methodLocation = methodInvocation.GetLocation();

                var location = Location.Create(
                    context.Node.SyntaxTree,
                    new TextSpan(methodLocation.SourceSpan.Start, invocationExpression.ArgumentList.GetLocation().SourceSpan.End - methodLocation.SourceSpan.Start));

                context.ReportDiagnostic(Rule, location, methodInvocation.Identifier.Text);

                return;
            }
        }
    }
}
