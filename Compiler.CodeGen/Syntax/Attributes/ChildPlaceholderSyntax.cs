using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
public sealed class ChildPlaceholderSyntax : TreeAttributeSyntax
{
    public override TypeSyntax? Type => null;

    public ChildPlaceholderSyntax(string name)
        : base(name) { }

    public override string ToString() => $"/{Name}/";
}
