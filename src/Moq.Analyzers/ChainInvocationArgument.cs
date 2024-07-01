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
        public ChainInvocationArgument(ArgumentSyntax syntax, ISymbol? symbol, IParameterSymbol parameterSymbol)
        {
            this.Syntax = syntax;
            this.Symbol = symbol;
            this.ParameterSymbol = parameterSymbol;
        }

        public ISymbol? Symbol { get; }

        public ArgumentSyntax Syntax { get; }

        public IParameterSymbol ParameterSymbol { get; }
    }
}
