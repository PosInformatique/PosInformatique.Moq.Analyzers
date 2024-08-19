//-----------------------------------------------------------------------
// <copyright file="SymbolExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class SymbolExtensions
    {
        public static bool IsOrInheritFrom(this ITypeSymbol? symbol, ITypeSymbol type)
        {
            if (symbol is null)
            {
                return false;
            }

            if (SymbolEqualityComparer.Default.Equals(symbol, type))
            {
                return true;
            }

            foreach (var implementedInterface in symbol.Interfaces)
            {
                if (IsOrInheritFrom(implementedInterface, type))
                {
                    return true;
                }
            }

            return IsOrInheritFrom(symbol.BaseType, type);
        }

        public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol symbol, string name)
        {
            IEnumerable<ISymbol> baseTypeMembers;

            if (symbol.BaseType is not null)
            {
                baseTypeMembers = symbol.BaseType.GetAllMembers(name);
            }
            else
            {
                baseTypeMembers = Enumerable.Empty<ISymbol>();
            }

            return symbol.GetMembers(name).Concat(baseTypeMembers);
        }
    }
}
