//-----------------------------------------------------------------------
// <copyright file="ChainInvocationArgument.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class ChainInvocationArgument
    {
        public ChainInvocationArgument(ArgumentSyntax syntax, IParameterSymbol symbol)
        {
            this.Syntax = syntax;
            this.ParameterSymbol = symbol;
        }

        public ArgumentSyntax Syntax { get; }

        public IParameterSymbol ParameterSymbol { get; }
    }
}
