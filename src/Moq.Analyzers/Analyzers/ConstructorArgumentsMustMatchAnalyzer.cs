//-----------------------------------------------------------------------
// <copyright file="ConstructorArgumentsMustMatchAnalyzer.cs" company="P.O.S Informatique">
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
    public class ConstructorArgumentsMustMatchAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "PosInfoMoq2005",
            "Constructor arguments must match the constructors of the mocked class",
            "Constructor arguments must match the constructors of the mocked class",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Constructor arguments must match the constructors of the mocked class.");

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

            var moqExpressionAnalyzer = new MoqExpressionAnalyzer(context.SemanticModel);

            // Check there is "new Mock<I>()" statement.
            var mockedType = moqExpressionAnalyzer.GetMockedType(moqSymbols, objectCreation, out var _, context.CancellationToken);
            if (mockedType is null)
            {
                return;
            }

            // Check the type is mockable
            if (!moqSymbols.IsMockable(mockedType))
            {
                return;
            }

            // Check the type is a named type
            if (mockedType is not INamedTypeSymbol namedTypeSymbol)
            {
                return;
            }

            // Gets the list of the constructor arguments
            var constructorArguments = new List<ArgumentSyntax>();

            if (objectCreation.ArgumentList is not null)
            {
                constructorArguments.AddRange(objectCreation.ArgumentList.Arguments);
            }

            // Gets the first argument, check if it is MockBehavior argument and skip it.
            var firstArgument = constructorArguments.FirstOrDefault();

            if (firstArgument is not null)
            {
                if (moqExpressionAnalyzer.IsStrictBehaviorArgument(moqSymbols, firstArgument, out var _, context.CancellationToken))
                {
                    constructorArguments.RemoveAt(0);
                }
            }

            var matchedConstructor = true;

            // Iterate on each constructor and check if the arguments match.
            foreach (var constructor in namedTypeSymbol.Constructors)
            {
                matchedConstructor = true;

                // If the number of arguments is different, check the next constructor definition.
                if (constructor.Parameters.Length != constructorArguments.Count)
                {
                    matchedConstructor = false;
                    continue;
                }

                for (var i = 0; i < constructorArguments.Count; i++)
                {
                    var constructorArgumentSymbol = context.SemanticModel.GetTypeInfo(constructorArguments[i].Expression, context.CancellationToken);

                    if (!SymbolEqualityComparer.Default.Equals(constructorArgumentSymbol.Type, constructor.Parameters[i].Type))
                    {
                        matchedConstructor = false;
                        break;
                    }
                }

                if (matchedConstructor)
                {
                    break;
                }
            }

            if (matchedConstructor)
            {
                return;
            }

            Location location;

            if (constructorArguments.Count == 0)
            {
                location = objectCreation.ArgumentList!.GetLocation();
            }
            else
            {
                var firstLocation = constructorArguments.First().GetLocation();
                var lastLocation = constructorArguments.Last().GetLocation();

                location = Location.Create(context.Node.SyntaxTree, new TextSpan(firstLocation.SourceSpan.Start, lastLocation.SourceSpan.End - firstLocation.SourceSpan.Start));
            }

            var diagnostic = Diagnostic.Create(Rule, location);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
