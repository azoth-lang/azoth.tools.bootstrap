using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class AggregateAttributeSyntax : AspectAttributeSyntax
{
    public override TypeSyntax? Type => null;

    public AggregateAttributeSyntax(SymbolSyntax node, string name)
        : base(false, null, node, name, false)
    {
    }

    public override string ToString() => $"↗↖ {Node}.{Name}";
}
