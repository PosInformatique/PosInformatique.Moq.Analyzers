//-----------------------------------------------------------------------
// <copyright file="SetBehaviorToStrictCodeFixProvider.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SetBehaviorToStrictCodeFixProvider))]
    [Shared]
    public class SetBehaviorToStrictCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MockInstanceShouldBeStrictBehaviorAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            if (root is null)
            {
                return;
            }

            // Gets the location where is the issue in the code.
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the "ObjectCreationExpressionSyntax" or "InvocationExpression" in the parent of the location where is located the issue in the code.
            var parent = root.FindToken(diagnosticSpan.Start).Parent;

            if (parent is null)
            {
                return;
            }

            var mockCreationExpression = parent.AncestorsAndSelf().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

            if (mockCreationExpression is null)
            {
                var invocationExpression = parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Defines the MockBehavior to Strict",
                        createChangedDocument: cancellationToken => AddMockBehiavorStrictArgumentAsync(context.Document, invocationExpression, cancellationToken),
                        equivalenceKey: "Defines the MockBehavior to Strict"),
                    diagnostic);
            }
            else
            {
                // Register a code to fix the enumeration.
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Defines the MockBehavior to Strict",
                        createChangedDocument: cancellationToken => AddMockBehiavorStrictArgumentAsync(context.Document, mockCreationExpression, cancellationToken),
                        equivalenceKey: "Defines the MockBehavior to Strict"),
                    diagnostic);
            }
        }

        private static async Task<Document> AddMockBehiavorStrictArgumentAsync(Document document, ObjectCreationExpressionSyntax oldMockCreationExpression, CancellationToken cancellationToken)
        {
            var arguments = new List<ArgumentSyntax>()
            {
                CreateMockBehaviorArgument(),
            };

            if (oldMockCreationExpression.ArgumentList is not null && oldMockCreationExpression.ArgumentList.Arguments.Count > 0)
            {
                var firstArgument = oldMockCreationExpression.ArgumentList.Arguments.First();

                if (IsLambdaExpressionArgument(firstArgument))
                {
                    // The old first argument is lambda expression, so we insert it at the first argument position.
                    arguments.Insert(0, firstArgument);
                }
                else
                {
                    if (IsMockBehaviorArgument(firstArgument))
                    {
                        // The old first argument is MockBehavior.xxxxx, so we take the following arguments
                        // and ignore it.
                        arguments.AddRange(oldMockCreationExpression.ArgumentList.Arguments.Skip(1));
                    }
                    else
                    {
                        // Retrieves all the arguments of the "new Mock<T>(...)" instantiation.
                        arguments.AddRange(oldMockCreationExpression.ArgumentList.Arguments);
                    }
                }
            }

            var newMockCreationExpression = oldMockCreationExpression.WithArgumentList(
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            if (oldRoot is null)
            {
                return document;
            }

            var newRoot = oldRoot.ReplaceNode(oldMockCreationExpression, newMockCreationExpression);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> AddMockBehiavorStrictArgumentAsync(Document document, InvocationExpressionSyntax oldInvocationExpression, CancellationToken cancellationToken)
        {
            var arguments = new List<ArgumentSyntax>();

            if (oldInvocationExpression.ArgumentList is not null && oldInvocationExpression.ArgumentList.Arguments.Count > 0)
            {
                var lastArgument = oldInvocationExpression.ArgumentList.Arguments.Last();

                if (IsMockBehaviorArgument(lastArgument))
                {
                    // The old last argument is MockBehavior.xxxxx, so we skip the last argument.
                    arguments.AddRange(oldInvocationExpression.ArgumentList.Arguments.Take(oldInvocationExpression.ArgumentList.Arguments.Count - 1));
                }
                else
                {
                    // Retrieves all the arguments of the "Mock.Of<T>()" instantiation.
                    arguments.AddRange(oldInvocationExpression.ArgumentList.Arguments);
                }
            }

            arguments.Add(CreateMockBehaviorArgument());

            var newInvocationExpression = oldInvocationExpression.WithArgumentList(
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            if (oldRoot is null)
            {
                return document;
            }

            var newRoot = oldRoot.ReplaceNode(oldInvocationExpression, newInvocationExpression);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ArgumentSyntax CreateMockBehaviorArgument()
        {
            return SyntaxFactory.Argument(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("MockBehavior"),
                    SyntaxFactory.IdentifierName("Strict")));
        }

        private static bool IsMockBehaviorArgument(ArgumentSyntax argument)
        {
            if (argument.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            {
                return false;
            }

            if (memberAccessExpression.Expression is not IdentifierNameSyntax targetExpression)
            {
                return false;
            }

            if (targetExpression.Identifier.ValueText != "MockBehavior")
            {
                return false;
            }

            return true;
        }

        private static bool IsLambdaExpressionArgument(ArgumentSyntax argument)
        {
            if (argument.Expression is not ParenthesizedLambdaExpressionSyntax lambdaExpression)
            {
                return false;
            }

            if (lambdaExpression.ParameterList.Parameters.Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}
