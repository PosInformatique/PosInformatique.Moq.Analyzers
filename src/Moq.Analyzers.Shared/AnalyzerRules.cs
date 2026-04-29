//-----------------------------------------------------------------------
// <copyright file="AnalyzerRules.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Moq.Analyzers
{
    internal static class AnalyzerRules
    {
        public const string VerifyAllShouldBeCalledAnalyzerRuleId = "PosInfoMoq1000";

        public const string MockInstanceShouldBeStrictBehaviorAnalyzerRuleId = "PosInfoMoq1001";

        public const string UseSetupSetWithGenericArgumentRuleId = "PosInfoMoq1005";
    }
}