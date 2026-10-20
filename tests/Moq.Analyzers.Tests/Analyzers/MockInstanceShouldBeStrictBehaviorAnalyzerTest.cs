//-----------------------------------------------------------------------
// <copyright file="MockInstanceShouldBeStrictBehaviorAnalyzerTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers.Tests
{
    using Verifier = MoqCSharpAnalyzerVerifier<MockInstanceShouldBeStrictBehaviorAnalyzer>;

    public class MockInstanceShouldBeStrictBehaviorAnalyzerTest
    {
        private const string TestTypes =
            """
            public interface I { }

            public class C : I { }
            """;

        public static TheoryData<string> ValidTestCases =>
        [
            """var obj = new object();""",

            """string.Format("Ignored");""",

            """var mock = new Mock<I>(MockBehavior.Strict);""",

            """var mock = new Mock<I>(MockBehavior.Strict, 1, 2);""",

            """var mock = new Mock<I>(() => new C(), MockBehavior.Strict);""",

            """var mock = new Mock<I>((Expression<Func<I>>)null, MockBehavior.Strict);""",

            """var mock = Mock.Of<I>(MockBehavior.Strict);""",

            """var mock = Mock.Of<I>(i => true, MockBehavior.Strict);""",
        ];

        public static TheoryData<string> InvalidTestCases =>
        [
            """var mock = {|PosInfoMoq1001:new Mock<I>(MockBehavior.Loose)|};""",

            """var mock = {|PosInfoMoq1001:new Mock<I>(MockBehavior.Default)|};""",

            """var mock = {|PosInfoMoq1001:new Mock<I>(MockBehavior.Loose, 1, 2)|};""",

            """var mock = {|PosInfoMoq1001:new Mock<I>(MockBehavior.Default, 1, 2)|};""",

            """var mock = {|PosInfoMoq1001:new Mock<I>(() => new C(), MockBehavior.Loose)|};""",

            """var mock = {|PosInfoMoq1001:new Mock<I>(() => new C(), MockBehavior.Default)|};""",

            """var mock = {|PosInfoMoq1001:new Mock<I>()|};""",

            """var mock = {|PosInfoMoq1001:new Mock<I> { }|};""",

            """var mock = {|PosInfoMoq1001:new Mock<I>(default, 1, 2)|};""",

            """var mock = {|PosInfoMoq1001:Mock.Of<I>(MockBehavior.Loose)|};""",

            """var mock = {|PosInfoMoq1001:Mock.Of<I>(MockBehavior.Default)|};""",

            """var mock = {|PosInfoMoq1001:Mock.Of<I>(i => true, MockBehavior.Loose)|};""",

            """var mock = {|PosInfoMoq1001:Mock.Of<I>(i => true, MockBehavior.Default)|};""",

            """var mock = {|PosInfoMoq1001:Mock.Of<I>()|};""",

            """var mock = {|PosInfoMoq1001:Mock.Of<I>(i => true)|};""",
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
                    using System.Linq.Expressions;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
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
                    using System.Linq.Expressions;

                    public class TestClass
                    {
                        public void TestMethod()
                        {
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