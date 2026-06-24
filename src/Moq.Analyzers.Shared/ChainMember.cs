//-----------------------------------------------------------------------
// <copyright file="ChainMember.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal class ChainMember
    {
        public ChainMember(NameSyntax syntax, ISymbol symbol)
        {
            this.Syntax = syntax;
            this.Symbol = symbol;
        }

        public NameSyntax Syntax { get; }

        public ISymbol Symbol { get; }
    }
}
