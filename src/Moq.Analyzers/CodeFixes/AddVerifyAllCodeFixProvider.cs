//-----------------------------------------------------------------------
// <copyright file="AddVerifyAllCodeFixProvider.cs" company="P.O.S Informatique">
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddVerifyAllCodeFixProvider))]
    [Shared]
    public class AddVerifyAllCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(VerifyAllShouldBeCalledAnalyzer.Rule.Id); }
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

            var variableName = SyntaxNodeHelper.GetVariableNameSyntax(node);

            if (variableName is null)
            {
                return;
            }

            var parentMethod = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            if (parentMethod is null)
            {
                // Parent method not found, skip it.
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Add VerifyAll() call at the end of the unit test method.",
                    createChangedDocument: cancellationToken => AddVerifyAllAsync(context.Document, parentMethod, variableName.Identifier.ValueText, cancellationToken),
                    equivalenceKey: "Add VerifyAll() call at the end of the unit test method."),
                diagnostic);

            return;
        }

        private static async Task<Document> AddVerifyAllAsync(Document document, MethodDeclarationSyntax oldUnitTestMethod, string variableName, CancellationToken cancellationToken)
        {
            if (oldUnitTestMethod.Body is null)
            {
                return document;
            }

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (semanticModel is null)
            {
                return document;
            }

            var moqSymbols = MoqSymbols.FromCompilation(semanticModel.Compilation);

            if (moqSymbols is null)
            {
                return document;
            }

            // Find the location where we can insert the mock.
            var index = oldUnitTestMethod.Body.Statements.Count;
            bool insertNewLine = true;

            foreach (var statement in oldUnitTestMethod.Body.Statements.Reverse())
            {
                if (statement is not ExpressionStatementSyntax expressionStatement)
                {
                    continue;
                }

                if (!IsVerifyAll(semanticModel, moqSymbols, expressionStatement, out var otherVariableName, cancellationToken))
                {
                    break;
                }

                insertNewLine = false;

                if (variableName.CompareTo(otherVariableName) >= 0)
                {
                    // The variable name to add is in alphabetic order after the "variable.VerifyAll()",
                    // so will insert the statement here.
                    break;
                }

                // We continue to iterate
                index--;
            }

            var verifyAllCallStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(variableName),
                        SyntaxFactory.IdentifierName("VerifyAll"))));

            var newUnitTestMethodBody = InsertStatement(oldUnitTestMethod.Body, index, verifyAllCallStatement, insertNewLine);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (oldRoot is null)
            {
                return document;
            }

            var newRoot = oldRoot.ReplaceNode(oldUnitTestMethod, oldUnitTestMethod.WithBody(newUnitTestMethodBody));

            return document.WithSyntaxRoot(newRoot);
        }

        private static BlockSyntax InsertStatement(BlockSyntax unitTestMethodBody, int index, StatementSyntax statement, bool insertNewLine)
        {
            var newStatements = new List<StatementSyntax>(unitTestMethodBody.Statements);

            if (index < newStatements.Count)
            {
                var leadingTrivia = newStatements[index].GetLeadingTrivia();

                // Remove the leading trivia of the current node and transfert it to the new statement to insert.
                newStatements[index] = newStatements[index].ReplaceNode(newStatements[index], newStatements[index].WithoutLeadingTrivia());

                statement = statement.WithLeadingTrivia(leadingTrivia);
            }

            if (insertNewLine)
            {
                // Special case, we insert a new line at the end of the unit test (we also retrieve the comment at the end if need).
                // Because we want that the VerifyAll() blocks are detached with a new line.
                // It is useful if developers want to call the fixer for all the solution.
                statement = statement.WithLeadingTrivia(
                    unitTestMethodBody.CloseBraceToken.LeadingTrivia.Add(SyntaxFactory.CarriageReturnLineFeed));

                unitTestMethodBody = unitTestMethodBody.WithCloseBraceToken(unitTestMethodBody.CloseBraceToken.WithLeadingTrivia());
            }

            newStatements.Insert(index, statement);

            return unitTestMethodBody.WithStatements(SyntaxFactory.List(newStatements));
        }

        private static bool IsVerifyAll(SemanticModel semanticModel, MoqSymbols moqSymbols, ExpressionStatementSyntax statement, out string? variableName, CancellationToken cancellationToken)
        {
            var expressionSymbol = semanticModel.GetSymbolInfo(statement.Expression, cancellationToken);

            if (!moqSymbols.IsVerifyAllMethod(expressionSymbol.Symbol))
            {
                variableName = null;
                return false;
            }

            if (statement.Expression is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                variableName = null;
                return false;
            }

            if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                variableName = null;
                return false;
            }

            if (memberAccessExpressionSyntax.Expression is not IdentifierNameSyntax identifierNameSyntax)
            {
                variableName = null;
                return false;
            }

            variableName = identifierNameSyntax.Identifier.ValueText;
            return true;
        }
    }
}
