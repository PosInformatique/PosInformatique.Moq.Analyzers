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

                public enum MockBehavior
                {
                    Strict,
                }

                public abstract class Mock
                {
                    public static T Of<T>() { return default; }

                    public static T Of<T>(MockBehavior behavior) { return default; }

                    public static T Of<T>(Expression<Func<T, bool>> predicate) { return default; }

                    public static T Of<T>(Expression<Func<T, bool>> predicate, MockBehavior behavior) { return default; }

                    public static void Verify(params Mock[] mocks) { }

                    public static void VerifyAll(params Mock[] mocks) { }
                }

                public class Mock<T> : Mock
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
