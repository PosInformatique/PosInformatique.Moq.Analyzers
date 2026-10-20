//-----------------------------------------------------------------------
// <copyright file="MoqAnalyzersTests.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    public static class MoqAnalyzersTests
    {
        public const string FakeMoqLibrary =
            """
            namespace FakeMoqLibrary
            {
                using System;
                using System.Linq.Expressions;

                public class Mock<T>
                {
                    public void As<TInterface>() { }

                    public void Setup(Expression<Action<T>> expression) { }

                    public void SetupSet(Action<T> action) { }

                    public void SetupSet<TProperty>(Action<T> action) { }

                    public void SetupSet<TProperty>(Func<T, TProperty> action) { }
                }
            }
            """;
    }
}
