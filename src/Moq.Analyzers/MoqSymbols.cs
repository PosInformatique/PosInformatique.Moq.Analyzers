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

        private readonly INamedTypeSymbol mockGenericClass;

        private readonly IReadOnlyList<IMethodSymbol> setupMethods;

        private readonly IReadOnlyList<IMethodSymbol> setupProtectedMethods;

        private readonly IReadOnlyList<IMethodSymbol> verifyMethods;

        private readonly Lazy<IMethodSymbol> staticVerifyMethod;

        private readonly Lazy<IMethodSymbol> staticVerifyAllMethod;

        private readonly Lazy<IMethodSymbol> verifyAllMethod;

        private readonly ISymbol mockBehaviorStrictField;

        private readonly ISymbol isAnyTypeClass;

        private readonly ISymbol asMethod;

        private MoqSymbols(INamedTypeSymbol mockGenericClass, INamedTypeSymbol mockBehaviorEnum, ISymbol isAnyTypeClass, INamedTypeSymbol protectedMockInterface)
        {
            this.mockGenericClass = mockGenericClass;
            this.mockBehaviorEnum = mockBehaviorEnum;
            this.isAnyTypeClass = isAnyTypeClass;

            this.setupMethods = mockGenericClass.GetMembers("Setup").OfType<IMethodSymbol>().ToArray();
            this.mockBehaviorStrictField = mockBehaviorEnum.GetMembers("Strict").First();
            this.setupProtectedMethods = protectedMockInterface.GetMembers("Setup").OfType<IMethodSymbol>().ToArray();
            this.asMethod = mockGenericClass.GetMembers("As").Single();
            this.verifyMethods = mockGenericClass.GetMembers("Verify").OfType<IMethodSymbol>().ToArray();

            this.staticVerifyMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.BaseType!.GetMembers("Verify").Where(m => m.IsStatic).OfType<IMethodSymbol>().Single());
            this.staticVerifyAllMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.BaseType!.GetMembers("VerifyAll").Where(m => m.IsStatic).OfType<IMethodSymbol>().Single());
            this.verifyAllMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.GetMembers("VerifyAll").OfType<IMethodSymbol>().Single());
        }

        public static MoqSymbols? FromCompilation(Compilation compilation)
        {
            var mockGenericClass = compilation.GetTypeByMetadataName("Moq.Mock`1");

            if (mockGenericClass is null)
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

            return new MoqSymbols(mockGenericClass, mockBehaviorEnum, isAnyTypeClass, protectedMockInterface);
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

            if (!SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, this.mockGenericClass))
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

        public bool IsVerifyMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            var originalDefinition = symbol.OriginalDefinition;

            foreach (var verifyMethod in this.verifyMethods)
            {
                if (SymbolEqualityComparer.Default.Equals(originalDefinition, verifyMethod))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsVerifyStaticMethod(ISymbol symbol)
        {
            if (!SymbolEqualityComparer.Default.Equals(symbol, this.staticVerifyMethod.Value))
            {
                return false;
            }

            return true;
        }

        public bool IsVerifyAllMethod(ISymbol symbol)
        {
            if (!SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, this.verifyAllMethod.Value))
            {
                return false;
            }

            return true;
        }

        public bool IsVerifyAllStaticMethod(ISymbol symbol)
        {
            if (!SymbolEqualityComparer.Default.Equals(symbol, this.staticVerifyAllMethod.Value))
            {
                return false;
            }

            return true;
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
