//-----------------------------------------------------------------------
// <copyright file="MoqLibrary.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    public static class MoqLibrary
    {
        public const string Code = @"
            namespace Moq
            {
                using System;
                using System.Linq.Expressions;

                public class Mock<T>
                {
                    public Mock(MockBehavior _ = MockBehavior.Loose, params object[] args) { }

                    public Mock(params object[] args) { }

                    public Mock<TInterface> As<TInterface>() { return null; }

                    public ISetup Setup(Action<T> act) { return null; }

                    public ISetup<TResult> Setup<TResult>(Func<T, TResult> func) { return default; }

                    public void VerifyAll() { }

                    public void Verify() { }

                    public void Verify(Expression<Action<T>> _) { }

                    public void Verify<TResult>(Expression<Func<T, TResult>> _) { }

                    public void Verify(Expression<Action<T>> _, string failedMessage) { }

                    public void Verify<TResult>(Expression<Func<T, TResult>> _, string failedMessage) { }

                    public object Property { get; set; }
                }

                public enum MockBehavior { Strict, Loose }

                public interface ISetup
                {
                    ISetup Callback();

                    ISetup Callback(Action _);

                    ISetup Callback<T1>(Action<T1> _);

                    ISetup Callback<T1, T2>(Action<T1, T2> _);

                    ISetup Callback<T1, T2, T3>(Action<T1, T2, T3> _);

                    ISetup Callback<T1, TReturn>(Func<T1, TReturn> _);

                    ISetup Callback<T1, T2, TReturn>(Func<T1, T2, TReturn> _);

                    ISetup Callback<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> _);

                    ISetup Property { get; }

                    ISetup Throws();

                    ISetup ThrowsAsync();
                }

                public interface ISetup<TResult>
                {
                    ISetup<TResult> Callback();

                    ISetup<TResult> Callback(Action _);

                    ISetup<TResult> Callback<T1>(Action<T1> _);

                    ISetup<TResult> Callback<T1, T2>(Action<T1, T2> _);

                    ISetup<TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> _);

                    ISetup<TResult> Callback<T1, TReturn>(Func<T1, TReturn> _);

                    ISetup<TResult> Callback<T1, T2, TReturn>(Func<T1, T2, TReturn> _);

                    ISetup<TResult> Callback<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> _);

                    ISetup<TResult> Property { get; }

                    ISetup<TResult> Returns(TResult result);

                    ISetup<TResult> ReturnsAsync(TResult result);

                    ISetup<TResult> Throws();

                    ISetup<TResult> ThrowsAsync();
                }

                public static class It
                {
                    public static TValue IsAny<TValue>() { return default; }

                    public sealed class IsAnyType
                    {
                    }
                }
            }
            namespace Moq.Protected
            {
                public static class ProtectedExtension
                {
                    /// <summary>
                    public static IProtectedMock<T> Protected<T>(this Mock<T> mock)
                        where T : class
                    {
                        return null;
                    }
                }

                public interface IProtectedMock<TMock>
                {
                    ISetup Setup(string voidMethodName, params object[] args);

                    ISetup Setup<TResult>(string voidMethodName, params object[] args);
                }
            }
";
    }
}
