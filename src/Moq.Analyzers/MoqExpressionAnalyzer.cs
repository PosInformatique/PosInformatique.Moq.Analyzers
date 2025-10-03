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
        private readonly MoqSymbols moqSymbols;

        private readonly SemanticModel semanticModel;

        public MoqExpressionAnalyzer(MoqSymbols moqSymbols, SemanticModel semanticModel)
        {
            this.moqSymbols = moqSymbols;
            this.semanticModel = semanticModel;
        }

        public bool IsMockCreation(ObjectCreationExpressionSyntax expression, CancellationToken cancellationToken)
        {
            if (this.GetMockedType(expression, out var _, cancellationToken) is null)
            {
                return false;
            }

            return true;
        }

        public ITypeSymbol? GetMockedType(IdentifierNameSyntax expression, CancellationToken cancellationToken)
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

            if (!this.moqSymbols.IsMock(typeSymbol))
            {
                return null;
            }

            if (typeSymbol.TypeArguments.Length != 1)
            {
                return null;
            }

            return typeSymbol.TypeArguments[0];
        }

        public ITypeSymbol? GetMockedType(ObjectCreationExpressionSyntax expression, out TypeSyntax? typeExpression, CancellationToken cancellationToken)
        {
            typeExpression = null;

            var symbolInfo = this.semanticModel.GetSymbolInfo(expression.Type, cancellationToken);

            if (symbolInfo.Symbol is not INamedTypeSymbol typeSymbol)
            {
                return null;
            }

            if (!this.moqSymbols.IsMock(typeSymbol))
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

        public ISymbol? GetMockVariable(InvocationExpressionSyntax invocationExpression, out IdentifierNameSyntax? localVariableExpression, CancellationToken cancellationToken)
        {
            localVariableExpression = invocationExpression.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();

            if (localVariableExpression is null)
            {
                return null;
            }

            var localVariable = this.semanticModel.GetSymbolInfo(localVariableExpression, cancellationToken);

            if (localVariable.Symbol is null)
            {
                return null;
            }

            return localVariable.Symbol;
        }

        public bool IsMockSetupMethod(InvocationExpressionSyntax invocationExpression, out IdentifierNameSyntax? localVariableExpression, CancellationToken cancellationToken)
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

            if (!this.moqSymbols.IsMock(instanceVariableSymbol.Type))
            {
                return false;
            }

            // Gets the method and check it is Setup() method.
            return this.IsMockSetupMethod(invocationExpression, cancellationToken);
        }

        public bool IsMockSetupMethodProtected(InvocationExpressionSyntax invocationExpression, out IdentifierNameSyntax? localVariableExpression, CancellationToken cancellationToken)
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

            if (!this.moqSymbols.IsProtectedMethod(method.Symbol))
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

            if (!this.moqSymbols.IsMock(instanceVariableSymbol.Type))
            {
                return false;
            }

            // Gets the method and check it is Setup() method.
            var methodSymbolInfo = this.semanticModel.GetSymbolInfo(invocationExpression.Expression, cancellationToken);

            if (!this.moqSymbols.IsSetupProtectedMethod(methodSymbolInfo.Symbol))
            {
                return false;
            }

            return true;
        }

        public bool IsMockCreationStrictBehavior(ObjectCreationExpressionSyntax mockCreation, CancellationToken cancellationToken)
        {
            // Check that the "new Mock<I>()" statement have at least one argument (else Strict is missing...).
            if (mockCreation.ArgumentList is null)
            {
                return false;
            }

            var argument = mockCreation.ArgumentList.Arguments.FirstOrDefault();

            if (argument is null)
            {
                return false;
            }

            // Check if the mock creation have factory parameter
            var constructorSymbol = this.semanticModel.GetSymbolInfo(mockCreation, cancellationToken);

            if (this.moqSymbols.IsMockConstructorWithFactory(constructorSymbol.Symbol))
            {
                // The first argument is a lambda expression "Mock<T>(Expression<Func<T>>)" to create
                // instance of the mock, so check the second argument.
                if (mockCreation.ArgumentList.Arguments.Count < 2)
                {
                    return false;
                }

                argument = mockCreation.ArgumentList.Arguments[1];
            }

            return this.IsStrictBehaviorArgument(argument, cancellationToken);
        }

        public bool IsMockOfStrictBehavior(ArgumentListSyntax? argumentList, CancellationToken cancellationToken)
        {
            // Check that the "Mock.Of<T>()" statement have at least one argument (else Strict is missing...).
            if (argumentList is null)
            {
                return false;
            }

            var lastArgument = argumentList.Arguments.LastOrDefault();

            if (lastArgument is null)
            {
                return false;
            }

            return this.IsStrictBehaviorArgument(lastArgument, cancellationToken);
        }

        public bool IsStrictBehaviorArgument(ArgumentSyntax argument, out MemberAccessExpressionSyntax? memberAccessExpression, CancellationToken cancellationToken)
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

            if (!this.moqSymbols.IsMockBehaviorEnum(firstArgumentType.Symbol))
            {
                return false;
            }

            return true;
        }

        public bool IsStrictBehavior(IdentifierNameSyntax localVariableExpression, CancellationToken cancellationToken)
        {
            // Go back to the parents nodes and iterate all the statements in the parents blocks to find
            // the Mock instantiation (new Mock<I>(...) to determines the mock behavior.
            foreach (var block in localVariableExpression.Ancestors().OfType<BlockSyntax>())
            {
                var mockCreation = FindMockCreation(block, localVariableExpression.Identifier.ValueText);

                if (mockCreation is not null)
                {
                    if (this.IsMockCreationStrictBehavior(mockCreation, cancellationToken))
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

        public ChainMembersInvocation? ExtractSetupMethod(InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
        {
            var setupMethod = this.GetSetupMethod(invocationExpression, cancellationToken);

            if (setupMethod is null)
            {
                return null;
            }

            var chain = this.ExtractChainedMembersInvocationFromLambdaExpression(setupMethod, cancellationToken);

            return chain;
        }

        public ChainMembersInvocation? ExtractChainedMembersInvocationFromLambdaExpression(InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
        {
            // Check the invocation expression is Setup(m => xxxxx) / Verify(m => xxxx) expression which contains a lambda expression.
            if (invocationExpression.ArgumentList.Arguments.Count == 0)
            {
                return null;
            }

            if (invocationExpression.ArgumentList.Arguments[0].Expression is not SimpleLambdaExpressionSyntax lambdaExpression)
            {
                return null;
            }

            // Extract inside the body expression the chained members.
            ExpressionSyntax bodyExpression;

            var invocationMemberExpression = lambdaExpression.Body as InvocationExpressionSyntax;

            if (invocationMemberExpression is not null)
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

                // Special case for the SetupSet(), if the body expression is an assignment syntax
                // "mock => mock.Property = xxx", so we get the left part of the assignment.
                if (bodyExpression is AssignmentExpressionSyntax assignmentExpressionSyntax)
                {
                    bodyExpression = assignmentExpressionSyntax.Left;
                }
            }

            var members = new List<ChainMember>();

            MemberAccessExpressionSyntax? memberAccessExpression;

            while ((memberAccessExpression = bodyExpression as MemberAccessExpressionSyntax) != null)
            {
                var symbol = this.semanticModel.GetSymbolInfo(memberAccessExpression, cancellationToken);

                if (symbol.Symbol is null)
                {
                    return null;
                }

                members.Add(new ChainMember(memberAccessExpression.Name, symbol.Symbol));

                bodyExpression = memberAccessExpression.Expression;
            }

            var lastMember = members.FirstOrDefault();

            if (lastMember is null)
            {
                return null;
            }

            var invocationArguments = new List<ChainInvocationArgument>();

            if (invocationMemberExpression is not null)
            {
                if (lastMember.Symbol is not IMethodSymbol methodSymbol)
                {
                    return null;
                }

                for (var i = 0; i < invocationMemberExpression.ArgumentList.Arguments.Count; i++)
                {
                    var parameter = methodSymbol.Parameters[i];
                    var argumentSyntax = invocationMemberExpression.ArgumentList.Arguments[i];
                    var argumentSymbol = this.semanticModel.GetSymbolInfo(argumentSyntax.Expression, cancellationToken);

                    invocationArguments.Add(new ChainInvocationArgument(argumentSyntax, argumentSymbol.Symbol, parameter));
                }
            }

            return new ChainMembersInvocation(members, invocationArguments);
        }

        public IMethodSymbol? ExtractCallBackLambdaExpressionMethod(InvocationExpressionSyntax invocationExpression, out ParenthesizedLambdaExpressionSyntax? lambdaExpression, CancellationToken cancellationToken)
        {
            lambdaExpression = null;

            // Try to determine if the invocation expression is a Callback() expression.
            var callbackMethodSymbol = this.semanticModel.GetSymbolInfo(invocationExpression, cancellationToken);

            if (!this.moqSymbols.IsCallback(callbackMethodSymbol.Symbol))
            {
                return null;
            }

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

        public ITypeSymbol? ExtractAsMethodType(InvocationExpressionSyntax invocationExpression, out TypeSyntax? typeSyntax, CancellationToken cancellationToken)
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

            if (!this.moqSymbols.IsAsMethod(methodSymbol))
            {
                return null;
            }

            typeSyntax = genericName.TypeArgumentList.Arguments[0];

            return methodSymbol.TypeArguments[0];
        }

        public RaiseMethodCall? ExtractRaiseMethodCall(InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
        {
            // Check if the method analyzed is a Raise() method.
            var methodSymbol = this.semanticModel.GetSymbolInfo(invocationExpression, cancellationToken);

            if (!this.moqSymbols.IsRaiseMethod(methodSymbol.Symbol) && !this.moqSymbols.IsRaiseAsyncMethod(methodSymbol.Symbol))
            {
                return null;
            }

            // Gets the event
            // 1 - Check the first argument is a lambda expression (Raise(x => ...))
            if (invocationExpression.ArgumentList.Arguments[0].Expression is not LambdaExpressionSyntax lambdaExpressionSyntax)
            {
                return null;
            }

            // 2 - If the body of the lambda expression is an assignment to an event (Raise(x => x.Event += null))
            if (lambdaExpressionSyntax.Body is not AssignmentExpressionSyntax assignmentExpressionSyntax)
            {
                return null;
            }

            // 3 - Check the left of assignment is a member access.
            if (assignmentExpressionSyntax.Left is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return null;
            }

            // 4 - Gets the event symbol
            var eventSymbol = this.semanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Name, cancellationToken);

            if (eventSymbol.Symbol is null)
            {
                return null;
            }

            // 5 - Gets the parameters of the Raise() method after the "x => x.Event += null".
            var arguments = invocationExpression.ArgumentList.Arguments.Skip(1).ToArray();

            var parameterSymbols = new List<ITypeSymbol?>(arguments.Length);

            foreach (var argument in arguments)
            {
                var parameterSymbol = this.semanticModel.GetTypeInfo(argument.Expression, cancellationToken);

                parameterSymbols.Add(parameterSymbol.Type);
            }

            return new RaiseMethodCall((IMethodSymbol)methodSymbol.Symbol, parameterSymbols, arguments, (IEventSymbol)eventSymbol.Symbol);
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

        private InvocationExpressionSyntax? GetSetupMethod(InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
        {
            var followingMethods = invocationExpression.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>();

            foreach (var followingMethod in followingMethods)
            {
                if (this.IsMockSetupMethod(followingMethod, cancellationToken))
                {
                    return followingMethod;
                }
            }

            return null;
        }

        private bool IsMockSetupMethod(InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
        {
            // Gets the method and check it is Setup() method.
            var methodSymbolInfo = this.semanticModel.GetSymbolInfo(invocationExpression.Expression, cancellationToken);

            if (!this.moqSymbols.IsSetupMethod(methodSymbolInfo.Symbol))
            {
                return false;
            }

            return true;
        }

        private bool IsStrictBehaviorArgument(ArgumentSyntax argument, CancellationToken cancellationToken)
        {
            if (!this.IsStrictBehaviorArgument(argument, out var memberAccessExpression, cancellationToken))
            {
                return false;
            }

            // Check that the memberAccessExpression.Name reference the Strict field
            var lastArgumentField = this.semanticModel.GetSymbolInfo(memberAccessExpression!.Name, cancellationToken);

            if (!this.moqSymbols.IsMockBehaviorStrictField(lastArgumentField.Symbol))
            {
                return false;
            }

            return true;
        }
    }
}
