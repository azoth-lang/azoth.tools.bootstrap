using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static class BindingAmbiguousNamesAspect
{
    public static IFixedList<IDeclarationNode> StandardNameExpression_ReferencedDeclarations(IStandardNameExpressionNode node)
        => node.ContainingLexicalScope.Lookup(node.Name).ToFixedList();
}
