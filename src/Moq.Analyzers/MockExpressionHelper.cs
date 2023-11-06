//-----------------------------------------------------------------------
// <copyright file="MockExpressionHelper.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class MockExpressionHelper
    {
        public static bool IsMockCreation(MoqSymbols moqSymbols, SemanticModel semanticModel, ObjectCreationExpressionSyntax expression)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(expression.Type);

            if (!moqSymbols.IsMock(symbolInfo.Symbol))
            {
                return false;
            }

            return true;
        }

        public static bool IsMockSetupMethod(MoqSymbols moqSymbols, SemanticModel semanticModel, InvocationExpressionSyntax invocationExpression, out IdentifierNameSyntax localVariableExpression)
        {
            localVariableExpression = null;

            // Gets the member access expression "mock.XXXXX"
            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            {
                return false;
            }

            if (memberAccessExpression.Expression is not IdentifierNameSyntax lv)
            {
                return false;
            }

            localVariableExpression = lv;

            var instanceVariable = semanticModel.GetSymbolInfo(memberAccessExpression.Expression);

            if (instanceVariable.Symbol is not ILocalSymbol instanceVariableSymbol)
            {
                return false;
            }

            if (!moqSymbols.IsMock(instanceVariableSymbol.Type))
            {
                return false;
            }

            // Gets the method and check it is Setup() method.
            var methodSymbolInfo = semanticModel.GetSymbolInfo(invocationExpression.Expression);

            if (!moqSymbols.IsSetupMethod(methodSymbolInfo.Symbol))
            {
                return false;
            }

            return true;
        }

        public static bool IsStrictBehavior(MoqSymbols moqSymbols, SemanticModel semanticModel, IdentifierNameSyntax localVariableExpression)
        {
            foreach (var block in localVariableExpression.Ancestors().OfType<BlockSyntax>())
            {
                var mockCreation = FindMockCreation(moqSymbols, semanticModel, block);

                if (mockCreation is not null)
                {
                    // Check that the "new Mock<I>()" statement have at least one argument (else Strict is missing...).
                    if (mockCreation.ArgumentList is null)
                    {
                        return false;
                    }

                    var firstArgument = mockCreation.ArgumentList.Arguments.FirstOrDefault();

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
                    var firstArgumentType = semanticModel.GetSymbolInfo(memberAccessExpression.Expression);

                    if (!moqSymbols.IsMockBehaviorEnum(firstArgumentType.Symbol))
                    {
                        return false;
                    }

                    // Check that the memberAccessExpression.Name reference the Strict field
                    var firstArgumentField = semanticModel.GetSymbolInfo(memberAccessExpression.Name);

                    if (!moqSymbols.IsMockBehaviorStrictField(firstArgumentField.Symbol))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static ITypeSymbol? GetSetupMethodReturnSymbol(MoqSymbols moqSymbols, SemanticModel semanticModel, InvocationExpressionSyntax setupInvocationExpression)
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

            var methodSymbolInfo = semanticModel.GetSymbolInfo(methodExpression);

            if (methodSymbolInfo.Symbol is not IMethodSymbol methodSymbol)
            {
                return null;
            }

            return methodSymbol.ReturnType;
        }

        private static ObjectCreationExpressionSyntax FindMockCreation(MoqSymbols moqSymbols, SemanticModel semanticModel, BlockSyntax block)
        {
            foreach (var statement in block.Statements)
            {
                foreach (var objectCreationExpressionSyntax in statement.DescendantNodes().OfType<ObjectCreationExpressionSyntax>())
                {
                    if (IsMockCreation(moqSymbols, semanticModel, objectCreationExpressionSyntax))
                    {
                        return objectCreationExpressionSyntax;
                    }
                }
            }

            return null;
        }
    }
}
