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

                    public void VerifyAll() { }

                    public void Verify() { }

                    public void Verify(int a, int b) { }

                    public object Property { get; set; }
                }

                public enum MockBehavior { Strict, Loose }

                public interface ISetup
                {
                    ISetup Callback();

                    ISetup Property { get; }

                    ISetup Returns();

                    ISetup ReturnsAsync();

                    ISetup Throws();

                    ISetup ThrowsAsync();
                }
            }";
    }
}
