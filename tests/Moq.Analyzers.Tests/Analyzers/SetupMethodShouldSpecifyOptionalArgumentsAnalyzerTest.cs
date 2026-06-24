//-----------------------------------------------------------------------
// <copyright file="SetupMethodShouldSpecifyOptionalArgumentsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<SetupMethodShouldSpecifyOptionalArgumentsAnalyzer>;

    public class SetupMethodShouldSpecifyOptionalArgumentsAnalyzerTest
    {
        [Theory]
        [InlineData("")]
        [InlineData(".InSequence(sequence)")]
        public async Task SetupMethodOptionalArguments_NoDiagnosticReported(string sequence)
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
                            var sequence = new MockSequence();

                            var mock1 = new Mock<I>();

                            mock1" + sequence + @".Setup(m => m.TestMethod())
                                .Callback(() => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestMethod(""A""))
                                .Callback((string x) => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestMethod(""A"", 10))
                                .Callback((string x, int y) => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.TestMethodWithDefaultParameters(""A"", 10, 20, ""B""))
                                .Callback((string x, int y, int z, string w) => { })
                                .Throws(new Exception());
                        }
                    }

                    public interface I
                    {
                        void TestMethod();

                        void TestMethod(string a);

                        void TestMethod(string a, int b);

                        void TestMethodWithDefaultParameters(string a, int b, int c = 42, string d = ""default"");
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("")]
        [InlineData(".InSequence(sequence)")]
        public async Task SetupMethodOptionalArguments_MissingArguments_DiagnosticReported(string sequence)
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
                            var sequence = new MockSequence();

                            var mock1 = new Mock<I>();

                            mock1" + sequence + @".Setup(m => m.{|PosInfoMoq1011:TestMethodWithDefaultParameters|}(""A"", 10))
                                .Callback(() => { })
                                .Throws(new Exception());
                            mock1" + sequence + @".Setup(m => m.{|PosInfoMoq1011:TestMethodWithDefaultParameters|}(""A"", 10, 20))
                                .Callback(() => { })
                                .Throws(new Exception());
                        }
                    }

                    public interface I
                    {
                        void TestMethod();

                        void TestMethod(string a);

                        void TestMethod(string a, int b);

                        void TestMethodWithDefaultParameters(string a, int b, int c = 42, string d = ""D"");
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task CustomSetupMethod_NoDiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using Moq.Language.Flow;
                    using System;
                    using System.Linq.Expressions;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var m = new Mock<IRepository>(MockBehavior.Strict);

                            m.MySetup(i => i.Get(""Foobar""));
                        }
                    }

                    public static class MoqExtensions
                    {
                        public static ISetup<T> MySetup<T>(this Mock<T> mock, Expression<Action<T>> exp)
                            where T : class
                        {
                            return mock.Setup(exp);
                        }
                    }

                    public interface IRepository
                    {
                        void Get(string s);
                    }
                }
                ";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoMoqLibrary()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using OtherNamespace;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            mock1.Setup(m => m.TestMethod());
                        }
                    }

                    public interface I
                    {
                        void TestMethod();
                    }
                }

                namespace OtherNamespace
                {
                    using System;

                    public class Mock<T>
                    {
                        public Mock(MockBehavior _) { }
 
                        public void Setup(Action<T> act) { }
                   }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}