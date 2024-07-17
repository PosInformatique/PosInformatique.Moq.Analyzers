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
        private static readonly DiagnosticDescriptor ConstructorArgumentsMustMatchMockedClassRule = new DiagnosticDescriptor(
            "PosInfoMoq2005",
            "Constructor arguments must match the constructors of the mocked class",
            "Constructor arguments must match the constructors of the mocked class",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Constructor arguments must match the constructors of the mocked class.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2005.html");

        private static readonly DiagnosticDescriptor ConstructorMockedClassMustBeAccessibleRule = new DiagnosticDescriptor(
            "PosInfoMoq2011",
            "Constructor of the mocked class must be accessible",
            "Constructor of the mocked class must be accessible",
            "Compilation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Constructor of the mocked class must be accessible.",
            helpLinkUri: "https://posinformatique.github.io/PosInformatique.Moq.Analyzers/docs/Compilation/PosInfoMoq2011.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ConstructorArgumentsMustMatchMockedClassRule, ConstructorMockedClassMustBeAccessibleRule);

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
            var mockedType = moqExpressionAnalyzer.GetMockedType(objectCreation, out var _, context.CancellationToken);
            if (mockedType is null)
            {
                return;
            }

            // Check the type is mockable
            if (!moqSymbols.IsMockable(mockedType))
            {
                return;
            }

            // Check the type is a class (other type are ignored)
            if (mockedType.TypeKind != TypeKind.Class)
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
                if (moqExpressionAnalyzer.IsStrictBehaviorArgument(firstArgument, out var _, context.CancellationToken))
                {
                    constructorArguments.RemoveAt(0);
                }
            }

            var matchedConstructor = default(MatchedConstructor);

            // Iterate on each constructor and check if the arguments match.
            foreach (var constructor in namedTypeSymbol.Constructors)
            {
                matchedConstructor.Try(constructor);

                // If the number of arguments is different, check the next constructor definition.
                if (constructor.Parameters.Length != constructorArguments.Count)
                {
                    matchedConstructor.Cancel();
                    continue;
                }

                for (var i = 0; i < constructorArguments.Count; i++)
                {
                    if (constructorArguments[i].Expression.IsKind(SyntaxKind.NullLiteralExpression))
                    {
                        // Null parameter, just check the parameter type is a reference type.
                        if (!constructor.Parameters[i].Type.IsReferenceType)
                        {
                            matchedConstructor.Cancel();
                            break;
                        }

                        continue;
                    }

                    if (constructorArguments[i].Expression.IsKind(SyntaxKind.DefaultLiteralExpression))
                    {
                        // Default parameter, skip the parameter.
                        continue;
                    }

                    var constructorArgumentSymbol = context.SemanticModel.GetTypeInfo(constructorArguments[i].Expression, context.CancellationToken);

                    if (!constructorArgumentSymbol.Type.IsOrInheritFrom(constructor.Parameters[i].Type))
                    {
                        matchedConstructor.Cancel();
                        break;
                    }
                }

                if (matchedConstructor.IsMatched)
                {
                    break;
                }
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

            if (!matchedConstructor.IsMatched)
            {
                context.ReportDiagnostic(ConstructorArgumentsMustMatchMockedClassRule, location);
                return;
            }

            if (matchedConstructor.Constructor is not null && matchedConstructor.Constructor.DeclaredAccessibility == Accessibility.Private)
            {
                context.ReportDiagnostic(ConstructorMockedClassMustBeAccessibleRule, location);
            }
        }

        private struct MatchedConstructor
        {
            public MatchedConstructor()
            {
                this.IsMatched = true;
            }

            public bool IsMatched { get; private set; }

            public IMethodSymbol? Constructor { get; private set; }

            public void Cancel()
            {
                this.IsMatched = false;
                this.Constructor = null;
            }

            public void Try(IMethodSymbol constructor)
            {
                this.IsMatched = true;
                this.Constructor = constructor;
            }
        }
    }
}
