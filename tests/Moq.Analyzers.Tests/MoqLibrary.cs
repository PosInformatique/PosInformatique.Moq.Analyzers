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

                public class Mock<T>
                {
                    public Mock(MockBehavior _ = MockBehavior.Loose, params object[] args) { }

                    public Mock(params object[] args) { }

                    public ISetup Setup(Action<T> act) { return null; }

                    public ISetup Setup(Func<T, object> func) { return null; }

                    public void VerifyAll() { }

                    public void Verify() { }

                    public void Verify(int a, int b) { }

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

                    ISetup Returns();

                    ISetup ReturnsAsync();

                    ISetup Throws();

                    ISetup ThrowsAsync();
                }
            }";
    }
}
