using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class PlaceholderSyntax : TreeAttributeSyntax
{
    public override TypeSyntax? Type => null;

    public PlaceholderSyntax(string name)
        : base(name) { }

    public override string ToString() => $"/{Name}/";
}
