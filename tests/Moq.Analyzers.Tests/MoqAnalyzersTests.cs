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
                using System.Collections.Generic;
                using System.Linq.Expressions;

                public static class It
                {
                    public static TValue IsAny<TValue>() { return default; }

                    public static TValue Is<TValue>(Expression<Func<TValue, bool>> match) { return default; }

                    public static TValue Is<TValue>(TValue value, IEqualityComparer<TValue> comparer) { return default; }
                }

                public enum MockBehavior
                {
                    Strict,
                }

                public struct Times
                {
                    public static Times AtLeast(int callCount) { return default; }

                    public static Times Exactly(int callCount) { return default; }

                    public static Times Never() { return default; }

                    public static Times Once() { return default; }
                }

                public class SetupResult
                {
                    public void Verifiable() { }

                    public void Verifiable(string failMessage) { }

                    public void Verifiable(Times times, string failMessage = null) { }

                    public void Verifiable(Func<Times> times, string failMessage = null) { }
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
                    public Mock() { }

                    public Mock(MockBehavior behavior) { }

                    public Mock(params object[] args) { }

                    public Mock(MockBehavior behavior, params object[] args) { }

                    public Mock(Expression<Func<T>> newExpression, MockBehavior behavior = default) { }

                    public void As<TInterface>() { }

                    public void Setup(Expression<Action<T>> expression) { }

                    public SetupResult Setup<TResult>(Expression<Func<T, TResult>> expression) { return default; }

                    public void SetupSet(Action<T> action) { }

                    public void SetupSet<TProperty>(Action<T> action) { }

                    public void SetupSet<TProperty>(Func<T, TProperty> action) { }

                    public void Verify() { }

                    public void Verify(Expression<Action<T>> expression, string failMessage = null) { }

                    public void Verify(Expression<Action<T>> expression, Times times, string failMessage = null) { }

                    public void Verify(Expression<Action<T>> expression, Func<Times> times, string failMessage = null) { }
                }
            }

            namespace FakeMoqLibrary.Protected
            {
                public static class ProtectedExtension
                {
                    public static IProtectedMock<T> Protected<T>(this Mock<T> mock) where T : class { return default; }
                }

                public interface IProtectedMock<T> where T : class
                {
                    void Setup(string methodName, params object[] args);

                    void Setup<TResult>(string methodName, params object[] args);
                }
            }
            """;
    }
}
