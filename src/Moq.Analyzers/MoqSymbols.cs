//-----------------------------------------------------------------------
// <copyright file="MoqSymbols.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using System;
    using Microsoft.CodeAnalysis;

    internal sealed class MoqSymbols
    {
        private readonly INamedTypeSymbol mockBehaviorEnum;

        private readonly INamedTypeSymbol mockClass;

        private readonly IReadOnlyList<IMethodSymbol> setupMethods;

        private readonly IReadOnlyList<IMethodSymbol> setupProtectedMethods;

        private readonly ISymbol mockBehaviorStrictField;

        private readonly ISymbol isAnyTypeClass;

        private readonly ISymbol asMethod;

        private MoqSymbols(INamedTypeSymbol mockClass, INamedTypeSymbol mockBehaviorEnum, ISymbol isAnyTypeClass, INamedTypeSymbol protectedMockInterface)
        {
            this.mockClass = mockClass;
            this.mockBehaviorEnum = mockBehaviorEnum;
            this.isAnyTypeClass = isAnyTypeClass;

            this.setupMethods = mockClass.GetMembers("Setup").OfType<IMethodSymbol>().ToArray();
            this.mockBehaviorStrictField = mockBehaviorEnum.GetMembers("Strict").First();
            this.setupProtectedMethods = protectedMockInterface.GetMembers("Setup").OfType<IMethodSymbol>().ToArray();
            this.asMethod = mockClass.GetMembers("As").Single();
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

            var isAnyTypeClass = compilation.GetTypeByMetadataName("Moq.It+IsAnyType");

            if (isAnyTypeClass is null)
            {
                return null;
            }

            var protectedMockInterface = compilation.GetTypeByMetadataName("Moq.Protected.IProtectedMock`1");

            if (protectedMockInterface is null)
            {
                return null;
            }

            return new MoqSymbols(mockClass, mockBehaviorEnum, isAnyTypeClass, protectedMockInterface);
        }

        public bool IsAnyType(ITypeSymbol symbol)
        {
            if (!SymbolEqualityComparer.Default.Equals(symbol, this.isAnyTypeClass))
            {
                return false;
            }

            return true;
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

        public bool IsSetupProtectedMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            var originalDefinition = symbol.OriginalDefinition;

            foreach (var setupProtectedMethod in this.setupProtectedMethods)
            {
                if (SymbolEqualityComparer.Default.Equals(originalDefinition, setupProtectedMethod))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCallback(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (symbol.Name != "Callback")
            {
                return false;
            }

            return true;
        }

        public bool IsProtectedMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            if (symbol.Name != "Protected")
            {
                return false;
            }

            return true;
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

        public bool IsThrowsMethod(ISymbol symbol)
        {
            if (symbol.Name != "Throws")
            {
                return false;
            }

            return true;
        }

        public bool IsThrowsAsyncMethod(ISymbol symbol)
        {
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

        public bool IsMockable(ITypeSymbol type)
        {
            if (type.TypeKind == TypeKind.Interface)
            {
                return true;
            }

            if (type.IsAbstract)
            {
                return true;
            }

            if (!type.IsSealed)
            {
                return true;
            }

            return false;
        }

        public bool IsAsMethod(IMethodSymbol method)
        {
            if (!SymbolEqualityComparer.Default.Equals(method.OriginalDefinition, this.asMethod))
            {
                return false;
            }

            return true;
        }
    }
}
