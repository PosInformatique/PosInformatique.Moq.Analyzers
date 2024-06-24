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
        private readonly Lazy<INamedTypeSymbol> mockBehaviorEnum;

        private readonly INamedTypeSymbol mockGenericClass;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> setupMethods;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> setupProtectedMethods;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> verifyMethods;

        private readonly Lazy<IMethodSymbol> staticVerifyMethod;

        private readonly Lazy<IMethodSymbol> staticVerifyAllMethod;

        private readonly Lazy<IMethodSymbol> verifyAllMethod;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> verifiableMethods;

        private readonly Lazy<ISymbol> mockBehaviorStrictField;

        private readonly Lazy<INamedTypeSymbol> isAnyTypeClass;

        private readonly Lazy<ISymbol> asMethod;

        private readonly Lazy<INamedTypeSymbol> verifiesInterface; 

        private MoqSymbols(INamedTypeSymbol mockGenericClass, Compilation compilation)
        {
            this.mockGenericClass = mockGenericClass;
            this.mockBehaviorEnum = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("Moq.MockBehavior")!);
            this.isAnyTypeClass = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("Moq.It+IsAnyType")!);
            this.verifiesInterface = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("Moq.Language.IVerifies")!);

            this.setupMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.GetMembers("Setup").OfType<IMethodSymbol>().ToArray());
            this.mockBehaviorStrictField = new Lazy<ISymbol>(() => this.mockBehaviorEnum.Value.GetMembers("Strict").First());
            this.setupProtectedMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => compilation.GetTypeByMetadataName("Moq.Protected.IProtectedMock`1")!.GetMembers("Setup").OfType<IMethodSymbol>().ToArray());
            this.asMethod = new Lazy<ISymbol>(() => mockGenericClass.GetMembers("As").Single());
            this.verifyMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.GetMembers("Verify").Concat(mockGenericClass.BaseType!.GetMembers("Verify")).Where(m => !m.IsStatic).OfType<IMethodSymbol>().ToArray());

            this.staticVerifyMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.BaseType!.GetMembers("Verify").Where(m => m.IsStatic).OfType<IMethodSymbol>().Single());
            this.staticVerifyAllMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.BaseType!.GetMembers("VerifyAll").Where(m => m.IsStatic).OfType<IMethodSymbol>().Single());
            this.verifyAllMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.BaseType!.GetMembers("VerifyAll").Where(m => !m.IsStatic).OfType<IMethodSymbol>().Single());

            this.verifiableMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => this.verifiesInterface.Value.GetMembers("Verifiable").OfType<IMethodSymbol>().ToArray());
        }

        public static MoqSymbols? FromCompilation(Compilation compilation)
        {
            var mockGenericClass = compilation.GetTypeByMetadataName("Moq.Mock`1");

            if (mockGenericClass is null)
            {
                return null;
            }

            return new MoqSymbols(mockGenericClass, compilation);
        }

        public bool IsAnyType(ITypeSymbol symbol)
        {
            if (!SymbolEqualityComparer.Default.Equals(symbol, this.isAnyTypeClass.Value))
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

            foreach (var setupMethod in this.setupMethods.Value)
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

            foreach (var setupProtectedMethod in this.setupProtectedMethods.Value)
            {
                if (SymbolEqualityComparer.Default.Equals(originalDefinition, setupProtectedMethod))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsVerifiableMethod(ISymbol? symbol)
        {
            if (symbol is null)
            {
                return false;
            }

            var originalDefinition = symbol.OriginalDefinition;

            foreach (var verifiableMethod in this.verifiableMethods.Value)
            {
                if (SymbolEqualityComparer.Default.Equals(originalDefinition, verifiableMethod))
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

            foreach (var verifyMethod in this.verifyMethods.Value)
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

            if (!SymbolEqualityComparer.Default.Equals(symbol, this.mockBehaviorEnum.Value))
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

            if (!SymbolEqualityComparer.Default.Equals(symbol, this.mockBehaviorStrictField.Value))
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
            if (!SymbolEqualityComparer.Default.Equals(method.OriginalDefinition, this.asMethod.Value))
            {
                return false;
            }

            return true;
        }
    }
}
