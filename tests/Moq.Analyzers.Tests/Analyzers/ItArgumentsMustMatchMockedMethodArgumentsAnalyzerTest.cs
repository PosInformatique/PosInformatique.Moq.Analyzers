//-----------------------------------------------------------------------
// <copyright file="ItArgumentsMustMatchMockedMethodArgumentsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<ItArgumentsMustMatchMockedMethodArgumentsAnalyzer>;

    public class ItArgumentsMustMatchMockedMethodArgumentsAnalyzerTest
    {
        [Fact]
        public async Task Match_NoDiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Collections.Generic;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();

                            mock1.Setup(m => m.NoParametersMethod());
                            mock1.Setup(m => m.TestMethod(It.IsAny<int?>(), 1, ""Ignored"", C.OtherMethod()));
                            mock1.Setup(m => m.TestMethod(It.Is<int?>(p => p == 10), 1, ""Ignored"", C.OtherMethod()));
                            mock1.Setup(m => m.TestMethod(It.Is<int?>(10, EqualityComparer<int?>.Default), 1, ""Ignored"", C.OtherMethod()));

                            mock1.Setup(m => m.TestMethodWithBaseClass(null));
                            mock1.Setup(m => m.TestMethodWithBaseClass(It.IsAny<BaseClass>()));
                            mock1.Setup(m => m.TestMethodWithBaseClass(It.IsAny<InheritedClass>()));
                            mock1.Setup(m => m.TestMethodWithBaseClass(It.Is<BaseClass>(c => c.Property == 10)));
                            mock1.Setup(m => m.TestMethodWithBaseClass(It.Is<InheritedClass>(c => c.Property == 10)));
                       }
                    }

                    public interface I
                    {
                        void NoParametersMethod();

                        void TestMethod(int? a, int b, string c, int d);

                        void TestMethodWithBaseClass(BaseClass a);
                    }

                    public class C
                    {
                        public static int OtherMethod() => default;
                    }

                    public abstract class BaseClass
                    {
                        public int Property { get; set; }
                    }

                    public class InheritedClass : BaseClass
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NotMatch_DiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Collections.Generic;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>();
                            mock1.Setup(m => m.TestMethod({|#0:It.IsAny<int>()|#0}, 1, ""Ignored""));
                            mock1.Setup(m => m.TestMethod({|#1:It.Is<int>(p => p == 10)|#1}, 1, ""Ignored""));
                            mock1.Setup(m => m.TestMethod({|#2:It.Is<int>(10, EqualityComparer<int>.Default)|#2}, 1, ""Ignored""));
                       }
                    }

                    public interface I
                    {
                        void TestMethod(int? a, int b, string c);
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                [
                    new DiagnosticResult(ItArgumentsMustMatchMockedMethodArgumentsAnalyzer.Rule)
                        .WithLocation(0),
                    new DiagnosticResult(ItArgumentsMustMatchMockedMethodArgumentsAnalyzer.Rule)
                        .WithLocation(1),
                    new DiagnosticResult(ItArgumentsMustMatchMockedMethodArgumentsAnalyzer.Rule)
                        .WithLocation(2),
                ]);
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