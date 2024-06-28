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
