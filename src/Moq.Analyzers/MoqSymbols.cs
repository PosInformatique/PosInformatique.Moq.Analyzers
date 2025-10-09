//-----------------------------------------------------------------------
// <copyright file="MoqSymbols.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal sealed class MoqSymbols
    {
        private readonly Lazy<INamedTypeSymbol> mockBehaviorEnum;

        private readonly INamedTypeSymbol mockGenericClass;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> raiseMethods;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> raiseAsyncMethods;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> setupMethods;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> setupProtectedMethods;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> verifyMethods;

        private readonly Lazy<IMethodSymbol> staticVerifyMethod;

        private readonly Lazy<IMethodSymbol> staticVerifyAllMethod;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> mockOfMethods;

        private readonly Lazy<IMethodSymbol> verifyAllMethod;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> verifiableMethods;

        private readonly Lazy<ISymbol> mockBehaviorStrictField;

        private readonly Lazy<INamedTypeSymbol> isAnyTypeClass;

        private readonly Lazy<IReadOnlyList<ISymbol>> itIsMethods;

        private readonly Lazy<ISymbol> itIsAnyMethod;

        private readonly Lazy<ISymbol> asMethod;

        private readonly Lazy<INamedTypeSymbol> verifiesInterface;

        private readonly Lazy<IMethodSymbol> mockConstructorWithFactory;

        private readonly Lazy<IMethodSymbol> setupSetMethodWithoutGenericArgument;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> setupSequenceMethods;

        private readonly Lazy<IReadOnlyList<IMethodSymbol>> setupSetMethods;

        private readonly Lazy<INamedTypeSymbol> timesClass;

        private readonly Lazy<INamedTypeSymbol> eventArgsClass;

        private readonly Lazy<INamedTypeSymbol> funcClass;

        private readonly Lazy<INamedTypeSymbol> taskClass;

        private readonly Lazy<INamedTypeSymbol> taskGenericClass;

        private MoqSymbols(INamedTypeSymbol mockGenericClass, Compilation compilation)
        {
            this.mockGenericClass = mockGenericClass;

            var setupConditionResultInterface = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("Moq.Language.ISetupConditionResult`1")!);

            this.mockBehaviorEnum = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("Moq.MockBehavior")!);
            this.isAnyTypeClass = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("Moq.It+IsAnyType")!);
            this.itIsMethods = new Lazy<IReadOnlyList<ISymbol>>(() => compilation.GetTypeByMetadataName("Moq.It")!.GetMembers("Is").ToArray());
            this.itIsAnyMethod = new Lazy<ISymbol>(() => compilation.GetTypeByMetadataName("Moq.It")!.GetMembers("IsAny").Single());
            this.verifiesInterface = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("Moq.Language.IVerifies")!);

            this.raiseMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.GetMembers("Raise").OfType<IMethodSymbol>().ToArray());
            this.raiseAsyncMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.GetMembers("RaiseAsync").OfType<IMethodSymbol>().ToArray());
            this.setupMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.GetMembers("Setup").Concat(setupConditionResultInterface.Value.GetMembers("Setup")).OfType<IMethodSymbol>().ToArray());
            this.mockBehaviorStrictField = new Lazy<ISymbol>(() => this.mockBehaviorEnum.Value.GetMembers("Strict").First());
            this.setupProtectedMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => compilation.GetTypeByMetadataName("Moq.Protected.IProtectedMock`1")!.GetMembers("Setup").OfType<IMethodSymbol>().ToArray());
            this.asMethod = new Lazy<ISymbol>(() => mockGenericClass.GetMembers("As").Single());
            this.verifyMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.GetMembers("Verify").Concat(mockGenericClass.BaseType!.GetMembers("Verify")).Where(m => !m.IsStatic).OfType<IMethodSymbol>().ToArray());

            this.staticVerifyMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.BaseType!.GetMembers("Verify").Where(m => m.IsStatic).OfType<IMethodSymbol>().Single());
            this.staticVerifyAllMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.BaseType!.GetMembers("VerifyAll").Where(m => m.IsStatic).OfType<IMethodSymbol>().Single());
            this.verifyAllMethod = new Lazy<IMethodSymbol>(() => mockGenericClass.BaseType!.GetMembers("VerifyAll").Where(m => !m.IsStatic).OfType<IMethodSymbol>().Single());

            this.verifiableMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => this.verifiesInterface.Value.GetMembers("Verifiable").OfType<IMethodSymbol>().ToArray());
            this.mockOfMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.BaseType!.GetMembers("Of").Where(m => m.IsStatic).OfType<IMethodSymbol>().ToArray());

            this.mockConstructorWithFactory = new Lazy<IMethodSymbol>(() => mockGenericClass.Constructors.Single(c => c.Parameters.Length == 2 && c.Parameters[0].Type.Name == "Expression"));

            this.setupSequenceMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.GetMembers("SetupSequence").OfType<IMethodSymbol>().ToArray());
            this.setupSetMethodWithoutGenericArgument = new Lazy<IMethodSymbol>(() => mockGenericClass.GetMembers("SetupSet").OfType<IMethodSymbol>().Single(c => c.TypeArguments.Length == 1));
            this.setupSetMethods = new Lazy<IReadOnlyList<IMethodSymbol>>(() => mockGenericClass.GetMembers("SetupSet").OfType<IMethodSymbol>().ToArray());

            this.timesClass = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("Moq.Times")!);

            this.eventArgsClass = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("System.EventArgs")!);
            this.funcClass = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("System.Func`1")!);
            this.taskClass = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!);
            this.taskGenericClass = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1")!);
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

        public bool ContainsTimesParameters(IMethodSymbol method)
        {
            foreach (var parameter in method.Parameters)
            {
                if (SymbolEqualityComparer.Default.Equals(parameter.Type, this.timesClass.Value))
                {
                    return true;
                }

                // Try the Func<Times> alternative.
                if (!SymbolEqualityComparer.Default.Equals(parameter.Type.OriginalDefinition, this.funcClass.Value))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public bool IsAnyType(ITypeSymbol symbol)
            => AreEqual(symbol, this.isAnyTypeClass);

        public ITypeSymbol? GetItIsType(ISymbol? symbol)
        {
            if (symbol is not IMethodSymbol methodSymbol)
            {
                return null;
            }

            foreach (var itIsMethod in this.itIsMethods.Value)
            {
                if (SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, itIsMethod))
                {
                    return methodSymbol.TypeArguments[0];
                }
            }

            return null;
        }

        public bool IsEventArgs(ISymbol? symbol)
            => AreEqual(symbol, this.eventArgsClass);

        public bool IsItIsAny(ISymbol? symbol)
            => AreEqual(symbol, this.itIsAnyMethod);

        public ITypeSymbol? GetItIsAnyType(ISymbol? symbol)
        {
            if (symbol is not IMethodSymbol methodSymbol)
            {
                return null;
            }

            if (!SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, this.itIsAnyMethod.Value))
            {
                return null;
            }

            return methodSymbol.TypeArguments[0];
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

        public bool IsRaiseMethod([NotNullWhen(true)] ISymbol? symbol)
            => AreEqual(symbol, this.raiseMethods);

        public bool IsRaiseAsyncMethod([NotNullWhen(true)] ISymbol? symbol)
            => AreEqual(symbol, this.raiseAsyncMethods);

        public bool IsSetupMethod(ISymbol? symbol)
            => AreEqual(symbol, this.setupMethods);

        public bool IsSetupProtectedMethod(ISymbol? symbol)
            => AreEqual(symbol, this.setupProtectedMethods);

        public bool IsSetupSequenceMethod(ISymbol? symbol)
            => AreEqual(symbol, this.setupSequenceMethods);

        public bool IsSetupSetMethod(ISymbol? symbol)
            => AreEqual(symbol, this.setupSetMethods);

        public bool IsSetupSetMethodWithoutGenericArgument(ISymbol? symbol)
            => AreEqual(symbol, this.setupSetMethodWithoutGenericArgument);

        public bool IsTask(ISymbol? symbol)
            => AreEqual(symbol, this.taskClass);

        public bool IsTaskGeneric(ISymbol? symbol)
            => AreEqual(symbol, this.taskGenericClass);

        public bool IsVerifiableMethod([NotNullWhen(true)] ISymbol? symbol)
            => AreEqual(symbol, this.verifiableMethods);

        public bool IsVerifyMethod([NotNullWhen(true)] ISymbol? symbol)
            => AreEqual(symbol, this.verifyMethods);

        public bool IsVerifyStaticMethod([NotNullWhen(true)] ISymbol? symbol)
            => AreEqual(symbol, this.staticVerifyMethod);

        public bool IsVerifyAllMethod(ISymbol? symbol)
            => AreEqual(symbol, this.verifyAllMethod);

        public bool IsVerifyAllStaticMethod([NotNullWhen(true)] ISymbol? symbol)
            => AreEqual(symbol, this.staticVerifyAllMethod);

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
            => AreEqual(symbol, this.mockBehaviorEnum);

        public bool IsMockBehaviorStrictField(ISymbol? symbol)
            => AreEqual(symbol, this.mockBehaviorStrictField);

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

        public bool IsMockOfMethod(ISymbol? symbol)
            => AreEqual(symbol, this.mockOfMethods);

        public bool IsMockConstructorWithFactory(ISymbol? symbol)
            => AreEqual(symbol, this.mockConstructorWithFactory);

        public bool IsAsMethod(IMethodSymbol symbol)
            => AreEqual(symbol, this.asMethod);

        private static bool AreEqual<TSymbol>([NotNullWhen(true)] ISymbol? symbol1, Lazy<TSymbol> symbol2)
            where TSymbol : ISymbol
        {
            if (symbol1 is null)
            {
                return false;
            }

            if (!SymbolEqualityComparer.Default.Equals(symbol1.OriginalDefinition, symbol2.Value))
            {
                return false;
            }

            return true;
        }

        private static bool AreEqual<TSymbol>([NotNullWhen(true)] ISymbol? symbol1, Lazy<IReadOnlyList<TSymbol>> symbols2)
            where TSymbol : ISymbol
        {
            if (symbol1 is null)
            {
                return false;
            }

            var originalDefinition = symbol1.OriginalDefinition;

            foreach (var symbol in symbols2.Value)
            {
                if (SymbolEqualityComparer.Default.Equals(originalDefinition, symbol))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
