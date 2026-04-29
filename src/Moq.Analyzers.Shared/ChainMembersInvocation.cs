//-----------------------------------------------------------------------
// <copyright file="ChainMembersInvocation.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal sealed class ChainMembersInvocation
    {
        public ChainMembersInvocation(IReadOnlyList<ChainMember> members, IReadOnlyList<ChainInvocationArgument> invocationArguments)
        {
            this.Members = members;
            this.InvocationArguments = invocationArguments;
        }

        public IReadOnlyList<ChainMember> Members { get; }

        public IReadOnlyList<ChainInvocationArgument> InvocationArguments { get; }

        public ITypeSymbol ReturnType
        {
            get
            {
                if (this.Members[0].Symbol is IMethodSymbol methodSymbol)
                {
                    return methodSymbol.ReturnType;
                }

                return ((IPropertySymbol)this.Members[0].Symbol).Type;
            }
        }

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
    }
}
