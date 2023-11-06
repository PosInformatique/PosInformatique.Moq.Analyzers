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

        public bool IsMock(ISymbol symbol)
        {
            if (SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, this.mockClass))
            {
                return true;
            }

            return false;
        }

        public bool IsSetupMethod(ISymbol symbol)
        {
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

        public bool IsReturnsMethod(ISymbol symbol)
        {
            if (symbol.Name != "Returns")
            {
                return false;
            }

            return true;
        }

        public bool IsReturnsAsyncMethod(ISymbol symbol)
        {
            if (symbol.Name != "ReturnsAsync")
            {
                return false;
            }

            return true;
        }

        public bool IsMockBehaviorEnum(ISymbol symbol)
        {
            if (!SymbolEqualityComparer.Default.Equals(symbol, this.mockBehaviorEnum))
            {
                return false;
            }

            return true;
        }

        public bool IsMockBehaviorStrictField(ISymbol symbol)
        {
            if (!SymbolEqualityComparer.Default.Equals(symbol, this.mockBehaviorStrictField))
            {
                return false;
            }

            return true;
        }
    }
}
