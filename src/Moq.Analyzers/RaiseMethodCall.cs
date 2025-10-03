//-----------------------------------------------------------------------
// <copyright file="RaiseMethodCall.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class RaiseMethodCall
    {
        public RaiseMethodCall(IMethodSymbol method, IReadOnlyList<ITypeSymbol?> methodParameters, IReadOnlyList<ArgumentSyntax> methodArguments, IEventSymbol @event)
        {
            this.Method = method;
            this.Event = @event;
            this.MethodArguments = methodArguments;
            this.MethodParameters = methodParameters;

            this.EventParameters = ((INamedTypeSymbol)@event.Type).DelegateInvokeMethod!.Parameters;
        }

        public IMethodSymbol Method { get; }

        public IReadOnlyList<ArgumentSyntax> MethodArguments { get; }

        public IReadOnlyList<ITypeSymbol?> MethodParameters { get; }

        public IEventSymbol Event { get; }

        public IReadOnlyList<IParameterSymbol> EventParameters { get; }
    }
}
