using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SpecialTypeNameNode : TypeNameNode, ISpecialTypeNameNode
{
    public override ISpecialTypeNameSyntax Syntax { get; }
    public override SpecialTypeName Name => Syntax.Name;

    public SpecialTypeNameNode(ISpecialTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
