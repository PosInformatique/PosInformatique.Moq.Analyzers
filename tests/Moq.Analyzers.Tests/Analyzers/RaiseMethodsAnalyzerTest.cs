//-----------------------------------------------------------------------
// <copyright file="RaiseMethodsAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<RaiseMethodsAnalyzer>;

    public class RaiseMethodsAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I
            {
                event CustomEventHandler TheEvent;

                event AsyncEventHandler AsyncEvent;

                event AsyncResultEventHandler AsyncResultEvent;

                event ExtendedEventHandler ExtendedEvent;

                event ExtendedAsyncEventHandler ExtendedAsyncEvent;

                event ExtendedAsyncResultEventHandler ExtendedAsyncResultEvent;

                event EventHandler<CustomEventArgs> EventWithArgs;

                event EventHandler<string> EventWithSenderAndString;

                int Property { get; set; }
            }

            public delegate void CustomEventHandler(string a, int b, object c);

            public delegate Task AsyncEventHandler(string a, int b, object c);

            public delegate Task<int> AsyncResultEventHandler(string a, int b, object c);

            public delegate void ExtendedEventHandler(string a, int b, object c, int d, string e, int f);

            public delegate Task ExtendedAsyncEventHandler(string a, int b, object c, int d, string e, int f);

            public delegate Task<int> ExtendedAsyncResultEventHandler(string a, int b, object c, int d, string e, int f);

            public class CustomEventArgs : EventArgs { }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """mock.Raise(m => m.TheEvent += null, "OK", 1, null);""",

            """mock.Raise(m => m.TheEvent += null, "OK", 1, 10);""",

            """mock.Raise(m => m.TheEvent += null, "OK", 1, new object());""",

            """mock.Raise(m => m.TheEvent -= null, "OK", 1, null);""",

            """mock.Raise(m => m.TheEvent -= null, "OK", 1, 10);""",

            """mock.Raise(m => m.TheEvent -= null, "OK", 1, new object());""",

            """mock.RaiseAsync(m => m.AsyncEvent += null, "OK", 1, null);""",

            """mock.RaiseAsync(m => m.AsyncEvent += null, "OK", 1, 10);""",

            """mock.RaiseAsync(m => m.AsyncEvent += null, "OK", 1, new object());""",

            """mock.RaiseAsync(m => m.AsyncEvent -= null, "OK", 1, null);""",

            """mock.RaiseAsync(m => m.AsyncEvent -= null, "OK", 1, 10);""",

            """mock.RaiseAsync(m => m.AsyncEvent -= null, "OK", 1, new object());""",

            """mock.RaiseAsync(m => m.AsyncResultEvent += null, "OK", 1, null);""",

            """mock.RaiseAsync(m => m.AsyncResultEvent += null, "OK", 1, 10);""",

            """mock.RaiseAsync(m => m.AsyncResultEvent += null, "OK", 1, new object());""",

            """mock.RaiseAsync(m => m.AsyncResultEvent -= null, "OK", 1, null);""",

            """mock.RaiseAsync(m => m.AsyncResultEvent -= null, "OK", 1, 10);""",

            """mock.RaiseAsync(m => m.AsyncResultEvent -= null, "OK", 1, new object());""",

            """mock.Raise(m => m.EventWithArgs += null, new CustomEventArgs());""",

            """mock.Raise(m => m.EventWithArgs += null, null, new CustomEventArgs());""",

            """mock.Raise(m => m.EventWithArgs += null, 10, new CustomEventArgs());""",

            """mock.Raise(m => m.EventWithArgs += null, new object(), new CustomEventArgs());""",

            """mock.Raise(m => m.EventWithArgs -= null, new CustomEventArgs());""",

            """mock.Raise(m => m.EventWithArgs -= null, null, new CustomEventArgs());""",

            """mock.Raise(m => m.EventWithArgs -= null, 10, new CustomEventArgs());""",

            """mock.Raise(m => m.EventWithArgs -= null, new object(), new CustomEventArgs());""",
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """mock.{|PosInfoMoq2017:Raise|}(m => m.TheEvent += null);""",

            """mock.{|PosInfoMoq2017:Raise|}(m => m.TheEvent += null, 1);""",

            """mock.{|PosInfoMoq2017:Raise|}(m => m.TheEvent += null, 1, 2, "3", 4, 5, 6);""",

            """mock.Raise(m => m.ExtendedEvent += null, {|PosInfoMoq2017:1|}, 2, "OK", {|PosInfoMoq2017:"Wrong"|}, null, {|PosInfoMoq2017:null|});""",

            """mock.Raise({|PosInfoMoq2018:null|});""",

            """mock.Raise({|PosInfoMoq2018:null|}, "OK", 1, null);""",

            """mock.Raise({|PosInfoMoq2018:null|}, "OK", 1, 10);""",

            """mock.Raise({|PosInfoMoq2018:null|}, "OK", 1, new object());""",

            """mock.Raise({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, null);""",

            """mock.Raise({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, 10);""",

            """mock.Raise({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, new object());""",

            """
            var act = (I i) => i.TheEvent += null;
            mock.Raise({|PosInfoMoq2018:act|}, "OK", 1, null);
            """,

            """
            var act = (I i) => i.TheEvent += null;
            mock.Raise({|PosInfoMoq2018:act|}, "OK", 1, 10);
            """,

            """
            var act = (I i) => i.TheEvent += null;
            mock.Raise({|PosInfoMoq2018:act|}, "OK", 1, new object());
            """,

            """mock.Raise({|PosInfoMoq2018:m => new object()|}, "OK", 1, null);""",

            """mock.Raise({|PosInfoMoq2018:m => new object()|}, "OK", 1, 10);""",

            """mock.Raise({|PosInfoMoq2018:m => new object()|}, "OK", 1, new object());""",

            """
            var tmp = 0;
            mock.Raise({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, null);
            """,

            """
            var tmp = 0;
            mock.Raise({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, 10);
            """,

            """
            var tmp = 0;
            mock.Raise({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, new object());
            """,

            """mock.Raise({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, null);""",

            """mock.Raise({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, 10);""",

            """mock.Raise({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, new object());""",

            """
            mock.Raise(m => m.TheEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, null);
            static void MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """
            mock.Raise(m => m.TheEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, 10);
            static void MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """
            mock.Raise(m => m.TheEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, new object());
            static void MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """mock.Raise(m => m.TheEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, null);""",

            """mock.Raise(m => m.TheEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, 10);""",

            """mock.Raise(m => m.TheEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, new object());""",

            """mock.{|PosInfoMoq2017:RaiseAsync|}(m => m.AsyncEvent += null);""",

            """mock.{|PosInfoMoq2017:RaiseAsync|}(m => m.AsyncEvent += null, 1);""",

            """mock.{|PosInfoMoq2017:RaiseAsync|}(m => m.AsyncEvent += null, 1, 2, "3", 4, 5, 6);""",

            """mock.RaiseAsync(m => m.ExtendedAsyncEvent += null, {|PosInfoMoq2017:1|}, 2, "OK", {|PosInfoMoq2017:"Wrong"|}, null, {|PosInfoMoq2017:null|});""",

            """mock.RaiseAsync({|PosInfoMoq2018:null|});""",

            """mock.RaiseAsync({|PosInfoMoq2018:null|}, "OK", 1, null);""",

            """mock.RaiseAsync({|PosInfoMoq2018:null|}, "OK", 1, 10);""",

            """mock.RaiseAsync({|PosInfoMoq2018:null|}, "OK", 1, new object());""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, null);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, 10);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, new object());""",

            """
            var act = (I i) => i.AsyncEvent += null;
            mock.RaiseAsync({|PosInfoMoq2018:act|}, "OK", 1, null);
            """,

            """
            var act = (I i) => i.AsyncEvent += null;
            mock.RaiseAsync({|PosInfoMoq2018:act|}, "OK", 1, 10);
            """,

            """
            var act = (I i) => i.AsyncEvent += null;
            mock.RaiseAsync({|PosInfoMoq2018:act|}, "OK", 1, new object());
            """,

            """mock.RaiseAsync({|PosInfoMoq2018:m => new object()|}, "OK", 1, null);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => new object()|}, "OK", 1, 10);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => new object()|}, "OK", 1, new object());""",

            """
            var tmp = 0;
            mock.RaiseAsync({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, null);
            """,

            """
            var tmp = 0;
            mock.RaiseAsync({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, 10);
            """,

            """
            var tmp = 0;
            mock.RaiseAsync({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, new object());
            """,

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, null);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, 10);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, new object());""",

            """
            mock.RaiseAsync(m => m.AsyncEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, null);
            static Task MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """
            mock.RaiseAsync(m => m.AsyncEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, 10);
            static Task MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """
            mock.RaiseAsync(m => m.AsyncEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, new object());
            static Task MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """mock.RaiseAsync(m => m.AsyncEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, null);""",

            """mock.RaiseAsync(m => m.AsyncEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, 10);""",

            """mock.RaiseAsync(m => m.AsyncEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, new object());""",

            """mock.{|PosInfoMoq2017:RaiseAsync|}(m => m.AsyncResultEvent += null);""",

            """mock.{|PosInfoMoq2017:RaiseAsync|}(m => m.AsyncResultEvent += null, 1);""",

            """mock.{|PosInfoMoq2017:RaiseAsync|}(m => m.AsyncResultEvent += null, 1, 2, "3", 4, 5, 6);""",

            """mock.RaiseAsync(m => m.ExtendedAsyncResultEvent += null, {|PosInfoMoq2017:1|}, 2, "OK", {|PosInfoMoq2017:"Wrong"|}, null, {|PosInfoMoq2017:null|});""",

            """mock.RaiseAsync({|PosInfoMoq2018:null|});""",

            """mock.RaiseAsync({|PosInfoMoq2018:null|}, "OK", 1, null);""",

            """mock.RaiseAsync({|PosInfoMoq2018:null|}, "OK", 1, 10);""",

            """mock.RaiseAsync({|PosInfoMoq2018:null|}, "OK", 1, new object());""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, null);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, 10);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.ToString()|}, "OK", 1, new object());""",

            """
            var act = (I i) => i.AsyncResultEvent += null;
            mock.RaiseAsync({|PosInfoMoq2018:act|}, "OK", 1, null);
            """,

            """
            var act = (I i) => i.AsyncResultEvent += null;
            mock.RaiseAsync({|PosInfoMoq2018:act|}, "OK", 1, 10);
            """,

            """
            var act = (I i) => i.AsyncResultEvent += null;
            mock.RaiseAsync({|PosInfoMoq2018:act|}, "OK", 1, new object());
            """,

            """mock.RaiseAsync({|PosInfoMoq2018:m => new object()|}, "OK", 1, null);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => new object()|}, "OK", 1, 10);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => new object()|}, "OK", 1, new object());""",

            """
            var tmp = 0;
            mock.RaiseAsync({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, null);
            """,

            """
            var tmp = 0;
            mock.RaiseAsync({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, 10);
            """,

            """
            var tmp = 0;
            mock.RaiseAsync({|PosInfoMoq2018:m => tmp = 10|}, "OK", 1, new object());
            """,

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, null);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, 10);""",

            """mock.RaiseAsync({|PosInfoMoq2018:m => m.Property = 10|}, "OK", 1, new object());""",

            """
            mock.RaiseAsync(m => m.AsyncResultEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, null);
            static Task<int> MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """
            mock.RaiseAsync(m => m.AsyncResultEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, 10);
            static Task<int> MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """
            mock.RaiseAsync(m => m.AsyncResultEvent += {|PosInfoMoq1010:MethodMatchEventSignature|}, "OK", 1, new object());
            static Task<int> MethodMatchEventSignature(string a, int b, object c) { throw new NotSupportedException(); }
            """,

            """mock.RaiseAsync(m => m.AsyncResultEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, null);""",

            """mock.RaiseAsync(m => m.AsyncResultEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, 10);""",

            """mock.RaiseAsync(m => m.AsyncResultEvent += {|PosInfoMoq1010:(a, b, c) => throw new NotSupportedException()|}, "OK", 1, new object());""",

            """mock.Raise(m => m.EventWithArgs += null, {|PosInfoMoq2017:new EventArgs()|});""",

            """mock.{|PosInfoMoq2019:RaiseAsync|}(m => m.TheEvent += null, "OK", 1, new object());""",

            """mock.Raise(m => m.EventWithSenderAndString += null, {|PosInfoMoq2017:new EventArgs()|});""",

            """mock.{|PosInfoMoq2017:Raise|}(m => m.EventWithSenderAndString += null, 1, 2, 3, 4);""",
        ];

        [Theory]
        [MemberData(nameof(ValidTestCases))]
        [MemberData(nameof(InvalidTestCases))]
        public async Task Analyze(string code)
        {
            var source =
                $$"""
                namespace ConsoleApplication1
                {
                    using Moq;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<I>(MockBehavior.Strict);
                            {{code}}
                        }
                    }

                    {{TestTypes}}
                }
                """;

            await Verifier.VerifyAnalyzerAsync(source);
        }

        [Theory]
        [MemberData(nameof(ValidTestCases))]
        [MemberData(nameof(InvalidTestCases))]
        public async Task NoMoqLibrary(string code)
        {
            var source =
                $$"""
                namespace ConsoleApplication1
                {
                    using FakeMoqLibrary;
                    using System;
                    using System.Threading.Tasks;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
                            var mock = new Mock<I>(MockBehavior.Strict);
                            {{code}}
                        }
                    }

                    {{TestTypes}}
                }

                {{MoqAnalyzersTests.FakeMoqLibrary}}
                """;

            await Verifier.VerifyAnalyzerWithNoMoqLibraryAsync(source);
        }
    }
}