using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class OptionalTypeNode : TypeNode, IOptionalTypeNode
{
    public override IOptionalTypeSyntax Syntax { get; }
    public ITypeNode Referent { get; }

    public OptionalTypeNode(IOptionalTypeSyntax syntax, ITypeNode referent)
    {
        Syntax = syntax;
        Referent = referent;
    }
}
