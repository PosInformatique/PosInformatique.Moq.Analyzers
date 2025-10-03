//-----------------------------------------------------------------------
// <copyright file="RaiseParametersMustMatchEventSignatureAnalyzer.cs" company="P.O.S Informatique">
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
    public class RaiseParametersMustMatchEventSignatureAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2017",
            "The Raise() parameters must match the signature of the mocked event",
            "The Raise() parameters must match the signature of the mocked event. {0}.",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The Raise() parameters must match the signature of the mocked event.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2017.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var moqSymbols = MoqSymbols.FromCompilation(context.Compilation);

            if (moqSymbols is null)
            {
                return;
            }

            // Extract the Raise() method call info (event + parameters)
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, context.SemanticModel);

            var raiseMethod = moqExpressionAnalyzer.ExtractRaiseMethodCall(invocationExpression, context.CancellationToken);

            if (raiseMethod is null)
            {
                return;
            }

            var eventParameters = raiseMethod.EventParameters.ToList();

            // Check the overload of the Raise() method called.
            if (moqSymbols.IsEventArgs(raiseMethod.Method.Parameters[1].Type))
            {
                // Case calling the overload : Raise(x => x.Event, EventArgs)
                // We don't check in this case the "object sender" parameter
                // of the event, because Raise(x => x.Event, EventArgs)
                // don't propose to pass this parameter.
                eventParameters.RemoveAt(0);
            }

            // Check the parameters count match
            if (raiseMethod.MethodParameters.Count != eventParameters.Count)
            {
                var raiseMethodSyntax = GetRaiseMethodSyntax(invocationExpression);

                if (raiseMethodSyntax is null)
                {
                    return;
                }

                context.ReportDiagnostic(Rule, raiseMethodSyntax.GetLocation(), $"The event '{raiseMethod.Event.Name}' expects {raiseMethod.EventParameters.Count} argument(s) but {raiseMethod.MethodParameters.Count} were provided.");
                return;
            }

            // Check the arguments of the Raise() method match the parameters of the event.
            for (var i = 0; i < raiseMethod.MethodParameters.Count; i++)
            {
                var methodParameter = raiseMethod.MethodParameters[i];
                var eventParameter = eventParameters[i];

                if (methodParameter is null)
                {
                    // Check the event parameter is nullable.
                    if (!eventParameter.Type.IsReferenceType)
                    {
                        context.ReportDiagnostic(Rule, raiseMethod.MethodArguments[i].GetLocation(), $"The parameter '{eventParameter.Name}' of the event '{raiseMethod.Event.Name}' expects a value of type '{eventParameter.Type.Name}' but a value 'null' was provided.");
                    }

                    continue;
                }

                if (!methodParameter.IsOrInheritFrom(eventParameter.Type))
                {
                    context.ReportDiagnostic(Rule, raiseMethod.MethodArguments[i].GetLocation(), $"The parameter '{eventParameter.Name}' of the event '{raiseMethod.Event.Name}' expects a value of type '{eventParameter.Type.Name}' but a value of type '{methodParameter.Name}' was provided.");
                    continue;
                }
            }
        }

        private static SyntaxNode? GetRaiseMethodSyntax(InvocationExpressionSyntax expressionSyntax)
        {
            if (expressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return null;
            }

            return memberAccessExpressionSyntax.Name;
        }
    }
}
