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

        public bool IsMockCreation(MoqSymbols moqSymbols, ObjectCreationExpressionSyntax expression, CancellationToken cancellationToken)
        {
            if (this.GetMockedType(moqSymbols, expression, out var _, cancellationToken) is null)
            {
                return false;
            }

            return true;
        }

        public ITypeSymbol? GetMockedType(MoqSymbols moqSymbols, IdentifierNameSyntax expression, CancellationToken cancellationToken)
        {
            var symbolInfo = this.semanticModel.GetSymbolInfo(expression, cancellationToken);

            if (symbolInfo.Symbol is not ILocalSymbol localVariableSymbol)
            {
                return null;
            }

            if (localVariableSymbol.Type is not INamedTypeSymbol typeSymbol)
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

            return typeSymbol.TypeArguments[0];
        }

        public ITypeSymbol? GetMockedType(MoqSymbols moqSymbols, ObjectCreationExpressionSyntax expression, out TypeSyntax? typeExpression, CancellationToken cancellationToken)
        {
            typeExpression = null;

            var symbolInfo = this.semanticModel.GetSymbolInfo(expression.Type, cancellationToken);

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

        public bool IsMockSetupMethod(MoqSymbols moqSymbols, InvocationExpressionSyntax invocationExpression, out IdentifierNameSyntax? localVariableExpression, CancellationToken cancellationToken)
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

            var instanceVariable = this.semanticModel.GetSymbolInfo(memberAccessExpression.Expression, cancellationToken);

            if (instanceVariable.Symbol is not ILocalSymbol instanceVariableSymbol)
            {
                return false;
            }

            if (!moqSymbols.IsMock(instanceVariableSymbol.Type))
            {
                return false;
            }

            // Gets the method and check it is Setup() method.
            var methodSymbolInfo = this.semanticModel.GetSymbolInfo(invocationExpression.Expression, cancellationToken);

            if (!moqSymbols.IsSetupMethod(methodSymbolInfo.Symbol))
            {
                return false;
            }

            return true;
        }

        public bool IsMockSetupMethodProtected(MoqSymbols moqSymbols, InvocationExpressionSyntax invocationExpression, out IdentifierNameSyntax? localVariableExpression, CancellationToken cancellationToken)
        {
            localVariableExpression = null;

            // Gets the member access expression "mock.XXXXX"
            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            {
                return false;
            }

            if (memberAccessExpression.Expression is not InvocationExpressionSyntax protectedInvocationExpression)
            {
                return false;
            }

            // Check it is a Protected() method
            var method = this.semanticModel.GetSymbolInfo(protectedInvocationExpression.Expression, cancellationToken);

            if (!moqSymbols.IsProtectedMethod(method.Symbol))
            {
                return false;
            }

            // Retrieve the "mock" variable
            if (protectedInvocationExpression.Expression is not MemberAccessExpressionSyntax protectedMemberAccessExpression)
            {
                return false;
            }

            if (protectedMemberAccessExpression.Expression is not IdentifierNameSyntax identifierName)
            {
                return false;
            }

            localVariableExpression = identifierName;

            var instanceVariable = this.semanticModel.GetSymbolInfo(identifierName, cancellationToken);

            if (instanceVariable.Symbol is not ILocalSymbol instanceVariableSymbol)
            {
                return false;
            }

            if (!moqSymbols.IsMock(instanceVariableSymbol.Type))
            {
                return false;
            }

            // Gets the method and check it is Setup() method.
            var methodSymbolInfo = this.semanticModel.GetSymbolInfo(invocationExpression.Expression, cancellationToken);

            if (!moqSymbols.IsSetupProtectedMethod(methodSymbolInfo.Symbol))
            {
                return false;
            }

            return true;
        }

        public bool IsStrictBehavior(MoqSymbols moqSymbols, ObjectCreationExpressionSyntax mockCreationExpression, CancellationToken cancellationToken)
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
            if (!this.IsStrictBehaviorArgument(moqSymbols, firstArgument, out var memberAccessExpression, cancellationToken))
            {
                return false;
            }

            // Check that the memberAccessExpression.Name reference the Strict field
            var firstArgumentField = this.semanticModel.GetSymbolInfo(memberAccessExpression!.Name, cancellationToken);

            if (!moqSymbols.IsMockBehaviorStrictField(firstArgumentField.Symbol))
            {
                return false;
            }

            return true;
        }

        public bool IsStrictBehaviorArgument(MoqSymbols moqSymbols, ArgumentSyntax argument, out MemberAccessExpressionSyntax? memberAccessExpression, CancellationToken cancellationToken)
        {
            memberAccessExpression = null;

            // Check it is a MemberAccessExpressionSyntax (because we searching for MockBehavior.XXXXX).
            if (argument.Expression is not MemberAccessExpressionSyntax expression)
            {
                return false;
            }

            memberAccessExpression = expression;

            // Check that the "memberAccessExpression.Expression" is applied on the Moq MockBehavior type.
            var firstArgumentType = this.semanticModel.GetSymbolInfo(memberAccessExpression.Expression, cancellationToken);

            if (!moqSymbols.IsMockBehaviorEnum(firstArgumentType.Symbol))
            {
                return false;
            }

            return true;
        }

        public bool IsStrictBehavior(MoqSymbols moqSymbols, IdentifierNameSyntax localVariableExpression, CancellationToken cancellationToken)
        {
            // Go back to the parents nodes and iterate all the statements in the parents blocks to find
            // the Mock instantiation (new Mock<I>(...) to determines the mock behavior.
            foreach (var block in localVariableExpression.Ancestors().OfType<BlockSyntax>())
            {
                var mockCreation = FindMockCreation(block, localVariableExpression.Identifier.ValueText);

                if (mockCreation is not null)
                {
                    if (this.IsStrictBehavior(moqSymbols, mockCreation, cancellationToken))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public ITypeSymbol? GetSetupMethodReturnSymbol(InvocationExpressionSyntax setupInvocationExpression, CancellationToken cancellationToken)
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

            var methodSymbolInfo = this.semanticModel.GetSymbolInfo(lambdaExpression.Body, cancellationToken);

            if (methodSymbolInfo.Symbol is IMethodSymbol methodSymbol)
            {
                return methodSymbol.ReturnType;
            }

            if (methodSymbolInfo.Symbol is IPropertySymbol propertySymbol)
            {
                return propertySymbol.Type;
            }

            return null;
        }

        public IMethodSymbol? ExtractSetupMethod(InvocationExpressionSyntax invocationExpression, out NameSyntax? memberIdentifierName, CancellationToken cancellationToken)
        {
            memberIdentifierName = null;

            var members = this.ExtractSetupMembers(invocationExpression, cancellationToken);

            var member = members.FirstOrDefault();

            if (member is null)
            {
                return null;
            }

            if (member.Symbol is not IMethodSymbol methodSymbol)
            {
                return null;
            }

            memberIdentifierName = member.Syntax;

            return methodSymbol;
        }

        public IReadOnlyList<SetupMember> ExtractSetupMembers(InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
        {
            if (invocationExpression.ArgumentList.Arguments.Count != 1)
            {
                return Array.Empty<SetupMember>();
            }

            if (invocationExpression.ArgumentList.Arguments[0].Expression is not SimpleLambdaExpressionSyntax lambdaExpression)
            {
                return Array.Empty<SetupMember>();
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
                    return Array.Empty<SetupMember>();
                }

                bodyExpression = lambdaExpression.ExpressionBody;
            }

            var members = new List<SetupMember>();

            MemberAccessExpressionSyntax? memberAccessExpression;

            while ((memberAccessExpression = bodyExpression as MemberAccessExpressionSyntax) != null)
            {
                var symbol = this.semanticModel.GetSymbolInfo(memberAccessExpression, cancellationToken);

                if (symbol.Symbol is null)
                {
                    return Array.Empty<SetupMember>();
                }

                members.Add(new SetupMember(memberAccessExpression.Name, symbol.Symbol));

                bodyExpression = memberAccessExpression.Expression;
            }

            return members;
        }

        public IMethodSymbol? ExtractCallBackLambdaExpressionMethod(InvocationExpressionSyntax invocationExpression, out ParenthesizedLambdaExpressionSyntax? lambdaExpression, CancellationToken cancellationToken)
        {
            lambdaExpression = null;

            if (invocationExpression.ArgumentList.Arguments.Count != 1)
            {
                return null;
            }

            if (invocationExpression.ArgumentList.Arguments[0].Expression is not ParenthesizedLambdaExpressionSyntax lambdaExpressionFound)
            {
                return null;
            }

            var symbol = this.semanticModel.GetSymbolInfo(lambdaExpressionFound, cancellationToken);

            if (symbol.Symbol is not IMethodSymbol methodSymbol)
            {
                return null;
            }

            lambdaExpression = lambdaExpressionFound;

            return methodSymbol;
        }

        public ITypeSymbol? ExtractAsMethodType(MoqSymbols moqSymbols, InvocationExpressionSyntax invocationExpression, out TypeSyntax? typeSyntax, CancellationToken cancellationToken)
        {
            typeSyntax = null;

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            {
                return null;
            }

            if (memberAccessExpression.Name is not GenericNameSyntax genericName)
            {
                return null;
            }

            var symbol = this.semanticModel.GetSymbolInfo(memberAccessExpression.Name, cancellationToken);

            if (symbol.Symbol is not IMethodSymbol methodSymbol)
            {
                return null;
            }

            if (!moqSymbols.IsAsMethod(methodSymbol))
            {
                return null;
            }

            typeSyntax = genericName.TypeArgumentList.Arguments[0];

            return methodSymbol.TypeArguments[0];
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
