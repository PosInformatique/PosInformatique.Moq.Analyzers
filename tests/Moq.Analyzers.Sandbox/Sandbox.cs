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
        public void T()
        {
            var repository = Mock.Of<IRepository>();
        }
    }
}


















    public class IRepository
    {
        //void Method(int a, string s);
    }
