//-----------------------------------------------------------------------
// <copyright file="NotNullWhenAttribute.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Diagnostics.CodeAnalysis
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) => this.ReturnValue = returnValue;

        public bool ReturnValue { get; }
    }
}
