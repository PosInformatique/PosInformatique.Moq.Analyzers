//-----------------------------------------------------------------------
// <copyright file="SetGenericArgumentSetupSetCodeFixProvider.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SetGenericArgumentSetupSetCodeFixProvider))]
    [Shared]
    public class SetGenericArgumentSetupSetCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(SetupSetAnalyzer.UseSetupSetWithGenericArgumentRule.Id); }
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

            // Gets the syntax node where is located the issue in the code.
            var node = root.FindNode(diagnosticSpan, getInnermostNodeForTie: true);

            if (node is not IdentifierNameSyntax identifierNameSyntax)
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Set the generic argument with the mocked property type to the SetupSet() method.",
                    createChangedDocument: cancellationToken => AddMockBehiavorStrictArgumentAsync(context.Document, identifierNameSyntax, cancellationToken),
                    equivalenceKey: "Set the generic argument with the mocked property type to the SetupSet() method."),
                diagnostic);

            return;
        }

        private static async Task<Document> AddMockBehiavorStrictArgumentAsync(Document document, IdentifierNameSyntax oldIdentifierNameSyntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync();

            if (semanticModel is null)
            {
                return document;
            }

            var moqSymbols = MoqSymbols.FromCompilation(semanticModel.Compilation);

            if (moqSymbols is null)
            {
                return document;
            }

            // Retrieve the invocation expression
            if (oldIdentifierNameSyntax.Parent is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return document;
            }

            if (memberAccessExpressionSyntax.Parent is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return document;
            }

            // Gets the chained members from the lambda expression (to determine the type of the last property in the members chain).
            var expressionAnalyzer = new MoqExpressionAnalyzer(moqSymbols, semanticModel);

            var chainedMembers = expressionAnalyzer.ExtractChainedMembersInvocationFromLambdaExpression(invocationExpressionSyntax, cancellationToken);

            if (chainedMembers is null)
            {
                return document;
            }

            // Update the IdentifierNameSyntax with a GenericName (which contains the type of the property).
            var propertyType = SyntaxFactory.ParseTypeName(chainedMembers.ReturnType.ToDisplayString());

            var newIdentifierNameSyntax = SyntaxFactory.GenericName(
                oldIdentifierNameSyntax.Identifier,
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SingletonSeparatedList(propertyType)));

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            if (oldRoot is null)
            {
                return document;
            }

            var newRoot = oldRoot.ReplaceNode(oldIdentifierNameSyntax, newIdentifierNameSyntax);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
