namespace PosInformatique.Moq.Analyzers
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class MockExpressionHelper
    {
        public static bool IsMockCreation(ObjectCreationExpressionSyntax expression)
        {
            if (expression.Type is not GenericNameSyntax genericName)
            {
                return false;
            }

            if (genericName.Identifier.ValueText != "Mock")
            {
                return false;
            }

            return true;
        }
    }
}
