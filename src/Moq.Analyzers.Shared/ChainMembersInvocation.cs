//-----------------------------------------------------------------------
// <copyright file="ChainMembersInvocation.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class ChainMembersInvocation
    {
        public ChainMembersInvocation(IReadOnlyList<ChainMember> members, IReadOnlyList<ChainInvocationArgument> invocationArguments)
        {
            this.Members = members;
            this.InvocationArguments = invocationArguments;

            if (this.Members[0].Symbol is IMethodSymbol methodSymbol)
            {
                this.Parameters = methodSymbol.Parameters;
                this.ReturnType = methodSymbol.ReturnType;
            }
            else
            {
                var propertySymbol = (IPropertySymbol)this.Members[0].Symbol;

                this.Parameters = propertySymbol.Parameters;
                this.ReturnType = propertySymbol.Type;
            }
        }

        public IReadOnlyList<ChainMember> Members { get; }

        public NameSyntax InvocationExpression => this.Members[0].Syntax;

        public IReadOnlyList<ChainInvocationArgument> InvocationArguments { get; }

        public IReadOnlyList<IParameterSymbol> Parameters { get; }

        public ITypeSymbol ReturnType { get; }

        public bool IsProperty
        {
            get
            {
                if (this.Members[0].Symbol is IMethodSymbol methodSymbol)
                {
                    return false;
                }

                return true;
            }
        }

        public bool HasSameMembers(ChainMembersInvocation otherChainInvocation)
        {
            if (this.Members.Count != otherChainInvocation.Members.Count)
            {
                return false;
            }

            for (var i = 0; i < this.Members.Count; i++)
            {
                if (!SymbolEqualityComparer.Default.Equals(this.Members[i].Symbol, otherChainInvocation.Members[i].Symbol))
                {
                    return false;
                }
            }

            return true;
        }

        public IParameterSymbol? GetInvocationArgument(IParameterSymbol parameter)
        {
            foreach (var invocationArgument in this.InvocationArguments)
            {
                if (SymbolEqualityComparer.Default.Equals(invocationArgument.ParameterSymbol, parameter))
                {
                    return invocationArgument.ParameterSymbol;
                }
            }

            return null;
        }
    }
}
