//-----------------------------------------------------------------------
// <copyright file="SyntaxNodeHelper.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SyntaxNodeHelper
    {
        public static VariableDeclaratorSyntax? GetVariableNameSyntax(SyntaxNode node)
        {
            var variableName = node.Ancestors().OfType<VariableDeclaratorSyntax>().FirstOrDefault();

            if (variableName is null)
            {
                // No variable set on the left.
                return null;
            }

            return variableName;
        }
    }
}
