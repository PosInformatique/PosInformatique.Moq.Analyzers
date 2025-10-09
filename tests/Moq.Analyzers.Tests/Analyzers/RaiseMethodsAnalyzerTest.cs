//-----------------------------------------------------------------------
// <copyright file="RaiseMethodsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.Testing;
    using Verifier = MoqCSharpAnalyzerVerifier<RaiseMethodsAnalyzer>;

    public class RaiseMethodsAnalyzerTest
    {
        [Theory]
        [InlineData("Raise", "void")]
        [InlineData("RaiseAsync", "Task")]
        [InlineData("RaiseAsync", "Task<int>")]
        public async Task Raise_WithParams_NoDiagnosticReported(string method, string eventReturnType)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);

                            mock1." + method + @"(m => m.TheEvent += null, ""OK"", 1, null);
                            mock1." + method + @"(m => m.TheEvent += null, ""OK"", 1, 10);
                            mock1." + method + @"(m => m.TheEvent += null, ""OK"", 1, new object());

                            mock1." + method + @"(m => m.TheEvent -= null, ""OK"", 1, null);
                            mock1." + method + @"(m => m.TheEvent -= null, ""OK"", 1, 10);
                            mock1." + method + @"(m => m.TheEvent -= null, ""OK"", 1, new object());
                       }
                    }

                    public interface I
                    {
                        event CustomEventHandler TheEvent;
                    }

                    public delegate " + eventReturnType + @" CustomEventHandler(string a, int b, object c);
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task Raise_WithEventArgs_NoDiagnosticReported()
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
                            var mock1 = new Mock<I>(MockBehavior.Strict);

                            mock1.Raise(m => m.TheEvent += null, new CustomEventArgs());
                            mock1.Raise(m => m.TheEvent += null, null, new CustomEventArgs());
                            mock1.Raise(m => m.TheEvent += null, 10, new CustomEventArgs());
                            mock1.Raise(m => m.TheEvent += null, new object(), new CustomEventArgs());

                            mock1.Raise(m => m.TheEvent -= null, new CustomEventArgs());
                            mock1.Raise(m => m.TheEvent -= null, null, new CustomEventArgs());
                            mock1.Raise(m => m.TheEvent -= null, 10, new CustomEventArgs());
                            mock1.Raise(m => m.TheEvent -= null, new object(), new CustomEventArgs());
                        }
                    }

                    public interface I
                    {
                        event EventHandler<CustomEventArgs> TheEvent;
                    }

                    public class CustomEventArgs : EventArgs
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("Raise", "void")]
        [InlineData("RaiseAsync", "Task")]
        [InlineData("RaiseAsync", "Task<int>")]
        public async Task Raise_WithParams_MissingArgument_DiagnosticReported(string method, string eventReturnType)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);

                            mock1.{|#0:" + method + @"|#0}(m => m.TheEvent += null);
                            mock1.{|#1:" + method + @"|#1}(m => m.TheEvent += null, 1);
                            mock1.{|#2:" + method + @"|#2}(m => m.TheEvent += null, 1, 2, ""3"", 4, 5, 6);
                       }
                    }

                    public interface I
                    {
                        event CustomEventHandler TheEvent;
                    }

                    public delegate " + eventReturnType + @" CustomEventHandler(string a, int b, object c);
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                [
                    new DiagnosticResult(RaiseMethodsAnalyzer.ParametersMustMatchSignature)
                        .WithLocation(0)
                        .WithArguments("The event 'TheEvent' expects 3 argument(s) but 0 were provided."),
                    new DiagnosticResult(RaiseMethodsAnalyzer.ParametersMustMatchSignature)
                        .WithLocation(1)
                        .WithArguments("The event 'TheEvent' expects 3 argument(s) but 1 were provided."),
                    new DiagnosticResult(RaiseMethodsAnalyzer.ParametersMustMatchSignature)
                        .WithLocation(2)
                        .WithArguments("The event 'TheEvent' expects 3 argument(s) but 6 were provided."),
                ]);
        }

        [Theory]
        [InlineData("Raise", "void")]
        [InlineData("RaiseAsync", "Task")]
        [InlineData("RaiseAsync", "Task<int>")]
        public async Task Raise_WithParams_WrongType_DiagnosticReported(string method, string eventReturnType)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);

                            mock1." + method + @"(m => m.TheEvent += null, {|#0:1|#0}, 2, ""OK"", {|#1:""Wrong""|#1}, null, {|#2:null|#2});
                        }
                    }

                    public interface I
                    {
                        event CustomEventHandler TheEvent;
                    }

                    public delegate " + eventReturnType + @" CustomEventHandler(string a, int b, object c, int d, string e, int f);
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                [
                    new DiagnosticResult(RaiseMethodsAnalyzer.ParametersMustMatchSignature)
                        .WithLocation(0)
                        .WithArguments("The parameter 'a' of the event 'TheEvent' expects a value of type 'String' but a value of type 'Int32' was provided."),
                    new DiagnosticResult(RaiseMethodsAnalyzer.ParametersMustMatchSignature)
                        .WithLocation(1)
                        .WithArguments("The parameter 'd' of the event 'TheEvent' expects a value of type 'Int32' but a value of type 'String' was provided."),
                    new DiagnosticResult(RaiseMethodsAnalyzer.ParametersMustMatchSignature)
                        .WithLocation(2)
                        .WithArguments("The parameter 'f' of the event 'TheEvent' expects a value of type 'Int32' but a value 'null' was provided."),
                ]);
        }

        [Fact]
        public async Task Raise_WithEventArgs_WrongType_DiagnosticReported()
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
                            var mock1 = new Mock<I>(MockBehavior.Strict);

                            mock1.Raise(m => m.TheEvent += null, {|#0:new EventArgs()|#0});
                        }
                    }

                    public interface I
                    {
                        event EventHandler<CustomEventArgs> TheEvent;
                    }

                    public class CustomEventArgs : EventArgs
                    {
                    }
                }";

            await Verifier.VerifyAnalyzerAsync(
                source,
                [
                    new DiagnosticResult(RaiseMethodsAnalyzer.ParametersMustMatchSignature)
                        .WithLocation(0)
                        .WithArguments("The parameter 'e' of the event 'TheEvent' expects a value of type 'CustomEventArgs' but a value of type 'EventArgs' was provided."),
                ]);
        }

        [Theory]
        [InlineData("Raise", "void", "\"OK\", 1, null")]
        [InlineData("Raise", "void", "\"OK\", 1, 10")]
        [InlineData("Raise", "void", "\"OK\", 1, new object()")]
        [InlineData("RaiseAsync", "Task", "\"OK\", 1, null")]
        [InlineData("RaiseAsync", "Task", "\"OK\", 1, 10")]
        [InlineData("RaiseAsync", "Task", "\"OK\", 1, new object()")]
        [InlineData("RaiseAsync", "Task<int>", "\"OK\", 1, null")]
        [InlineData("RaiseAsync", "Task<int>", "\"OK\", 1, 10")]
        [InlineData("RaiseAsync", "Task<int>", "\"OK\", 1, new object()")]
        public async Task Raise_WrongLambdaExpression_DiagnosticReported(string method, string eventReturnType, string parameters)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var act = (I i) => i.TheEvent += null;
                            var tmp = 0;

                            var mock1 = new Mock<I>(MockBehavior.Strict);

                            mock1." + method + @"({|PosInfoMoq2018:null|});
                            mock1." + method + @"({|PosInfoMoq2018:null|}, " + parameters + @");
                            mock1." + method + @"({|PosInfoMoq2018:m => m.ToString()|}, "" + parameters + @"");
                            mock1." + method + @"({|PosInfoMoq2018:act|}, "" + parameters + @"");
                            mock1." + method + @"({|PosInfoMoq2018:m => new object()|}, "" + parameters + @"");
                            mock1." + method + @"({|PosInfoMoq2018:m => tmp = 10|}, "" + parameters + @"");
                            mock1." + method + @"({|PosInfoMoq2018:m => m.Property = 10|}, "" + parameters + @"");
                       }
                    }

                    public interface I
                    {
                        event CustomEventHandler TheEvent;

                        int Property { get; set; }
                    }

                    public delegate " + eventReturnType + @" CustomEventHandler(string a, int b, object c);
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [InlineData("Raise", "void")]
        [InlineData("RaiseAsync", "Task")]
        [InlineData("RaiseAsync", "Task<int>")]
        public async Task Raise_WithNotNullDelegate_NoDiagnosticReported(string method, string eventReturnType)
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);

                            mock1." + method + @"(m => m.TheEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, ""OK"", 1, null);
                            mock1." + method + @"(m => m.TheEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, ""OK"", 1, 10);
                            mock1." + method + @"(m => m.TheEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, ""OK"", 1, new object());

                            mock1." + method + @"(m => m.TheEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, ""OK"", 1, null);
                            mock1." + method + @"(m => m.TheEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, ""OK"", 1, 10);
                            mock1." + method + @"(m => m.TheEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, ""OK"", 1, new object());
                        }

                        public static " + eventReturnType + @" MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
                    }

                    public interface I
                    {
                        event CustomEventHandler TheEvent;
                    }

                    public delegate " + eventReturnType + @" CustomEventHandler(string a, int b, object c);
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task RaiseAsync_WithNoAsynchronousEvent_NoDiagnosticReported()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);

                            mock1.{|PosInfoMoq2019:RaiseAsync|}(m => m.TheEvent += null, ""OK"", 1, new object());
                       }
                    }

                    public interface I
                    {
                        event CustomEventHandler TheEvent;
                    }

                    public delegate void CustomEventHandler(string a, int b, object c);
                }";

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task NoMoqLibrary()
        {
            var source = @"
                namespace ConsoleApplication1
                {
                    using System;
                    using OtherNamespace;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock1 = new Mock<I>(MockBehavior.Strict);
                            mock1.Raise(m => m.EventWithSenderAndString += null, new EventArgs());
                            mock1.Raise(m => m.EventWithSenderAndString += null, 1, 2, 3, 4);
                        }
                    }

                    public interface I
                    {
                        event EventHandler<string> EventWithSenderAndString;
                    }
                }

                namespace OtherNamespace
                {
                    using System;

                    public class Mock<T>
                    {
                        public Mock(MockBehavior _) { }
 
                        public void Raise(Action<T> act, EventArgs e) { }
                        public void Raise(Action<T> act, params object[] args) { }
                   }

                    public enum MockBehavior { Strict, Loose }
                }";

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}