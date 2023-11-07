//-----------------------------------------------------------------------
// <copyright file="MoqSymbols.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal sealed class MoqSymbols
    {
        private readonly INamedTypeSymbol mockBehaviorEnum;

        private readonly INamedTypeSymbol mockClass;

        private readonly IReadOnlyList<IMethodSymbol> setupMethods;

        private readonly ISymbol mockBehaviorStrictField;

        private MoqSymbols(INamedTypeSymbol mockClass, INamedTypeSymbol mockBehaviorEnum)
        {
            this.mockClass = mockClass;
            this.mockBehaviorEnum = mockBehaviorEnum;

            this.setupMethods = mockClass.GetMembers("Setup").OfType<IMethodSymbol>().ToArray();
            this.mockBehaviorStrictField = mockBehaviorEnum.GetMembers("Strict").First();
        }

        public static MoqSymbols? FromCompilation(Compilation compilation)
        {
            var mockClass = compilation.GetTypeByMetadataName("Moq.Mock`1");

            if (mockClass is null)
            {
                return null;
            }

            var mockBehaviorEnum = compilation.GetTypeByMetadataName("Moq.MockBehavior");

            if (mockBehaviorEnum is null)
            {
                return null;
            }

            return new MoqSymbols(mockClass, mockBehaviorEnum);
        }

        public bool IsMock(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (!SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, this.mockClass))
            {
                return false;
            }

            return true;
        }

        public bool IsSetupMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            var originalDefinition = symbol.OriginalDefinition;

            foreach (var setupMethod in this.setupMethods)
            {
                if (SymbolEqualityComparer.Default.Equals(originalDefinition, setupMethod))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsReturnsMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (symbol.Name != "Returns")
            {
                return false;
            }

            return true;
        }

        public bool IsReturnsAsyncMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (symbol.Name != "ReturnsAsync")
            {
                return false;
            }

            return true;
        }

        public bool IsThrowsMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (symbol.Name != "Throws")
            {
                return false;
            }

            return true;
        }

        public bool IsThrowsAsyncMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (symbol.Name != "ThrowsAsync")
            {
                return false;
            }

            return true;
        }

        public bool IsMockBehaviorEnum(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (!SymbolEqualityComparer.Default.Equals(symbol, this.mockBehaviorEnum))
            {
                return false;
            }

            return true;
        }

        public bool IsMockBehaviorStrictField(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (!SymbolEqualityComparer.Default.Equals(symbol, this.mockBehaviorStrictField))
            {
                return false;
            }

            return true;
        }

        public bool IsOverridable(ISymbol method)
        {
            if (method.ContainingType.TypeKind == TypeKind.Interface)
            {
                return true;
            }

            if (method.IsAbstract)
            {
                return true;
            }

            if (method.IsVirtual)
            {
                return true;
            }

            if (method.IsOverride)
            {
                return true;
            }

            return false;
        }
    }
}
