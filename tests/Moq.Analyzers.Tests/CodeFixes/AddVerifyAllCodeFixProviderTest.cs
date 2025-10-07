//-----------------------------------------------------------------------
// <copyright file="AddVerifyAllCodeFixProviderTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpCodeFixVerifier<VerifyAllShouldBeCalledAnalyzer, AddVerifyAllCodeFixProvider>;

    public class AddVerifyAllCodeFixProviderTest
    {
        [Fact]
        public async Task AddVerifyAllBeforeOtherMocks_Fix()
        {
            var source = @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mock1 = {|PosInfoMoq1000:new Mock<I>()|};
            mock1.Setup(i => i.Method());

            var mock2 = new Mock<I>();
            mock2.Setup(i => i.Method());

            var mock3 = new Mock<I>();
            mock3.Setup(i => i.Method());

            // No changes
            mock2.VerifyAll();
            mock3.VerifyAll();
        }
    }

    public interface I
    {
        void Method();
    }
}";

            var expectedFixedSource =
            @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mock1 = new Mock<I>();
            mock1.Setup(i => i.Method());

            var mock2 = new Mock<I>();
            mock2.Setup(i => i.Method());

            var mock3 = new Mock<I>();
            mock3.Setup(i => i.Method());

            // No changes
            mock1.VerifyAll();
            mock2.VerifyAll();
            mock3.VerifyAll();
        }
    }

    public interface I
    {
        void Method();
    }
}";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Fact]
        public async Task AddVerifyAllInTheMiddleOfOtherMocks_Fix()
        {
            var source = @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mock1 = new Mock<I>();
            mock1.Setup(i => i.Method());

            var mock2 = {|PosInfoMoq1000:new Mock<I>()|};
            mock2.Setup(i => i.Method());

            var mock3 = new Mock<I>();
            mock3.Setup(i => i.Method());

            // No changes
            mock1.VerifyAll();
            mock3.VerifyAll();
        }
    }

    public interface I
    {
        void Method();
    }
}";

            var expectedFixedSource =
            @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mock1 = new Mock<I>();
            mock1.Setup(i => i.Method());

            var mock2 = new Mock<I>();
            mock2.Setup(i => i.Method());

            var mock3 = new Mock<I>();
            mock3.Setup(i => i.Method());

            // No changes
            mock1.VerifyAll();
            mock2.VerifyAll();
            mock3.VerifyAll();
        }
    }

    public interface I
    {
        void Method();
    }
}";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Fact]
        public async Task AddVerifyAllAfterOtherMocks_Fix()
        {
            var source = @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mockzzz = {|PosInfoMoq1000:new Mock<I>()|};
            mockzzz.Setup(i => i.Method());

            var mock2 = new Mock<I>();
            mock2.Setup(i => i.Method());

            var mock3 = new Mock<I>();
            mock3.Setup(i => i.Method());

            // No changes
            mock2.VerifyAll();
            mock3.VerifyAll();
        }
    }

    public interface I
    {
        void Method();
    }
}";

            var expectedFixedSource =
            @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mockzzz = new Mock<I>();
            mockzzz.Setup(i => i.Method());

            var mock2 = new Mock<I>();
            mock2.Setup(i => i.Method());

            var mock3 = new Mock<I>();
            mock3.Setup(i => i.Method());

            // No changes
            mock2.VerifyAll();
            mock3.VerifyAll();
            mockzzz.VerifyAll();
        }
    }

    public interface I
    {
        void Method();
    }
}";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Fact]
        public async Task AddVerifyAllSingleMock()
        {
            var source = @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mock1 = {|PosInfoMoq1000:new Mock<I>()|};
            mock1.Setup(i => i.Method());
        }
    }

    public interface I
    {
        void Method();
    }
}";

            var expectedFixedSource =
            @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mock1 = new Mock<I>();
            mock1.Setup(i => i.Method());

            mock1.VerifyAll();
        }
    }

    public interface I
    {
        void Method();
    }
}";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }

        [Fact]
        public async Task AddVerifyAllSingleMock_WithCommentAtTheEnd()
        {
            var source = @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mock1 = {|PosInfoMoq1000:new Mock<I>()|};
            mock1.Setup(i => i.Method());

            // End of unit tests
        }
    }

    public interface I
    {
        void Method();
    }
}";

            var expectedFixedSource =
            @"
namespace ConsoleApplication1
{
    using Moq;
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            var mock1 = new Mock<I>();
            mock1.Setup(i => i.Method());

            // End of unit tests

            mock1.VerifyAll();
        }
    }

    public interface I
    {
        void Method();
    }
}";

            await Verifier.VerifyCodeFixAsync(source, expectedFixedSource);
        }
    }
}
