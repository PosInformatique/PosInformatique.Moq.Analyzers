//-----------------------------------------------------------------------
// <copyright file="MoqExpressionAnalyzer.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal class MoqExpressionAnalyzer
    {
        private readonly SemanticModel semanticModel;

        public MoqExpressionAnalyzer(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public bool IsMockCreation(MoqSymbols moqSymbols, ObjectCreationExpressionSyntax expression)
        {
            if (this.GetMockedType(moqSymbols, expression, out var _) is null)
            {
                return false;
            }

            return true;
        }

        public ITypeSymbol? GetMockedType(MoqSymbols moqSymbols, ObjectCreationExpressionSyntax expression, out TypeSyntax? typeExpression)
        {
            typeExpression = null;

            var symbolInfo = this.semanticModel.GetSymbolInfo(expression.Type);

            if (symbolInfo.Symbol is not INamedTypeSymbol typeSymbol)
            {
                return null;
            }

            if (!moqSymbols.IsMock(typeSymbol))
            {
                return null;
            }

            if (typeSymbol.TypeArguments.Length != 1)
            {
                return null;
            }

            typeExpression = ((GenericNameSyntax)expression.Type).TypeArgumentList.Arguments[0];

            return typeSymbol.TypeArguments[0];
        }

        public bool IsMockSetupMethod(MoqSymbols moqSymbols, InvocationExpressionSyntax invocationExpression, out IdentifierNameSyntax? localVariableExpression)
        {
            localVariableExpression = null;

            // Gets the member access expression "mock.XXXXX"
            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            {
                return false;
            }

            if (memberAccessExpression.Expression is not IdentifierNameSyntax identifierName)
            {
                return false;
            }

            localVariableExpression = identifierName;

            var instanceVariable = this.semanticModel.GetSymbolInfo(memberAccessExpression.Expression);

            if (instanceVariable.Symbol is not ILocalSymbol instanceVariableSymbol)
            {
                return false;
            }

            if (!moqSymbols.IsMock(instanceVariableSymbol.Type))
            {
                return false;
            }

            // Gets the method and check it is Setup() method.
            var methodSymbolInfo = this.semanticModel.GetSymbolInfo(invocationExpression.Expression);

            if (!moqSymbols.IsSetupMethod(methodSymbolInfo.Symbol))
            {
                return false;
            }

            return true;
        }

        public bool IsStrictBehavior(MoqSymbols moqSymbols, ObjectCreationExpressionSyntax mockCreationExpression)
        {
            // Check that the "new Mock<I>()" statement have at least one argument (else Strict is missing...).
            if (mockCreationExpression.ArgumentList is null)
            {
                return false;
            }

            var firstArgument = mockCreationExpression.ArgumentList.Arguments.FirstOrDefault();

            if (firstArgument is null)
            {
                return false;
            }

            // Gets the first argument of "new Mock<I>(...)" and ensures it is a MemberAccessExpressionSyntax
            // (because we searching for MockBehavior.Strict).
            if (firstArgument.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            {
                return false;
            }

            // Check that the "memberAccessExpression.Expression" is applied on the Moq MockBehavior type.
            var firstArgumentType = this.semanticModel.GetSymbolInfo(memberAccessExpression.Expression);

            if (!moqSymbols.IsMockBehaviorEnum(firstArgumentType.Symbol))
            {
                return false;
            }

            // Check that the memberAccessExpression.Name reference the Strict field
            var firstArgumentField = this.semanticModel.GetSymbolInfo(memberAccessExpression.Name);

            if (!moqSymbols.IsMockBehaviorStrictField(firstArgumentField.Symbol))
            {
                return false;
            }

            return true;
        }

        public bool IsStrictBehavior(MoqSymbols moqSymbols, IdentifierNameSyntax localVariableExpression)
        {
            // Go back to the parents nodes and iterate all the statements in the parents blocks to find
            // the Mock instantiation (new Mock<I>(...) to determines the mock behavior.
            foreach (var block in localVariableExpression.Ancestors().OfType<BlockSyntax>())
            {
                var mockCreation = FindMockCreation(block, localVariableExpression.Identifier.ValueText);

                if (mockCreation is not null)
                {
                    if (this.IsStrictBehavior(moqSymbols, mockCreation))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public ITypeSymbol? GetSetupMethodReturnSymbol(MoqSymbols moqSymbols, InvocationExpressionSyntax setupInvocationExpression)
        {
            if (setupInvocationExpression.ArgumentList is null)
            {
                return null;
            }

            if (setupInvocationExpression.ArgumentList.Arguments.Count != 1)
            {
                return null;
            }

            if (setupInvocationExpression.ArgumentList.Arguments[0].Expression is not SimpleLambdaExpressionSyntax lambdaExpression)
            {
                return null;
            }

            if (lambdaExpression.Body is not InvocationExpressionSyntax methodExpression)
            {
                return null;
            }

            var methodSymbolInfo = this.semanticModel.GetSymbolInfo(methodExpression);

            if (methodSymbolInfo.Symbol is not IMethodSymbol methodSymbol)
            {
                return null;
            }

            return methodSymbol.ReturnType;
        }

        public ISymbol? ExtractSetupMember(InvocationExpressionSyntax invocationExpression, out NameSyntax? memberIdentifierName)
        {
            memberIdentifierName = null;

            if (invocationExpression.ArgumentList.Arguments.Count != 1)
            {
                return null;
            }

            if (invocationExpression.ArgumentList.Arguments[0].Expression is not SimpleLambdaExpressionSyntax lambdaExpression)
            {
                return null;
            }

            ExpressionSyntax bodyExpression;

            if (lambdaExpression.Body is InvocationExpressionSyntax invocationMemberExpression)
            {
                // It is a method in the Setup() method.
                bodyExpression = invocationMemberExpression.Expression;
            }
            else
            {
                // It is a property in the Setup() method.
                if (lambdaExpression.ExpressionBody is null)
                {
                    return null;
                }

                bodyExpression = lambdaExpression.ExpressionBody;
            }

            if (bodyExpression is not MemberAccessExpressionSyntax memberExpression)
            {
                return null;
            }

            memberIdentifierName = memberExpression.Name;

            var symbol = this.semanticModel.GetSymbolInfo(memberExpression);

            return symbol.Symbol;
        }

        private static ObjectCreationExpressionSyntax? FindMockCreation(BlockSyntax block, string variableName)
        {
            foreach (var statement in block.Statements.OfType<LocalDeclarationStatementSyntax>())
            {
                foreach (var variable in statement.Declaration.Variables)
                {
                    if (variable.Identifier.Text != variableName)
                    {
                        continue;
                    }

                    if (variable.Initializer is null)
                    {
                        continue;
                    }

                    if (variable.Initializer.Value is not ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
                    {
                        continue;
                    }

                    return objectCreationExpressionSyntax;
                }
            }

            return null;
        }
    }
}
