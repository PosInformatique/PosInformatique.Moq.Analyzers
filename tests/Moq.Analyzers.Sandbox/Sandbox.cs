//-----------------------------------------------------------------------
// <copyright file="Sandbox.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Sandbox
{
    using global::Moq;

    public class Sandbox
    {
        [Fact]
        public void TestCodeHere()
        {
            var m = new Mock<IRepository>();
            m.Setup(m => m.GetData())
                .Verifiable();

            m.VerifyAll();
        }

        public interface IRepository
        {
            void GetData();
        }
    }
}