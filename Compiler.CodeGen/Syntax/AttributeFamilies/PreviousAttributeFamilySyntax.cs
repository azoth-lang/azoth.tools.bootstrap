using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;

public sealed class PreviousAttributeFamilySyntax : AttributeFamilySyntax
{
    public PreviousAttributeFamilySyntax(string name, TypeSyntax type)
        : base(name, type) { }

    public override string ToString() => $"⮡ *.{Name}: {Type}";
}
